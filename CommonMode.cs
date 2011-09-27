using System;
using System.Collections.Generic;
using System.Threading;
using VersionOne.Profile;
using VersionOne.ServiceHost.Core;
using VersionOne.ServiceHost.Core.Eventing;
using VersionOne.ServiceHost.Eventing;
using VersionOne.ServiceHost.Core.Logging;

namespace VersionOne.ServiceHost
{
	public abstract class CommonMode
	{
		private IEventManager _eventmanager;
		protected IEventManager EventManager
		{
			get
			{
                if (_eventmanager == null) 
                {
                    _eventmanager = new EventManager();
                }

				return _eventmanager;
			}
		}

        protected ILogger Logger 
        {
            get { return new Logger(EventManager); }
        }

		private IList<ServiceInfo> _services;
		protected IList<ServiceInfo> Services
		{
			get
			{
                if (_services == null) 
                {
                    _services = (IList<ServiceInfo>)System.Configuration.ConfigurationManager.GetSection("Services");
                }

				return _services;
			}
		}
		
		private IProfileStore _profilestore;
		protected IProfileStore ProfileStore
		{
			get
			{
                if (_profilestore == null) 
                {
                    _profilestore = new XmlProfileStore("profile.xml");
                }

				return _profilestore;
			}
		}

		protected void Startup()
		{
			AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

			foreach (ServiceInfo ss in Services)
			{
				Logger.Log(string.Format("Initializing {0}", ss.Name));
				ss.Service.Initialize(ss.Config, EventManager, ProfileStore[ss.Name]);
				Logger.Log(string.Format("Initialized {0}", ss.Name));
			}
            EventManager.Publish(ServiceHostState.Validate);
			EventManager.Publish(ServiceHostState.Startup);
			EventManager.Subscribe(typeof(FlushProfile), this._FlushProfile);
		}

		private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			Logger.Log("Service Host Caught Unhandled Exception", (Exception) e.ExceptionObject); 
		}

		protected void Shutdown()
		{
			EventManager.Publish(ServiceHostState.Shutdown);

			Thread.Sleep(5 * 1000);

			ProfileStore.Flush();
		}

		public class FlushProfile {}
		private void _FlushProfile (object o)
		{
			ProfileStore.Flush();
		}
	}
}