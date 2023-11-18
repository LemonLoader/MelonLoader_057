using System;

namespace MonkiiLoader
{
    [Obsolete("MonkiiLoader.MonkiiPluginInfoAttribute is Only Here for Compatibility Reasons. Please use MonkiiLoader.MonkiiInfo instead.")]
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
    public class MonkiiPluginInfoAttribute : MonkiiInfoAttribute
    {
        [Obsolete("MonkiiLoader.MonkiiPluginInfoAttribute.SystemType is Only Here for Compatibility Reasons. Please use MonkiiLoader.MonkiiInfo.SystemType instead.")]
        new public Type SystemType => base.SystemType;
        [Obsolete("MonkiiLoader.MonkiiPluginInfoAttribute.Name is Only Here for Compatibility Reasons. Please use MonkiiLoader.MonkiiInfo.Name instead.")]
        new public string Name => base.Name;
        [Obsolete("MonkiiLoader.MonkiiPluginInfoAttribute.Version is Only Here for Compatibility Reasons. Please use MonkiiLoader.MonkiiInfo.Version instead.")]
        new public string Version => base.Version;
        [Obsolete("MonkiiLoader.MonkiiPluginInfoAttribute.Author is Only Here for Compatibility Reasons. Please use MonkiiLoader.MonkiiInfo.Author instead.")]
        new public string Author => base.Author;
        [Obsolete("MonkiiLoader.MonkiiPluginInfoAttribute.DownloadLink is Only Here for Compatibility Reasons. Please use MonkiiLoader.MonkiiInfo.DownloadLink instead.")]
        new public string DownloadLink => base.DownloadLink;
        [Obsolete("MonkiiLoader.MonkiiPluginInfoAttribute is Only Here for Compatibility Reasons. Please use MonkiiLoader.MonkiiInfo instead.")]
        public MonkiiPluginInfoAttribute(Type type, string name, string version, string author, string downloadLink = null) : base(type, name, version, author, downloadLink) { }
    }
}