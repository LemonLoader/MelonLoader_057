using System;
using System.Runtime.CompilerServices;

namespace MonkiiLoader
{
    public static class MonkiiDebug
    {
        public static void Msg(object obj)
        {
            if (!IsEnabled())
                return;
            MonkiiLogger.Internal_Msg(ConsoleColor.Blue, MonkiiLogger.DefaultTextColor, "DEBUG", obj.ToString());
            MsgCallbackHandler?.Invoke(MonkiiLogger.DefaultTextColor, obj.ToString());
        }
        public static void Msg(string txt)
        {
            if (!IsEnabled())
                return;
            MonkiiLogger.Internal_Msg(ConsoleColor.Blue, MonkiiLogger.DefaultTextColor, "DEBUG", txt);
            MsgCallbackHandler?.Invoke(MonkiiLogger.DefaultTextColor, txt);
        }
        public static void Msg(string txt, params object[] args)
        {
            if (!IsEnabled())
                return;
            MonkiiLogger.Internal_Msg(ConsoleColor.Blue, MonkiiLogger.DefaultTextColor, "DEBUG", string.Format(txt, args));
            MsgCallbackHandler?.Invoke(MonkiiLogger.DefaultTextColor, string.Format(txt, args));
        }

        public static void Error(string txt)
        {
            if (!IsEnabled())
                return;
            MonkiiLogger.Internal_Error("DEBUG", txt);
            ErrorCallbackHandler?.Invoke(txt);
        }

        public static event Action<ConsoleColor, string> MsgCallbackHandler;
        public static event Action<string> ErrorCallbackHandler;
        //public static bool IsEnabled() => MonkiiLaunchOptions.Core.DebugMode;

#if !__ANDROID__
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static bool IsEnabled();
#else
        public static bool IsEnabled() => true;
#endif
    }
}