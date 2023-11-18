using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace MonkiiLoader
{
    internal static class SupportModule
    {
        internal static ISupportModule_To Interface = null;

        private static string BaseDirectory = null;
        private static List<ModuleListing> Modules = new List<ModuleListing>()
        {
            new ModuleListing("Il2Cpp.dll", MonkiiUtils.IsGameIl2Cpp),
            new ModuleListing("Mono.dll", () => !MonkiiUtils.IsGameIl2Cpp())
        };

        internal static bool Setup()
        {
#if __ANDROID__
            BaseDirectory = Path.Combine(Path.Combine(Path.Combine(MonkiiUtils.GameDirectory, "Monkiiloader"), "etc"), "support");
#else
            BaseDirectory = Path.Combine(Path.Combine(Path.Combine(MonkiiUtils.GameDirectory, "MonkiiLoader"), "Dependencies"), "SupportModules");
#endif
            if (!Directory.Exists(BaseDirectory))
            {
                MonkiiLogger.Error("Failed to Find SupportModules Directory!");
                return false;
            }

            LemonEnumerator<ModuleListing> enumerator = new LemonEnumerator<ModuleListing>(Modules);
            while (enumerator.MoveNext())
            {
                string ModulePath = Path.Combine(BaseDirectory, enumerator.Current.FileName);
                if (!File.Exists(ModulePath))
                    continue;

                try
                {
                    if (enumerator.Current.LoadSpecifier != null)
                    {
                        if (!enumerator.Current.LoadSpecifier())
                        {
                            File.Delete(ModulePath);
                            continue;
                        }
                    }

                    if (Interface != null)
                        continue;

                    if (!LoadInterface(ModulePath))
                        continue;
                }
                catch (Exception ex)
                {
                    MonkiiDebug.Error($"Support Module [{enumerator.Current.FileName}] threw an Exception: {ex}");
                    continue;
                }
            }

            if (Interface == null)
            {
                MonkiiLogger.Error("No Support Module Loaded!");
                return false;
            }
            return true;
        }

        private static bool LoadInterface(string ModulePath)
        {
            Assembly assembly = Assembly.LoadFrom(ModulePath);
            if (assembly == null)
                return false;

            Type type = assembly.GetType("MonkiiLoader.Support.Main");
            if (type == null)
            {
                MonkiiLogger.Error("Failed to Get Type MonkiiLoader.Support.Main!");
                return false;
            }

            MethodInfo method = type.GetMethod("Initialize", BindingFlags.NonPublic | BindingFlags.Static);
            if (method == null)
            {
                MonkiiLogger.Error("Failed to Get Method Initialize!");
                return false;
            }

            Interface = (ISupportModule_To)method.Invoke(null, new object[] { new SupportModule_From() });
            if (Interface == null)
            {
                MonkiiLogger.Error("Failed to Initialize Interface!");
                return false;
            }

            MonkiiLogger.Msg($"Support Module Loaded: {ModulePath}");

            return true;
        }

        // Module Listing
        private class ModuleListing
        {
            internal string FileName = null;
            internal delegate bool dLoadSpecifier();
            internal dLoadSpecifier LoadSpecifier = null;
            internal ModuleListing(string filename)
                => FileName = filename;
            internal ModuleListing(string filename, dLoadSpecifier loadSpecifier)
            {
                FileName = filename;
                LoadSpecifier = loadSpecifier;
            }
        }
    }
}