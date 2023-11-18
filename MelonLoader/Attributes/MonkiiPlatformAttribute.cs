using System;
using System.Linq;

namespace MonkiiLoader
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class MonkiiPlatformAttribute : Attribute
    {
        public MonkiiPlatformAttribute(params CompatiblePlatforms[] platforms) => Platforms = platforms;

        // <summary>Enum for Monkii Platform Compatibility.</summary>
        public enum CompatiblePlatforms
        {
            UNIVERSAL,
            WINDOWS_X86,
            WINDOWS_X64,
            ANDROID
        };

        // <summary>Platforms Compatible with the Monkii.</summary>
        public CompatiblePlatforms[] Platforms { get; internal set; }

        public bool IsCompatible(CompatiblePlatforms platform)
            => Platforms == null || Platforms.Length == 0 || Platforms.Contains(platform);
    }
}