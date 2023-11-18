using System;

namespace MonkiiLoader
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class MonkiiAdditionalDependenciesAttribute : Attribute
    {
        /// <summary>
        /// The (simple) assembly names of Additional Dependencies that aren't directly referenced but should still be regarded as important.
        /// </summary>
        public string[] AssemblyNames { get; internal set; }

        public MonkiiAdditionalDependenciesAttribute(params string[] assemblyNames) { AssemblyNames = assemblyNames; }
    }
}