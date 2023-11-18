using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.IO;

namespace MonkiiLoader.InternalUtils
{
    internal class DependencyGraph<T> where T : MonkiiBase
    {
        public static void TopologicalSort(IList<T> Monkiis)
        {
            if (Monkiis.Count <= 0)
                return;
            DependencyGraph<T> dependencyGraph = new DependencyGraph<T>(Monkiis);
            Monkiis.Clear();
            dependencyGraph.TopologicalSortInto(Monkiis);
        }

        private readonly Vertex[] vertices;

        private DependencyGraph(IList<T> Monkiis)
        {
            int size = Monkiis.Count;
            vertices = new Vertex[size];
            Dictionary<string, Vertex> nameLookup = new Dictionary<string, Vertex>(size);

            // Create a vertex in the dependency graph for each Monkii to load
            for (int i = 0; i < size; ++i)
            {
                Assembly MonkiiAssembly = Monkiis[i].MonkiiAssembly.Assembly;
                string MonkiiName = Monkiis[i].Info.Name;

                Vertex MonkiiVertex = new Vertex(i, Monkiis[i], MonkiiName);
                vertices[i] = MonkiiVertex;
                nameLookup[MonkiiAssembly.GetName().Name] = MonkiiVertex;
            }

            // Add an edge for each dependency between Monkiis
            SortedDictionary<string, IList<AssemblyName>> MonkiisWithMissingDeps = new SortedDictionary<string, IList<AssemblyName>>();
            SortedDictionary<string, IList<AssemblyName>> MonkiisWithIncompatibilities = new SortedDictionary<string, IList<AssemblyName>>();
            List<AssemblyName> missingDependencies = new List<AssemblyName>();
            List<AssemblyName> incompatibilities = new List<AssemblyName>();
            HashSet<string> optionalDependencies = new HashSet<string>();
            HashSet<string> additionalDependencies = new HashSet<string>();

            foreach (Vertex MonkiiVertex in vertices)
            {
                Assembly MonkiiAssembly = MonkiiVertex.Monkii.MonkiiAssembly.Assembly;
                missingDependencies.Clear();
                optionalDependencies.Clear();
                incompatibilities.Clear();
                additionalDependencies.Clear();

                MonkiiOptionalDependenciesAttribute optionals = (MonkiiOptionalDependenciesAttribute)Attribute.GetCustomAttribute(MonkiiAssembly, typeof(MonkiiOptionalDependenciesAttribute));
                if (optionals != null
                    && optionals.AssemblyNames != null)
                    optionalDependencies.UnionWith(optionals.AssemblyNames);

                MonkiiAdditionalDependenciesAttribute additionals = (MonkiiAdditionalDependenciesAttribute)Attribute.GetCustomAttribute(MonkiiAssembly, typeof(MonkiiAdditionalDependenciesAttribute));
                if (additionals != null
                    && additionals.AssemblyNames != null)
                    additionalDependencies.UnionWith(additionals.AssemblyNames);

                MonkiiIncompatibleAssembliesAttribute incompatibleAssemblies = (MonkiiIncompatibleAssembliesAttribute)Attribute.GetCustomAttribute(MonkiiAssembly, typeof(MonkiiIncompatibleAssembliesAttribute));
                if (incompatibleAssemblies != null
                    && incompatibleAssemblies.AssemblyNames != null)
                {
                    foreach (string name in incompatibleAssemblies.AssemblyNames)
                        foreach (Vertex v in vertices)
                        {
                            AssemblyName assemblyName = v.Monkii.MonkiiAssembly.Assembly.GetName();
                            if (v != MonkiiVertex
                                && assemblyName.Name == name)
                            {
                                incompatibilities.Add(assemblyName);
                                v.skipLoading = true;
                            }
                        }
                }

                foreach (AssemblyName dependency in MonkiiAssembly.GetReferencedAssemblies())
                {
                    if (nameLookup.TryGetValue(dependency.Name, out Vertex dependencyVertex))
                    {
                        MonkiiVertex.dependencies.Add(dependencyVertex);
                        dependencyVertex.dependents.Add(MonkiiVertex);
                    }
                    else if (!TryLoad(dependency) && !optionalDependencies.Contains(dependency.Name))
                        missingDependencies.Add(dependency);
                }

                foreach (string dependencyName in additionalDependencies)
                {
                    AssemblyName dependency = new AssemblyName(dependencyName);
                    if (nameLookup.TryGetValue(dependencyName, out Vertex dependencyVertex))
                    {
                        MonkiiVertex.dependencies.Add(dependencyVertex);
                        dependencyVertex.dependents.Add(MonkiiVertex);
                    }
                    else if (!TryLoad(dependency))
                        missingDependencies.Add(dependency);
                }

                if (missingDependencies.Count > 0)
                    // MonkiiVertex.skipLoading = true;
                    MonkiisWithMissingDeps.Add(MonkiiVertex.Monkii.Info.Name, missingDependencies.ToArray());

                if (incompatibilities.Count > 0)
                    MonkiisWithIncompatibilities.Add(MonkiiVertex.Monkii.Info.Name, incompatibilities.ToArray());
            }

            // Some Monkiis are missing dependencies. Don't load these Monkiis and show an error message
            if (MonkiisWithMissingDeps.Count > 0)
                MonkiiLogger.Warning(BuildMissingDependencyMessage(MonkiisWithMissingDeps));

            if (MonkiisWithIncompatibilities.Count > 0)
                MonkiiLogger.Warning(BuildIncompatibleAssembliesMessage(MonkiisWithIncompatibilities));
        }

        // Returns true if 'assembly' was already loaded or could be loaded, false if the required assembly was missing.
        private static bool TryLoad(AssemblyName assembly)
        {
            try
            {
                Assembly.Load(assembly);
                return true;
            }
            catch (FileNotFoundException) { return false; }
            catch (Exception ex)
            {
                MonkiiLogger.Error("Loading Monkii Dependency Failed: " + ex);
                return false;
            }
        }

        private static string BuildMissingDependencyMessage(IDictionary<string, IList<AssemblyName>> MonkiisWithMissingDeps)
        {
            StringBuilder messageBuilder = new StringBuilder("Some Monkiis are missing dependencies, which you may have to install.\n" +
                "If these are optional dependencies, mark them as optional using the MonkiiOptionalDependencies attribute.\n" +
                "This warning will turn into an error and Monkiis with missing dependencies will not be loaded in the next version of MonkiiLoader.\n");
            foreach (string MonkiiName in MonkiisWithMissingDeps.Keys)
            {
                messageBuilder.Append($"- '{MonkiiName}' is missing the following dependencies:\n");
                foreach (AssemblyName dependency in MonkiisWithMissingDeps[MonkiiName])
                    messageBuilder.Append($"    - '{dependency.Name}' v{dependency.Version}\n");
            }
            messageBuilder.Length -= 1; // Remove trailing newline
            return messageBuilder.ToString();
        }

        private static string BuildIncompatibleAssembliesMessage(IDictionary<string, IList<AssemblyName>> MonkiisWithIncompatibilities)
        {
            StringBuilder messageBuilder = new StringBuilder("Some Monkiis are marked as incompatible with each other.\n" +
                "To avoid any errors, these Monkiis will not be loaded.\n");
            foreach (string MonkiiName in MonkiisWithIncompatibilities.Keys)
            {
                messageBuilder.Append($"- '{MonkiiName}' is incompatible with the following Monkiis:\n");
                foreach (AssemblyName dependency in MonkiisWithIncompatibilities[MonkiiName])
                {
                    messageBuilder.Append($"    - '{dependency.Name}'\n");
                }
            }
            messageBuilder.Length -= 1; // Remove trailing newline
            return messageBuilder.ToString();
        }

        private void TopologicalSortInto(IList<T> loadedMonkiis)
        {
            int[] unloadedDependencies = new int[vertices.Length];
            SortedList<string, Vertex> loadableMonkiis = new SortedList<string, Vertex>();
            int skippedMonkiis = 0;

            // Find all sinks in the dependency graph, i.e. Monkiis without any dependencies on other Monkiis
            for (int i = 0; i < vertices.Length; ++i)
            {
                Vertex vertex = vertices[i];
                int dependencyCount = vertex.dependencies.Count;

                unloadedDependencies[i] = dependencyCount;
                if (dependencyCount == 0)
                    loadableMonkiis.Add(vertex.name, vertex);
            }

            // Perform the (reverse) topological sorting
            while (loadableMonkiis.Count > 0)
            {
                Vertex Monkii = loadableMonkiis.Values[0];
                loadableMonkiis.RemoveAt(0);

                if (!Monkii.skipLoading)
                    loadedMonkiis.Add(Monkii.Monkii);
                else
                    ++skippedMonkiis;

                foreach (Vertex dependent in Monkii.dependents)
                {
                    unloadedDependencies[dependent.index] -= 1;
                    dependent.skipLoading |= Monkii.skipLoading;

                    if (unloadedDependencies[dependent.index] == 0)
                        loadableMonkiis.Add(dependent.name, dependent);
                }
            }

            // Check if all Monkiis were either loaded or skipped. If this is not the case, there is a cycle in the dependency graph
            if (loadedMonkiis.Count + skippedMonkiis < vertices.Length)
            {
                StringBuilder errorMessage = new StringBuilder("Some Monkiis could not be loaded due to a cyclic dependency:\n");
                for (int i = 0; i < vertices.Length; ++i)
                    if (unloadedDependencies[i] > 0)
                        errorMessage.Append($"- '{vertices[i].name}'\n");
                errorMessage.Length -= 1; // Remove trailing newline
                MonkiiLogger.Error(errorMessage.ToString());
            }
        }

        private class Vertex
        {
            internal readonly int index;
            internal readonly T Monkii;
            internal readonly string name;

            internal readonly List<Vertex> dependencies;
            internal readonly List<Vertex> dependents;
            internal bool skipLoading;

            internal Vertex(int index, T Monkii, string name)
            {
                this.index = index;
                this.Monkii = Monkii;
                this.name = name;

                dependencies = new List<Vertex>();
                dependents = new List<Vertex>();
                skipLoading = false;
            }
        }
    }
}