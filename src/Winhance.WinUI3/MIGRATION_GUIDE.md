# Winhance WinUI 3 Migration

## Overview
This project represents the migration of Winhance from WPF to WinUI 3. The migration is a complete UI framework change while maintaining the Core and Infrastructure layers.

## Project Structure

```
src/Winhance.WinUI3/
??? App.xaml / App.xaml.cs              # Application entry point
??? Winhance.WinUI3.csproj              # WinUI 3 project file
??? app.manifest                         # Admin privileges & DPI awareness
??? Assets/                              # Application assets (icons, images)
??? Features/
?   ??? Common/
?   ?   ??? Extensions/DI/
?   ?   ?   ??? CompositionRoot.cs      # Dependency injection setup
?   ?   ??? Services/
?   ?   ?   ??? ThemeManager.cs         # Theme management
?   ?   ?   ??? DialogService.cs        # Dialog services
?   ?   ?   ??? LocalizationService.cs  # Localization
?   ?   ?   ??? UIServices.cs           # Other UI services
?   ?   ??? ViewModels/
?   ?   ?   ??? MainViewModel.cs        # Main window view model
?   ?   ?   ??? ...                     # Other view models
?   ?   ??? Views/
?   ?   ?   ??? MainWindow.xaml/cs      # Main application window
?   ?   ?   ??? LoadingWindow.xaml/cs   # Startup loading window
?   ?   ?   ??? ...                     # Other views
?   ?   ??? Resources/
?   ?       ??? Theme/                   # Theme resources
?   ?       ??? Styles/                  # XAML styles
?   ?       ??? Converters/              # Value converters
?   ??? SoftwareApps/                    # Software Apps feature
?   ??? Optimize/                        # System optimization feature
?   ??? Customize/                       # Windows customization feature
?   ??? AdvancedTools/                   # Advanced tools feature
??? Localization/                        # Localization JSON files
```

## Migration Status

### ? Completed
1. **Project Setup**
   - Created WinUI 3 project file with proper SDK references
   - Added Windows App SDK and WinUI 3 packages
   - Configured for .NET 9 targeting Windows 10.0.19041.0+
   
2. **Application Infrastructure**
   - Migrated App.xaml/cs with WinUI 3 lifecycle
   - Created composition root for dependency injection
   - Implemented single instance checking
   - Implemented admin privilege elevation
   - Set up global exception handling

3. **Core Windows**
   - MainWindow with NavigationView-based navigation
   - LoadingWindow for application startup
   - Basic navigation structure

4. **Service Layer**
   - ThemeManager (WinUI 3 compatible)
   - DialogService using ContentDialog
   - LocalizationService
   - Placeholder services for all UI functionality

5. **Feature Placeholders**
   - Created placeholder views for all main features
   - Created placeholder ViewModels
   - Set up proper folder structure

6. **Resources**
   - Basic theme color dictionary
   - Placeholder style resource dictionaries
   - Converter resource dictionary

### ?? In Progress / Next Steps

#### Phase 1: Core Infrastructure (High Priority)
1. **Window Management**
   - Implement window backdrop (Mica/Acrylic effects)
   - Custom title bar implementation
   - Window state management
   - Multi-window support

2. **Theme System**
   - Complete dark/light theme implementation
   - Theme resource dictionaries (colors, brushes)
   - Theme switching logic
   - System theme detection

3. **Navigation**
   - Complete NavigationView setup
   - View caching/pooling
   - Navigation parameters
   - Breadcrumb navigation

#### Phase 2: Migrate Core Features
1. **Software Apps Feature**
   - Port WindowsAppsViewModel
   - Port ExternalAppsViewModel
   - Create app list UI with WinUI 3 controls
   - Implement filtering and search
   - Port app installation/removal logic

2. **Optimize Feature**
   - Port all optimization ViewModels
   - Create settings UI with WinUI 3 controls
   - Implement SettingsCard layouts
   - Port power management UI
   - Port privacy settings UI

3. **Customize Feature**
   - Port customization ViewModels
   - Taskbar customization UI
   - Start menu customization UI
   - Explorer customization UI
   - Theme customization UI

4. **Advanced Tools**
   - Port WIM utility
   - Advanced tools menu

#### Phase 3: UI Components
1. **Custom Controls**
   - Port/recreate NumericUpDown control
   - Port SearchBox control
   - Port ProgressIndicator
   - Port TaskProgressControl
   - Port QuickNavControl

2. **Behaviors**
   - Port or replace WPF behaviors with WinUI 3 equivalents
   - GridViewColumnResizeBehavior ? adaptive column sizing
   - CheckBoxSelectionBehavior
   - ScrollViewer behaviors

3. **Converters**
   - Port or use CommunityToolkit.WinUI converters
   - Create custom converters where needed
   - BooleanToVisibilityConverter (use toolkit)
   - Status to color converters
   - Custom business logic converters

#### Phase 4: Dialogs & Overlays
1. **Dialog System**
   - UnifiedConfigurationDialog ? ContentDialog
   - ConfigImportOptionsDialog ? ContentDialog
   - CustomDialog ? ContentDialog
   - ModalDialog ? ContentDialog
   - UpdateDialog (already created)
   - DonationDialog

2. **Flyouts & Menus**
   - MoreMenuFlyout
   - AdvancedToolsMenuFlyout
   - Context menus

#### Phase 5: Advanced Features
1. **Data Virtualization**
   - Implement efficient ItemsRepeater for large lists
   - Virtualized app lists
   - Incremental loading

2. **Performance Optimization**
   - View caching
   - Lazy loading
   - Background task management

3. **Accessibility**
   - Narrator support
   - Keyboard navigation
   - High contrast themes
   - Screen reader support

#### Phase 6: Polish & Testing
1. **Styling & Animations**
   - Complete all style dictionaries
   - Add page transitions
   - Loading animations
   - Success/error animations

2. **Localization**
   - Complete localization JSON files
   - Test all languages
   - RTL support if needed

3. **Testing**
   - Unit tests for ViewModels
   - Integration tests
   - UI automation tests
   - Performance testing

## Key Differences from WPF

### XAML Namespace Changes
```xml
<!-- WPF -->
xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"

<!-- WinUI 3 -->
xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
xmlns:muxc="using:Microsoft.UI.Xaml.Controls"
```

### Control Replacements
| WPF Control | WinUI 3 Equivalent |
|-------------|-------------------|
| Window | Window (different API) |
| MessageBox | ContentDialog |
| UserControl | UserControl / Page |
| DataGrid | DataGrid / ItemsRepeater |
| GroupBox | Expander / CardPanel |
| ContextMenu | MenuFlyout |
| Popup | Flyout / Popup |

### New WinUI 3 Controls to Use
- **NavigationView**: Modern navigation
- **SettingsExpander**: Settings UI
- **InfoBar**: Notifications
- **TeachingTip**: Contextual help
- **ProgressRing**: Loading indicators
- **PersonPicture**: User avatars
- **ItemsRepeater**: Efficient lists
- **PipsPager**: Page indicators

### Architecture Changes
1. **No Application.Current.Dispatcher**
   - Use `DispatcherQueue` instead
   - Each window has its own DispatcherQueue

2. **XamlRoot Required for Dialogs**
   - ContentDialog needs XamlRoot property set
   - Get from any UI element: `element.XamlRoot`

3. **Window Management**
   - No `Application.Current.Windows`
   - Must track windows manually

4. **Resource Loading**
   - Different pack URI syntax
   - Assets folder instead of Resources

## Building the Project

### Prerequisites
- Visual Studio 2022 17.8+ with WinUI 3 workload
- Windows 10 SDK 10.0.19041.0 or higher
- .NET 9 SDK

### Build Commands
```powershell
# Restore packages
dotnet restore src/Winhance.WinUI3/Winhance.WinUI3.csproj

# Build
dotnet build src/Winhance.WinUI3/Winhance.WinUI3.csproj

# Run
dotnet run --project src/Winhance.WinUI3/Winhance.WinUI3.csproj
```

## Migration Guidelines

### When Migrating a WPF View to WinUI 3

1. **Update XAML Namespace**
   ```xml
   <!-- Add WinUI 3 controls -->
   xmlns:muxc="using:Microsoft.UI.Xaml.Controls"
   ```

2. **Replace Deprecated Controls**
   - Check control compatibility
   - Use WinUI 3 alternatives
   - Test visual appearance

3. **Update Code-Behind**
   - Replace `Window` with WinUI 3 `Window`
   - Update event handlers
   - Use `DispatcherQueue` for threading

4. **Update Bindings**
   - Check binding syntax (mostly compatible)
   - Use x:Bind for performance (compiled bindings)
   - Update converter references

5. **Test Thoroughly**
   - Visual appearance
   - Functionality
   - Theme switching
   - Accessibility

### Icon Replacement
WPF uses MahApps.Metro.IconPacks. For WinUI 3:
- Use **Segoe Fluent Icons** (built-in)
- Use **WinUI 3 SymbolIcon**
- Or use **FontIcon** with custom fonts
- Package: `Microsoft.UI.Xaml.Controls.AnimatedIcon` for animated icons

Example:
```xml
<!-- WPF -->
<iconPacks:PackIconMaterial Kind="Apps" />

<!-- WinUI 3 -->
<SymbolIcon Symbol="AllApps" />
<!-- or -->
<FontIcon Glyph="&#xE8FD;" />
```

## Known Issues & Limitations

1. **No Direct WPF Control Compatibility**
   - All custom controls need recreation
   - Third-party libraries may not be available

2. **Different Styling System**
   - WinUI 3 uses different default styles
   - May need to recreate custom styles

3. **Performance Considerations**
   - WinUI 3 uses different rendering pipeline
   - May need optimization for large lists

4. **Deployment**
   - Different packaging requirements
   - MSIX packaging recommended
   - Self-contained deployment available

## Testing Strategy

1. **Feature Parity Testing**
   - Ensure all WPF features work in WinUI 3
   - Create test checklist for each feature

2. **Visual Testing**
   - Compare UI appearance
   - Test dark/light themes
   - Test different window sizes

3. **Performance Testing**
   - Startup time
   - Navigation responsiveness
   - Memory usage
   - Large dataset handling

## Contributing to Migration

When migrating a feature:
1. Create feature branch: `feature/winui3-migrate-{feature-name}`
2. Migrate ViewModels (minimal changes needed)
3. Create new XAML views
4. Port required converters
5. Port required behaviors
6. Test thoroughly
7. Create pull request

## Resources

- [WinUI 3 Documentation](https://learn.microsoft.com/windows/apps/winui/winui3/)
- [Windows App SDK](https://learn.microsoft.com/windows/apps/windows-app-sdk/)
- [WinUI 3 Gallery](https://apps.microsoft.com/store/detail/winui-3-gallery/9P3JFPWWDZRC)
- [Community Toolkit](https://learn.microsoft.com/windows/communitytoolkit/winui-3/)
- [Migration Guide](https://learn.microsoft.com/windows/apps/windows-app-sdk/migrate-to-windows-app-sdk/)

## Current Status Summary

**Overall Progress**: ~15% Complete

- ? Project structure: 100%
- ? App infrastructure: 90%
- ?? Core windows: 50%
- ?? Services: 40%
- ? Feature views: 5%
- ? Custom controls: 0%
- ? Converters: 10%
- ? Styles: 20%
- ? Dialogs: 5%
- ? Testing: 0%

**Estimated Timeline**: 
- Phase 1-2: 2-3 weeks
- Phase 3-4: 2-3 weeks  
- Phase 5-6: 1-2 weeks
- **Total**: 5-8 weeks (depending on team size)

---

**Last Updated**: 2024
**Migration Start Date**: Today
**Target Framework**: .NET 9 + WinUI 3
