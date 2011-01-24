namespace Ellemy.CQRS.Config
{
    public static class Configure
    {
        private static Configuration _currentConfig;
        internal static Configuration CurrentConfig{get { return _currentConfig ?? (_currentConfig = new Configuration()); }
        }
        public static Configuration With()
        {
            return CurrentConfig;
        }
        
    }
}