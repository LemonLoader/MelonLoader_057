using System;

namespace MonkiiLoader
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class MonkiiIncompatibleAssembliesAttribute : Attribute
    {
        /// <summary>
        /// The (simple) assembly names of the mods that are incompatible.
        /// </summary>
        public string[] AssemblyNames { get; internal set; }

        public MonkiiIncompatibleAssembliesAttribute(params string[] assemblyNames) { AssemblyNames = assemblyNames; }
    }
}