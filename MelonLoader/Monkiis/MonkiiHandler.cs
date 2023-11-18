using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace MonkiiLoader
{
    public static class MonkiiHandler
    {
        /// <summary>
        /// Directory of Plugins.
        /// </summary>
        public static string PluginsDirectory { get; internal set; }

        /// <summary>
        /// Directory of Mods.
        /// </summary>
        public static string ModsDirectory { get; internal set; }

        internal static void Setup()
        {
            PluginsDirectory = Path.Combine(MonkiiUtils.BaseDirectory, "PLGNS");
            if (!Directory.Exists(PluginsDirectory))
                Directory.CreateDirectory(PluginsDirectory);
            ModsDirectory = Path.Combine(MonkiiUtils.BaseDirectory, "MDS");
            if (!Directory.Exists(ModsDirectory))
                Directory.CreateDirectory(ModsDirectory);
        }

        private static bool firstSpacer = false;
        public static void LoadMonkiisFromDirectory<T>(string path) where T : MonkiiTypeBase<T>
        {
#if !__ANDROID__
            path = Path.GetFullPath(path);
#else
            path = Path.Combine(MonkiiUtils.GetApplicationPath(), path);
#endif

            var loadingMsg = $"Loading {MonkiiTypeBase<T>.TypeName}s from '{path}'...";
            MonkiiLogger.WriteSpacer();
            MonkiiLogger.Msg(loadingMsg);

            bool hasWroteLine = false;

            var files = Directory.GetFiles(path, "*.dll");
            var MonkiiAssemblies = new List<MonkiiAssembly>();
            foreach (var f in files)
            {
                if (!hasWroteLine)
                {
                    hasWroteLine = true;
                    MonkiiLogger.WriteLine(ConsoleColor.Magenta);
                }

                var asm = MonkiiAssembly.LoadMonkiiAssembly(f, false);
                if (asm == null)
                    continue;

                MonkiiAssemblies.Add(asm);
            }

            var Monkiis = new List<T>();
            foreach (var asm in MonkiiAssemblies)
            {
                asm.LoadMonkiis();
                foreach (var m in asm.LoadedMonkiis)
                {
                    if (m is T t)
                    {
                        Monkiis.Add(t);
                    }
                    else
                    {
                        MonkiiLogger.Warning($"Failed to load Monkii '{m.Info.Name}' from '{path}': The given Monkii is a {m.MonkiiTypeName} and cannot be loaded as a {MonkiiTypeBase<T>.TypeName}. Make sure it's in the right folder.");
                        continue;
                    }
                }
            }

            if (hasWroteLine)
                MonkiiLogger.WriteSpacer();

            MonkiiBase.RegisterSorted(Monkiis);

            if (hasWroteLine)
                MonkiiLogger.WriteLine(ConsoleColor.Magenta);

            var count = MonkiiTypeBase<T>._registeredMonkiis.Count;
            MonkiiLogger.Msg($"{count} {MonkiiTypeBase<T>.TypeName.MakePlural(count)} loaded.");
            if (firstSpacer || (typeof(T) ==  typeof(MonkiiMod)))
                MonkiiLogger.WriteSpacer();
            firstSpacer = true;
        }

#region Obsolete Members
        /// <summary>
        /// List of Plugins.
        /// </summary>
        [Obsolete("Use 'MonkiiPlugin.RegisteredMonkiis' instead.")]
        public static List<MonkiiPlugin> Plugins => MonkiiTypeBase<MonkiiPlugin>.RegisteredMonkiis.ToList();

        /// <summary>
        /// List of Mods.
        /// </summary>
        [Obsolete("Use 'MonkiiMod.RegisteredMonkiis' instead.")]
        public static List<MonkiiMod> Mods => MonkiiTypeBase<MonkiiMod>.RegisteredMonkiis.ToList();

        [Obsolete("Use 'MonkiiBase.Load' and 'MonkiiBase.Register' instead.")]
        public static void LoadFromFile(string filelocation, bool is_plugin) => LoadFromFile(filelocation);

        [Obsolete("Use 'MonkiiBase.Load' and 'MonkiiBase.Register' instead.")]
        public static void LoadFromByteArray(byte[] filedata, string filelocation) => LoadFromByteArray(filedata, filepath: filelocation);

        [Obsolete("Use 'MonkiiBase.Load' and 'MonkiiBase.Register' instead.")]
        public static void LoadFromByteArray(byte[] filedata, string filelocation, bool is_plugin) => LoadFromByteArray(filedata, filepath: filelocation);

        [Obsolete("Use 'MonkiiBase.Load' and 'MonkiiBase.Register' instead.")]
        public static void LoadFromAssembly(Assembly asm, string filelocation, bool is_plugin) => LoadFromAssembly(asm, filelocation);

        [Obsolete("Use 'MonkiiBase.Hash' instead.")]
        public static string GetMonkiiHash(MonkiiBase MonkiiBase)
            => MonkiiBase.Hash;

        [Obsolete("Use 'MonkiiBase.RegisteredMonkiis.Exists(1)' instead.")]
        public static bool IsMonkiiAlreadyLoaded(string name)
            => MonkiiBase._registeredMonkiis.Exists(x => x.Info.Name == name);

        [Obsolete("Use 'MonkiiPlugin.RegisteredMonkiis.Exists(1)' instead.")]
        public static bool IsPluginAlreadyLoaded(string name)
            => MonkiiTypeBase<MonkiiPlugin>._registeredMonkiis.Exists(x => x.Info.Name == name);

        [Obsolete("Use 'MonkiiMod.RegisteredMonkiis.Exists(1)' instead.")]
        public static bool IsModAlreadyLoaded(string name)
            => MonkiiTypeBase<MonkiiMod>._registeredMonkiis.Exists(x => x.Info.Name == name);

        [Obsolete("Use 'MonkiiBase.Load' and 'MonkiiBase.Register' instead.")]
        public static void LoadFromFile(string filepath, string symbolspath = null)
        {
            var asm = MonkiiAssembly.LoadMonkiiAssembly(filepath);
            if (asm == null)
                return;

            MonkiiBase.RegisterSorted(asm.LoadedMonkiis);
        }

        [Obsolete("Use 'MonkiiBase.Load' and 'MonkiiBase.Register' instead.")]
        public static void LoadFromByteArray(byte[] filedata, byte[] symbolsdata = null, string filepath = null)
        {
            var asm = MonkiiAssembly.LoadRawMonkiiAssembly(filepath, filedata, symbolsdata);
            if (asm == null)
                return;

            MonkiiBase.RegisterSorted(asm.LoadedMonkiis);
        }

        [Obsolete("Use 'MonkiiBase.Load' and 'MonkiiBase.Register' instead.")]
        public static void LoadFromAssembly(Assembly asm, string filepath = null)
        {
            var ma = MonkiiAssembly.LoadMonkiiAssembly(filepath, asm);
            if (ma == null)
                return;

            MonkiiBase.RegisterSorted(ma.LoadedMonkiis);
        }
#endregion
    }
}
