using System.Xml;
using VersionOne.Profile;
using VersionOne.ServiceHost.Eventing;

namespace VersionOne.ServiceHost.HostedServices
{
	public interface IHostedService
	{
		void Initialize(XmlElement config, IEventManager eventManager, IProfile profile);
	}
}