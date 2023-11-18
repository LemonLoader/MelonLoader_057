using System;

namespace MonkiiLoader
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class MonkiiColorAttribute : Attribute
    {
        /// <summary>
        /// Color of the Monkii.
        /// </summary>
        public ConsoleColor Color { get; internal set; }

        public MonkiiColorAttribute() { Color = MonkiiLogger.DefaultMonkiiColor; }
        public MonkiiColorAttribute(ConsoleColor color) { Color = ((color == ConsoleColor.Black) ? MonkiiLogger.DefaultMonkiiColor : color); }
    }
}