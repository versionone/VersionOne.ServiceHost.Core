using System;
using System.Collections.Generic;
using VersionOne.ServiceHost.Logging;

namespace VersionOne.ServiceHost.Eventing
{
	public class EventManager : IEventManager
	{
		private IDictionary<Type, EventDelegate> _subscriptions = new Dictionary<Type, EventDelegate>();

		public void Publish(object pubobj)
		{
			EventDelegate subs;
			if (_subscriptions.TryGetValue(pubobj.GetType(), out subs))
					subs(pubobj);
		}

		public void Subscribe(Type pubtype, EventDelegate listener)
		{
			EventDelegate subs;
			EventDelegate newlistener = WrapListener(listener);
			if (!_subscriptions.TryGetValue(pubtype, out subs))
				_subscriptions[pubtype] = newlistener;
			else
				_subscriptions[pubtype] = (EventDelegate)Delegate.Combine(subs, newlistener);
		}

		private EventDelegate WrapListener(EventDelegate listener)
		{
			return delegate(object pubobj) 
			       	{
						try
						{
							listener(pubobj);
						}
						catch (Exception ex)
						{
							LogMessage.Log("Event Manager Caught Unhandled Exception", ex, this);
							LogMessage.Log(ex.Message, this);
						}
			       	};
		}
	}
}