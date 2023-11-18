using MonkiiLoader.Modules;
using System.Collections.Generic;

namespace MonkiiLoader.CompatibilityLayers
{
    internal class SLZ_Module : MonkiiModule
    {
        private static bool HasGotLoadingSceneIndex = false;
        private static int LoadingSceneIndex = -9;

        private static Dictionary<string, int> CompatibleGames = new Dictionary<string, int>
        {
            ["BONELAB"] = 0,
            ["BONEWORKS"] = 0
        };

        public override void OnInitialize()
        {
            if (!CompatibleGames.ContainsKey(InternalUtils.UnityInformationHandler.GameName))
                return;

            MonkiiEvents.OnSceneWasLoaded.Subscribe(OnSceneLoad, int.MinValue);
            MonkiiEvents.OnSceneWasInitialized.Subscribe(OnSceneInit, int.MinValue);
        }

        private static void OnSceneLoad(int buildIndex, string name)
        {
            if (HasGotLoadingSceneIndex)
            {
                if (buildIndex == LoadingSceneIndex)
                    PreSceneEvent();
                return;
            }

            if (buildIndex == 0)
                return;

            HasGotLoadingSceneIndex = true;
            LoadingSceneIndex = buildIndex;
            PreSceneEvent();
        }

        private static void OnSceneInit(int buildIndex, string name)
        {
            if (!HasGotLoadingSceneIndex
                || (buildIndex != LoadingSceneIndex))
                return;

            PostSceneEvent();
            MonkiiBase.SendMessageAll("OnLoadingScreen");
            MonkiiBase.SendMessageAll($"{InternalUtils.UnityInformationHandler.GameName}_OnLoadingScreen");
        }

        private static MonkiiEvent<int, string>.MonkiiEventSubscriber[] SceneInitBackup;
        private static void PreSceneEvent()
        {
            SceneInitBackup = MonkiiEvents.OnSceneWasInitialized.GetSubscribers();
            MonkiiEvents.OnSceneWasInitialized.UnsubscribeAll();
            MonkiiEvents.OnSceneWasInitialized.Subscribe(OnSceneInit, int.MinValue);
        }
        private static void PostSceneEvent()
        {
            MonkiiEvents.OnSceneWasInitialized.UnsubscribeAll();
            foreach (var sub in SceneInitBackup)
                MonkiiEvents.OnSceneWasInitialized.Subscribe(sub.del, sub.priority, sub.unsubscribeOnFirstInvocation);
            SceneInitBackup = null;
        }
    }
}