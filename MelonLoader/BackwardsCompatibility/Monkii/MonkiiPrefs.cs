using System;
using System.Collections.Generic;

namespace MonkiiLoader
{
    [Obsolete("MonkiiLoader.MonkiiPrefs is Only Here for Compatibility Reasons. Please use MonkiiLoader.MonkiiPreferences instead.")]
    public class MonkiiPrefs
    {
        [Obsolete("MonkiiLoader.MonkiiPrefs.RegisterCategory is Only Here for Compatibility Reasons. Please use MonkiiLoader.MonkiiPreferences.CreateCategory instead.")]
        public static void RegisterCategory(string name, string displayText) => MonkiiPreferences.CreateCategory(name, displayText);
        [Obsolete("MonkiiLoader.MonkiiPrefs.RegisterString is Only Here for Compatibility Reasons. Please use MonkiiLoader.MonkiiPreferences.CreateEntry instead.")]
        public static void RegisterString(string section, string name, string defaultValue, string displayText = null, bool hideFromList = false) => MonkiiPreferences.CreateEntry(section, name, defaultValue, displayText, hideFromList);
        [Obsolete("MonkiiLoader.MonkiiPrefs.RegisterBool is Only Here for Compatibility Reasons. Please use MonkiiLoader.MonkiiPreferences.CreateEntry instead.")]
        public static void RegisterBool(string section, string name, bool defaultValue, string displayText = null, bool hideFromList = false) => MonkiiPreferences.CreateEntry(section, name, defaultValue, displayText, hideFromList);
        [Obsolete("MonkiiLoader.MonkiiPrefs.RegisterInt is Only Here for Compatibility Reasons. Please use MonkiiLoader.MonkiiPreferences.CreateEntry instead.")]
        public static void RegisterInt(string section, string name, int defaultValue, string displayText = null, bool hideFromList = false) => MonkiiPreferences.CreateEntry(section, name, defaultValue, displayText, hideFromList);
        [Obsolete("MonkiiLoader.MonkiiPrefs.RegisterFloat is Only Here for Compatibility Reasons. Please use MonkiiLoader.MonkiiPreferences.CreateEntry instead.")]
        public static void RegisterFloat(string section, string name, float defaultValue, string displayText = null, bool hideFromList = false) => MonkiiPreferences.CreateEntry(section, name, defaultValue, displayText, hideFromList);
        [Obsolete("MonkiiLoader.MonkiiPrefs.HasKey is Only Here for Compatibility Reasons. Please use MonkiiLoader.MonkiiPreferences.HasEntry instead.")]
        public static bool HasKey(string section, string name) => MonkiiPreferences.HasEntry(section, name);
        [Obsolete("MonkiiLoader.MonkiiPrefs.GetPreferences is Only Here for Compatibility Reasons. Please use MonkiiLoader.MonkiiPreferences.Categories instead.")]
        public static Dictionary<string, Dictionary<string, MonkiiPreference>> GetPreferences()
        {
            Dictionary<string, Dictionary<string, MonkiiPreference>> output = new Dictionary<string, Dictionary<string, MonkiiPreference>>();
            if (MonkiiPreferences.Categories.Count <= 0)
                return output;
            foreach (MonkiiPreferences_Category category in MonkiiPreferences.Categories)
            {
                Dictionary<string, MonkiiPreference> newprefsdict = new Dictionary<string, MonkiiPreference>();
                foreach (MonkiiPreferences_Entry entry in category.Entries)
                {
                    Type reflectedType = entry.GetReflectedType();
                    if ((reflectedType != typeof(string))
                        && (reflectedType != typeof(bool))
                        && (reflectedType != typeof(int))
                        && (reflectedType != typeof(float))
                        && (reflectedType != typeof(double))
                        && (reflectedType != typeof(long)))
                        continue;
                    MonkiiPreference newpref = new MonkiiPreference(entry);
                    newprefsdict.Add(entry.Identifier, newpref);
                }
                output.Add(category.Identifier, newprefsdict);
            }
            return output;
        }
        [Obsolete("MonkiiLoader.MonkiiPrefs.GetCategoryDisplayName is Only Here for Compatibility Reasons. Please use MonkiiLoader.MonkiiPreferences.GetCategoryDisplayName instead.")]
        public static string GetCategoryDisplayName(string key) => MonkiiPreferences.GetCategory(key)?.DisplayName;
        [Obsolete("MonkiiLoader.MonkiiPrefs.SaveConfig is Only Here for Compatibility Reasons. Please use MonkiiLoader.MonkiiPreferences.Save instead.")]
        public static void SaveConfig() => MonkiiPreferences.Save();
        [Obsolete("MonkiiLoader.MonkiiPrefs.GetString is Only Here for Compatibility Reasons. Please use MonkiiLoader.MonkiiPreferences.GetEntryString instead.")]
        public static string GetString(string section, string name)
        {
            MonkiiPreferences_Category category = MonkiiPreferences.GetCategory(section);
            if (category == null)
                return null;
            MonkiiPreferences_Entry entry = category.GetEntry(name);
            if (entry == null)
                return null;
            return entry.GetValueAsString();
        }
        [Obsolete("MonkiiLoader.MonkiiPrefs.SetString is Only Here for Compatibility Reasons. Please use MonkiiLoader.MonkiiPreferences.SetEntryString instead.")]
        public static void SetString(string section, string name, string value)
        {
            MonkiiPreferences_Category category = MonkiiPreferences.GetCategory(section);
            if (category == null)
                return;
            MonkiiPreferences_Entry entry = category.GetEntry(name);
            if (entry == null)
                return;
            switch (entry)
            {
                case MonkiiPreferences_Entry<string> stringEntry:
                    stringEntry.Value = value;
                    break;
                case MonkiiPreferences_Entry<int> intEntry:
                    if (int.TryParse(value, out var parsedInt))
                        intEntry.Value = parsedInt;
                    break;
                case MonkiiPreferences_Entry<float> floatEntry:
                    if (float.TryParse(value, out var parsedFloat))
                        floatEntry.Value = parsedFloat;
                    break;
                case MonkiiPreferences_Entry<bool> boolEntry:
                    if (value.ToLower().StartsWith("true") || value.ToLower().StartsWith("false"))
                        boolEntry.Value = value.ToLower().StartsWith("true");
                    break;
            }
        }
        [Obsolete("MonkiiLoader.MonkiiPrefs.GetBool is Only Here for Compatibility Reasons. Please use MonkiiLoader.MonkiiPreferences.GetEntryBool instead.")]
        public static bool GetBool(string section, string name) => MonkiiPreferences.GetEntryValue<bool>(section, name);
        [Obsolete("MonkiiLoader.MonkiiPrefs.SetBool is Only Here for Compatibility Reasons. Please use MonkiiLoader.MonkiiPreferences.SetEntryBool instead.")]
        public static void SetBool(string section, string name, bool value) => MonkiiPreferences.SetEntryValue(section, name, value);
        [Obsolete("MonkiiLoader.MonkiiPrefs.GetInt is Only Here for Compatibility Reasons. Please use MonkiiLoader.MonkiiPreferences.GetEntryInt instead.")]
        public static int GetInt(string section, string name) => MonkiiPreferences.GetEntryValue<int>(section, name);
        [Obsolete("MonkiiLoader.MonkiiPrefs.SetInt is Only Here for Compatibility Reasons. Please use MonkiiLoader.MonkiiPreferences.SetEntryInt instead.")]
        public static void SetInt(string section, string name, int value) => MonkiiPreferences.SetEntryValue(section, name, value);
        [Obsolete("MonkiiLoader.MonkiiPrefs.GetFloat is Only Here for Compatibility Reasons. Please use MonkiiLoader.MonkiiPreferences.GetEntryFloat instead.")]
        public static float GetFloat(string section, string name) => MonkiiPreferences.GetEntryValue<float>(section, name);
        [Obsolete("MonkiiLoader.MonkiiPrefs.GetEntryFloat is Only Here for Compatibility Reasons. Please use MonkiiLoader.MonkiiPreferences.SetEntryFloat instead.")]
        public static void SetFloat(string section, string name, float value) => MonkiiPreferences.SetEntryValue(section, name, value);
        [Obsolete("MonkiiLoader.MonkiiPrefs.MonkiiPreferenceType is Only Here for Compatibility Reasons. Please use MonkiiLoader.MonkiiPreferences_Entry.TypeEnum instead.")]
        public enum MonkiiPreferenceType
        {
            STRING,
            BOOL,
            INT,
            FLOAT
        }
        [Obsolete("MonkiiLoader.MonkiiPrefs.MonkiiPreference is Only Here for Compatibility Reasons. Please use MonkiiLoader.MonkiiPreferences_Entry instead.")]
        public class MonkiiPreference
        {
            [Obsolete("MonkiiLoader.MonkiiPrefs.MonkiiPreference.Value is Only Here for Compatibility Reasons. Please use MonkiiLoader.MonkiiPreferences_Entry.GetValue instead.")]
            public string Value { get => GetString(Entry.Category.Identifier, Entry.Identifier); set => SetString(Entry.Category.Identifier, Entry.Identifier, value); }
            [Obsolete("MonkiiLoader.MonkiiPrefs.MonkiiPreference.ValueEdited is Only Here for Compatibility Reasons. Please use MonkiiLoader.MonkiiPreferences_Entry.GetValueEdited instead.")]
            public string ValueEdited { get => GetEditedString(Entry.Category.Identifier, Entry.Identifier); set => SetEditedString(Entry.Category.Identifier, Entry.Identifier, value); }
            [Obsolete("MonkiiLoader.MonkiiPrefs.MonkiiPreference.Type is Only Here for Compatibility Reasons. Please use MonkiiLoader.MonkiiPreferences_Entry.GetReflectedType instead.")]
            public MonkiiPreferenceType Type
            {
                get
                {
                    if (Entry.GetReflectedType() == typeof(string))
                        return MonkiiPreferenceType.STRING;
                    else if (Entry.GetReflectedType() == typeof(bool))
                        return MonkiiPreferenceType.BOOL;
                    else if ((Entry.GetReflectedType() == typeof(float))
                        || (Entry.GetReflectedType() == typeof(double)))
                        return MonkiiPreferenceType.FLOAT;
                    else if ((Entry.GetReflectedType() == typeof(int))
                        || (Entry.GetReflectedType() == typeof(long))
                        || (Entry.GetReflectedType() == typeof(byte)))
                        return MonkiiPreferenceType.INT;
                    return (MonkiiPreferenceType)4;
                }
            }
            [Obsolete("MonkiiLoader.MonkiiPrefs.MonkiiPreference.Hidden is Only Here for Compatibility Reasons. Please use MonkiiLoader.MonkiiPreferences_Entry.IsHidden instead.")]
            public bool Hidden { get => Entry.IsHidden; }
            [Obsolete("MonkiiLoader.MonkiiPrefs.MonkiiPreference.DisplayText is Only Here for Compatibility Reasons. Please use MonkiiLoader.MonkiiPreferences_Entry.DisplayName instead.")]
            public string DisplayText { get => Entry.DisplayName; }

            internal MonkiiPreference(MonkiiPreferences_Entry entry) => Entry = entry;
            internal MonkiiPreference(MonkiiPreference pref) => Entry = pref.Entry;
            private MonkiiPreferences_Entry Entry = null;
            private static string GetEditedString(string section, string name)
            {
                MonkiiPreferences_Category category = MonkiiPreferences.GetCategory(section);
                if (category == null)
                    return null;
                MonkiiPreferences_Entry entry = category.GetEntry(name);
                if (entry == null)
                    return null;

                return entry.GetEditedValueAsString();
            }
            private static void SetEditedString(string section, string name, string value)
            {
                MonkiiPreferences_Category category = MonkiiPreferences.GetCategory(section);
                if (category == null)
                    return;
                MonkiiPreferences_Entry entry = category.GetEntry(name);
                if (entry == null)
                    return;
                switch (entry)
                {
                    case MonkiiPreferences_Entry<string> stringEntry:
                        stringEntry.EditedValue = value;
                        break;
                    case MonkiiPreferences_Entry<int> intEntry:
                        if (int.TryParse(value, out var parsedInt))
                            intEntry.EditedValue = parsedInt;
                        break;
                    case MonkiiPreferences_Entry<float> floatEntry:
                        if (float.TryParse(value, out var parsedFloat))
                            floatEntry.EditedValue = parsedFloat;
                        break;
                    case MonkiiPreferences_Entry<bool> boolEntry:
                        if (value.ToLower().StartsWith("true") || value.ToLower().StartsWith("false"))
                            boolEntry.EditedValue = value.ToLower().StartsWith("true");
                        break;
                }
            }
        }
    }
}