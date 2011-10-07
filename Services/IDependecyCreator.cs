using Ninject;

namespace VersionOne.ServiceHost.Core.Services {
    public interface IDependecyCreator {
        IKernel GetContainer();
    }
}