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
		private string _filterpattern;
		private string _watchfolder;

		private IProfile ProcessedPaths
		{
			get
			{
				if (_processedPathsProfile == null)
					_processedPathsProfile = _profile["ProcessedFiles"]; // Retaining name "ProcessedFiles" for backward-compatibility
				return _processedPathsProfile;
			}
		}

		protected string FilterPattern
		{
			get { return _filterpattern; }
			set { _filterpattern = value; }
		}

		protected string WatchFolder
		{
			get { return _watchfolder; }
			set { _watchfolder = value; }
		}

		protected bool? GetState(string file)
		{
			string value = ProcessedPaths[file].Value;
			if (value == null)
				return null;
			return bool.Parse(value);
		}

		protected void SaveState(string file, bool? done)
		{
			ProcessedPaths[file].Value = done == null ? null : done.ToString();
		}

		public FileSystemMonitor(IProfile profile, string watchfolder, string filterpattern)
		{
			_profile = profile;
			WatchFolder = watchfolder;
			FilterPattern = filterpattern;
			if (string.IsNullOrEmpty(FilterPattern))
				FilterPattern = "*.*";

			string path = Path.GetFullPath(WatchFolder);
			if (!Directory.Exists(path))
				Directory.CreateDirectory(path);
		}

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

		public FolderMonitor(IProfile profile, string watchfolder, string filterpattern, ProcessFolderDelegate processor)
			: base(profile, watchfolder, filterpattern)
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
		public BatchFolderMonitor(IProfile profile, string watchfolder, string filterpattern, ProcessFolderBatchDelegate processor) : base(profile, watchfolder, filterpattern)
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