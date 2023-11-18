using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MonkiiLoader.MonoInternals;
using IllusionPlugin;
using System.IO;
using IllusionInjector;
using MonkiiLoader.Modules;

namespace MonkiiLoader.CompatibilityLayers
{
    internal class IPA_Module : MonkiiModule
    {
        public override void OnInitialize()
        {
			// To-Do:
			// Detect if IPA is already Installed
			// Point AssemblyResolveInfo to already installed IPA Assembly
			// Point GetResolverFromAssembly to Dummy MonkiiCompatibilityLayer.Resolver

			string[] assembly_list =
			{
				"IllusionPlugin",
				"IllusionInjector",
			};
			Assembly base_assembly = typeof(IPA_Module).Assembly;
			foreach (string assemblyName in assembly_list)
				MonoResolveManager.GetAssemblyResolveInfo(assemblyName).Override = base_assembly;

			MonkiiAssembly.CustomMonkiiResolvers += Resolve;
		}

        private ResolvedMonkiis Resolve(Assembly asm)
		{
			IEnumerable<Type> pluginTypes = asm.GetValidTypes(x =>
			{
				Type[] interfaces = x.GetInterfaces();
				return (interfaces != null) && interfaces.Any() && interfaces.Contains(typeof(IPlugin)); // To-Do: Change to Type Reflection based on Setup
			});
			if ((pluginTypes == null) || !pluginTypes.Any())
				return new ResolvedMonkiis(null, null);

			var Monkiis = new List<MonkiiBase>();
			var rotten = new List<RottenMonkii>();
			foreach (var t in pluginTypes)
            {
				var mel = LoadPlugin(asm, t, out RottenMonkii rm);
				if (mel != null)
					Monkiis.Add(mel);
				else
					rotten.Add(rm);
            }
			return new ResolvedMonkiis(Monkiis.ToArray(), rotten.ToArray());
		}

		private MonkiiBase LoadPlugin(Assembly asm, Type pluginType, out RottenMonkii rottenMonkii)
		{
			rottenMonkii = null;
			IPlugin pluginInstance;
			try
			{ pluginInstance = Activator.CreateInstance(pluginType) as IPlugin; }
            catch (Exception ex)
            {
				rottenMonkii = new RottenMonkii(pluginType, "Failed to create a new instance of the IPA Plugin.", ex);
				return null;
            }

			MonkiiProcessAttribute[] processAttrs = null;
			if (pluginInstance is IEnhancedPlugin enPl)
				processAttrs = enPl.Filter?.Select(x => new MonkiiProcessAttribute(x)).ToArray();

			string pluginName = pluginInstance.Name;
			if (string.IsNullOrEmpty(pluginName))
				pluginName = pluginType.FullName;

			string plugin_version = pluginInstance.Version;
			if (string.IsNullOrEmpty(plugin_version))
				plugin_version = asm.GetName().Version.ToString();
			if (string.IsNullOrEmpty(plugin_version) || plugin_version.Equals("0.0.0.0"))
				plugin_version = "1.0.0.0";

			var Monkii = MonkiiBase.CreateWrapper<IPAPluginWrapper>(pluginName, null, plugin_version, processes: processAttrs);

			Monkii.pluginInstance = pluginInstance;
			PluginManager._Plugins.Add(pluginInstance);
			return Monkii;
		}
	}
}