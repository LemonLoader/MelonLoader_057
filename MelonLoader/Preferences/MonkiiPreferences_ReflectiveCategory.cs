using System;
using Tomlet;
using Tomlet.Exceptions;
using Tomlet.Models;

namespace MonkiiLoader.Preferences
{
    public class MonkiiPreferences_ReflectiveCategory
    {
        private Type SystemType;
        private object value;
        internal IO.File File = null;
        
        public string Identifier { get;  internal set; }
        public string DisplayName { get; internal set; }

        internal static MonkiiPreferences_ReflectiveCategory Create<T>(string categoryName, string displayName) => new MonkiiPreferences_ReflectiveCategory(typeof(T), categoryName, displayName);
        
        private MonkiiPreferences_ReflectiveCategory(Type type, string categoryName, string displayName)
        {
            SystemType = type;
            Identifier = categoryName;
            DisplayName = displayName;

            IO.File currentFile = File;
            if (currentFile == null)
                currentFile = MonkiiPreferences.DefaultFile;
            if (!(currentFile.TryGetCategoryTable(Identifier) is { } table))
                LoadDefaults();
            else
                Load(table);

            MonkiiPreferences.ReflectiveCategories.Add(this);
        }

        internal void LoadDefaults() => value = Activator.CreateInstance(SystemType);

        internal void Load(TomlValue tomlValue)
        {
            try { value = TomletMain.To(SystemType, tomlValue); }
            catch (TomlTypeMismatchException)
            {
                return;
            }
            catch (TomlNoSuchValueException)
            {
                return;
            }
            catch (TomlEnumParseException)
            {
                return;
            }
        }

        internal TomlValue Save()
        {
            if(value == null)
                LoadDefaults();
            
            return TomletMain.ValueFrom(SystemType, value);
        }

        public T GetValue<T>() where T : new()
        {
            if (typeof(T) != SystemType)
                return default;
            if (value == null)
                LoadDefaults();
            return (T) value;
        }
        
        public void SetFilePath(string filepath, bool autoload = true, bool printmsg = true)
        {
            if (File != null)
            {
                IO.File oldfile = File;
                File = null;
                if (!MonkiiPreferences.IsFileInUse(oldfile))
                {
                    oldfile.FileWatcher.Destroy();
                    MonkiiPreferences.PrefFiles.Remove(oldfile);
                }
            }
            if (!string.IsNullOrEmpty(filepath) && !MonkiiPreferences.IsFilePathDefault(filepath))
            {
                File = MonkiiPreferences.GetPrefFileFromFilePath(filepath);
                if (File == null)
                {
                    File = new IO.File(filepath);
                    MonkiiPreferences.PrefFiles.Add(File);
                }
            }
            if (autoload)
                MonkiiPreferences.LoadFileAndRefreshCategories(File, printmsg);
        }

        public void ResetFilePath()
        {
            if (File == null)
                return;
            IO.File oldfile = File;
            File = null;
            if (!MonkiiPreferences.IsFileInUse(oldfile))
            {
                oldfile.FileWatcher.Destroy();
                MonkiiPreferences.PrefFiles.Remove(oldfile);
            }
            MonkiiPreferences.LoadFileAndRefreshCategories(MonkiiPreferences.DefaultFile);
        }
        
        public void SaveToFile(bool printmsg = true)
        {
            IO.File currentfile = File;
            if (currentfile == null)
                currentfile = MonkiiPreferences.DefaultFile;

            currentfile.document.PutValue(Identifier, Save());
            try
            {
                currentfile.Save();
            }
            catch (Exception ex)
            {
                MonkiiLogger.Error($"Error while Saving Preferences to {currentfile.FilePath}: {ex}");
                currentfile.WasError = true;
            }
            if (printmsg)
                MonkiiLogger.Msg($"MonkiiPreferences Saved to {currentfile.FilePath}");

            MonkiiPreferences.OnPreferencesSaved.Invoke(currentfile.FilePath);
        }

        public void LoadFromFile(bool printmsg = true)
        {
            IO.File currentfile = File;
            if (currentfile == null)
                currentfile = MonkiiPreferences.DefaultFile;
            MonkiiPreferences.LoadFileAndRefreshCategories(currentfile, printmsg);
        }

        public void DestroyFileWatcher() => File?.FileWatcher.Destroy();
    }
}