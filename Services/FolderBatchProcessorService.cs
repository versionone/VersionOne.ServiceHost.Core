using System;
using System.Xml;
using VersionOne.Profile;
using VersionOne.ServiceHost.Eventing;
using VersionOne.ServiceHost.HostedServices;
using VersionOne.ServiceHost.Logging;

namespace VersionOne.ServiceHost.Core
{
	public abstract class FolderBatchProcessorService : IHostedService
	{
		private IEventManager _eventManager;
		private BatchFolderMonitor _monitor;
		private string _folderFilterPattern;
		private string _suiteName;

		protected IEventManager EventManager { get { return _eventManager; } }

		public virtual void Initialize(XmlElement config, IEventManager eventManager, IProfile profile)
		{
			_folderFilterPattern = config["Filter"].InnerText;
			_suiteName = config["SuiteName"].InnerText;
			_monitor = new BatchFolderMonitor(profile, config["Watch"].InnerText, _folderFilterPattern, Process);
			_eventManager = eventManager;
			_eventManager.Subscribe(EventSinkType, _monitor.ProcessFolder);
		}

		private void Process(string[] folders)
		{
			try
			{
				LogMessage.Log(LogMessage.SeverityType.Debug, string.Format("Starting Processing Folder: {0}", string.Join(",", folders)), EventManager);
				InternalProcess(folders, _suiteName);
				LogMessage.Log(LogMessage.SeverityType.Debug, string.Format("Finished Processing Folder: {0}", string.Join(",", folders)), EventManager);
			}
			catch (Exception)
			{
				LogMessage.Log(LogMessage.SeverityType.Error, string.Format("Failed Processing Folder: {0}", string.Join(",", folders)), EventManager);
			}
		}

		protected abstract void InternalProcess(string[] folders, string suiteName);
		protected abstract Type EventSinkType { get; }
	}
}