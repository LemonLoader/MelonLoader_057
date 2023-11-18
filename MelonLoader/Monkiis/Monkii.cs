namespace MonkiiLoader
{
    public static class Monkii<T> where T : MonkiiBase
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance != null)
                    return _instance;

                var Monkii = MonkiiAssembly.FindMonkiiInstance<T>();
                if (Monkii == null)
                    return null;

                _instance = Monkii;
                return Monkii;
            }
        }

        public static MonkiiLogger.Instance Logger => Instance?.LoggerInstance;
    }
}
