using System;
using MonkiiLoader;

namespace IllusionPlugin
{
	public static class ModPrefs
	{
		public static string GetString(string section, string name, string defaultValue = "", bool autoSave = false)
		{
			MonkiiPreferences_Category category = MonkiiPreferences.GetCategory(section);
			if (category == null)
				category = MonkiiPreferences.CreateCategory(section);
			MonkiiPreferences_Entry<string> entry = category.GetEntry<string>(name);
			if (entry == null)
				entry = category.CreateEntry(name, defaultValue);
			return entry.Value;
		}

		public static int GetInt(string section, string name, int defaultValue = 0, bool autoSave = false)
		{
			MonkiiPreferences_Category category = MonkiiPreferences.GetCategory(section);
			if (category == null)
				category = MonkiiPreferences.CreateCategory(section);
			MonkiiPreferences_Entry<int> entry = category.GetEntry<int>(name);
			if (entry == null)
				entry = category.CreateEntry(name, defaultValue);
			return entry.Value;
		}

		public static float GetFloat(string section, string name, float defaultValue = 0f, bool autoSave = false)
		{
			MonkiiPreferences_Category category = MonkiiPreferences.GetCategory(section);
			if (category == null)
				category = MonkiiPreferences.CreateCategory(section);
			MonkiiPreferences_Entry<float> entry = category.GetEntry<float>(name);
			if (entry == null)
				entry = category.CreateEntry(name, defaultValue);
			return entry.Value;
		}

		public static bool GetBool(string section, string name, bool defaultValue = false, bool autoSave = false)
		{
			MonkiiPreferences_Category category = MonkiiPreferences.GetCategory(section);
			if (category == null)
				category = MonkiiPreferences.CreateCategory(section);
			MonkiiPreferences_Entry<bool> entry = category.GetEntry<bool>(name);
			if (entry == null)
				entry = category.CreateEntry(name, defaultValue);
			return entry.Value;
		}

		public static bool HasKey(string section, string name) => MonkiiPreferences.HasEntry(section, name);

		public static void SetFloat(string section, string name, float value)
		{
			MonkiiPreferences_Category category = MonkiiPreferences.GetCategory(section);
			if (category == null)
				category = MonkiiPreferences.CreateCategory(section);
			MonkiiPreferences_Entry<float> entry = category.GetEntry<float>(name);
			if (entry == null)
				entry = category.CreateEntry(name, value);
			entry.Value = value;
		}

		public static void SetInt(string section, string name, int value)
		{
			MonkiiPreferences_Category category = MonkiiPreferences.GetCategory(section);
			if (category == null)
				category = MonkiiPreferences.CreateCategory(section);
			MonkiiPreferences_Entry<int> entry = category.GetEntry<int>(name);
			if (entry == null)
				entry = category.CreateEntry(name, value);
			entry.Value = value;
		}

		public static void SetString(string section, string name, string value)
		{
			MonkiiPreferences_Category category = MonkiiPreferences.GetCategory(section);
			if (category == null)
				category = MonkiiPreferences.CreateCategory(section);
			MonkiiPreferences_Entry<string> entry = category.GetEntry<string>(name);
			if (entry == null)
				entry = category.CreateEntry(name, value);
			entry.Value = value;
		}

		public static void SetBool(string section, string name, bool value)
		{
			MonkiiPreferences_Category category = MonkiiPreferences.GetCategory(section);
			if (category == null)
				category = MonkiiPreferences.CreateCategory(section);
			MonkiiPreferences_Entry<bool> entry = category.GetEntry<bool>(name);
			if (entry == null)
				entry = category.CreateEntry(name, value);
			entry.Value = value;
		}
	}
}