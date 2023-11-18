using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace MonkiiLoader.Modules
{
    /// <summary>
    /// A base for external MonkiiLoader modules.
    /// </summary>
    public abstract class MonkiiModule
    {
        private Type moduleType;

        public string Name { get; private set; }
        public Assembly Assembly { get; private set; }
        public Info ModuleInfo { get; private set; }
        protected MonkiiLogger.Instance LoggerInstance { get; private set; }

        protected MonkiiModule() { }

        public virtual void OnInitialize() { }

        internal static MonkiiModule Load(Info moduleInfo)
        {
            if (!File.Exists(moduleInfo.fullPath))
            {
                MonkiiDebug.Msg($"MonkiiModule '{moduleInfo.fullPath}' doesn't exist, ignoring.");
                return null;
            }

            if (moduleInfo.shouldBeRemoved != null && moduleInfo.shouldBeRemoved())
            {
                MonkiiDebug.Msg($"Removing MonkiiModule '{moduleInfo.fullPath}'...");
                try
                {
                    File.Delete(moduleInfo.fullPath);
                }
                catch (Exception ex)
                {
                    MonkiiLogger.Warning($"Failed to remove MonkiiModule '{moduleInfo.fullPath}':\n{ex}");
                }
                return null;
            }

            if (moduleInfo.shouldBeIgnored != null && moduleInfo.shouldBeIgnored())
            {
                MonkiiDebug.Msg($"Ignoring MonkiiModule '{moduleInfo.fullPath}'...");
                return null;
            }

            Assembly asm;
            try
            {
                asm = Assembly.LoadFrom(moduleInfo.fullPath);
            }
            catch (Exception ex)
            {
                MonkiiLogger.Warning($"Failed to load Assembly of MonkiiModule '{moduleInfo.fullPath}':\n{ex}");
                return null;
            }

            var name = asm.GetName().Name;

            var type = asm.GetTypes().FirstOrDefault(x => typeof(MonkiiModule).IsAssignableFrom(x));
            if (type == null)
            {
                MonkiiLogger.Warning($"Failed to load MonkiiModule '{moduleInfo.fullPath}': No type deriving from MonkiiModule found.");
                return null;
            }

            MonkiiModule obj;
            try
            {
                obj = (MonkiiModule)Activator.CreateInstance(type, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, null, null);
            }
            catch (Exception ex)
            {
                MonkiiLogger.Warning($"Failed to initialize MonkiiModule '{moduleInfo.fullPath}':\n{ex}");
                return null;
            }

            obj.moduleType = type;
            obj.Name = name;
            obj.Assembly = asm;
            obj.ModuleInfo = moduleInfo;
            obj.LoggerInstance = new MonkiiLogger.Instance(name, ConsoleColor.Magenta); // Magenta cool :)

            try
            {
                obj.OnInitialize();
            }
            catch (Exception ex)
            {
                obj.LoggerInstance.Error($"Local initialization failed:\n{ex}");
                return null;
            }

            return obj;
        }

        public object SendMessage(string name, params object[] arguments)
        {
            var msg = moduleType.GetMethod(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);

            if (msg == null)
                return null;

            return msg.Invoke(msg.IsStatic ? null : this, arguments);
        }

        public class Info
        {
            public readonly string fullPath;
            internal readonly Func<bool> shouldBeRemoved;
            internal readonly Func<bool> shouldBeIgnored;

            internal Info(string path, Func<bool> shouldBeIgnored = null, Func<bool> shouldBeRemoved = null)
            {
                fullPath = Path.GetFullPath(path);
                this.shouldBeRemoved = shouldBeRemoved;
                this.shouldBeIgnored = shouldBeIgnored;
            }
        }
    }
}
