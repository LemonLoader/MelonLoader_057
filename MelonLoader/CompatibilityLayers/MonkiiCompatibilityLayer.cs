using MonkiiLoader.Modules;
using System;
using System.Collections.Generic;
using System.IO;

namespace MonkiiLoader
{
    public static class MonkiiCompatibilityLayer
    {
#if __ANDROID__
        public static string baseDirectory = $"Monkiiloader/etc/compatibilitylayers";
#else
        public static string baseDirectory = $"MonkiiLoader{Path.DirectorySeparatorChar}Dependencies{Path.DirectorySeparatorChar}CompatibilityLayers";
#endif

        private static List<MonkiiModule.Info> layers = new List<MonkiiModule.Info>()
        {
            // Il2Cpp Unity Tls
            new MonkiiModule.Info(Path.Combine(baseDirectory, "Il2CppUnityTls.dll"), () => !MonkiiUtils.IsGameIl2Cpp()),

            // Illusion Plugin Architecture
            new MonkiiModule.Info(Path.Combine(baseDirectory, "IPA.dll"), MonkiiUtils.IsGameIl2Cpp),
        };
        
        private static void CheckGameLayerWithPlatform(string name, Func<bool> shouldBeIgnored)
        {
            string nameNoSpaces = name.Replace(' ', '_');
            foreach (var file in Directory.GetFiles(baseDirectory))
            {
                string fileName = Path.GetFileNameWithoutExtension(file);
                if (fileName.StartsWith(nameNoSpaces))
                    layers.Add(new MonkiiModule.Info(file, shouldBeIgnored));
            }
        }

        private static void CheckGameLayer(string name)
        {
            CheckGameLayerWithPlatform(name, () => false);
            CheckGameLayerWithPlatform($"{name}_Mono", () => MonkiiUtils.IsGameIl2Cpp());
            CheckGameLayerWithPlatform($"{name}_Il2Cpp", () => !MonkiiUtils.IsGameIl2Cpp());
        }

        internal static void LoadModules()
        {
            if (!Directory.Exists(baseDirectory))
                return;

            CheckGameLayer(InternalUtils.UnityInformationHandler.GameName);
            CheckGameLayer(InternalUtils.UnityInformationHandler.GameDeveloper);
            CheckGameLayer($"{InternalUtils.UnityInformationHandler.GameDeveloper}_{InternalUtils.UnityInformationHandler.GameName}");

            foreach (var m in layers)
                MonkiiModule.Load(m);

            foreach (var file in Directory.GetFiles(baseDirectory))
            {
                string fileName = Path.GetFileName(file);
                if (layers.Find(x => Path.GetFileName(x.fullPath).Equals(fileName)) == null)
                {
                    File.Delete(file);
                }
            }
        }
    }
}