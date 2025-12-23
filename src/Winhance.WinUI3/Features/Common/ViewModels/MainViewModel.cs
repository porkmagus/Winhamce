using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using Winhance.Core.Features.Common.Interfaces;

namespace Winhance.WinUI3.Features.Common.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly ILocalizationService _localizationService;
    private readonly ILogService _logService;

    [ObservableProperty]
    private object? _currentView;

    [ObservableProperty]
    private string _currentViewName = "SoftwareApps";

    [ObservableProperty]
    private bool _isDialogOverlayVisible;

    [ObservableProperty]
    private bool _isMoreMenuFlyoutOpen;

    [ObservableProperty]
    private bool _isAdvancedToolsFlyoutOpen;

    public ObservableCollection<NavigationItem> NavigationItems { get; }

    public MainViewModel(
        ILocalizationService localizationService,
        ILogService logService)
    {
        _localizationService = localizationService;
        _logService = logService;

        NavigationItems = new ObservableCollection<NavigationItem>
        {
            new("SoftwareApps", _localizationService.GetString("Nav_SoftwareApps")),
            new("Optimize", _localizationService.GetString("Nav_Optimize")),
            new("Customize", _localizationService.GetString("Nav_Customize")),
            new("AdvancedTools", _localizationService.GetString("Nav_AdvancedTools"))
        };
    }

    public async Task InitializeApplicationAsync()
    {
        _logService.Log(Core.Features.Common.Enums.LogLevel.Info, "Initializing MainViewModel");
        await NavigateToAsync("SoftwareApps");
    }

    public async Task NavigateToAsync(string viewName)
    {
        CurrentViewName = viewName;
        _logService.Log(Core.Features.Common.Enums.LogLevel.Info, $"Navigating to {viewName}");
        
        // View navigation will be handled by the MainWindow
    }

    [RelayCommand]
    private void CloseMoreMenuFlyout()
    {
        IsMoreMenuFlyoutOpen = false;
    }

    [RelayCommand]
    private void CloseAdvancedToolsFlyout()
    {
        IsAdvancedToolsFlyoutOpen = false;
    }
}

public record NavigationItem(string Tag, string DisplayName);
