using System;
using System.Runtime.CompilerServices;

namespace MonkiiLoader
{
    public class MonkiiLogger
    {
        public static readonly ConsoleColor DefaultMonkiiColor = ConsoleColor.Cyan;
        public static readonly ConsoleColor DefaultTextColor = ConsoleColor.Gray;

        public static void Msg(object obj) => NativeMsg(DefaultMonkiiColor, DefaultTextColor, null, obj.ToString());
        public static void Msg(string txt) => NativeMsg(DefaultMonkiiColor, DefaultTextColor, null, txt);
        public static void Msg(string txt, params object[] args) => NativeMsg(DefaultMonkiiColor, DefaultTextColor, null, string.Format(txt, args));

        public static void Msg(ConsoleColor txt_color, object obj) => NativeMsg(DefaultMonkiiColor, txt_color, null, obj.ToString());
        public static void Msg(ConsoleColor txt_color, string txt) => NativeMsg(DefaultMonkiiColor, txt_color, null, txt);
        public static void Msg(ConsoleColor txt_color, string txt, params object[] args) => NativeMsg(DefaultMonkiiColor, txt_color, null, string.Format(txt, args));


        public static void Warning(object obj) => NativeWarning(null, obj.ToString());
        public static void Warning(string txt) => NativeWarning(null, txt);
        public static void Warning(string txt, params object[] args) => NativeWarning(null, string.Format(txt, args));


        public static void Error(object obj) => NativeError(null, obj.ToString());
        public static void Error(string txt) => NativeError(null, txt);
        public static void Error(string txt, params object[] args) => NativeError(null, string.Format(txt, args));
        public static void Error(string txt, Exception ex) => NativeError(null, $"{txt}\n{ex}");

        public static void WriteLine(int length = 30) => Msg(new string('-', length));
        public static void WriteLine(ConsoleColor color, int length = 30) => Msg(color, new string('-', length));

        private static void NativeMsg(ConsoleColor namesection_color, ConsoleColor txt_color, string namesection, string txt)
        {
            if (string.IsNullOrEmpty(namesection))
            {
                MonkiiBase Monkii = MonkiiUtils.GetMonkiiFromStackTrace();
                if (Monkii != null)
                {
                    namesection = Monkii.Info?.Name?.Replace(" ", "_");
                    namesection_color = Monkii.ConsoleColor;
                }
            }

            Internal_Msg(namesection_color, txt_color, namesection, txt ?? "null");
            RunMsgCallbacks(namesection_color, txt_color, namesection, txt ?? "null");
        }

        private static void NativeWarning(string namesection, string txt)
        {
            namesection ??= MonkiiUtils.GetMonkiiFromStackTrace()?.Info?.Name?.Replace(" ", "_");

            Internal_Warning(namesection, txt ?? "null");
            RunWarningCallbacks(namesection, txt ?? "null");
        }

        private static void NativeError(string namesection, string txt)
        {
            namesection ??= MonkiiUtils.GetMonkiiFromStackTrace()?.Info?.Name?.Replace(" ", "_");

            Internal_Error(namesection, txt ?? "null");
            RunErrorCallbacks(namesection, txt ?? "null");
        }

        internal static void RunMsgCallbacks(ConsoleColor namesection_color, ConsoleColor txt_color, string namesection, string txt) => MsgCallbackHandler?.Invoke(namesection_color, txt_color, namesection, txt);
        public static event Action<ConsoleColor, ConsoleColor, string, string> MsgCallbackHandler;
        internal static void RunWarningCallbacks(string namesection, string txt) => WarningCallbackHandler?.Invoke(namesection, txt);
        public static event Action<string, string> WarningCallbackHandler;
        internal static void RunErrorCallbacks(string namesection, string txt) => ErrorCallbackHandler?.Invoke(namesection, txt);
        public static event Action<string, string> ErrorCallbackHandler;

        public class Instance
        {
            private string Name = null;
            private ConsoleColor Color = DefaultMonkiiColor;
            public Instance(string name) => Name = name?.Replace(" ", "_");
            public Instance(string name, ConsoleColor color) : this(name) => Color = color;

            public void Msg(object obj) => NativeMsg(Color, DefaultTextColor, Name, obj.ToString());
            public void Msg(string txt) => NativeMsg(Color, DefaultTextColor, Name, txt);
            public void Msg(string txt, params object[] args) => NativeMsg(Color, DefaultTextColor, Name, string.Format(txt, args));

            public void Msg(ConsoleColor txt_color, object obj) => NativeMsg(Color, txt_color, Name, obj.ToString());
            public void Msg(ConsoleColor txt_color, string txt) => NativeMsg(Color, txt_color, Name, txt);
            public void Msg(ConsoleColor txt_color, string txt, params object[] args) => NativeMsg(Color, txt_color, Name, string.Format(txt, args));

            public void Warning(object obj) => NativeWarning(Name, obj.ToString());
            public void Warning(string txt) => NativeWarning(Name, txt);
            public void Warning(string txt, params object[] args) => NativeWarning(Name, string.Format(txt, args));

            public void Error(object obj) => NativeError(Name, obj.ToString());
            public void Error(string txt) => NativeError(Name, txt);
            public void Error(string txt, params object[] args) => NativeError(Name, string.Format(txt, args));
            public void Error(string txt, Exception ex) => NativeError(Name, $"{txt}\n{ex}");

            public void WriteSpacer() => MonkiiLogger.WriteSpacer();
            public void WriteLine(int length = 30) => MonkiiLogger.WriteLine(length);
            public void WriteLine(ConsoleColor color, int length = 30) => MonkiiLogger.WriteLine(color, length);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern static void Internal_Msg(ConsoleColor namesection_color, ConsoleColor txt_color, string namesection, string txt);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern static void Internal_Warning(string namesection, string txt);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern static void Internal_Error(string namesection, string txt);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern static void ThrowInternalFailure(string txt);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern static void WriteSpacer();
#if !__ANDROID__
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern static void Internal_PrintModName(ConsoleColor Monkiicolor, ConsoleColor authorcolor, string name, string author, string version, string id);
#else
        internal static void Internal_PrintModName(ConsoleColor Monkiicolor, ConsoleColor authorcolor, string name, string author, string version, string id)
        {
            // colors are ignored since they wouldn't show through logs or logcat
            string log = name + " v" + version;
            if (id != null)
                log += " (" + id + ")";

            Msg(log);

            if (author != null)
                Msg("by " + author);
        }
#endif
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern static void Flush();

        [Obsolete("Log is obsolete. Please use Msg instead.")]
        public static void Log(string txt) => Msg(txt);
        [Obsolete("Log is obsolete. Please use Msg instead.")]
        public static void Log(string txt, params object[] args) => Msg(txt, args);
        [Obsolete("Log is obsolete. Please use Msg instead.")]
        public static void Log(object obj) => Msg(obj);
        [Obsolete("Log is obsolete. Please use Msg instead.")]
        public static void Log(ConsoleColor color, string txt) => Msg(color, txt);
        [Obsolete("Log is obsolete. Please use Msg instead.")]
        public static void Log(ConsoleColor color, string txt, params object[] args) => Msg(color, txt, args);
        [Obsolete("Log is obsolete. Please use Msg instead.")]
        public static void Log(ConsoleColor color, object obj) => Msg(color, obj);
        [Obsolete("LogWarning is obsolete. Please use Warning instead.")]
        public static void LogWarning(string txt) => Warning(txt);
        [Obsolete("LogWarning is obsolete. Please use Warning instead.")]
        public static void LogWarning(string txt, params object[] args) => Warning(txt, args);
        [Obsolete("LogError is obsolete. Please use Error instead.")]
        public static void LogError(string txt) => Error(txt);
        [Obsolete("LogError is obsolete. Please use Error instead.")]
        public static void LogError(string txt, params object[] args) => Error(txt, args);
    }
}
