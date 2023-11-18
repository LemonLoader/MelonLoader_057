using Semver;
using System;

namespace MonkiiLoader
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class MonkiiInfoAttribute : Attribute
    {
        /// <summary>
        /// System.Type of the Monkii.
        /// </summary>
        public Type SystemType { get; internal set; }

        /// <summary>
        /// Name of the Monkii.
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// Version of the Monkii.
        /// </summary>
        public string Version { get; internal set; }

        /// <summary>
        /// Semantic Version of the Monkii. Will be null if Version is not using the <see href="https://semver.org">Semantic Versioning</see> format.
        /// </summary>
        public SemVersion SemanticVersion { get; internal set; }

        /// <summary>
        /// Author of the Monkii.
        /// </summary>
        public string Author { get; internal set; } // This used to be optional, but is now required

        /// <summary>
        /// Download Link of the Monkii.
        /// </summary>
        public string DownloadLink { get; internal set; } // Might get Removed. Not sure yet.

        /// <summary>
        /// MonkiiInfo constructor.
        /// </summary>
        /// <param name="type">The main Monkii type of the Monkii (for example TestMod)</param>
        /// <param name="name">Name of the Monkii</param>
        /// <param name="version">Version of the Monkii</param>
        /// <param name="author">Author of the Monkii</param>
        /// <param name="downloadLink">URL to the download link of the mod [optional]</param>
        public MonkiiInfoAttribute(Type type, string name, string version, string author, string downloadLink = null) 
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            SystemType = type;
            Name = name ?? "UNKNOWN";
            Author = author ?? "UNKNOWN";
            DownloadLink = downloadLink; // Might get Removed. Not sure yet.

            if (string.IsNullOrEmpty(version))
                Version = "1.0.0";
            else
                Version = version;

            if (SemVersion.TryParse(Version, out SemVersion semver))
                SemanticVersion = semver;
        }

        /// <summary>
        /// MonkiiInfo constructor.
        /// </summary>
        /// <param name="type">The main Monkii type of the Monkii (for example TestMod)</param>
        /// <param name="name">Name of the Monkii</param>
        /// <param name="versionMajor">Version Major of the Monkii (Using the <see href="https://semver.org">Semantic Versioning</see> format)</param>
        /// <param name="versionMinor">Version Minor of the Monkii (Using the <see href="https://semver.org">Semantic Versioning</see> format)</param>
        /// <param name="versionRevision">Version Revision of the Monkii (Using the <see href="https://semver.org">Semantic Versioning</see> format)</param>
        /// <param name="versionIdentifier">Version Identifier of the Monkii (Using the <see href="https://semver.org">Semantic Versioning</see> format)</param>
        /// <param name="author">Author of the Monkii</param>
        /// <param name="downloadLink">URL to the download link of the mod [optional]</param>
        public MonkiiInfoAttribute(Type type, string name, int versionMajor, int versionMinor, int versionRevision, string versionIdentifier, string author, string downloadLink = null)
            : this(type, name, $"{versionMajor}.{versionMinor}.{versionRevision}{(string.IsNullOrEmpty(versionIdentifier) ? "" : versionIdentifier)}", author, downloadLink) { }

        /// <summary>
        /// MonkiiInfo constructor.
        /// </summary>
        /// <param name="type">The main Monkii type of the Monkii (for example TestMod)</param>
        /// <param name="name">Name of the Monkii</param>
        /// <param name="versionMajor">Version Major of the Monkii (Using the <see href="https://semver.org">Semantic Versioning</see> format)</param>
        /// <param name="versionMinor">Version Minor of the Monkii (Using the <see href="https://semver.org">Semantic Versioning</see> format)</param>
        /// <param name="versionRevision">Version Revision of the Monkii (Using the <see href="https://semver.org">Semantic Versioning</see> format)</param>
        /// <param name="author">Author of the Monkii</param>
        /// <param name="downloadLink">URL to the download link of the mod [optional]</param>
        public MonkiiInfoAttribute(Type type, string name, int versionMajor, int versionMinor, int versionRevision, string author, string downloadLink = null)
            : this(type, name, versionMajor, versionMinor, versionRevision, null, author, downloadLink) { }
    }
}