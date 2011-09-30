using Ninject;

namespace VersionOne.ServiceHost.Core {
    public class ModeBase {
        protected IKernel Container;
        protected readonly CommonMode Starter;

        protected ModeBase() {
            Starter = CommonModeFactory.Instance.CreateStartup();
        }
    }
}