using System;

namespace MonkiiLoader
{
    [Obsolete("MonkiiLoader.MonkiiPluginGameAttribute is Only Here for Compatibility Reasons. Please use MonkiiLoader.MonkiiGame instead.")]
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class MonkiiPluginGameAttribute : MonkiiGameAttribute
    {
        [Obsolete("MonkiiLoader.MonkiiPluginGameAttribute.Developer is Only Here for Compatibility Reasons. Please use MonkiiLoader.MonkiiGame.Developer instead.")]
        new public string Developer => base.Developer;
        [Obsolete("MonkiiLoader.MonkiiPluginGameAttribute.GameName is Only Here for Compatibility Reasons. Please use MonkiiLoader.MonkiiGame.Name instead.")]
        public string GameName => Name;
        [Obsolete("MonkiiLoader.MonkiiPluginGameAttribute is Only Here for Compatibility Reasons. Please use MonkiiLoader.MonkiiGame instead.")]
        public MonkiiPluginGameAttribute(string developer = null, string gameName = null) : base(developer, gameName) { }
    }
}