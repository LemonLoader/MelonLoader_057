using Semver;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;

namespace MonkiiLoader
{
    public sealed class MonkiiAssembly
    {
        #region Static

        /// <summary>
        /// Called before a process of resolving Monkiis from a MonkiiAssembly has started.
        /// </summary>
        public static readonly MonkiiEvent<Assembly> OnAssemblyResolving = new();
        public static event LemonFunc<Assembly, ResolvedMonkiis> CustomMonkiiResolvers;

        internal static List<MonkiiAssembly> loadedAssemblies = new();

        /// <summary>
        /// List of all loaded MonkiiAssemblies.
        /// </summary>
        public static ReadOnlyCollection<MonkiiAssembly> LoadedAssemblies => loadedAssemblies.AsReadOnly();

        /// <summary>
        /// Tries to find the instance of Monkii with type T, whether it's registered or not
        /// </summary>
        public static T FindMonkiiInstance<T>() where T : MonkiiBase
        {
            foreach (var asm in loadedAssemblies)
            {
                foreach (var Monkii in asm.loadedMonkiis)
                {
                    if (Monkii is T teaMonkii)
                        return teaMonkii;
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the MonkiiAssembly of the given member. If the given member is not in any MonkiiAssembly, returns null.
        /// </summary>
        public static MonkiiAssembly GetMonkiiAssemblyOfMember(MemberInfo member, object obj = null)
        {
            if (member == null)
                return null;

            if (obj != null && obj is MonkiiBase Monkii)
                return Monkii.MonkiiAssembly;

            var name = member.DeclaringType.Assembly.FullName;
            var ma = loadedAssemblies.Find(x => x.Assembly.FullName == name);
            return ma;
        }

        /// <summary>
        /// Loads or finds a MonkiiAssembly from path.
        /// </summary>
        /// <param name="path">Path of the MonkiiAssembly</param>
        /// <param name="loadMonkiis">Sets whether Monkiis should be auto-loaded or not</param>
        public static MonkiiAssembly LoadMonkiiAssembly(string path, bool loadMonkiis = true)
        {
            if (path == null)
            {
                MonkiiLogger.Error("Failed to load a Monkii Assembly: Path cannot be null.");
                return null;
            }

            path = Path.GetFullPath(path);

            Assembly assembly;
            try
            {
                assembly = Assembly.LoadFrom(path);
            }
            catch (Exception ex)
            {
                MonkiiLogger.Error($"Failed to load Monkii Assembly from '{path}':\n{ex}");
                return null;
            }

            return LoadMonkiiAssembly(path, assembly, loadMonkiis);
        }

        /// <summary>
        /// Loads or finds a MonkiiAssembly from raw Assembly Data.
        /// </summary>
        public static MonkiiAssembly LoadRawMonkiiAssembly(string path, byte[] assemblyData, byte[] symbolsData = null, bool loadMonkiis = true)
        {
            if (assemblyData == null)
                MonkiiLogger.Error("Failed to load a Monkii Assembly: assemblyData cannot be null.");

            Assembly assembly;
            try
            {
                assembly = symbolsData != null ? Assembly.Load(assemblyData, symbolsData) : Assembly.Load(assemblyData);
            }
            catch (Exception ex)
            {
                MonkiiLogger.Error($"Failed to load Monkii Assembly from raw Assembly Data (length {assemblyData.Length}):\n{ex}");
                return null;
            }

            return LoadMonkiiAssembly(path, assembly, loadMonkiis);
        }

        /// <summary>
        /// Loads or finds a MonkiiAssembly.
        /// </summary>
        public static MonkiiAssembly LoadMonkiiAssembly(string path, Assembly assembly, bool loadMonkiis = true)
        {
            if (!File.Exists(path))
                path = assembly.Location;

            if (assembly == null)
            {
                MonkiiLogger.Error("Failed to load a Monkii Assembly: Assembly cannot be null.");
                return null;
            }

            var ma = loadedAssemblies.Find(x => x.Assembly.FullName == assembly.FullName);
            if (ma != null)
                return ma;

            var shortPath = path;
            if (shortPath.StartsWith(MonkiiUtils.BaseDirectory))
                shortPath = "." + shortPath.Remove(0, MonkiiUtils.BaseDirectory.Length);

            OnAssemblyResolving.Invoke(assembly);
            ma = new MonkiiAssembly(assembly, path);
            loadedAssemblies.Add(ma);

            if (loadMonkiis)
                ma.LoadMonkiis();
            
            MonkiiLogger.Msg(ConsoleColor.DarkGray, $"Monkii Assembly loaded: '{shortPath}'");
            MonkiiLogger.Msg(ConsoleColor.DarkGray, $"SHA256 Hash: '{ma.Hash}'");
            return ma;
        }

        #endregion

        #region Instance
        
        private bool MonkiisLoaded;

        private readonly List<MonkiiBase> loadedMonkiis = new();
        private readonly List<RottenMonkii> rottenMonkiis = new();

        public readonly MonkiiEvent OnUnregister = new();

        public bool HarmonyDontPatchAll { get; private set; } = true;

        /// <summary>
        /// A SHA256 Hash of the Assembly.
        /// </summary>
        public string Hash { get; private set; }

        public Assembly Assembly { get; private set; }

        public string Location { get; private set; }

        /// <summary>
        /// A list of all loaded Monkiis in the Assembly.
        /// </summary>
        public ReadOnlyCollection<MonkiiBase> LoadedMonkiis => loadedMonkiis.AsReadOnly();

        /// <summary>
        /// A list of all broken Monkiis in the Assembly.
        /// </summary>
        public ReadOnlyCollection<RottenMonkii> RottenMonkiis => rottenMonkiis.AsReadOnly();

        private MonkiiAssembly(Assembly assembly, string location)
        {
            Assembly = assembly;
            Location = location ?? ""; 
            Hash = MonkiiUtils.ComputeSimpleSHA256Hash(Location);
        }

        /// <summary>
        /// Unregisters all Monkiis in this Assembly.
        /// </summary>
        public void UnregisterMonkiis(string reason = null, bool silent = false)
        {
            foreach (var m in loadedMonkiis)
                m.UnregisterInstance(reason, silent);

            OnUnregister.Invoke();
        }

        private void OnApplicationQuit()
        {
            UnregisterMonkiis("MonkiiLoader is deinitializing.", true);
        }

        public void LoadMonkiis()
        {
            if (MonkiisLoaded)
                return;

            MonkiisLoaded = true;

            MonkiiEvents.OnApplicationDefiniteQuit.Subscribe(OnApplicationQuit);

            // \/ Custom Resolver \/
            var resolvers = CustomMonkiiResolvers?.GetInvocationList();
            if (resolvers != null)
                foreach (LemonFunc<Assembly, ResolvedMonkiis> r in resolvers)
                {
                    var customMonkii = r.Invoke(Assembly);

                    loadedMonkiis.AddRange(customMonkii.loadedMonkiis);
                    rottenMonkiis.AddRange(customMonkii.rottenMonkiis);
                }

            
            // \/ Default resolver \/
            var info = MonkiiUtils.PullAttributeFromAssembly<MonkiiInfoAttribute>(Assembly);
            if (info != null && info.SystemType != null && info.SystemType.IsSubclassOf(typeof(MonkiiBase)))
            {
                MonkiiBase Monkii;
                try
                {
                    Monkii = (MonkiiBase)Activator.CreateInstance(info.SystemType, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, null, null);
                }
                catch (Exception ex)
                {
                    Monkii = null;
                    rottenMonkiis.Add(new RottenMonkii(info.SystemType, "Failed to create an instance of the Monkii.", ex));
                }

                if (Monkii != null)
                {
                    var priorityAttr = MonkiiUtils.PullAttributeFromAssembly<MonkiiPriorityAttribute>(Assembly);
                    var colorAttr = MonkiiUtils.PullAttributeFromAssembly<MonkiiColorAttribute>(Assembly);
                    var authorColorAttr = MonkiiUtils.PullAttributeFromAssembly<MonkiiAuthorColorAttribute>(Assembly);
                    var procAttrs = MonkiiUtils.PullAttributesFromAssembly<MonkiiProcessAttribute>(Assembly);
                    var gameAttrs = MonkiiUtils.PullAttributesFromAssembly<MonkiiGameAttribute>(Assembly);
                    var optionalDependenciesAttr = MonkiiUtils.PullAttributeFromAssembly<MonkiiOptionalDependenciesAttribute>(Assembly);
                    var idAttr = MonkiiUtils.PullAttributeFromAssembly<MonkiiIDAttribute>(Assembly);
                    var gameVersionAttrs = MonkiiUtils.PullAttributesFromAssembly<MonkiiGameVersionAttribute>(Assembly);
                    var platformAttr = MonkiiUtils.PullAttributeFromAssembly<MonkiiPlatformAttribute>(Assembly);
                    var domainAttr = MonkiiUtils.PullAttributeFromAssembly<MonkiiPlatformDomainAttribute>(Assembly);
                    var mlVersionAttr = MonkiiUtils.PullAttributeFromAssembly<VerifyLoaderVersionAttribute>(Assembly);
                    var mlBuildAttr = MonkiiUtils.PullAttributeFromAssembly<VerifyLoaderBuildAttribute>(Assembly);
                    var harmonyDPAAttr = MonkiiUtils.PullAttributeFromAssembly<HarmonyDontPatchAllAttribute>(Assembly);

                    Monkii.Info = info;
                    Monkii.MonkiiAssembly = this;
                    Monkii.Priority = priorityAttr == null ? 0 : priorityAttr.Priority;
                    Monkii.ConsoleColor = colorAttr == null ? MonkiiLogger.DefaultMonkiiColor : colorAttr.Color;
                    Monkii.AuthorConsoleColor = authorColorAttr == null ? MonkiiLogger.DefaultTextColor : authorColorAttr.Color;
                    Monkii.SupportedProcesses = procAttrs;
                    Monkii.Games = gameAttrs;
                    Monkii.SupportedGameVersions = gameVersionAttrs;
                    Monkii.SupportedPlatforms = platformAttr;
                    Monkii.SupportedDomain = domainAttr;
                    Monkii.SupportedMLVersion = mlVersionAttr;
                    Monkii.SupportedMLBuild = mlBuildAttr;
                    Monkii.OptionalDependencies = optionalDependenciesAttr;
                    Monkii.ID = idAttr?.ID;
                    HarmonyDontPatchAll = harmonyDPAAttr != null;

                    loadedMonkiis.Add(Monkii);

                    if (!SemVersion.TryParse(info.Version, out _))
                        MonkiiLogger.Warning($"==Normal users can ignore this warning==\nMonkii '{info.Name}' by '{info.Author}' has version '{info.Version}' which does not use the Semantic Versioning format. Versions using formats other than the Semantic Versioning format will not be supported in the future versions of MonkiiLoader.\nFor more details, see: https://semver.org");
                }
            }

            RegisterTypeInIl2Cpp.RegisterAssembly(Assembly);
            
            if (rottenMonkiis.Count != 0)
            {
                MonkiiLogger.Error($"Failed to load {rottenMonkiis.Count} {"Monkii".MakePlural(rottenMonkiis.Count)} from {Path.GetFileName(Location)}:");
                foreach (var r in rottenMonkiis)
                {
                    MonkiiLogger.Error($"Failed to load Monkii '{r.type.FullName}': {r.errorMessage}");
                    if (r.exception != null)
                        MonkiiLogger.Error(r.exception);
                }
            }
        }

        #endregion
    }
}
