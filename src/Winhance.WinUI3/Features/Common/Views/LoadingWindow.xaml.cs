using Microsoft.UI.Xaml;
using Winhance.Core.Features.Common.Interfaces;

namespace Winhance.WinUI3.Features.Common.Views;

public sealed partial class LoadingWindow : Window
{
    private readonly IThemeManager _themeManager;
    private readonly ITaskProgressService _progressService;
    private readonly ILocalizationService _localizationService;

    public LoadingWindow(
        IThemeManager themeManager,
        ITaskProgressService progressService,
        ILocalizationService localizationService)
    {
        InitializeComponent();
        
        _themeManager = themeManager;
        _progressService = progressService;
        _localizationService = localizationService;

        // Apply theme
        if (Content is FrameworkElement rootElement)
        {
            rootElement.RequestedTheme = _themeManager.IsDarkTheme 
                ? ElementTheme.Dark 
                : ElementTheme.Light;
        }

        // Bind to progress service updates
        _progressService.ProgressChanged += OnProgressChanged;
    }

    private void OnProgressChanged(object? sender, (int current, int total, string message) e)
    {
        DispatcherQueue.TryEnqueue(() =>
        {
            ProgressBar.Maximum = e.total;
            ProgressBar.Value = e.current;
            StatusText.Text = e.message;
        });
    }
}
