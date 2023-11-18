using System;
using System.Runtime.CompilerServices;

namespace MonkiiLoader.Fixes
{
	internal static class DateTimeOverride
	{
		private static TimeZoneInfo localTimeZone;

		internal static void Install()
		{
			string ltzId = GetLocalTimeZone();
			MonkiiDebug.Msg("Local Time Zone ID = " + ltzId);
			localTimeZone = TimeZoneInfo.FindSystemTimeZoneById(ltzId);
			Core.HarmonyInstance.Patch(typeof(TimeZoneInfo).GetProperty("Local").GetGetMethod(), typeof(DateTimeOverride).GetMethod(nameof(TimeZoneInfoLocal)).ToNewHarmonyMethod());

			// Overrides the modified DateTime.Now so that it actually works
			Core.HarmonyInstance.Patch(typeof(DateTime).GetProperty("Now").GetGetMethod(), typeof(DateTimeOverride).GetMethod(nameof(DateTimeNow)).ToNewHarmonyMethod());
		}

		public static bool DateTimeNow(ref DateTime __result)
		{
			__result = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.Local);
			return false;
        }

		public static bool TimeZoneInfoLocal(ref TimeZoneInfo __result)
		{
			__result = localTimeZone;
			return false;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern static string GetLocalTimeZone();
	}
}