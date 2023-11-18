using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MonkiiLoader.Modules;
using MonkiiLoader.MonoInternals;
using ModHelper;

namespace MonkiiLoader.CompatibilityLayers
{
    internal class Muse_Dash_Mono_Module : MonkiiModule
    {
        public override void OnInitialize()
        {
            // To-Do:
            // Detect if MuseDashModLoader is already Installed
            // Point AssemblyResolveInfo to already installed MuseDashModLoader Assembly
            // Inject Custom Resolver

            string[] assembly_list =
            {
                "ModHelper",
                "ModLoader",
            };
            Assembly base_assembly = typeof(Muse_Dash_Mono_Module).Assembly;
            foreach (string assemblyName in assembly_list)
                MonoResolveManager.GetAssemblyResolveInfo(assemblyName).Override = base_assembly;

            MonkiiAssembly.CustomMonkiiResolvers += Resolve;
        }

        private ResolvedMonkiis Resolve(Assembly asm)
        {
            IEnumerable<Type> modTypes = asm.GetValidTypes(x =>
            {
                Type[] interfaces = x.GetInterfaces();
                return (interfaces != null) && interfaces.Any() && interfaces.Contains(typeof(IMod));  // To-Do: Change to Type Reflection based on Setup
            });
            if ((modTypes == null) || !modTypes.Any())
                return new ResolvedMonkiis(null, null);

            var Monkiis = new List<MonkiiBase>();
            var rotten = new List<RottenMonkii>();
            foreach (var t in modTypes)
            {
                var mel = LoadMod(asm, t, out RottenMonkii rm);
                if (mel != null)
                    Monkiis.Add(mel);
                else
                    rotten.Add(rm);
            }
            return new ResolvedMonkiis(Monkiis.ToArray(), rotten.ToArray());
        }

        private MonkiiBase LoadMod(Assembly asm, Type modType, out RottenMonkii rottenMonkii)
        {
            rottenMonkii = null;

            IMod modInstance;
            try { modInstance = Activator.CreateInstance(modType) as IMod; }
            catch (Exception ex)
            {
                rottenMonkii = new RottenMonkii(modType, "Failed to create an instance of the MMDL Mod.", ex);
                return null;
            }

            var modName = modInstance.Name;

            if (string.IsNullOrEmpty(modName))
                modName = modType.FullName;

            var modVersion = asm.GetName().Version.ToString();
            if (string.IsNullOrEmpty(modVersion) || modVersion.Equals("0.0.0.0"))
                modVersion = "1.0.0.0";

            var Monkii = MonkiiBase.CreateWrapper<MuseDashModWrapper>(modName, null, modVersion);
            Monkii.modInstance = modInstance;
            ModLoader.ModLoader.mods.Add(modInstance);
            ModLoader.ModLoader.LoadDependency(asm);
            return Monkii;
        }
    }
}