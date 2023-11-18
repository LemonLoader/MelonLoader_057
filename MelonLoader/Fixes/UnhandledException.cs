using System;

namespace MonkiiLoader.Fixes
{
	internal static class UnhandledException
	{
		internal static void Install(AppDomain domain) =>
			domain.UnhandledException +=
				(sender, args) =>
					MonkiiLogger.Error((args.ExceptionObject as Exception).ToString());
	}
}