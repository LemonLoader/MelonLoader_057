using System;
using System.Diagnostics;
using MonkiiLoader.InternalUtils;
using MonkiiLoader.MonoInternals;
using bHapticsLib;
using System.IO;
#pragma warning disable IDE0051 // Prevent the IDE from complaining about private unreferenced methods

namespace MonkiiLoader
{
	internal static class Core
    {
        internal static HarmonyLib.Harmony HarmonyInstance;

        private static int Initialize()
        {
            AppDomain curDomain = AppDomain.CurrentDomain;
            Fixes.UnhandledException.Install(curDomain);
            Fixes.ServerCertificateValidation.Install();

            MonkiiUtils.Setup(curDomain);
            Assertions.LemonAssertMapping.Setup();

            JNISharp.NativeInterface.JNI.Initialize(new JNISharp.NativeInterface.JavaVMInitArgs());

            // TODO: MonoLibrary stuff
#if !__ANDROID__
            if (!MonoLibrary.Setup()
                || !MonoResolveManager.Setup())
                return 1;
#else
            foreach (var file in Directory.GetFiles(MonkiiUtils.UserLibsDirectory, "*.dll"))
            {
                try
                {
                    System.Reflection.Assembly.LoadFrom(file);
                    MonkiiDebug.Msg("Loaded " + Path.GetFileName(file) + " from UserLibs!");
                }
                catch (Exception e)
                {
                    MonkiiLogger.Msg("Failed to load " + Path.GetFileName(file) + " from UserLibs!");
                    MonkiiLogger.Error(e.ToString());
                }
            }
#endif

            bool bypassHarmony = false;
            if (File.Exists(Path.Combine(MonkiiUtils.BaseDirectory, "isEmulator.txt")))
            {
                bypassHarmony = true;
                // Tells Harmony that it already did some internal patching junk so that I don't have to modify the code myself
                typeof(HarmonyLib.Traverse).Assembly.GetType("HarmonyLib.Internal.RuntimeFixes.StackTraceFixes").GetField("_applied", HarmonyLib.AccessTools.all).SetValue(null, true);
            }

            HarmonyInstance = new HarmonyLib.Harmony(BuildInfo.Name);

            if (!bypassHarmony)
            {
                Fixes.ForcedCultureInfo.Install();
                Fixes.InstancePatchFix.Install();
                Fixes.ProcessFix.Install();
#if __ANDROID__
                Fixes.DateTimeOverride.Install();
                Fixes.LemonCrashFixes.Install();
#endif
#if !__ANDROID__
                PatchShield.Install();
#endif
            }

            MonkiiPreferences.Load();

            MonkiiCompatibilityLayer.LoadModules();

            bHapticsManager.Connect(BuildInfo.Name, UnityInformationHandler.GameName);

            MonkiiHandler.LoadMonkiisFromDirectory<MonkiiPlugin>(MonkiiHandler.PluginsDirectory);
            MonkiiEvents.MonkiiHarmonyEarlyInit.Invoke();
            MonkiiEvents.OnPreInitialization.Invoke();

            return 0;
        }

        private static int PreStart()
        {
            MonkiiEvents.OnApplicationEarlyStart.Invoke();
#if !__ANDROID__
            return MonkiiStartScreen.LoadAndRun(Il2CppGameSetup);
#else
            return Il2CppGameSetup();
#endif
        }

        private static int Il2CppGameSetup()
            => Il2CppAssemblyGenerator.Run() ? 0 : 1;

        private static int Start()
        {
            MonkiiEvents.OnPreModsLoaded.Invoke();
            MonkiiHandler.LoadMonkiisFromDirectory<MonkiiMod>(MonkiiHandler.ModsDirectory);

            MonkiiEvents.OnPreSupportModule.Invoke();
            if (!SupportModule.Setup())
                return 1;

            AddUnityDebugLog();
            RegisterTypeInIl2Cpp.SetReady();

            MonkiiEvents.MonkiiHarmonyInit.Invoke();
            MonkiiEvents.OnApplicationStart.Invoke();

            return 0;
        }

        internal static void Quit()
        {
            MonkiiPreferences.Save();

            HarmonyInstance.UnpatchSelf();
            bHapticsManager.Disconnect();

            MonkiiLogger.Flush();

            if (MonkiiLaunchOptions.Core.QuitFix)
                Process.GetCurrentProcess().Kill();
        }

        private static void Pause()
        {
            MonkiiPreferences.Save();
        }

        private static void AddUnityDebugLog()
        {
            var msg = "~   This Game has been MODIFIED using MonkiiLoader. DO NOT report any issues to the Game Developers!   ~";
            var line = new string('-', msg.Length);
            SupportModule.Interface.UnityDebugLog(line);
            SupportModule.Interface.UnityDebugLog(msg);
            SupportModule.Interface.UnityDebugLog(line);
        }
    }
}