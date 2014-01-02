using System.Xml;
using VersionOne.Profile;
using VersionOne.ServiceHost.Eventing;

namespace VersionOne.ServiceHost.Core.Services 
{
    public interface IHostedService 
    {
        void Initialize(XmlElement config, IEventManager eventManager, IProfile profile);
        void Start();
    }
}