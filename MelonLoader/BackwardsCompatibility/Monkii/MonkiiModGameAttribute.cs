using System;

namespace MonkiiLoader
{
    [Obsolete("MonkiiLoader.MonkiiModGameAttribute is Only Here for Compatibility Reasons. Please use MonkiiLoader.MonkiiGame instead.")]
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class MonkiiModGameAttribute : MonkiiGameAttribute
    {
        [Obsolete("MonkiiLoader.MonkiiModGameAttribute.Developer is Only Here for Compatibility Reasons. Please use MonkiiLoader.MonkiiGame.Developer instead.")]
        new public string Developer => base.Developer;
        [Obsolete("MonkiiLoader.MonkiiModGameAttribute.GameName is Only Here for Compatibility Reasons. Please use MonkiiLoader.MonkiiGame.Name instead.")]
        public string GameName => Name;
        [Obsolete("MonkiiLoader.MonkiiModGameAttribute is Only Here for Compatibility Reasons. Please use MonkiiLoader.MonkiiGame instead.")]
        public MonkiiModGameAttribute(string developer = null, string gameName = null) : base(developer, gameName) { }
    }
}