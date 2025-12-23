using Microsoft.UI.Xaml.Controls;
using System.Collections.Generic;
using System.Linq;
using Winhance.Core.Features.Common.Interfaces;
using Winhance.Core.Features.Common.Models;

namespace Winhance.WinUI3.Features.Common.Views;

/// <summary>
/// Unified configuration dialog for save/import operations in WinUI 3
/// Replaces the WPF Window-based UnifiedConfigurationDialog
/// </summary>
public sealed partial class UnifiedConfigurationDialog : ContentDialog
{
    private readonly ILocalizationService _localizationService;
    private readonly Dictionary<string, (bool IsSelected, bool IsAvailable, int ItemCount)> _sections;
    private readonly bool _isSaveDialog;

    public Dictionary<string, bool> SelectedSections { get; private set; } = new();
    public ImportOptions Options { get; private set; } = new();

    public UnifiedConfigurationDialog(
        string title,
        string description,
        Dictionary<string, (bool IsSelected, bool IsAvailable, int ItemCount)> sections,
        bool isSaveDialog,
        ILocalizationService localizationService)
    {
        InitializeComponent();

        _localizationService = localizationService;
        _sections = sections;
        _isSaveDialog = isSaveDialog;

        // Set dialog properties
        Title = title;
        TitleTextBlock.Text = title;
        DescriptionTextBlock.Text = description;

        PrimaryButtonText = localizationService.GetString("Dialog_OK");
        CloseButtonText = localizationService.GetString("Dialog_Cancel");
        DefaultButton = ContentDialogButton.Primary;

        // Populate sections
        PopulateSections();

        // Handle button clicks
        PrimaryButtonClick += OnPrimaryButtonClick;
    }

    private void PopulateSections()
    {
        SectionsPanel.Children.Clear();

        foreach (var section in _sections)
        {
            var checkBox = new CheckBox
            {
                Content = $"{section.Key} ({section.Value.ItemCount} items)",
                IsChecked = section.Value.IsSelected,
                IsEnabled = section.Value.IsAvailable,
                Margin = new Microsoft.UI.Xaml.Thickness(0, 8, 0, 0)
            };

            checkBox.Tag = section.Key;
            SectionsPanel.Children.Add(checkBox);
        }
    }

    private void OnPrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        // Get selected sections
        SelectedSections = new Dictionary<string, bool>();
        
        foreach (var child in SectionsPanel.Children)
        {
            if (child is CheckBox checkBox && checkBox.Tag is string key)
            {
                SelectedSections[key] = checkBox.IsChecked == true;
            }
        }

        // Check if at least one section is selected
        if (!SelectedSections.Any(s => s.Value))
        {
            // Cancel the dialog close
            args.Cancel = true;

            // Show error
            ShowNoSelectionError();
        }
    }

    private async void ShowNoSelectionError()
    {
        var errorDialog = new ContentDialog
        {
            Title = _localizationService.GetString("Dialog_Unified_Error_NoSelection_Title"),
            Content = _localizationService.GetString("Dialog_Unified_Error_NoSelection"),
            CloseButtonText = _localizationService.GetString("Dialog_OK"),
            XamlRoot = this.XamlRoot
        };

        await errorDialog.ShowAsync();
    }

    /// <summary>
    /// Shows the unified configuration dialog
    /// </summary>
    public static async Task<(Dictionary<string, bool> sections, ImportOptions options)?> ShowAsync(
        string title,
        string description,
        Dictionary<string, (bool IsSelected, bool IsAvailable, int ItemCount)> sections,
        bool isSaveDialog,
        ILocalizationService localizationService,
        Microsoft.UI.Xaml.XamlRoot xamlRoot)
    {
        var dialog = new UnifiedConfigurationDialog(
            title,
            description,
            sections,
            isSaveDialog,
            localizationService)
        {
            XamlRoot = xamlRoot
        };

        var result = await dialog.ShowAsync();

        if (result == ContentDialogResult.Primary)
        {
            return (dialog.SelectedSections, dialog.Options);
        }

        return null;
    }
}
