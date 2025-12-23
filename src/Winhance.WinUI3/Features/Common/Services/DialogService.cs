using Microsoft.UI.Xaml.Controls;
using Winhance.Core.Features.Common.Interfaces;

namespace Winhance.WinUI3.Features.Common.Services;

public class DialogService : IDialogService
{
    private readonly ILocalizationService _localizationService;

    public DialogService(ILocalizationService localizationService)
    {
        _localizationService = localizationService;
    }

    public async Task<bool> ShowConfirmationAsync(string title, string message)
    {
        var dialog = new ContentDialog
        {
            Title = title,
            Content = message,
            PrimaryButtonText = _localizationService.GetString("Dialog_Yes"),
            CloseButtonText = _localizationService.GetString("Dialog_No"),
            DefaultButton = ContentDialogButton.Primary,
            XamlRoot = GetCurrentXamlRoot()
        };

        var result = await dialog.ShowAsync();
        return result == ContentDialogResult.Primary;
    }

    public async Task ShowMessageAsync(string title, string message)
    {
        var dialog = new ContentDialog
        {
            Title = title,
            Content = message,
            CloseButtonText = _localizationService.GetString("Dialog_OK"),
            XamlRoot = GetCurrentXamlRoot()
        };

        await dialog.ShowAsync();
    }

    public async Task ShowErrorAsync(string message)
    {
        await ShowMessageAsync(
            _localizationService.GetString("Dialog_Error_Title"),
            message
        );
    }

    public async Task ShowWarningAsync(string message)
    {
        await ShowMessageAsync(
            _localizationService.GetString("Dialog_Warning_Title"),
            message
        );
    }

    public async Task ShowInfoAsync(string message)
    {
        await ShowMessageAsync(
            _localizationService.GetString("Dialog_Info_Title"),
            message
        );
    }

    private Microsoft.UI.Xaml.XamlRoot? GetCurrentXamlRoot()
    {
        // Get the XamlRoot from the current window
        // In WinUI 3, dialogs need XamlRoot to be set
        var window = App.Current.Services.GetService(typeof(Views.MainWindow)) as Views.MainWindow;
        return window?.Content?.XamlRoot;
    }
}
