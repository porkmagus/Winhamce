using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Winhance.Core.Features.Common.Models;

namespace Winhance.WinUI3.Features.Common.Views;

public static class UpdateDialog
{
    public static async Task<bool> ShowAsync(
        string currentVersion,
        VersionInfo latestVersion,
        Func<Task> downloadAndInstallAction)
    {
        var dialog = new ContentDialog
        {
            Title = "Update Available",
            Content = $"A new version ({latestVersion.Version}) is available.\n" +
                     $"Current version: {currentVersion}\n\n" +
                     $"Would you like to download and install it now?",
            PrimaryButtonText = "Update Now",
            CloseButtonText = "Later",
            DefaultButton = ContentDialogButton.Primary
        };

        // Get XamlRoot from current window
        var window = App.Current.Services.GetService(typeof(MainWindow)) as MainWindow;
        if (window?.Content is FrameworkElement element)
        {
            dialog.XamlRoot = element.XamlRoot;
        }

        var result = await dialog.ShowAsync();

        if (result == ContentDialogResult.Primary)
        {
            await downloadAndInstallAction();
            return true;
        }

        return false;
    }
}
