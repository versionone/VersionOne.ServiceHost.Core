using System;
using System.Collections.Generic;
using System.Threading;
using VersionOne.Profile;
using VersionOne.ServiceHost.Core.Eventing;
using VersionOne.ServiceHost.Eventing;
using VersionOne.ServiceHost.Core.Logging;

namespace VersionOne.ServiceHost.Core {
    public abstract class CommonMode {
        public class FlushProfile { }

        private IEventManager eventmanager;
        private IList<ServiceInfo> services;
        private IProfileStore profilestore;

        protected IEventManager EventManager {
            get { return eventmanager ?? (eventmanager = new EventManager()); }
        }

        protected ILogger Logger {
            get { return new Logger(EventManager); }
        }

        protected IEnumerable<ServiceInfo> Services {
            get { return services ?? (services = (IList<ServiceInfo>)System.Configuration.ConfigurationManager.GetSection("Services")); }
        }


        protected IProfileStore ProfileStore {
            get { return profilestore ?? (profilestore = new XmlProfileStore("profile.xml")); }
        }

        protected void Startup() {
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

            foreach(var ss in Services) {
                Logger.Log(string.Format("Initializing {0}", ss.Name));
                ss.Service.Initialize(ss.Config, EventManager, ProfileStore[ss.Name]);
                Logger.Log(string.Format("Initialized {0}", ss.Name));
            }

            EventManager.Publish(ServiceHostState.Validate);
            EventManager.Publish(ServiceHostState.Startup);
            EventManager.Subscribe(typeof(FlushProfile), FlushProfileImpl);
        }

        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e) {
            Logger.Log("Service Host Caught Unhandled Exception", (Exception)e.ExceptionObject);
        }

        protected void Shutdown() {
            EventManager.Publish(ServiceHostState.Shutdown);
            Thread.Sleep(5 * 1000);
            ProfileStore.Flush();
        }

        private void FlushProfileImpl(object o) {
            ProfileStore.Flush();
        }
    }
}