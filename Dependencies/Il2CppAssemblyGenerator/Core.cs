using System;
using System.IO;
using System.Net;
using MonkiiLoader.Il2CppAssemblyGenerator.Packages;
using MonkiiLoader.Modules;

namespace MonkiiLoader.Il2CppAssemblyGenerator
{
    internal class Core : MonkiiModule
    {
        internal static string BasePath = null;
        internal static string GameAssemblyPath = null;
        internal static string ManagedPath = null;

        internal static WebClient webClient = null;

        internal static Packages.Models.ExecutablePackage dumper = null;
        internal static Il2CppAssemblyUnhollower il2cppassemblyunhollower = null;
        internal static UnityDependencies unitydependencies = null;
        internal static DeobfuscationMap deobfuscationMap = null;
        internal static DeobfuscationRegex deobfuscationRegex = null;

        internal static bool AssemblyGenerationNeeded = false;

        internal static MonkiiLogger.Instance Logger;

        public override void OnInitialize()
        {
            Logger = LoggerInstance;
            AssemblyGenerationNeeded = MonkiiLaunchOptions.Il2CppAssemblyGenerator.ForceRegeneration;

#if !__ANDROID__
            webClient = new WebClient();
            webClient.Headers.Add("User-Agent", $"{BuildInfo.Name} v{BuildInfo.Version}");

            GameAssemblyPath = Path.Combine(MonkiiUtils.GameDirectory, "GameAssembly.dll");

            BasePath = Path.GetDirectoryName(Assembly.Location);
#else
            BasePath = Path.Combine(string.Copy(MonkiiUtils.GetApplicationPath()), "Monkiiloader", "etc", "assembly_generation");
            // TODO: Read APK file instead
            GameAssemblyPath = Path.Combine(string.Copy(MonkiiUtils.GetMainAssemblyLoc()));
#endif

            ManagedPath = string.Copy(MonkiiUtils.GetManagedDirectory());
        }

        private static int Run()
        {
            Config.Initialize();

#if !__ANDROID__
            if (!MonkiiLaunchOptions.Il2CppAssemblyGenerator.OfflineMode)
                RemoteAPI.Contact();

                // Temporary Workaround for Cpp2IL Failing on Unsupported OSes
            if (!MonkiiUtils.IsUnderWineOrSteamProton() && ((Environment.OSVersion.Version.Major < 6) // Is Older than Vista
                || ((Environment.OSVersion.Version.Major == 6) && (Environment.OSVersion.Version.Minor < 1)))) // Is Older than Windows 7 or Server 2008 R2
                dumper = new Il2CppDumper();
            else
#endif
            dumper = new Packages.Cpp2IL();

            il2cppassemblyunhollower = new Il2CppAssemblyUnhollower();

#if !__ANDROID__
            unitydependencies = new UnityDependencies();
            deobfuscationMap = new DeobfuscationMap();
            deobfuscationRegex = new DeobfuscationRegex();

            Logger.Msg($"Using Dumper Version: {(string.IsNullOrEmpty(dumper.Version) ? "null" : dumper.Version)}");
            Logger.Msg($"Using Il2CppAssemblyUnhollower Version = {(string.IsNullOrEmpty(il2cppassemblyunhollower.Version) ? "null" : il2cppassemblyunhollower.Version)}");
            Logger.Msg($"Using Unity Dependencies Version = {(string.IsNullOrEmpty(unitydependencies.Version) ? "null" : unitydependencies.Version)}");
            Logger.Msg($"Using Deobfuscation Regex = {(string.IsNullOrEmpty(deobfuscationRegex.Regex) ? "null" : deobfuscationRegex.Regex)}");

            if (!dumper.Setup()
                || !il2cppassemblyunhollower.Setup()
                || !unitydependencies.Setup()
                || !deobfuscationMap.Setup())
                return 1;

            deobfuscationRegex.Setup();
#endif

            string CurrentGameAssemblyHash;
            Logger.Msg("Checking GameAssembly...");
            MonkiiDebug.Msg($"Last GameAssembly Hash: {Config.Values.GameAssemblyHash}");
            MonkiiDebug.Msg($"Current GameAssembly Hash: {CurrentGameAssemblyHash = FileHandler.Hash(GameAssemblyPath)}");

            if (string.IsNullOrEmpty(Config.Values.GameAssemblyHash)
                    || !Config.Values.GameAssemblyHash.Equals(CurrentGameAssemblyHash))
                    AssemblyGenerationNeeded = true;

            if (!AssemblyGenerationNeeded)
            {
                Logger.Msg("Assembly is up to date. No Generation Needed.");
                return 0;
            }
            Logger.Msg("Assembly Generation Needed!");

            dumper.Cleanup();
            il2cppassemblyunhollower.Cleanup();

            if (!dumper.Execute())
            {
                dumper.Cleanup();
                return 1;
            }

            if (!il2cppassemblyunhollower.Execute())
            {
                dumper.Cleanup();
                il2cppassemblyunhollower.Cleanup();
                return 1;
            }

            OldFiles_Cleanup();
            OldFiles_LAM();

            dumper.Cleanup();
            il2cppassemblyunhollower.Cleanup();

            Logger.Msg("Assembly Generation Successful!");

#if !__ANDROID__
            deobfuscationRegex.Save();
#endif
            Config.Values.GameAssemblyHash = CurrentGameAssemblyHash;
            Config.Save();

            return 0;
        }

        private static void OldFiles_Cleanup()
        {
            if (Config.Values.OldFiles.Count <= 0)
                return;
            for (int i = 0; i < Config.Values.OldFiles.Count; i++)
            {
                string filename = Config.Values.OldFiles[i];
                string filepath = Path.Combine(ManagedPath, filename);
                if (File.Exists(filepath))
                {
                    Logger.Msg("Deleting " + filename);
                    File.Delete(filepath);
                }
            }
            Config.Values.OldFiles.Clear();
        }

        private static void OldFiles_LAM()
        {
            string[] filepathtbl = Directory.GetFiles(il2cppassemblyunhollower.OutputFolder);
            for (int i = 0; i < filepathtbl.Length; i++)
            {
                string filepath = filepathtbl[i];
                string filename = Path.GetFileName(filepath);
                Logger.Msg("Moving " + filename);
                Config.Values.OldFiles.Add(filename);
                string newfilepath = Path.Combine(ManagedPath, filename);
                if (File.Exists(newfilepath))
                    File.Delete(newfilepath);
                File.Move(filepath, newfilepath);
            }
            Config.Save();
        }
    }
}