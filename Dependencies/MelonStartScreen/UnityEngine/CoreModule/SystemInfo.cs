using MonkiiLoader;
using MonkiiLoader.MonkiiStartScreen.NativeUtils;
using UnhollowerMini;

namespace MonkiiUnityEngine.CoreModule
{
    internal sealed class SystemInfo
    {
        private delegate uint d_GetGraphicsDeviceType();
        private static readonly d_GetGraphicsDeviceType m_GetGraphicsDeviceType;

        unsafe static SystemInfo()
        {
            if (NativeSignatureResolver.IsUnityVersionOverOrEqual(MonkiiLoader.InternalUtils.UnityInformationHandler.EngineVersion.ToStringWithoutType(), new string[] { "2018.1.0" }))
                m_GetGraphicsDeviceType = UnityInternals.ResolveICall<d_GetGraphicsDeviceType>("UnityEngine.SystemInfo::GetGraphicsDeviceType");
            else
                m_GetGraphicsDeviceType = UnityInternals.ResolveICall<d_GetGraphicsDeviceType>("UnityEngine.SystemInfo::get_graphicsDeviceType");
        }

        public static uint GetGraphicsDeviceType() =>
            m_GetGraphicsDeviceType();
    }
}
