using System;
using System.Timers;
using System.Xml;
using VersionOne.Profile;
using VersionOne.ServiceHost.Eventing;
using VersionOne.ServiceHost.HostedServices;
using VersionOne.ServiceHost.Logging;

namespace VersionOne.ServiceHost.Core.Services
{
	public class TimePublisherService : IHostedService
	{
		private double _interval = 0;
		private Type _publishtype = null;
		private IEventManager _eventmanager;
		private Timer _timer;

		public void Initialize(XmlElement config, IEventManager eventmanager, IProfile profile)
		{
			if (!double.TryParse(config["Interval"].InnerText, out _interval))
				_interval = -1;
			_publishtype = Type.GetType(config["PublishClass"].InnerText);
			
			_eventmanager = eventmanager;
			_eventmanager.Subscribe(typeof(ServiceHostState),HostStateChanged);

			_timer = new Timer(_interval);
			_timer.Enabled = false;
			_timer.Elapsed += Timer_Elapsed;
		}

		private void HostStateChanged(object pubobj)
		{
			ServiceHostState state = (ServiceHostState) pubobj;
			if (state == ServiceHostState.Startup)
				_timer.Enabled = true;
			else if (state == ServiceHostState.Shutdown)
				_timer.Enabled = false;			
		}

		void Timer_Elapsed(object sender, ElapsedEventArgs e)
		{
			object pub = Activator.CreateInstance(_publishtype);
			LogMessage.Log(LogMessage.SeverityType.Debug,string.Format("Timer Elapsed {0} {1} {2}",_interval,_publishtype.Name, e.SignalTime),_eventmanager);
			_eventmanager.Publish(pub);
		}
	}
}