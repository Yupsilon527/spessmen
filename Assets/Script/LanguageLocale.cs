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
}