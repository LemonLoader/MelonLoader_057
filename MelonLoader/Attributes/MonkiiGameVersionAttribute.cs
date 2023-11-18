using System;

namespace MonkiiLoader
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class MonkiiGameVersionAttribute : Attribute
    {
        public MonkiiGameVersionAttribute(string version = null)
            => Version = version;

        /// <summary>
        /// Version of the Game.
        /// </summary>
        public string Version { get; internal set; }

        /// <summary>
        /// If the Attribute is set as Universal or not.
        /// </summary>
        public bool Universal { get => string.IsNullOrEmpty(Version); }
    }
}