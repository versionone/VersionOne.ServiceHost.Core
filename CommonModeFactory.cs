namespace VersionOne.ServiceHost.Core {
    public class CommonModeFactory {
        private static CommonModeFactory instance;

        public static CommonModeFactory Instance {
            get { return instance ?? (instance = new CommonModeFactory()); }
        }

        public CommonStartup CreateStartup() {
            var starter = new CommonStartup();
            starter.Initialize();
            return starter;
        }
    }
}