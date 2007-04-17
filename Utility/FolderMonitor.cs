using System.IO;
using VersionOne.Profile;

namespace VersionOne.ServiceHost.Core
{
	public delegate void ProcessFileDelegate(string file);	
	public class FolderMonitor
	{
		private IProfile _profile;
		private IProfile _processfilesprofile;
		private string _watchfolder;
		private string _filterpattern;
		private ProcessFileDelegate _processor;

		private IProfile ProcessedFiles
		{
			get
			{
				if (_processfilesprofile == null)
					_processfilesprofile = _profile["ProcessedFiles"];
				return _processfilesprofile;
			}
		}			
		public FolderMonitor(IProfile profile, string watchfolder, string filterpattern, ProcessFileDelegate processor)
		{
			_profile = profile;			
			_watchfolder = watchfolder;
			_filterpattern = filterpattern;
			_processor = processor;
			
			if (string.IsNullOrEmpty(_filterpattern))
				_filterpattern = "*.*";
			string path = Path.GetFullPath(_watchfolder);
			if (!Directory.Exists(path))
				Directory.CreateDirectory(path);						
		}

		public void ProcessFolder(object pubobj)
		{
			string path = Path.GetFullPath(_watchfolder);
			string[] files = Directory.GetFiles(path, _filterpattern);
			foreach (string file in files)
				ProcessFile(file);
		}

		private void ProcessFile(string file)
		{
			if (GetState(file) == null)
			{
				SaveState(file, false);
				_processor(file);
				SaveState(file, true);
			}
		}

		private bool? GetState(string file)
		{
			string value = ProcessedFiles[file].Value;
			if (value == null)
				return null;
			return bool.Parse(value);
		}
		
		private void SaveState(string file, bool? done)
		{
			ProcessedFiles[file].Value = done == null ? null : done.ToString();
		}
	}
}