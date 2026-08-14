using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization;
using System.Linq;

public class LanguageController : Initializable
{
    public static readonly string[] TableNames = { "Environments", "Leaderboard", "UI Table" , "Abilities", "Modifiers", "Racers", "Parts" };
    public static LanguageController main;
    protected override void Initialize()
    {
        base.Initialize();
        if (main == null)
        {
            main = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private async void Start()
    {
        await LocalizationSettings.InitializationOperation.Task;
        RebuildLocales();
    }

    LanguageLocale active;
    HashSet< LanguageLocale> languageData;
    public void RebuildLocales()
    {
        languageData = new();
        foreach (var loc in LocalizationSettings.AvailableLocales.Locales)
        {
            languageData.Add(new(loc.Identifier.Code));
        }
        OnChangeLanguage(LocalizationSettings.Instance.GetSelectedLocale());
    }

    public void OnChangeLanguage(Locale locale)
    {
        Inspect("Change language " + locale.Identifier.Code);
        active = languageData.FirstOrDefault(l => l.localeCode == locale.Identifier.Code);
    }
    public void ChangeLanguage(string localeCode)
    {
        Locale locale = LocalizationSettings.AvailableLocales.Locales
            .Find(l => l.Identifier.Code == localeCode);
        OnChangeLanguage(locale);
        if (locale != null)
        {
            LocalizationSettings.SelectedLocale = locale;
        }
        else
        {
            Debug.LogWarning($"Locale '{localeCode}' not found.");
        }
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        LocalizationSettings.SelectedLocaleChanged += OnChangeLanguage;
    }

    private void OnDisable()
    {
        LocalizationSettings.SelectedLocaleChanged -= OnChangeLanguage;
    }
    public string TranslateName(string table, string stringID)
    {
        string name = stringID.ToLower()
        .Replace(' ', '_');
        return Translate(table, name);
    }
    public string Translate(string table, string stringID)
    {
        if (active == null) return stringID;
        return active?.Translate(table,stringID) ?? stringID;
    }
    public bool HasTranslation(string table, string stringID)
    {
        return active?.HasTranslation(table,stringID) ?? false;
    }
}
