using Ninject;

namespace VersionOne.ServiceHost.Core {
    public class DependencyInjector {
        private readonly IKernel container;

        public DependencyInjector(IKernel container) {
            this.container = container;
        }

        public void Inject(object consumer) {
            container.Inject(consumer);
        }
    }
}