using System;
using System.Xml;
using VersionOne.Profile;
using VersionOne.ServiceHost.Eventing;
using VersionOne.ServiceHost.HostedServices;
using VersionOne.ServiceHost.Logging;

namespace VersionOne.ServiceHost.Core
{
	public abstract class FolderProcessorService : IHostedService
	{
		private IEventManager _eventmanager;		
		private FolderMonitor _monitor;

		protected IEventManager EventManager { get { return _eventmanager; } }
		
		public void Initialize(XmlElement config, IEventManager eventManager, IProfile profile)
		{
			_monitor = new FolderMonitor(profile, config["Watch"].InnerText, config["Filter"].InnerText, Process);
			_eventmanager = eventManager;			
			_eventmanager.Subscribe(EventSinkType, _monitor.ProcessFolder);
		}

		private void Process(string file)
		{
			try
			{
				LogMessage.Log(string.Format("Starting Processing File: {0}", file),EventManager);
				InternalProcess(file);
				LogMessage.Log(string.Format("Finished Processing File: {0}", file), EventManager);
			}
			catch (Exception ex)
			{
				LogMessage.Log(string.Format("Failed Processing File: {0}", file), ex, EventManager);
			}
		}
		
		protected abstract void InternalProcess(string file);
		protected abstract Type EventSinkType { get; }
	}
}