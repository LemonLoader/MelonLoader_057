using System;
using System.Collections.Generic;

namespace MonkiiLoader
{
    [Obsolete("MonkiiLoader.Main is Only Here for Compatibility Reasons.")]
    public static class Main
    {
        [Obsolete("MonkiiLoader.Main.Mods is Only Here for Compatibility Reasons. Please use MonkiiLoader.MonkiiHandler.Mods instead.")]
        public static List<MonkiiMod> Mods = null;
        [Obsolete("MonkiiLoader.Main.Plugins is Only Here for Compatibility Reasons. Please use MonkiiLoader.MonkiiHandler.Plugins instead.")]
        public static List<MonkiiPlugin> Plugins = null;
        [Obsolete("MonkiiLoader.Main.IsBoneworks is Only Here for Compatibility Reasons. Please use MonkiiLoader.MonkiiUtils.IsBONEWORKS instead.")]
        public static bool IsBoneworks = false;
        [Obsolete("MonkiiLoader.Main.GetUnityVersion is Only Here for Compatibility Reasons. Please use  MonkiiLoader.InternalUtils.UnityInformationHandler.EngineVersion instead.")]
        public static string GetUnityVersion() => InternalUtils.UnityInformationHandler.EngineVersion.ToStringWithoutType();
        [Obsolete("MonkiiLoader.Main.GetUserDataPath is Only Here for Compatibility Reasons. Please use MonkiiLoader.MonkiiUtils.GetUserDataDirectory instead.")]
        public static string GetUserDataPath() => MonkiiUtils.UserDataDirectory;
    }
}