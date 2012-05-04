using Ninject;
using Ninject.Parameters;

namespace VersionOne.ServiceHost.Core {
    // TODO factory hierarchy is overly complex and unnecessary
    public abstract class ContainerBasedFactory {
        protected readonly IKernel Container;

        protected ContainerBasedFactory(IKernel container) {
            Container = container;
        }

        protected IParameter Parameter(string name, object value) {
            return new ConstructorArgument(name, value);
        }
    }
}