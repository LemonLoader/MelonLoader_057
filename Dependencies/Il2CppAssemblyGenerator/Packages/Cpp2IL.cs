using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Semver;

#if __ANDROID__
using Cpp2IL.Core;
using LibCpp2IL;
using LibCpp2IL.Wasm;
#endif

namespace MonkiiLoader.Il2CppAssemblyGenerator.Packages
{
    internal class Cpp2IL : Models.ExecutablePackage
    {
#if !__ANDROID__
        internal Cpp2IL()
        {
            Version = MonkiiLaunchOptions.Il2CppAssemblyGenerator.ForceVersion_Dumper;
#if !DEBUG
            if (string.IsNullOrEmpty(Version) || Version.Equals("0.0.0.0"))
                Version = RemoteAPI.Info.ForceDumperVersion;
#endif
            if (string.IsNullOrEmpty(Version) || Version.Equals("0.0.0.0"))
                Version = "2022.1.0-pre-release.8";

            Name = nameof(Cpp2IL);
            Destination = Path.Combine(Core.BasePath, Name);
            OutputFolder = Path.Combine(Destination, "cpp2il_out");
            URL = $"https://github.com/SamboyCoding/{Name}/releases/download/{Version}/{Name}-{Version}-Windows-Netframework472.zip";
            ExeFilePath = Path.Combine(Destination, $"{Name}.exe");
            FilePath = Path.Combine(Core.BasePath, $"{Name}_{Version}.zip");
        }

        internal override bool ShouldSetup() 
            => string.IsNullOrEmpty(Config.Values.DumperVersion) 
            || !Config.Values.DumperVersion.Equals(Version);

        internal override void Cleanup() { }

        internal override void Save()
            => Save(ref Config.Values.DumperVersion);

        internal override bool Execute()
        {
            if (SemVersion.Parse(Version) <= SemVersion.Parse("2022.0.999"))
                return ExecuteOld();
            return ExecuteNew();
        }

        private bool ExecuteNew()
        {
            if (Execute(new string[] {
                MonkiiDebug.IsEnabled() ? "--verbose" : string.Empty,
                "--game-path",
                "\"" + Path.GetDirectoryName(Core.GameAssemblyPath) + "\"",
                "--exe-name",
                "\"" + Process.GetCurrentProcess().ProcessName + "\"",

                "--use-processor",
                "attributeinjector",
                "--output-as",
                "dummydll"

            }, false, new Dictionary<string, string>() {
                {"NO_COLOR", "1"}
            }))
                return true;

            return false;
        }

        private bool ExecuteOld()
        {
            if (Execute(new string[] {
                MonkiiDebug.IsEnabled() ? "--verbose" : string.Empty,
                "--game-path",
                "\"" + Path.GetDirectoryName(Core.GameAssemblyPath) + "\"",
                "--exe-name",
                "\"" + Process.GetCurrentProcess().ProcessName + "\"",

                "--skip-analysis",
                "--skip-metadata-txts",
                "--disable-registration-prompts"

            }, false, new Dictionary<string, string>() {
                {"NO_COLOR", "1"}
            }))
                return true;

            return false;
        }
#else
        internal Cpp2IL()
        {
            Name = nameof(Cpp2IL);
            Destination = Path.Combine(Core.BasePath, Name);
            OutputFolder = Path.Combine(Destination, "cpp2il_out/");
        }

        internal override bool Execute()
        {
            MonkiiLogger.Msg("Executing Cpp2IL...");
            string metadata_path = Path.Combine(Path.Combine(Path.Combine(string.Copy(MonkiiUtils.GetGameDataDirectory()), "il2cpp"), "Metadata"), "global-metadata.dat");
            return Main(string.Copy(MonkiiUtils.GetMainAssemblyLoc()), metadata_path, OutputFolder);
        }

        private bool Main(string gameAssembly, string metadata, string output)
        {
            Logger.InfoLog += (msg, source) => { MonkiiLogger.Msg($"[Cpp2IL] [{source}] {msg}"); };
            Logger.WarningLog += (msg, source) => { MonkiiLogger.Warning($"[Cpp2IL] [{source}] {msg}"); };
            Logger.ErrorLog += (msg, source) => { MonkiiLogger.Error($"[Cpp2IL] [{source}] {msg}"); };
            Logger.VerboseLog += (msg, source) => { MonkiiLogger.Msg(System.ConsoleColor.Magenta, $"[Cpp2IL] [{source}] {msg}"); };

            string restore = Directory.GetCurrentDirectory();
            if (!Directory.Exists(output))
                Directory.CreateDirectory(output);
            //Core.OverrideAppDomainBase(output);

            var engineVersionInfo = InternalUtils.UnityInformationHandler.EngineVersion;
            var engineVersionArray = new int[3] { engineVersionInfo.Major, engineVersionInfo.Minor, engineVersionInfo.Build };
            Cpp2IlApi.InitializeLibCpp2Il(gameAssembly, metadata, engineVersionArray, false);

            Cpp2IlApi.SaveAssemblies(output, Cpp2IlApi.MakeDummyDLLs());
            //Core.OverrideAppDomainBase(restore);
            return true;
        }

        internal override void Cleanup()
        {
        }
#endif
    }
}
