using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Winhance.Core.Features.Common.Interfaces;
using Winhance.WinUI3.Features.Common.ViewModels;

namespace Winhance.WinUI3.Features.Common.Views;

/// <summary>
/// Main application window for Winhance WinUI 3
/// </summary>
public sealed partial class MainWindow : Window
{
    private readonly IApplicationCloseService _applicationCloseService;
    private readonly MainViewModel _viewModel;

    public MainWindow(
        IApplicationCloseService applicationCloseService,
        MainViewModel viewModel)
    {
        InitializeComponent();
        
        _applicationCloseService = applicationCloseService;
        _viewModel = viewModel;
        
        // Set the root content DataContext
        if (Content is FrameworkElement rootElement)
        {
            rootElement.DataContext = _viewModel;
        }

        // Handle window closing
        this.Closed += async (s, e) =>
        {
            await _applicationCloseService.CheckOperationsAndCloseAsync();
        };
    }

    public MainViewModel ViewModel => _viewModel;

    private void NavView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
        if (args.SelectedItemContainer != null)
        {
            var navItemTag = args.SelectedItemContainer.Tag?.ToString();
            if (!string.IsNullOrEmpty(navItemTag))
            {
                NavigateToPage(navItemTag);
            }
        }
    }

    private void NavigateToPage(string pageTag)
    {
        Type? pageType = pageTag switch
        {
            "SoftwareApps" => typeof(SoftwareApps.Views.SoftwareAppsView),
            "Optimize" => typeof(Optimize.Views.OptimizeView),
            "Customize" => typeof(Customize.Views.CustomizeView),
            "AdvancedTools" => typeof(AdvancedTools.Views.AdvancedToolsView),
            "Settings" => typeof(WinhanceSettingsView),
            _ => null
        };

        if (pageType != null)
        {
            ContentFrame.Navigate(pageType);
        }
    }
}
