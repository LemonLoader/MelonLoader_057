using System;

namespace MonkiiLoader
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class MonkiiIDAttribute : Attribute
    {
        /// <summary>ID of the Monkii.</summary>
        public string ID { get; internal set; }

        public MonkiiIDAttribute(string id)
            => ID = id;
        public MonkiiIDAttribute(int id)
            => ID = id.ToString();
    }
}