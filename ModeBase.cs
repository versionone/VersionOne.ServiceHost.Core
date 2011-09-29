using Ninject;

namespace VersionOne.ServiceHost.Core {
    public class ModeBase {
        protected IKernel Container;
        protected readonly CommonStartup Starter;

        protected ModeBase() {
            Starter = CommonModeFactory.Instance.CreateStartup();
        }
    }
}