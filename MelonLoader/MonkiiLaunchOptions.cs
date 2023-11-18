using System;
using System.Collections.Generic;

namespace MonkiiLoader
{
    public static class MonkiiLaunchOptions
    {
        private static Dictionary<string, Action> WithoutArg = new Dictionary<string, Action>();
        private static Dictionary<string, Action<string>> WithArg = new Dictionary<string, Action<string>>();

        static MonkiiLaunchOptions()
        {
            AnalyticsBlocker.Setup();
            Core.Setup();
            Console.Setup();
            Debug.Setup();
            Il2CppAssemblyGenerator.Setup();
            Logger.Setup();
        }

        internal static void Load()
        {
            List<string> foundOptions = new List<string>();

            LemonEnumerator<string> argEnumerator = new LemonEnumerator<string>(Environment.GetCommandLineArgs());
            while (argEnumerator.MoveNext())
            {
                string fullcmd = argEnumerator.Current;
                if (string.IsNullOrEmpty(fullcmd))
                    continue;

                if (!fullcmd.StartsWith("--"))
                    continue;

                string cmd = fullcmd.Remove(0, 2);

                if (WithoutArg.TryGetValue(cmd, out Action withoutArgFunc))
                {
                    foundOptions.Add(fullcmd);
                    withoutArgFunc();
                }
                else if (WithArg.TryGetValue(cmd, out Action<string> withArgFunc))
                {
                    if (!argEnumerator.MoveNext())
                        continue;

                    string cmdArg = argEnumerator.Current;
                    if (string.IsNullOrEmpty(cmdArg))
                        continue;

                    if (cmdArg.StartsWith("--"))
                        continue;

                    foundOptions.Add($"{fullcmd} = {cmdArg}");
                    withArgFunc(cmdArg);
                }
            }

            if (foundOptions.Count <= 0)
                return;

            MonkiiLogger.WriteLine(ConsoleColor.Magenta);
            MonkiiLogger.Msg(ConsoleColor.Cyan, "Launch Options:");
            foreach (string cmd in foundOptions)
                MonkiiLogger.Msg($"\t{cmd}");
            MonkiiLogger.WriteLine(ConsoleColor.Magenta);
            MonkiiLogger.WriteSpacer();
        }

#region Args
        public static class AnalyticsBlocker
        {
            public static bool ShouldDAB { get; internal set; }

            internal static void Setup()
            {
                WithoutArg["Monkiiloader.dab"] = () => ShouldDAB = true;

            }
        }

        public static class Core
        {
            public enum LoadModeEnum
            {
                NORMAL,
                DEV,
                BOTH
            }
            public static LoadModeEnum LoadMode_Plugins { get; internal set; }
            public static LoadModeEnum LoadMode_Mods { get; internal set; }
            public static bool QuitFix { get; internal set; }
            public static bool StartScreen { get; internal set; } = true;
            public static string UnityVersion { get; internal set; }

            internal static void Setup()
            {
                WithoutArg["quitfix"] = () => QuitFix = true;
                WithoutArg["Monkiiloader.disablestartscreen"] = () => StartScreen = false;
                WithArg["Monkiiloader.loadmodeplugins"] = (string arg) =>
                {
                    if (int.TryParse(arg, out int valueint))
                        LoadMode_Plugins = (LoadModeEnum)MonkiiUtils.Clamp(valueint, (int)LoadModeEnum.NORMAL, (int)LoadModeEnum.BOTH);
                };
                WithArg["Monkiiloader.loadmodemods"] = (string arg) =>
                {
                    if (int.TryParse(arg, out int valueint))
                        LoadMode_Mods = (LoadModeEnum)MonkiiUtils.Clamp(valueint, (int)LoadModeEnum.NORMAL, (int)LoadModeEnum.BOTH);
                };
                WithArg["Monkiiloader.unityversion"] = (string arg) => UnityVersion = arg;
            }
        }

        public static class Console
        {
            public enum DisplayMode
            {
                NORMAL,
                MAGENTA,
                RAINBOW,
                RANDOMRAINBOW,
                LEMON
            };
            public static DisplayMode Mode { get; internal set; }
            public static bool CleanUnityLogs { get; internal set; } = true;
            public static bool ShouldSetTitle { get; internal set; } = true;
            public static bool AlwaysOnTop { get; internal set; }
            public static bool ShouldHide { get; internal set; }
            public static bool HideWarnings { get; internal set; }

            internal static void Setup()
            {
                WithoutArg["Monkiiloader.disableunityclc"] = () => CleanUnityLogs = false;
                WithoutArg["Monkiiloader.consoledst"] = () => ShouldSetTitle = false;
                WithoutArg["Monkiiloader.consoleontop"] = () => AlwaysOnTop = true;
                WithoutArg["Monkiiloader.hideconsole"] = () => ShouldHide = true;
                WithoutArg["Monkiiloader.hidewarnings"] = () => HideWarnings = true;

                WithArg["Monkiiloader.consolemode"] = (string arg) =>
                {
                    if (int.TryParse(arg, out int valueint))
                        Mode = (DisplayMode)MonkiiUtils.Clamp(valueint, (int)DisplayMode.NORMAL, (int)DisplayMode.LEMON);
                };
            }
        }

        public static class Debug
        {
            public static bool Enabled { get; internal set; }

            internal static void Setup()
            {
                WithoutArg["Monkiiloader.debug"] = () => Enabled = true;

            }
        }

        public static class Il2CppAssemblyGenerator
        {
            public static bool ForceRegeneration { get; internal set; }
            public static bool OfflineMode { get; internal set; }
            public static string ForceVersion_Dumper { get; internal set; }
            public static string ForceVersion_Il2CppAssemblyUnhollower { get; internal set; }
            public static string ForceRegex { get; internal set; }

            internal static void Setup()
            {
                WithoutArg["Monkiiloader.agfoffline"] = () => OfflineMode = true;
                WithoutArg["Monkiiloader.agfregenerate"] = () => ForceRegeneration = true;
                WithArg["Monkiiloader.agfvdumper"] = (string arg) => ForceVersion_Dumper = arg;
                WithArg["Monkiiloader.agfvunhollower"] = (string arg) => ForceVersion_Il2CppAssemblyUnhollower = arg;
                WithArg["Monkiiloader.agfregex"] = (string arg) => ForceRegex = arg;
            }
        }

        public static class Logger
        {
            public static int MaxLogs { get; internal set; } = 10;
            public static int MaxWarnings { get; internal set; } = 10;
            public static int MaxErrors { get; internal set; } = 10;

            internal static void Setup()
            {
                WithArg["Monkiiloader.maxlogs"] = (string arg) =>
                {
                    if (int.TryParse(arg, out int valueint))
                        MaxLogs = valueint;
                };
                WithArg["Monkiiloader.maxwarnings"] = (string arg) =>
                {
                    if (int.TryParse(arg, out int valueint))
                        MaxWarnings = valueint;
                };
                WithArg["Monkiiloader.maxerrors"] = (string arg) =>
                {
                    if (int.TryParse(arg, out int valueint))
                        MaxErrors = valueint;
                };
            }
        }
        #endregion
    }
}