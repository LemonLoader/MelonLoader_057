namespace MonkiiLoader
{
    internal class SupportModule_From : ISupportModule_From
    {
        public void OnApplicationLateStart()
            => MonkiiEvents.OnApplicationLateStart.Invoke();

        public void OnSceneWasLoaded(int buildIndex, string sceneName)
            => MonkiiEvents.OnSceneWasLoaded.Invoke(buildIndex, sceneName);

        public void OnSceneWasInitialized(int buildIndex, string sceneName)
            => MonkiiEvents.OnSceneWasInitialized.Invoke(buildIndex, sceneName);

        public void OnSceneWasUnloaded(int buildIndex, string sceneName)
            => MonkiiEvents.OnSceneWasUnloaded.Invoke(buildIndex, sceneName);

        public void Update()
            => MonkiiEvents.OnUpdate.Invoke();

        public void FixedUpdate()
            => MonkiiEvents.OnFixedUpdate.Invoke();

        public void LateUpdate()
            => MonkiiEvents.OnLateUpdate.Invoke();

        public void OnGUI()
            => MonkiiEvents.OnGUI.Invoke();

        public void Quit()
            => MonkiiEvents.OnApplicationQuit.Invoke();

        public void DefiniteQuit()
        {
            MonkiiEvents.OnApplicationDefiniteQuit.Invoke();
            Core.Quit();
        }

        public void SetUnhollowerSupportInterface(UnhollowerSupport.Interface unhollower)
        {
            if (UnhollowerSupport.SMInterface == null)
                UnhollowerSupport.SMInterface = unhollower;
        }
    }
}