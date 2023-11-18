using System;

namespace MonkiiLoader
{
    [Obsolete("MonkiiLoader.MonkiiConsole is Only Here for Compatibility Reasons.")]
    public class MonkiiConsole
    {
        [Obsolete("MonkiiLoader.MonkiiConsole.SetTitle is Only Here for Compatibility Reasons. Please use MonkiiLoader.MonkiiUtils.SetConsoleTitle instead.")]
        public static void SetTitle(string title) => MonkiiUtils.SetConsoleTitle(title);
    }
}