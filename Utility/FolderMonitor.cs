using System;
using System.Collections.Generic;
using System.IO;
using VersionOne.Profile;

namespace VersionOne.ServiceHost.Core
{
	public delegate void ProcessFileDelegate(string file);
	public delegate void ProcessFolderDelegate(string folder);
	public delegate void ProcessFolderBatchDelegate(string[] folders);

	public abstract class FileSystemMonitor
	{
		private IProfile _profile;
		private IProfile _processedPathsProfile;

		private IProfile ProcessedPaths
		{
			get
			{
				if (_processedPathsProfile == null)
					_processedPathsProfile = _profile["ProcessedFiles"]; // Retaining name "ProcessedFiles" for backward-compatibility
				return _processedPathsProfile;
			}
		}

		#region FilterPattern property
		private string _filterPattern;
		protected string FilterPattern
		{
			get { return _filterPattern; }
			set { _filterPattern = value; }
		}
		#endregion

		#region WatchFolder property
		private string _watchFolder;
		protected string WatchFolder
		{
			get { return _watchFolder; }
			set { _watchFolder = value; }
		}
		#endregion

		/// <summary>
		/// Get the processed state of the given file from the profile.
		/// </summary>
		/// <param name="file">File to look up.</param>
		/// <returns>True if processed. False if not processed. Null if not in profile.</returns>
		protected bool? GetState(string file)
		{
			string value = ProcessedPaths[file].Value;
			if (value == null)
				return null;
			return bool.Parse(value);
		}

		/// <summary>
		/// Save the processing state for the given file to the profile.
		/// </summary>
		/// <param name="file">File in question.</param>
		/// <param name="done">True if processed.</param>
		protected void SaveState(string file, bool? done)
		{
			ProcessedPaths[file].Value = done == null ? null : done.ToString();
		}

		public FileSystemMonitor(IProfile profile, string watchFolder, string filterPattern)
		{
			_profile = profile;
			WatchFolder = watchFolder;
			FilterPattern = filterPattern;
			if (string.IsNullOrEmpty(FilterPattern))
				FilterPattern = "*.*";

			string path = Path.GetFullPath(WatchFolder);
			if (!Directory.Exists(path))
				Directory.CreateDirectory(path);
		}

		/// <summary>
		/// Perform the basic processing pattern.
		/// </summary>
		/// <param name="path">A file or directory name, depending on the subclass implementation.</param>
		protected void ProcessPath(string path)
		{
			if (GetState(path) == null)
			{
				SaveState(path, false);
				InvokeProcessor(path);
				SaveState(path, true);
			}
		}

		protected abstract void InvokeProcessor(string path);
	}

	public class FileMonitor : FileSystemMonitor
	{
		private ProcessFileDelegate _processor;

		public FileMonitor(IProfile profile, string watchfolder, string filterpattern, ProcessFileDelegate processor)
			: base(profile, watchfolder, filterpattern)
		{
			_processor = processor;			
		}

		public void ProcessFolder(object pubobj)
		{
			string path = Path.GetFullPath(WatchFolder);
			string[] files = Directory.GetFiles(path, FilterPattern);
			foreach (string file in files)
				ProcessPath(file);
		}

		protected override void InvokeProcessor(string path)
		{
			_processor(path);
		}
	}

	public class FolderMonitor : FileSystemMonitor
	{
		private ProcessFolderDelegate _processor;

		public FolderMonitor(IProfile profile, string watchFolder, string filterPattern, ProcessFolderDelegate processor)
			: base(profile, watchFolder, filterPattern)
		{
			_processor = processor;
		}

		public void ProcessFolder(object pubobj)
		{
			string path = Path.GetFullPath(WatchFolder);
			string[] subFolders = Directory.GetDirectories(path, FilterPattern);
			foreach (string subFolder in subFolders)
				ProcessPath(subFolder);
		}

		protected override void InvokeProcessor(string path)
		{
			_processor(path);
		}
	}

	public class BatchFolderMonitor : FileSystemMonitor
	{
		private ProcessFolderBatchDelegate _processor;
		public BatchFolderMonitor(IProfile profile, string watchFolder, string filterPattern, ProcessFolderBatchDelegate processor) : base(profile, watchFolder, filterPattern)
		{
			_processor = processor;
		}

		public void ProcessFolder(object pubobj)
		{
			string path = Path.GetFullPath(WatchFolder);
			string[] subFolders = Directory.GetDirectories(path, FilterPattern, SearchOption.AllDirectories);

			List <string> notProcessed = new List<string>();
			foreach (string subFolder in subFolders)
			{
				if (GetState(subFolder) == null)
					notProcessed.Add(subFolder);
			}

			if (notProcessed.Count == 0)
				return;

			foreach(string subFolder in notProcessed)
				SaveState(subFolder, false);

			_processor(notProcessed.ToArray());

			foreach (string subFolder in notProcessed)
				SaveState(subFolder, true);
		}

		protected override void InvokeProcessor(string path)
		{
			// TODO: Fix this smell
		}
	}
}