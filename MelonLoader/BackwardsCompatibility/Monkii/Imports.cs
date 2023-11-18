using System;

namespace MonkiiLoader
{
    [Obsolete("MonkiiLoader.Imports is Only Here for Compatibility Reasons.")]
    public static class Imports
    {
        [Obsolete("MonkiiLoader.Imports.GetCompanyName is Only Here for Compatibility Reasons. Please use MonkiiLoader.InternalUtils.UnityInformationHandler.GameDeveloper instead.")]
        public static string GetCompanyName() => InternalUtils.UnityInformationHandler.GameDeveloper;
        [Obsolete("MonkiiLoader.Imports.GetProductName is Only Here for Compatibility Reasons. Please use MonkiiLoader.InternalUtils.UnityInformationHandler.GameName instead.")]
        public static string GetProductName() => InternalUtils.UnityInformationHandler.GameName;
        [Obsolete("MonkiiLoader.Imports.GetGameDirectory is Only Here for Compatibility Reasons. Please use MonkiiLoader.MonkiiUtils.GetGameDirectory instead.")]
        public static string GetGameDirectory() => MonkiiUtils.GameDirectory;
        [Obsolete("MonkiiLoader.Imports.GetGameDataDirectory is Only Here for Compatibility Reasons. Please use MonkiiLoader.MonkiiUtils.GetGameDataDirectory instead.")]
        public static string GetGameDataDirectory() => MonkiiUtils.GetGameDataDirectory();
        [Obsolete("MonkiiLoader.Imports.GetAssemblyDirectory is Only Here for Compatibility Reasons. Please use MonkiiLoader.MonkiiUtils.GetManagedDirectory instead.")]
        public static string GetAssemblyDirectory() => MonkiiUtils.GetManagedDirectory();
        [Obsolete("MonkiiLoader.Imports.IsIl2CppGame is Only Here for Compatibility Reasons. Please use MonkiiLoader.MonkiiUtils.IsGameIl2Cpp instead.")]
        public static bool IsIl2CppGame() => MonkiiUtils.IsGameIl2Cpp();
        [Obsolete("MonkiiLoader.Imports.IsDebugMode is Only Here for Compatibility Reasons. Please use MonkiiLoader.MonkiiDebug.IsEnabled instead.")]
        public static bool IsDebugMode() => MonkiiDebug.IsEnabled();
        [Obsolete("MonkiiLoader.Imports.Hook is Only Here for Compatibility Reasons. Please use MonkiiLoader.MonkiiUtils.NativeHookAttach instead.")]
        public static void Hook(IntPtr target, IntPtr detour) => MonkiiUtils.NativeHookAttach(target, detour);
        [Obsolete("MonkiiLoader.Imports.Unhook is Only Here for Compatibility Reasons. Please use MonkiiLoader.MonkiiUtils.NativeHookDetach instead.")]
        public static void Unhook(IntPtr target, IntPtr detour) => MonkiiUtils.NativeHookDetach(target, detour);
    }
}