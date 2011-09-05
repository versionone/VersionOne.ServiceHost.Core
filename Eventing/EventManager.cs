using System;
using System.Collections.Generic;
using VersionOne.ServiceHost.Core.Logging;
using VersionOne.ServiceHost.Core;

namespace VersionOne.ServiceHost.Eventing
{
	public class EventManager : IEventManager
	{
		private readonly IDictionary<Type, EventDelegate> _subscriptions = new Dictionary<Type, EventDelegate>();

        private readonly ILogger logger;

        public EventManager() 
        {
            logger = new Logger(this);
        }

        public void Publish(object pubobj) 
        {
            EventDelegate subs;
            if (_subscriptions.TryGetValue(pubobj.GetType(), out subs))
            {
                try
                {
                    subs(pubobj);
                }
                catch (Exception ex)
                {
                    logger.Log("Event Manager Caught Unhandled Exception", ex);
                    logger.Log(ex.Message);
                    //TODO find smart way to make startup validation crash
                    if (pubobj is ServiceHostState && ServiceHostState.Validate.Equals(pubobj)) {
                        throw;
                    }
                }
            }
        }

	    public void Subscribe(Type pubtype, EventDelegate listener)
		{
			EventDelegate subs;
			if (!_subscriptions.TryGetValue(pubtype, out subs))
				_subscriptions[pubtype] = listener;
			else
				_subscriptions[pubtype] = (EventDelegate)Delegate.Combine(subs, listener);
		}

        public void Unsubscribe(Type pubtype, EventDelegate listener)
        {
            EventDelegate subscription;
            
            if(_subscriptions.TryGetValue(pubtype, out subscription))
            {
                EventDelegate updatedSubscription = (EventDelegate) Delegate.Remove(subscription, listener);
                
                if (updatedSubscription == null)
                {
                    _subscriptions.Remove(pubtype);
                    return;
                }

                _subscriptions[pubtype] = updatedSubscription;
            }
        }
	}
}