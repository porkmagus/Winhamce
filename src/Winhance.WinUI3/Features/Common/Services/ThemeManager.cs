using Microsoft.UI.Xaml;
using Winhance.Core.Features.Common.Interfaces;

namespace Winhance.WinUI3.Features.Common.Services;

public class ThemeManager : IThemeManager, IDisposable
{
    private bool _isDarkTheme;
    private readonly IUserPreferencesService _preferencesService;

    public event EventHandler<bool>? ThemeChanged;

    public bool IsDarkTheme
    {
        get => _isDarkTheme;
        private set
        {
            if (_isDarkTheme != value)
            {
                _isDarkTheme = value;
                ThemeChanged?.Invoke(this, value);
            }
        }
    }

    public ThemeManager(IUserPreferencesService preferencesService)
    {
        _preferencesService = preferencesService;
        _ = InitializeThemeAsync();
    }

    private async Task InitializeThemeAsync()
    {
        var savedTheme = await _preferencesService.GetPreferenceAsync("Theme", "Dark");
        IsDarkTheme = savedTheme == "Dark";
    }

    public async Task SetThemeAsync(bool isDark)
    {
        IsDarkTheme = isDark;
        await _preferencesService.SavePreferenceAsync("Theme", isDark ? "Dark" : "Light");
        
        // Apply theme to all windows
        foreach (var window in WindowHelper.GetActiveWindows())
        {
            if (window.Content is FrameworkElement rootElement)
            {
                rootElement.RequestedTheme = isDark ? ElementTheme.Dark : ElementTheme.Light;
            }
        }
    }

    public void Dispose()
    {
        ThemeChanged = null;
    }
}

internal static class WindowHelper
{
    public static IEnumerable<Window> GetActiveWindows()
    {
        // WinUI 3 doesn't provide a built-in way to enumerate windows
        // This is a simplified implementation
        // In production, you'd track windows in a collection
        yield break;
    }
}
