using System;

namespace MonkiiLoader
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class MonkiiPriorityAttribute : Attribute
    {
        /// <summary>
        /// Priority of the Monkii.
        /// </summary>
        public int Priority;

        public MonkiiPriorityAttribute(int priority = 0) => Priority = priority;
    }
}