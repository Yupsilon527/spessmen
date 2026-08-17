using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
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
    }
    public async Task LoadEntries()
    {
        LanguageController.main.Inspect("Init locale " + localeCode);
        tables = new();

        Locale locale = LocalizationSettings.AvailableLocales.Locales
            .Find(l => l.Identifier.Code == localeCode);

        foreach (var name in LanguageController.TableNames)
        {
            try
            {
                var handle = LocalizationSettings.StringDatabase.GetTableAsync(name, locale);
                var table = await handle.Task;

                if (handle.Status != UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
                {
                    LanguageController.main.Inspect($"FAILED table '{name}': {handle.OperationException}");
                    continue;
                }

                tables[name] = table;
                LanguageController.main.Inspect($"loaded table '{name}' ok");
            }
            catch (System.Exception e)
            {
                LanguageController.main.Inspect($"EXCEPTION loading table '{name}': {e}");
            }
        }

        LanguageController.main.Inspect("done loading tables for " + localeCode);
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
        LanguageController.main.Inspect($"{name} key does not exist in language {localeCode}");
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
        LanguageController.main.Inspect($"(LOCALE MISSING ERROR) {name} key does not exist in language {localeCode}");
#if UNITY_EDITOR
        return "�" + name + "?";
#else
            return name;
#endif
    }
}