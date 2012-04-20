using System.Collections.Generic;
using VersionOne.ServiceHost.Eventing;

namespace VersionOne.ServiceHost.Core.StartupValidation {
    public abstract class StartupCheckerBase {
        private readonly IEventManager eventManager;
        private readonly DependencyInjector dependencyInjector;

        protected StartupCheckerBase(IEventManager eventManager, DependencyInjector dependencyInjector) {
            this.eventManager = eventManager;
            this.dependencyInjector = dependencyInjector;
        }

        protected StartupCheckerBase(IEventManager eventManager) : this(eventManager, null) { }

        public void Initialize() {
            eventManager.Subscribe(typeof(ServiceHostState), Run);
        }

        public void Run(object obj) {
            if(obj == null || !ServiceHostState.Validate.Equals(obj)) {
                return;
            }

            var steps = CreateValidators();

            foreach(var step in steps) {
                // TODO inject dependencies
                step.Run();
            }

            Complete();
        }

        protected abstract IEnumerable<IValidationStep> CreateValidators();

        protected virtual void Complete() {}
    }
}