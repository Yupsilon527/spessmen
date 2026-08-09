using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;

public class LanguageLocale
{
    public string localeCode;
    public Dictionary<string, StringTable> tables;

    public LanguageLocale(string n)
    {
        localeCode = n;
        LoadEntries();
    }
     async void LoadEntries()
    {
        tables = new();
        Locale locale = LocalizationSettings.AvailableLocales.Locales
            .Find(l => l.Identifier.Code == localeCode);

        var tasks = LanguageController.TableNames.Select(name =>
            LocalizationSettings.StringDatabase.GetTableAsync(name, locale).Task
        ).ToArray();

        var output = await Task.WhenAll(tasks);

        for (int i = 0; i < LanguageController.TableNames.Length; i++)
            tables[LanguageController.TableNames[i]] = output[i];
    }
    public bool HasTranslation(string table, string name)
    {
        if (name == null)
        {
            LanguageController.main.Inspect("Tried to translate null value!");
            return false;
        }
        name = name.ToLower();
        if (tables.ContainsKey(table))
        {
            return true;
        }
        LanguageController.main.Inspect($"{name} key does not exist in language {name}");
        return false;
    }
    public string Translate(string table, string name)
    {
        if (name == null)
        {
            LanguageController.main.Inspect("Tried to translate null value!");
            return "MISSINGNO";
        }
        name = name.ToLower();
        if (tables.TryGetValue(table, out var entry))
        {
        return entry?.GetEntry(name)?.GetLocalizedString() ?? name;
        }
        LanguageController.main.Inspect($"(LOCALE MISSING ERROR) {name} key does not exist in language {name}");
#if UNITY_EDITOR
        return "�" + name + "?";
#else
            return name;
#endif
    }
}