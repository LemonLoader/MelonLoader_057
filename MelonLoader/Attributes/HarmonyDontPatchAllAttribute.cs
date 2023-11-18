using System;

namespace MonkiiLoader
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class HarmonyDontPatchAllAttribute : Attribute { public HarmonyDontPatchAllAttribute() { } }
}