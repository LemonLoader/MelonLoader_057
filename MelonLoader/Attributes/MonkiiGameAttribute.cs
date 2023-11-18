using System;

namespace MonkiiLoader
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class MonkiiGameAttribute : Attribute
    {
        public MonkiiGameAttribute(string developer = null, string name = null) { Developer = developer; Name = name; }

        /// <summary>
        /// Developer of the Game.
        /// </summary>
        public string Developer { get; internal set; }

        /// <summary>
        /// Name of the Game.
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// If the Attribute is set as Universal or not.
        /// </summary>
        public bool Universal { get => (string.IsNullOrEmpty(Developer) || Developer.Equals("UNKNOWN") || string.IsNullOrEmpty(Name) || Name.Equals("UNKNOWN")); }

        /// <summary>
        /// Returns true or false if the Game is compatible with this Assembly.
        /// </summary>
        public bool IsCompatible(string developer, string gameName) => (Universal || (!string.IsNullOrEmpty(developer) && Developer.Equals(developer) && !string.IsNullOrEmpty(gameName) && Name.Equals(gameName)));

        /// <summary>
        /// Returns true or false if the Game is compatible with this Assembly.
        /// </summary>
#if __ANDROID__
        public bool IsCompatible(MonkiiGameAttribute att) => (IsCompatibleBecauseUniversal(att) || (att.Developer.Equals(Developer) && att.Name.Equals(Name)) || (att.Developer.Replace(" ", "").Equals(Developer.Replace(" ", "")) && att.Name.Replace(" ", "").Equals(Name.Replace(" ", ""))));
#else
        public bool IsCompatible(MonkiiGameAttribute att) => (IsCompatibleBecauseUniversal(att) || (att.Developer.Equals(Developer) && att.Name.Equals(Name)));
#endif

        /// <summary>
        /// Returns true or false if the Game is compatible with this Assembly specifically because of Universal Compatibility.
        /// </summary>
        public bool IsCompatibleBecauseUniversal(MonkiiGameAttribute att) => ((att == null) || Universal || att.Universal);

        [Obsolete("IsCompatible(MonkiiModGameAttribute) is obsolete. Please use IsCompatible(MonkiiGameAttribute) instead.")]
        public bool IsCompatible(MonkiiModGameAttribute att) => ((att == null) || IsCompatibleBecauseUniversal(att) || (att.Developer.Equals(Developer) && att.GameName.Equals(Name)));
        [Obsolete("IsCompatible(MonkiiPluginGameAttribute) is obsolete. Please use IsCompatible(MonkiiGameAttribute) instead.")]
        public bool IsCompatible(MonkiiPluginGameAttribute att) => ((att == null) || IsCompatibleBecauseUniversal(att) || (att.Developer.Equals(Developer) && att.GameName.Equals(Name)));
        [Obsolete("IsCompatibleBecauseUniversal(MonkiiModGameAttribute) is obsolete. Please use IsCompatible(MonkiiGameAttribute) instead.")]
        public bool IsCompatibleBecauseUniversal(MonkiiModGameAttribute att) => ((att == null) || Universal || (string.IsNullOrEmpty(att.Developer) || string.IsNullOrEmpty(att.GameName)));
        [Obsolete("IsCompatibleBecauseUniversal(MonkiiPluginGameAttribute) is obsolete. Please use IsCompatible(MonkiiGameAttribute) instead.")]
        public bool IsCompatibleBecauseUniversal(MonkiiPluginGameAttribute att) => ((att == null) || Universal || (string.IsNullOrEmpty(att.Developer) || string.IsNullOrEmpty(att.GameName)));
    }
}