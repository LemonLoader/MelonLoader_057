using System.IO;

#if __ANDROID__
using AssemblyUnhollower;
using AssemblyUnhollower.Contexts;
using AssemblyUnhollower.MetadataAccess;
using AssemblyUnhollower.Passes;
using System;
using System.Collections.Generic;
using System.Linq;
using UnhollowerBaseLib;
using UnhollowerRuntimeLib;
#endif

namespace MonkiiLoader.Il2CppAssemblyGenerator.Packages
{
    internal class Il2CppAssemblyUnhollower : Models.ExecutablePackage
    {
#if !__ANDROID__
        internal Il2CppAssemblyUnhollower()
        {
            Version = MonkiiLaunchOptions.Il2CppAssemblyGenerator.ForceVersion_Il2CppAssemblyUnhollower;
#if !DEBUG
            if (string.IsNullOrEmpty(Version) || Version.Equals("0.0.0.0"))
                Version = RemoteAPI.Info.ForceUnhollowerVersion;
#endif
            if (string.IsNullOrEmpty(Version) || Version.Equals("0.0.0.0"))
                Version = "0.4.18.0";

            Name = nameof(Il2CppAssemblyUnhollower);
            URL = $"https://github.com/knah/{Name}/releases/download/v{Version}/{Name}.{Version}.zip";
            Destination = Path.Combine(Core.BasePath, Name);
            OutputFolder = Path.Combine(Destination, "Managed");
            ExeFilePath = Path.Combine(Destination, "AssemblyUnhollower.exe");
            FilePath = Path.Combine(Core.BasePath, $"{Name}_{Version}.zip");
        }

        internal override bool ShouldSetup()
            => string.IsNullOrEmpty(Config.Values.UnhollowerVersion) 
            || !Config.Values.UnhollowerVersion.Equals(Version);

        internal override void Save()
            => Save(ref Config.Values.UnhollowerVersion);

        internal override bool Execute()
        {
            if (Execute(new string[] {
                $"--input={ Core.dumper.OutputFolder }",
                $"--output={ OutputFolder }",
                $"--mscorlib={ Path.Combine(Core.ManagedPath, "mscorlib.dll") }",
                $"--unity={ Core.unitydependencies.Destination }",
                $"--gameassembly={ Core.GameAssemblyPath }",
                string.IsNullOrEmpty(Core.deobfuscationMap.Version) ? string.Empty : $"--rename-map={ Core.deobfuscationMap.Destination }",
                string.IsNullOrEmpty(Core.deobfuscationRegex.Regex) ? string.Empty : $"--obf-regex={ Core.deobfuscationRegex.Regex }",
                "--add-prefix-to=ICSharpCode",
                "--add-prefix-to=Newtonsoft",
                "--add-prefix-to=TinyJson",
                "--add-prefix-to=Valve.Newtonsoft"
            }))
                return true;
            return false;
        }
#else
        internal Il2CppAssemblyUnhollower()
        {
            Name = nameof(Il2CppAssemblyUnhollower);
            Destination = Path.Combine(Core.BasePath, Name);
            OutputFolder = Path.Combine(Destination, "Managed");
        }

        internal override bool Execute()
        {
            MonkiiLogger.Msg("Executing Il2CppAssemblyUnhollower...");

            InstallLogger();

            var options = GenerateOptions();

            return Main(options);
        }

        private bool Main(UnhollowerOptions options)
        {
            if (!Directory.Exists(options.OutputDir))
                Directory.CreateDirectory(options.OutputDir);

            RewriteGlobalContext rewriteContext;
            IIl2CppMetadataAccess gameAssemblies;
            IMetadataAccess systemAssemblies;
            IMetadataAccess unityAssemblies;

            using (new TimingCookie("Reading assemblies"))
                gameAssemblies = new CecilMetadataAccess(Directory.EnumerateFiles(options.SourceDir, "*.dll"));

            using (new TimingCookie("Reading system assemblies"))
            {
                if (!string.IsNullOrEmpty(options.SystemLibrariesPath))
                    systemAssemblies = new CecilMetadataAccess(Directory.EnumerateFiles(options.SystemLibrariesPath, "*.dll")
                        .Where(it => Path.GetFileName(it).StartsWith("System.") || Path.GetFileName(it) == "mscorlib.dll" || Path.GetFileName(it) == "netstandard.dll"));
                else
                    systemAssemblies = new CecilMetadataAccess(new[] { options.MscorlibPath });
            }

            if (!string.IsNullOrEmpty(options.UnityBaseLibsDir))
            {
                using (new TimingCookie("Reading unity assemblies"))
                    unityAssemblies = new CecilMetadataAccess(Directory.EnumerateFiles(options.UnityBaseLibsDir, "*.dll"));
            }
            else
                unityAssemblies = NullMetadataAccess.Instance;

            using (new TimingCookie("Creating rewrite assemblies"))
                rewriteContext = new RewriteGlobalContext(options, gameAssemblies, systemAssemblies, unityAssemblies);

            using (new TimingCookie("Computing renames"))
                Pass05CreateRenameGroups.DoPass(rewriteContext);
            using (new TimingCookie("Creating typedefs"))
                Pass10CreateTypedefs.DoPass(rewriteContext);
            using (new TimingCookie("Computing struct blittability"))
                Pass11ComputeTypeSpecifics.DoPass(rewriteContext);
            using (new TimingCookie("Filling typedefs"))
                Pass12FillTypedefs.DoPass(rewriteContext);
            using (new TimingCookie("Filling generic constraints"))
                Pass13FillGenericConstraints.DoPass(rewriteContext);
            using (new TimingCookie("Creating members"))
                Pass15GenerateMemberContexts.DoPass(rewriteContext);
            using (new TimingCookie("Scanning method cross-references"))
                Pass16ScanMethodRefs.DoPass(rewriteContext, options);
            using (new TimingCookie("Finalizing method declarations"))
                Pass18FinalizeMethodContexts.DoPass(rewriteContext);
            LogSupport.Info($"{Pass18FinalizeMethodContexts.TotalPotentiallyDeadMethods} total potentially dead methods");
            using (new TimingCookie("Filling method parameters"))
                Pass19CopyMethodParameters.DoPass(rewriteContext);

            using (new TimingCookie("Creating static constructors"))
                Pass20GenerateStaticConstructors.DoPass(rewriteContext);
            using (new TimingCookie("Creating value type fields"))
                Pass21GenerateValueTypeFields.DoPass(rewriteContext);
            using (new TimingCookie("Creating enums"))
                Pass22GenerateEnums.DoPass(rewriteContext);
            using (new TimingCookie("Creating IntPtr constructors"))
                Pass23GeneratePointerConstructors.DoPass(rewriteContext);
            using (new TimingCookie("Creating type getters"))
                Pass24GenerateTypeStaticGetters.DoPass(rewriteContext);
            using (new TimingCookie("Creating non-blittable struct constructors"))
                Pass25GenerateNonBlittableValueTypeDefaultCtors.DoPass(rewriteContext);

            using (new TimingCookie("Creating generic method static constructors"))
                Pass30GenerateGenericMethodStoreConstructors.DoPass(rewriteContext);
            using (new TimingCookie("Creating field accessors"))
                Pass40GenerateFieldAccessors.DoPass(rewriteContext);
            using (new TimingCookie("Filling methods"))
                Pass50GenerateMethods.DoPass(rewriteContext);
            using (new TimingCookie("Generating implicit conversions"))
                Pass60AddImplicitConversions.DoPass(rewriteContext);
            using (new TimingCookie("Creating properties"))
                Pass70GenerateProperties.DoPass(rewriteContext);

            if (options.UnityBaseLibsDir != null)
            {
                using (new TimingCookie("Unstripping types"))
                    Pass79UnstripTypes.DoPass(rewriteContext);
                using (new TimingCookie("Unstripping fields"))
                    Pass80UnstripFields.DoPass(rewriteContext);
                using (new TimingCookie("Unstripping methods"))
                    Pass80UnstripMethods.DoPass(rewriteContext);
                using (new TimingCookie("Unstripping method bodies"))
                    Pass81FillUnstrippedMethodBodies.DoPass(rewriteContext);
            }
            else
                LogSupport.Warning("Not performing unstripping as unity libs are not specified");

            using (new TimingCookie("Generating forwarded types"))
                Pass89GenerateForwarders.DoPass(rewriteContext);

            using (new TimingCookie("Writing xref cache"))
                Pass89GenerateMethodXrefCache.DoPass(rewriteContext, options);

            using (new TimingCookie("Writing assemblies"))
                Pass90WriteToDisk.DoPass(rewriteContext, options);

            using (new TimingCookie("Writing method pointer map"))
                Pass91GenerateMethodPointerMap.DoPass(rewriteContext, options);

            if (!options.NoCopyUnhollowerLibs)
            {
                File.Copy(typeof(IL2CPP).Assembly.Location, Path.Combine(options.OutputDir, typeof(IL2CPP).Assembly.GetName().Name + ".dll"), true);
                File.Copy(typeof(RuntimeLibMarker).Assembly.Location, Path.Combine(options.OutputDir, typeof(RuntimeLibMarker).Assembly.GetName().Name + ".dll"), true);
                //File.Copy(typeof(Decoder).Assembly.Location, Path.Combine(options.OutputDir, typeof(Decoder).Assembly.GetName().Name + ".dll"), true);
            }

            LogSupport.Info("Done!");

            rewriteContext.Dispose();

            return true;
        }

        private void InstallLogger()
        {
            LogSupport.InfoHandler += (msg) => { MonkiiLogger.Msg(ConsoleColor.Magenta, $"[{nameof(AssemblyUnhollower)}] {msg}"); };
            LogSupport.WarningHandler += (msg) => { MonkiiLogger.Warning($"[{nameof(AssemblyUnhollower)}] {msg}"); };
            LogSupport.ErrorHandler += (msg) => { MonkiiLogger.Error($"[{nameof(AssemblyUnhollower)}] {msg}"); };

            //if (MonkiiDebug.IsEnabled())
            //UnhollowerBaseLib.LogSupport.TraceHandler += (msg) => { MonkiiLogger.Error($"[{nameof(AssemblyUnhollower)}] {msg}"); };
        }

        private UnhollowerOptions GenerateOptions()
        {
            MonkiiDebug.Msg("Cpp2IL Output: " + new Cpp2IL().OutputFolder);
            MonkiiDebug.Msg("Il2CppAssemblyUnhollower Output: " + OutputFolder);
            var options = new UnhollowerOptions
            {
                Verbose = true,
                SourceDir = new Cpp2IL().OutputFolder,
                OutputDir = OutputFolder,
                MscorlibPath = Path.Combine(string.Copy(MonkiiUtils.GetManagedDirectory()), "mscorlib.dll"),
                UnityBaseLibsDir = Path.Combine(Core.BasePath, "unity"),
                GameAssemblyPath = Core.GameAssemblyPath,
                SystemLibrariesPath = MonkiiUtils.GetManagedDirectory()
            };

            options.AdditionalAssembliesBlacklist.AddRange(new List<string> { "Mono.Security", "Newtonsoft.Json", "Valve.Newtonsoft.Json" });

            return options;
        }
#endif
    }
}
