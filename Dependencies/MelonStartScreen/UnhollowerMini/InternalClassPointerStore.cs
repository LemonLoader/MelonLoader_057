using MonkiiLoader;
using System;
using System.Runtime.CompilerServices;
#pragma warning disable 0649

namespace UnhollowerMini
{
    internal static class InternalClassPointerStore<T>
    {
        public static IntPtr NativeClassPtr;
        public static Type CreatedTypeRedirect;

        static InternalClassPointerStore()
        {
            var targetType = typeof(T);

            RuntimeHelpers.RunClassConstructor(targetType.TypeHandle);

            if (targetType.IsPrimitive || targetType == typeof(string))
            {
                MonkiiDebug.Msg("Running class constructor on Il2Cpp" + targetType.FullName);
                RuntimeHelpers.RunClassConstructor(typeof(InternalClassPointerStore<>).Assembly.GetType("Il2Cpp" + targetType.FullName).TypeHandle);
                MonkiiDebug.Msg("Done running class constructor");
            }
        }
    }
}
