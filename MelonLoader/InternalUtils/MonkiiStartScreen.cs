using System.IO;
using MonkiiLoader.Modules;
using UnityVersion = AssetRipper.VersionUtilities.UnityVersion;

namespace MonkiiLoader.InternalUtils
{
    internal class MonkiiStartScreen
    {
        // Doesn't support Unity versions lower than 2017.2.0 (yet?)
        // Doesn't support Unity versions lower than 2018 (Crashing Issue)
        // Doesn't support Unity versions higher than to 2020.3.21 (Crashing Issue)
        internal static readonly MonkiiModule.Info moduleInfo = new MonkiiModule.Info($"MonkiiLoader{Path.DirectorySeparatorChar}Dependencies{Path.DirectorySeparatorChar}MonkiiStartScreen.dll"
            , () => !MonkiiLaunchOptions.Core.StartScreen || UnityInformationHandler.EngineVersion < new UnityVersion(2018));

        internal static int LoadAndRun(LemonFunc<int> functionToWaitForAsync)
        {
            var module = MonkiiModule.Load(moduleInfo);
            if (module == null)
                return functionToWaitForAsync();

            var result = module.SendMessage("LoadAndRun", functionToWaitForAsync);
            if (result is int resultCode)
                return resultCode;

            return -1;
        }
    }
}
