using Ninject;

namespace VersionOne.ServiceHost.Core {
    public class CommonModeFactory {
        private static CommonModeFactory instance;

        public static CommonModeFactory Instance {
            get { return instance ?? (instance = new CommonModeFactory()); }
        }

        public CommonMode CreateStartup(IKernel container) {
            var starter = container.Get<CommonMode>();
            //var starter = new CommonMode();
            starter.Initialize();
            return starter;
        }
    }
}