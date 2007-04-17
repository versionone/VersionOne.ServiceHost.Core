using System;

namespace VersionOne.ServiceHost.Eventing
{
	public delegate void EventDelegate(object pubobj);
	
	public interface IEventManager
	{
		void Publish(object pubobj);
		void Subscribe(Type pubtype, EventDelegate listener);
	}
}