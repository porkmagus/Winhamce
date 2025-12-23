using Winhance.Core.Features.Common.Interfaces;

namespace Winhance.WinUI3.Features.Common.Services;

public class LocalizationService : ILocalizationService
{
    private readonly Dictionary<string, Dictionary<string, string>> _translations;
    private string _currentLanguage = "en";

    public event EventHandler? LanguageChanged;

    public LocalizationService()
    {
        _translations = new Dictionary<string, Dictionary<string, string>>();
        LoadTranslations();
    }

    public string GetString(string key)
    {
        if (_translations.TryGetValue(_currentLanguage, out var languageDict))
        {
            if (languageDict.TryGetValue(key, out var value))
            {
                return value;
            }
        }
        return key; // Return key if translation not found
    }

    public void SetLanguage(string languageCode)
    {
        if (_currentLanguage != languageCode)
        {
            _currentLanguage = languageCode;
            LanguageChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public string GetCurrentLanguage() => _currentLanguage;

    public string[] GetAvailableLanguages() => _translations.Keys.ToArray();

    private void LoadTranslations()
    {
        // Load translations from JSON files
        // This is a simplified implementation
        _translations["en"] = new Dictionary<string, string>
        {
            ["Nav_SoftwareApps"] = "Software Apps",
            ["Nav_Optimize"] = "Optimize",
            ["Nav_Customize"] = "Customize",
            ["Nav_AdvancedTools"] = "Advanced Tools",
            ["Dialog_Yes"] = "Yes",
            ["Dialog_No"] = "No",
            ["Dialog_OK"] = "OK",
            ["Dialog_Cancel"] = "Cancel",
            ["Dialog_Error_Title"] = "Error",
            ["Dialog_Warning_Title"] = "Warning",
            ["Dialog_Info_Title"] = "Information"
        };
    }
}

public class LocalizationManager
{
    private static LocalizationManager? _instance;
    private ILocalizationService? _localizationService;

    public static LocalizationManager Instance => _instance ??= new LocalizationManager();

    private LocalizationManager() { }

    public void Initialize(ILocalizationService localizationService)
    {
        _localizationService = localizationService;
    }

    public string GetString(string key)
    {
        return _localizationService?.GetString(key) ?? key;
    }
}
