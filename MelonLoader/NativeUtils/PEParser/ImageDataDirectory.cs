using System.Runtime.InteropServices;

namespace MonkiiLoader.NativeUtils.PEParser
{
    [StructLayout(LayoutKind.Explicit)]
    public struct ImageDataDirectory
    {
        [FieldOffset(0)]
        public uint virtualAddress;
        [FieldOffset(4)]
        public uint size;
    }
}
