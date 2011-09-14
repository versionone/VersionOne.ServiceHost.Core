using System.Collections.Generic;
using VersionOne.ServiceHost.Eventing;

namespace VersionOne.ServiceHost.Core.StartupValidation {
    public abstract class StartupCheckerBase {
        protected StartupCheckerBase(IEventManager eventManager) {
            eventManager.Subscribe(typeof(ServiceHostState), Run);
        }

        public void Run(object obj) {
            if(obj == null || !ServiceHostState.Validate.Equals(obj)) {
                return;
            }

            var steps = CreateValidators();

            foreach(var step in steps) {
                step.Run();
            }
        }

        protected abstract IEnumerable<IValidationStep> CreateValidators();
    }
}