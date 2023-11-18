using MonkiiLoader.InternalUtils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
#pragma warning disable 0618

namespace MonkiiLoader
{
    public abstract class MonkiiBase
    {
        #region Static

        /// <summary>
        /// Called once a Monkii is fully registered.
        /// </summary>
        public static readonly MonkiiEvent<MonkiiBase> OnMonkiiRegistered = new();

        /// <summary>
        /// Called when a Monkii unregisters.
        /// </summary>
        public static readonly MonkiiEvent<MonkiiBase> OnMonkiiUnregistered = new();

        /// <summary>
        /// Called before a Monkii starts initializing.
        /// </summary>
        public static readonly MonkiiEvent<MonkiiBase> OnMonkiiInitializing = new();

        public static ReadOnlyCollection<MonkiiBase> RegisteredMonkiis => _registeredMonkiis.AsReadOnly();
        internal static List<MonkiiBase> _registeredMonkiis = new();

        /// <summary>
        /// Creates a new Monkii instance for a Wrapper.
        /// </summary>
        public static T CreateWrapper<T>(string name, string author, string version, MonkiiGameAttribute[] games = null, MonkiiProcessAttribute[] processes = null, int priority = 0, ConsoleColor? color = null, ConsoleColor? authorColor = null, string id = null) where T : MonkiiBase, new()
        {
            var Monkii = new T
            {
                Info = new MonkiiInfoAttribute(typeof(T), name, version, author),
                MonkiiAssembly = MonkiiAssembly.LoadMonkiiAssembly(null, typeof(T).Assembly),
                Priority = priority,
                ConsoleColor = color ?? MonkiiLogger.DefaultMonkiiColor,
                AuthorConsoleColor = authorColor ?? MonkiiLogger.DefaultTextColor,
                SupportedProcesses = processes,
                Games = games,
                OptionalDependencies = null,
                ID = id
            };

            return Monkii;
        }

        /// <summary>
        /// Registers a List of Monkiis in the right order.
        /// </summary>
        public static void RegisterSorted<T>(IEnumerable<T> Monkiis) where T : MonkiiBase
        {
            if (Monkiis == null)
                return;

            var collection = Monkiis.ToList();
            SortMonkiis(ref collection);

            foreach (var m in Monkiis)
                m.Register();
        }

        private static void SortMonkiis<T>(ref List<T> Monkiis) where T : MonkiiBase
        {
            DependencyGraph<T>.TopologicalSort(Monkiis);
            Monkiis = Monkiis.OrderBy(x => x.Priority).ToList();
        }

        #endregion

        #region Instance

        private MonkiiGameAttribute[] _games = new MonkiiGameAttribute[0];
        private MonkiiProcessAttribute[] _processes = new MonkiiProcessAttribute[0];
        private MonkiiGameVersionAttribute[] _gameVersions = new MonkiiGameVersionAttribute[0];

        public readonly MonkiiEvent OnRegister = new();
        public readonly MonkiiEvent OnUnregister = new();

        /// <summary>
        /// MonkiiAssembly of the Monkii.
        /// </summary>
        public MonkiiAssembly MonkiiAssembly { get; internal set; }

        /// <summary>
        /// Priority of the Monkii.
        /// </summary>
        public int Priority { get; internal set; }

        /// <summary>
        /// Console Color of the Monkii.
        /// </summary>
        public ConsoleColor ConsoleColor { get; internal set; }

        /// <summary>
        /// Console Color of the Author that made this Monkii.
        /// </summary>
        public ConsoleColor AuthorConsoleColor { get; internal set; }

        /// <summary>
        /// Info Attribute of the Monkii.
        /// </summary>
        public MonkiiInfoAttribute Info { get; internal set; }

        /// <summary>
        /// Process Attributes of the Monkii.
        /// </summary>
        public MonkiiProcessAttribute[] SupportedProcesses
        {
            get => _processes;
            internal set => _processes = (value == null || value.Any(x => x.Universal)) ? new MonkiiProcessAttribute[0] : value;
        }

        /// <summary>
        /// Game Attributes of the Monkii.
        /// </summary>
        public MonkiiGameAttribute[] Games
        {
            get => _games;
            internal set => _games = (value == null || value.Any(x => x.Universal)) ? new MonkiiGameAttribute[0] : value;
        }

        /// <summary>
        /// Game Version Attributes of the Monkii.
        /// </summary>
        public MonkiiGameVersionAttribute[] SupportedGameVersions
        {
            get => _gameVersions;
            internal set => _gameVersions = (value == null || value.Any(x => x.Universal)) ? new MonkiiGameVersionAttribute[0] : value;
        }

        /// <summary>
        /// Optional Dependencies Attribute of the Monkii.
        /// </summary>
        public MonkiiOptionalDependenciesAttribute OptionalDependencies { get; internal set; }

        /// <summary>
        /// Platform Attribute of the Monkii.
        /// </summary>
        public MonkiiPlatformAttribute SupportedPlatforms { get; internal set; }

        /// <summary>
        /// Platform Attribute of the Monkii.
        /// </summary>
        public MonkiiPlatformDomainAttribute SupportedDomain { get; internal set; }

        /// <summary>
        /// Verify Loader Version Attribute of the Monkii.
        /// </summary>
        public VerifyLoaderVersionAttribute SupportedMLVersion { get; internal set; }

        /// <summary>
        /// Verify Build Version Attribute of the Monkii.
        /// </summary>
        public VerifyLoaderBuildAttribute SupportedMLBuild { get; internal set; }

        /// <summary>
        /// Auto-Created Harmony Instance of the Monkii.
        /// </summary>
        public HarmonyLib.Harmony HarmonyInstance { get; internal set; }

        /// <summary>
        /// Auto-Created MonkiiLogger Instance of the Monkii.
        /// </summary>
        public MonkiiLogger.Instance LoggerInstance { get; internal set; }

        /// <summary>
        /// Optional ID of the Monkii.
        /// </summary>
        public string ID { get; internal set; }

        /// <summary>
        /// <see langword="true"/> if the Monkii is registered.
        /// </summary>
        public bool Registered { get; private set; }

        /// <summary>
        /// Name of the current Monkii Type.
        /// </summary>
        public abstract string MonkiiTypeName { get; }

        #region Callbacks

        /// <summary>
        /// Runs before Support Module Initialization and after Assembly Generation for Il2Cpp Games.
        /// </summary>
        public virtual void OnPreSupportModule() { }

        /// <summary>
        /// Runs once per frame.
        /// </summary>
        public virtual void OnUpdate() { }

        /// <summary>
        /// Can run multiple times per frame. Mostly used for Physics.
        /// </summary>
        public virtual void OnFixedUpdate() { }

        /// <summary>
        /// Runs once per frame, after <see cref="OnUpdate"/>.
        /// </summary>
        public virtual void OnLateUpdate() { }

        /// <summary>
        /// Can run multiple times per frame. Mostly used for Unity's IMGUI.
        /// </summary>
        public virtual void OnGUI() { }

        /// <summary>
        /// Runs on a quit request. It is possible to abort the request in this callback.
        /// </summary>
        public virtual void OnApplicationQuit() { }

        /// <summary>
        /// Runs when Monkii Preferences get saved.
        /// </summary>
        public virtual void OnPreferencesSaved() { }

        /// <summary>
        /// Runs when Monkii Preferences get saved. Gets passed the Preferences's File Path.
        /// </summary>
        public virtual void OnPreferencesSaved(string filepath) { }

        /// <summary>
        /// Runs when Monkii Preferences get loaded.
        /// </summary>
        public virtual void OnPreferencesLoaded() { }

        /// <summary>
        /// Runs when Monkii Preferences get loaded. Gets passed the Preferences's File Path.
        /// </summary>
        public virtual void OnPreferencesLoaded(string filepath) { }

        /// <summary>
        /// Runs when the Monkii is registered. Executed before the Monkii's info is printed to the console. This callback should only be used a constructor for the Monkii.
        /// </summary>
        /// <remarks>
        /// Please note that this callback may run before the Support Module is loaded.
        /// <br>As a result, using unhollowed assemblies may not be possible yet and you would have to override <see cref="OnInitializeMonkii"/> instead.</br>
        /// </remarks>
        public virtual void OnEarlyInitializeMonkii() { }

        /// <summary>
        /// Runs after the Monkii has registered. This callback waits until MonkiiLoader has fully initialized (<see cref="MonkiiEvents.OnApplicationStart"/>).
        /// </summary>
        public virtual void OnInitializeMonkii() { }

        /// <summary>
        /// Runs after <see cref="OnInitializeMonkii"/>. This callback waits until Unity has invoked the first 'Start' messages (<see cref="MonkiiEvents.OnApplicationLateStart"/>).
        /// </summary>
        public virtual void OnLateInitializeMonkii() { }

        /// <summary>
        /// Runs when the Monkii is unregistered. Also runs before the Application is closed (<see cref="MonkiiEvents.OnApplicationDefiniteQuit"/>).
        /// </summary>
        public virtual void OnDeinitializeMonkii() { }

        #endregion

        public Incompatibility[] FindIncompatiblities(MonkiiGameAttribute game, string processName, string gameVersion,
            string mlVersion, string mlBuildHashCode, MonkiiPlatformAttribute.CompatiblePlatforms platform,
            MonkiiPlatformDomainAttribute.CompatibleDomains domain)
        {
            var result = new List<Incompatibility>();
            if (!(Games.Length == 0 || Games.Any(x => x.IsCompatible(game))))
                result.Add(Incompatibility.Game);
            else
            {
                if (!(SupportedGameVersions.Length == 0 || SupportedGameVersions.Any(x => x.Version == gameVersion)))
                    result.Add(Incompatibility.GameVersion);

                if (!(SupportedProcesses.Length == 0 || SupportedProcesses.Any(x => x.IsCompatible(processName))))
                    result.Add(Incompatibility.ProcessName);

                if (!(SupportedPlatforms == null || SupportedPlatforms.IsCompatible(platform)))
                    result.Add(Incompatibility.Platform);

                if (!(SupportedDomain == null || SupportedDomain.IsCompatible(domain)))
                    result.Add(Incompatibility.Domain);
            }
            if (!(SupportedMLVersion == null || SupportedMLVersion.IsCompatible(mlVersion)))
                result.Add(Incompatibility.MLVersion);
            else
            {
                if (!(SupportedMLBuild == null || SupportedMLBuild.IsCompatible(mlBuildHashCode)))
                    result.Add(Incompatibility.MLBuild);
            }

            return result.ToArray();
        }

        public Incompatibility[] FindIncompatiblitiesFromContext()
        {
            return FindIncompatiblities(MonkiiUtils.CurrentGameAttribute, Process.GetCurrentProcess().ProcessName, MonkiiUtils.GameVersion, BuildInfo.Version, MonkiiUtils.HashCode, MonkiiUtils.CurrentPlatform, MonkiiUtils.CurrentDomain);
        }

        public static void PrintIncompatibilities(Incompatibility[] incompatibilities, MonkiiBase Monkii)
        {
            if (incompatibilities == null || incompatibilities.Length == 0)
                return;

            MonkiiLogger.WriteLine(ConsoleColor.Red);
            MonkiiLogger.Msg(ConsoleColor.DarkRed, $"'{Monkii.Info.Name} v{Monkii.Info.Version}' is incompatible:");
            if (incompatibilities.Contains(Incompatibility.Game))
            {
                MonkiiLogger.Msg($"- {Monkii.Info.Name} is only compatible with the following Games:");

                foreach (var g in Monkii.Games)
                    MonkiiLogger.Msg($"    - '{g.Name}' by {g.Developer}");
            }
            if (incompatibilities.Contains(Incompatibility.GameVersion))
            {
                MonkiiLogger.Msg($"- {Monkii.Info.Name} is only compatible with the following Game Versions:");

                foreach (var g in Monkii.SupportedGameVersions)
                    MonkiiLogger.Msg($"    - {g.Version}");
            }
            if (incompatibilities.Contains(Incompatibility.ProcessName))
            {
                MonkiiLogger.Msg($"- {Monkii.Info.Name} is only compatible with the following Process Names:");

                foreach (var p in Monkii.SupportedProcesses)
                    MonkiiLogger.Msg($"    - '{p.EXE_Name}'");
            }
            if (incompatibilities.Contains(Incompatibility.Platform))
            {
                MonkiiLogger.Msg($"- {Monkii.Info.Name} is only compatible with the following Platforms:");

                foreach (var p in Monkii.SupportedPlatforms.Platforms)
                    MonkiiLogger.Msg($"    - {p}");
            }
            if (incompatibilities.Contains(Incompatibility.Domain))
            {
                MonkiiLogger.Msg($"- {Monkii.Info.Name} is only compatible with the following Domain:");
                MonkiiLogger.Msg($"    - {Monkii.SupportedDomain.Domain}");
            }
            if (incompatibilities.Contains(Incompatibility.MLVersion))
            {
                MonkiiLogger.Msg($"- {Monkii.Info.Name}  is only compatible with the following MonkiiLoader Versions:");
                MonkiiLogger.Msg($"    - {Monkii.SupportedMLVersion.SemVer}{(Monkii.SupportedMLVersion.IsMinimum ? " or higher" : "")}");
            }
            if (incompatibilities.Contains(Incompatibility.MLBuild))
            {
                MonkiiLogger.Msg($"- {Monkii.Info.Name} is only compatible with the following MonkiiLoader Build Hash Codes:");
                MonkiiLogger.Msg($"    - {Monkii.SupportedMLBuild.HashCode}");
            }

            MonkiiLogger.WriteLine(ConsoleColor.Red);
            MonkiiLogger.WriteSpacer();
        }

        /// <summary>
        /// Registers the Monkii.
        /// </summary>
        public bool Register()
        {
            if (Registered)
                return false;

            if (FindMonkii(Info.Name, Info.Author) != null)
            {
                MonkiiLogger.Warning($"Failed to register {MonkiiTypeName} '{Location}': A Monkii with the same Name and Author is already registered!");
                return false;
            }

            var comp = FindIncompatiblitiesFromContext();
            if (comp.Length != 0)
            {
                PrintIncompatibilities(comp, this);
                return false;
            }

            OnMonkiiInitializing.Invoke(this);

            LoggerInstance ??= new MonkiiLogger.Instance(string.IsNullOrEmpty(ID) ? Info.Name : $"{ID}:{Info.Name}", ConsoleColor);
            HarmonyInstance ??= new HarmonyLib.Harmony($"{Assembly.FullName}:{Info.Name}");

            Registered = true; // this has to be true before the Monkii can subscribe to any events
            RegisterCallbacks();

            try
            {
                OnEarlyInitializeMonkii();
            }
            catch (Exception ex)
            {
                MonkiiLogger.Error($"Failed to register {MonkiiTypeName} '{Location}': Monkii failed to initialize!");
                MonkiiLogger.Error(ex.ToString());
                Registered = false;
                return false;
            }

            if (!RegisterInternal())
                return false;

            _registeredMonkiis.Add(this);

            PrintLoadInfo();

            OnRegister.Invoke();
            OnMonkiiRegistered.Invoke(this);

            if (MonkiiEvents.OnApplicationStart.Disposed)
                LoaderInitialized();
            else
                MonkiiEvents.OnApplicationStart.Subscribe(LoaderInitialized, Priority, true);

            if (MonkiiEvents.OnApplicationLateStart.Disposed)
                OnLateInitializeMonkii();
            else
                MonkiiEvents.OnApplicationLateStart.Subscribe(OnLateInitializeMonkii, Priority, true);

            return true;
        }

        private void HarmonyInit()
        {
            if (!MonkiiAssembly.HarmonyDontPatchAll)
                HarmonyInstance.PatchAll(MonkiiAssembly.Assembly);
        }

        private void LoaderInitialized()
        {
            try
            {
                OnInitializeMonkii();
            }
            catch (Exception ex)
            {
                LoggerInstance.Error(ex);
            }
        }

        protected private virtual bool RegisterInternal()
        {
            return true;
        }

        protected private virtual void UnregisterInternal() { }

        protected private virtual void RegisterCallbacks()
        {
            MonkiiEvents.OnApplicationQuit.Subscribe(OnApplicationQuit, Priority);
            MonkiiEvents.OnUpdate.Subscribe(OnUpdate, Priority);
            MonkiiEvents.OnLateUpdate.Subscribe(OnLateUpdate, Priority);
            MonkiiEvents.OnGUI.Subscribe(OnGUI, Priority);
            MonkiiEvents.OnFixedUpdate.Subscribe(OnFixedUpdate, Priority);
            MonkiiEvents.OnApplicationLateStart.Subscribe(OnApplicationLateStart, Priority);

            MonkiiPreferences.OnPreferencesLoaded.Subscribe(PrefsLoaded, Priority);
            MonkiiPreferences.OnPreferencesSaved.Subscribe(PrefsSaved, Priority);
        }

        private void PrefsSaved(string path)
        {
            OnPreferencesSaved(path);
            OnPreferencesSaved();
            OnModSettingsApplied();
        }

        private void PrefsLoaded(string path)
        {
            OnPreferencesLoaded(path);
            OnPreferencesLoaded();
        }

        /// <summary>
        /// Tries to find a registered Monkii that matches the given Info.
        /// </summary>
        public static MonkiiBase FindMonkii(string MonkiiName, string MonkiiAuthor)
        {
            return _registeredMonkiis.Find(x => x.Info.Name == MonkiiName && x.Info.Author == MonkiiAuthor);
        }

        /// <summary>
        /// Unregisters the Monkii and all other Monkiis located in the same Assembly.
        /// <para>This only unsubscribes the Monkiis from all Callbacks/<see cref="MonkiiEvent"/>s and unpatches all Methods that were patched by Harmony, but doesn't actually unload the whole Assembly.</para>
        /// </summary>
        public void Unregister(string reason = null, bool silent = false)
        {
            if (!Registered)
                return;

            MonkiiAssembly.UnregisterMonkiis(reason, silent);
        }

        internal void UnregisterInstance(string reason, bool silent)
        {
            if (!Registered)
                return;

            try
            {
                OnDeinitializeMonkii();
            }
            catch (Exception ex)
            {
                MonkiiLogger.Error($"Failed to properly unregister {MonkiiTypeName} '{Location}': Monkii failed to deinitialize!");
                MonkiiLogger.Error(ex.ToString());
            }

            UnregisterInternal();

            _registeredMonkiis.Remove(this);
            HarmonyInstance.UnpatchSelf();
            Registered = false;

            if (!silent)
                PrintUnloadInfo(reason);

            OnUnregister.Invoke();
            OnMonkiiUnregistered.Invoke(this);
        }

        private void PrintLoadInfo()
        {
            MonkiiLogger.WriteLine(ConsoleColor.DarkGreen);
            
            MonkiiLogger.Internal_PrintModName(ConsoleColor, AuthorConsoleColor, Info.Name, Info.Author, Info.Version, ID);
            MonkiiLogger.Msg(ConsoleColor.DarkGray, $"Assembly: {Path.GetFileName(MonkiiAssembly.Location)}");

            MonkiiLogger.WriteLine(ConsoleColor.DarkGreen);
        }

        private void PrintUnloadInfo(string reason)
        {
            MonkiiLogger.WriteLine(ConsoleColor.DarkRed);

            MonkiiLogger.Msg(ConsoleColor.DarkGray, MonkiiTypeName + " deinitialized:");
            MonkiiLogger.Internal_PrintModName(ConsoleColor, AuthorConsoleColor, Info.Name, Info.Author, Info.Version, ID);

            if (!string.IsNullOrEmpty(reason))
            {
                MonkiiLogger.Msg(string.Empty);
                MonkiiLogger.Msg($"Reason: '{reason}'");
            }

            MonkiiLogger.WriteLine(ConsoleColor.DarkRed);
        }

        public static void ExecuteAll(LemonAction<MonkiiBase> func, bool unregisterOnFail = false, string unregistrationReason = null)
        {
            ExecuteList(func, _registeredMonkiis, unregisterOnFail, unregistrationReason);
        }

        public static void ExecuteList<T>(LemonAction<T> func, List<T> Monkiis, bool unregisterOnFail = false, string unregistrationReason = null) where T : MonkiiBase
        {
            var failedMonkiis = (unregisterOnFail ? new List<T>() : null);

            LemonEnumerator<T> enumerator = new(Monkiis.ToArray());
            while (enumerator.MoveNext())
            {
                var Monkii = enumerator.Current;
                if (!Monkii.Registered)
                    continue;

                try { func(Monkii); }
                catch (Exception ex)
                {
                    Monkii.LoggerInstance.Error(ex.ToString());
                    if (unregisterOnFail)
                        failedMonkiis.Add(Monkii);
                }
            }

            if (unregisterOnFail)
            {
                foreach (var m in failedMonkiis)
                    m.Unregister(unregistrationReason);
            }
        }

        public static void SendMessageAll(string name, params object[] arguments)
        {
            LemonEnumerator<MonkiiBase> enumerator = new(_registeredMonkiis.ToArray());
            while (enumerator.MoveNext())
            {
                var Monkii = enumerator.Current;
                if (!Monkii.Registered)
                    continue;

                try { Monkii.SendMessage(name, arguments); }
                catch (Exception ex) { Monkii.LoggerInstance.Error(ex.ToString()); }
            }
        }

        public object SendMessage(string name, params object[] arguments)
        {
            var msg = Info.SystemType.GetMethod(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);

            if (msg == null)
                return null;

            return msg.Invoke(msg.IsStatic ? null : this, arguments);
        }
        #endregion

        #region Obsolete Members

        private Harmony.HarmonyInstance _OldHarmonyInstance;

        [Obsolete("Please use either the OnLateInitializeMonkii callback, or the 'MonkiiEvents::OnApplicationLateStart' event instead.")]
        public virtual void OnApplicationLateStart() { }

        [Obsolete("For mods, use OnInitializeMonkii instead. For plugins, use OnPreModsLoaded instead.")]
        public virtual void OnApplicationStart() { }

        [Obsolete("Please use OnPreferencesSaved instead.")]
        public virtual void OnModSettingsApplied() { }

        [Obsolete("Please use HarmonyInstance instead.")]
#pragma warning disable IDE1006 // Naming Styles
        public Harmony.HarmonyInstance harmonyInstance { get { _OldHarmonyInstance ??= new Harmony.HarmonyInstance(HarmonyInstance.Id); return _OldHarmonyInstance; } }
#pragma warning restore IDE1006 // Naming Styles

        [Obsolete("Please use HarmonyInstance instead.")]
        public Harmony.HarmonyInstance Harmony { get { _OldHarmonyInstance ??= new Harmony.HarmonyInstance(HarmonyInstance.Id); return _OldHarmonyInstance; } }

        [Obsolete("Please use MonkiiAssembly.Assembly instead.")]
        public Assembly Assembly => MonkiiAssembly.Assembly;

        [Obsolete("Please use MonkiiAssembly.HarmonyDontPatchAll instead.")]
        public bool HarmonyDontPatchAll => MonkiiAssembly.HarmonyDontPatchAll;

        [Obsolete("Please use MonkiiAssembly.Hash instead.")]
        public string Hash => MonkiiAssembly.Hash;

        [Obsolete("Please use MonkiiAssembly.Location instead.")]
        public string Location => MonkiiAssembly.Location;

        #endregion


        public enum Incompatibility
        {
            MLVersion,
            MLBuild,
            Game,
            GameVersion,
            ProcessName,
            Domain,
            Platform
        }
    }
}