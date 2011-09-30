namespace VersionOne.ServiceHost.Core {
    public class CommonModeFactory {
        private static CommonModeFactory instance;

        public static CommonModeFactory Instance {
            get { return instance ?? (instance = new CommonModeFactory()); }
        }

        public CommonMode CreateStartup() {
            var starter = new CommonMode();
            starter.Initialize();
            return starter;
        }
    }
}