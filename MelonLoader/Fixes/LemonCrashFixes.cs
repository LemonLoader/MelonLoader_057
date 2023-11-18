using System.Net.Sockets;
using System.Threading;

namespace MonkiiLoader.Fixes
{
    internal static class LemonCrashFixes
    {
        internal static void Install()
        {
            Core.HarmonyInstance.Patch(typeof(SocketAsyncEventArgs).GetMethod("OnCompleted", HarmonyLib.AccessTools.all), typeof(LemonCrashFixes).GetMethod(nameof(DontRunMe)).ToNewHarmonyMethod());
            Core.HarmonyInstance.Patch(typeof(SynchronizationContext).GetMethod("GetThreadLocalContext", HarmonyLib.AccessTools.all), typeof(LemonCrashFixes).GetMethod(nameof(NoThreadContext)).ToNewHarmonyMethod());
        }

        public static bool DontRunMe() => false;

        public static bool NoThreadContext(ref SynchronizationContext __result)
        {
            __result = null;
            return false;
        }
    }
}
