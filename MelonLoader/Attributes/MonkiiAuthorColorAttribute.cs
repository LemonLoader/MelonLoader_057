using System;

namespace MonkiiLoader
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class MonkiiAuthorColorAttribute : Attribute
    {
        /// <summary>
        /// Color of the Author Log.
        /// </summary>
        public ConsoleColor Color { get; internal set; }

        public MonkiiAuthorColorAttribute() { Color = MonkiiLogger.DefaultTextColor; }
        public MonkiiAuthorColorAttribute(ConsoleColor color) { Color = ((color == ConsoleColor.Black) ? MonkiiLogger.DefaultMonkiiColor : color); }
    }
}