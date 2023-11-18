using System;

namespace MonkiiLoader
{
    [Obsolete("MonkiiLoader.MonkiiLoaderBase is Only Here for Compatibility Reasons.")]
    public static class MonkiiLoaderBase
    {
        [Obsolete("MonkiiLoader.MonkiiLoaderBase.UserDataPath is Only Here for Compatibility Reasons. Please use MonkiiLoader.MonkiiUtils.GetUserDataDirectory instead.")]
        public static string UserDataPath { get => MonkiiUtils.UserDataDirectory; }
        [Obsolete("MonkiiLoader.MonkiiLoaderBase.UnityVersion is Only Here for Compatibility Reasons. Please use MonkiiLoader.InternalUtils.UnityInformationHandler.EngineVersion instead.")]
        public static string UnityVersion { get => InternalUtils.UnityInformationHandler.EngineVersion.ToStringWithoutType(); }
    }
}