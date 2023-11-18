using System;

namespace MonkiiLoader
{
    /// <summary>
    /// An info class for broken Monkiis.
    /// </summary>
    public sealed class RottenMonkii
    {
        public readonly MonkiiAssembly assembly;
        public readonly Type type;
        public readonly string errorMessage;
        public readonly Exception exception;

        public RottenMonkii(Type type, string errorMessage, Exception exception = null)
        {
            assembly = MonkiiAssembly.LoadMonkiiAssembly(null, type.Assembly);
            this.type = type;
            this.errorMessage = errorMessage;
            this.exception = exception;
        }
    }
}
