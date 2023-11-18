using System;

namespace MonkiiLoader
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class MonkiiPlatformDomainAttribute : Attribute
    {
        public MonkiiPlatformDomainAttribute(CompatibleDomains domain = CompatibleDomains.UNIVERSAL) => Domain = domain;

        // <summary>Enum for Monkii Platform Domain Compatibility.</summary>
        public enum CompatibleDomains
        {
            UNIVERSAL,
            MONO,
            IL2CPP
        };

        // <summary>Platform Domain Compatibility of the Monkii.</summary>
        public CompatibleDomains Domain { get; internal set; }

        public bool IsCompatible(CompatibleDomains domain)
            => Domain == CompatibleDomains.UNIVERSAL || domain == CompatibleDomains.UNIVERSAL || Domain == domain;
    }
}