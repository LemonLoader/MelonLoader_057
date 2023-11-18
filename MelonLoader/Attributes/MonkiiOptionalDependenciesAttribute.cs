using System;

namespace MonkiiLoader
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class MonkiiOptionalDependenciesAttribute : Attribute
    {
        /// <summary>
        /// The (simple) assembly names of the dependencies that should be regarded as optional.
        /// </summary>
        public string[] AssemblyNames { get; internal set; }

        public MonkiiOptionalDependenciesAttribute(params string[] assemblyNames) { AssemblyNames = assemblyNames; }
    }
}