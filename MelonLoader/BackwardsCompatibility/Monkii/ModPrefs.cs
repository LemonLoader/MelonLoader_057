using System;
using System.Collections.Generic;
using System.Linq;
#pragma warning disable 0108

namespace MonkiiLoader
{
    [Obsolete("MonkiiLoader.ModPrefs is Only Here for Compatibility Reasons. Please use MonkiiLoader.MonkiiPreferences instead.")]
    public class ModPrefs : MonkiiPrefs
    {
        [Obsolete("MonkiiLoader.ModPrefs.GetPrefs is Only Here for Compatibility Reasons. Please use MonkiiLoader.MonkiiPreferences instead.")]
        public static Dictionary<string, Dictionary<string, PrefDesc>> GetPrefs()
        {
            Dictionary<string, Dictionary<string, PrefDesc>> output = new Dictionary<string, Dictionary<string, PrefDesc>>();
            Dictionary<string, Dictionary<string, MonkiiPreference>> prefs = GetPreferences();
            for (int i = 0; i < prefs.Values.Count; i++)
            {
                Dictionary<string, MonkiiPreference> prefsdict = prefs.Values.ElementAt(i);
                Dictionary<string, PrefDesc> newprefsdict = new Dictionary<string, PrefDesc>();
                for (int j = 0; j < prefsdict.Values.Count; j++)
                {
                    MonkiiPreference pref = prefsdict.Values.ElementAt(j);
                    PrefDesc newpref = new PrefDesc(pref);
                    newpref.ValueEdited = pref.ValueEdited;
                    newprefsdict.Add(prefsdict.Keys.ElementAt(j), newpref);
                }
                output.Add(prefs.Keys.ElementAt(i), newprefsdict);
            }
            return output;
        }
        [Obsolete("MonkiiLoader.ModPrefs.RegisterPrefString is Only Here for Compatibility Reasons. Please use MonkiiLoader.MonkiiPreferences.CreateEntry instead.")]
        public static void RegisterPrefString(string section, string name, string defaultValue, string displayText = null, bool hideFromList = false) => RegisterString(section, name, defaultValue, displayText, hideFromList);
        [Obsolete("MonkiiLoader.ModPrefs.RegisterPrefBool is Only Here for Compatibility Reasons. Please use MonkiiLoader.MonkiiPreferences.CreateEntry instead.")]
        public static void RegisterPrefBool(string section, string name, bool defaultValue, string displayText = null, bool hideFromList = false) => RegisterBool(section, name, defaultValue, displayText, hideFromList);
        [Obsolete("MonkiiLoader.ModPrefs.RegisterPrefInt is Only Here for Compatibility Reasons. Please use MonkiiLoader.MonkiiPreferences.CreateEntry instead.")]
        public static void RegisterPrefInt(string section, string name, int defaultValue, string displayText = null, bool hideFromList = false) => RegisterInt(section, name, defaultValue, displayText, hideFromList);
        [Obsolete("MonkiiLoader.ModPrefs.RegisterPrefFloat is Only Here for Compatibility Reasons. Please use MonkiiLoader.MonkiiPreferences.CreateEntry instead.")]
        public static void RegisterPrefFloat(string section, string name, float defaultValue, string displayText = null, bool hideFromList = false) => RegisterFloat(section, name, defaultValue, displayText, hideFromList);
        [Obsolete("MonkiiLoader.ModPrefs.PrefType is Only Here for Compatibility Reasons. Please use MonkiiLoader.MonkiiPreferences_Entry.TypeEnum instead.")]
        public enum PrefType
        {
            STRING,
            BOOL,
            INT,
            FLOAT
        }
        [Obsolete("MonkiiLoader.ModPrefs.PrefDesc is Only Here for Compatibility Reasons. Please use MonkiiLoader.MonkiiPreferences_Entry instead.")]
        public class PrefDesc : MonkiiPreference
        {
            [Obsolete("MonkiiLoader.ModPrefs.PrefDesc.Type is Only Here for Compatibility Reasons. Please use MonkiiLoader.MonkiiPreferences_Entry.Type instead.")]
            public PrefType Type { get => (PrefType)base.Type; }
            [Obsolete("MonkiiLoader.ModPrefs.PrefDesc is Only Here for Compatibility Reasons. Please use MonkiiLoader.MonkiiPreferences_Entry instead.")]
            public PrefDesc(MonkiiPreferences_Entry entry) : base(entry) { }
            [Obsolete("MonkiiLoader.ModPrefs.PrefDesc is Only Here for Compatibility Reasons. Please use MonkiiLoader.MonkiiPreferences_Entry instead.")]
            public PrefDesc(MonkiiPreference pref) : base(pref) { }
        }
    }
}