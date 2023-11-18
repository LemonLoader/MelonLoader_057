using System;
using System.Collections.Generic;
#pragma warning disable 0618 // Disabling the obsolete references warning to prevent the IDE going crazy when subscribing deprecated methods to some events in RegisterCallbacks

namespace MonkiiLoader
{
    public abstract class MonkiiMod : MonkiiTypeBase<MonkiiMod>
    {
        static MonkiiMod()
        {
            TypeName = "Mod";
        }

        protected private override bool RegisterInternal()
        {
            try
            {
                OnPreSupportModule();
            }
            catch (Exception ex)
            {
                MonkiiLogger.Error($"Failed to register {MonkiiTypeName} '{Location}': Monkii failed to initialize in the deprecated OnPreSupportModule callback!");
                MonkiiLogger.Error(ex.ToString());
                return false;
            }

            if (!base.RegisterInternal())
                return false;

            if (MonkiiEvents.MonkiiHarmonyInit.Disposed)
                HarmonyInit();
            else
                MonkiiEvents.MonkiiHarmonyInit.Subscribe(HarmonyInit, Priority, true);

            return true;
        }
        private void HarmonyInit()
        {
            if (!MonkiiAssembly.HarmonyDontPatchAll)
                HarmonyInstance.PatchAll(MonkiiAssembly.Assembly);
        }

        protected private override void RegisterCallbacks()
        {
            base.RegisterCallbacks();

            MonkiiEvents.OnSceneWasLoaded.Subscribe(OnSceneWasLoaded, Priority);
            MonkiiEvents.OnSceneWasInitialized.Subscribe(OnSceneWasInitialized, Priority);
            MonkiiEvents.OnSceneWasUnloaded.Subscribe(OnSceneWasUnloaded, Priority);

            MonkiiEvents.OnSceneWasLoaded.Subscribe((idx, name) => OnLevelWasLoaded(idx), Priority);
            MonkiiEvents.OnSceneWasInitialized.Subscribe((idx, name) => OnLevelWasInitialized(idx), Priority);
            MonkiiEvents.OnApplicationStart.Subscribe(OnApplicationStart, Priority);
        }

        #region Callbacks

        /// <summary>
        /// Runs when a new Scene is loaded.
        /// </summary>
        public virtual void OnSceneWasLoaded(int buildIndex, string sceneName) { }

        /// <summary>
        /// Runs once a Scene is initialized.
        /// </summary>
        public virtual void OnSceneWasInitialized(int buildIndex, string sceneName) { }

        /// <summary>
        /// Runs once a Scene unloads.
        /// </summary>
        public virtual void OnSceneWasUnloaded(int buildIndex, string sceneName) { }

        #endregion

        #region Obsolete Members
        [Obsolete("Override OnSceneWasLoaded instead.")]
        public virtual void OnLevelWasLoaded(int level) { }
        [Obsolete("Override OnSceneWasInitialized instead.")]
        public virtual void OnLevelWasInitialized(int level) { }

        [Obsolete()]
        private MonkiiModInfoAttribute _LegacyInfoAttribute = null;
        [Obsolete("Use MonkiiBase.Info instead.")]
        public MonkiiModInfoAttribute InfoAttribute { get { if (_LegacyInfoAttribute == null) _LegacyInfoAttribute = new MonkiiModInfoAttribute(Info.SystemType, Info.Name, Info.Version, Info.Author, Info.DownloadLink); return _LegacyInfoAttribute; } }
        [Obsolete()]
        private MonkiiModGameAttribute[] _LegacyGameAttributes = null;
        [Obsolete("Use MonkiiBase.Games instead.")]
        public MonkiiModGameAttribute[] GameAttributes { get {
                if (_LegacyGameAttributes != null)
                    return _LegacyGameAttributes;
                List<MonkiiModGameAttribute> newatts = new();
                foreach (MonkiiGameAttribute att in Games)
                    newatts.Add(new MonkiiModGameAttribute(att.Developer, att.Name));
                _LegacyGameAttributes = newatts.ToArray();
                return _LegacyGameAttributes;
            } }

        #endregion
    }
}