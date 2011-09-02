using System.Collections.ObjectModel;
using VersionOne.ServiceHost.Eventing;

namespace VersionOne.ServiceHost.Core.StartupValidation {
    public abstract class StartupCheckerBase {
        private readonly IEventManager eventManager;

        protected StartupCheckerBase(IEventManager eventManager) {
            this.eventManager = eventManager;
            eventManager.Subscribe(typeof(ServiceHostState), Run);
        }

        public void Run(object obj) {
            if (obj == null || !ServiceHostState.Validate.Equals(obj)) {
                return;
            }

            var steps = CreateValidators();

            foreach(var step in steps) {
                step.Run();
            }
        }

        private void Shutdown() {
            eventManager.Publish(ServiceHostState.Shutdown);
        }

        protected abstract ReadOnlyCollection<IValidationStep> CreateValidators();


        
    }
}