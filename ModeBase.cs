using Ninject;

namespace VersionOne.ServiceHost.Core {
    public class ModeBase {
        protected readonly IKernel Container;
        protected readonly CommonMode Starter;

        protected ModeBase() {
            Container = new StandardKernel();
            Starter = CommonModeFactory.Instance.CreateStartup(Container);
        }
    }
}