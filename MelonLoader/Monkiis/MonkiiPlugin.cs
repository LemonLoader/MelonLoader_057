using System;
using System.Collections.Generic;
#pragma warning disable 0618 // Disabling the obsolete references warning to prevent the IDE going crazy when subscribing deprecated methods to some events in RegisterCallbacks

namespace MonkiiLoader
{
    public abstract class MonkiiPlugin : MonkiiTypeBase<MonkiiPlugin>
    {
        static MonkiiPlugin()
        {
            TypeName = "Plugin";
        }

        protected private override void RegisterCallbacks()
        {
            base.RegisterCallbacks();

            MonkiiEvents.OnPreInitialization.Subscribe(OnPreInitialization, Priority);
            MonkiiEvents.OnApplicationEarlyStart.Subscribe(OnApplicationEarlyStart, Priority);
            MonkiiEvents.OnPreModsLoaded.Subscribe(OnPreModsLoaded, Priority);
            MonkiiEvents.OnPreModsLoaded.Subscribe(OnApplicationStart, Priority);
            MonkiiEvents.OnApplicationStart.Subscribe(OnApplicationStarted, Priority);
            MonkiiEvents.OnPreSupportModule.Subscribe(OnPreSupportModule, Priority);
        }

        protected private override bool RegisterInternal()
        {
            if (!base.RegisterInternal())
                return false;

            if (MonkiiEvents.MonkiiHarmonyEarlyInit.Disposed)
                HarmonyInit();
            else
                MonkiiEvents.MonkiiHarmonyEarlyInit.Subscribe(HarmonyInit, Priority, true);

            return true;
        }
        private void HarmonyInit()
        {
            if (!MonkiiAssembly.HarmonyDontPatchAll)
                HarmonyInstance.PatchAll(MonkiiAssembly.Assembly);
        }

        #region Callbacks

        /// <summary>
        /// Runs before Game Initialization.
        /// </summary>
        public virtual void OnPreInitialization() { }

        /// <summary>
        /// Runs after Game Initialization, before OnApplicationStart and before Assembly Generation on Il2Cpp games
        /// </summary>
        public virtual void OnApplicationEarlyStart() { }

        /// <summary>
        /// Runs before MonkiiMods from the Mods folder are loaded.
        /// </summary>
        public virtual void OnPreModsLoaded() { }

        /// <summary>
        /// Runs after all MonkiiLoader components are fully initialized (including all MonkiiMods).
        /// </summary>
        public virtual void OnApplicationStarted() { }

        #endregion

        #region Obsolete Members

        [Obsolete()]
        private MonkiiPluginInfoAttribute _LegacyInfoAttribute = null;
        [Obsolete("MonkiiPlugin.InfoAttribute is obsolete. Please use MonkiiBase.Info instead.")]
        public MonkiiPluginInfoAttribute InfoAttribute { get { if (_LegacyInfoAttribute == null) _LegacyInfoAttribute = new MonkiiPluginInfoAttribute(Info.SystemType, Info.Name, Info.Version, Info.Author, Info.DownloadLink); return _LegacyInfoAttribute; } }
        [Obsolete()]
        private MonkiiPluginGameAttribute[] _LegacyGameAttributes = null;
        [Obsolete("MonkiiPlugin.GameAttributes is obsolete. Please use MonkiiBase.Games instead.")]
        public MonkiiPluginGameAttribute[] GameAttributes
        {
            get
            {
                if (_LegacyGameAttributes != null)
                    return _LegacyGameAttributes;
                List<MonkiiPluginGameAttribute> newatts = new();
                foreach (MonkiiGameAttribute att in Games)
                    newatts.Add(new MonkiiPluginGameAttribute(att.Developer, att.Name));
                _LegacyGameAttributes = newatts.ToArray();
                return _LegacyGameAttributes;
            }
        }

        #endregion
    }
}