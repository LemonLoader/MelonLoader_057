using System.Collections.Generic;
using System.Reflection;
using Boardgame.Modding;
using Prototyping;
using MonkiiLoader.Modules;

namespace MonkiiLoader.CompatibilityLayers
{
    internal class Demeo_Module : MonkiiModule
    {
        private static Dictionary<MonkiiBase, ModdingAPI.ModInformation> ModInformation = new Dictionary<MonkiiBase, ModdingAPI.ModInformation>();

        public override void OnInitialize()
        {
            MonkiiEvents.OnApplicationStart.Subscribe(OnPreAppStart, int.MaxValue);
            MonkiiBase.OnMonkiiRegistered.Subscribe(ParseMonkii, int.MaxValue);
            MonkiiBase.OnMonkiiUnregistered.Subscribe(OnUnregister, int.MaxValue);
        }

        private static void OnPreAppStart()
        {
            HarmonyLib.Harmony harmony = new("DemeoIntegration");

            harmony.Patch(Assembly.Load("Assembly-CSharp").GetType("Prototyping.RG").GetMethod("Initialize", BindingFlags.Public | BindingFlags.Static),
                typeof(Demeo_Module).GetMethod("InitFix", BindingFlags.NonPublic | BindingFlags.Static).ToNewHarmonyMethod());

            foreach (var m in MonkiiPlugin.RegisteredMonkiis)
            {
                ParseMonkii(m);
            }
            foreach (var m in MonkiiMod.RegisteredMonkiis)
            {
                ParseMonkii(m);
            }
        }

        private static void OnUnregister(MonkiiBase Monkii)
        {
            if (Monkii == null)
                return;

            if (!ModInformation.ContainsKey(Monkii))
                return;

            ModInformation.Remove(Monkii);

            if (ModdingAPI.ExternallyInstalledMods == null)
                ModdingAPI.ExternallyInstalledMods = new List<ModdingAPI.ModInformation>();
            else
                ModdingAPI.ExternallyInstalledMods.Remove(ModInformation[Monkii]);
        }

        private static void ParseMonkii<T>(T Monkii) where T : MonkiiBase
        {

            if (Monkii == null)
                return;

            if (ModInformation.ContainsKey(Monkii))
                return;

            ModdingAPI.ModInformation info = new ModdingAPI.ModInformation();
            info.SetName(Monkii.Info.Name);
            info.SetVersion(Monkii.Info.Version);
            info.SetAuthor(Monkii.Info.Author);
            info.SetDescription(Monkii.Info.DownloadLink);
            info.SetIsNetworkCompatible(MonkiiUtils.PullAttributeFromAssembly<Demeo_LobbyRequirement>(Monkii.MonkiiAssembly.Assembly) == null);

            ModInformation.Add(Monkii, info);

            if (ModdingAPI.ExternallyInstalledMods == null)
                ModdingAPI.ExternallyInstalledMods = new List<ModdingAPI.ModInformation>();
            ModdingAPI.ExternallyInstalledMods.Add(info);
        }

        private static bool InitFix()
        {
            if (MotherbrainGlobalVars.IsRunningOnDesktop)
                RG.SetVrMode(false);
            else
                RG.SetVrMode(RG.XRDeviceIsPresent());
            return true;
        }
    }
}