# Winhance - WinUI 3 Edition

> **Status**: ?? Active Migration - ~15% Complete

This is the WinUI 3 version of Winhance, migrated from the original WPF application. The migration maintains all core functionality while leveraging modern WinUI 3 controls and the Windows App SDK.

## ?? Project Goals

- Modernize UI with WinUI 3 and Fluent Design
- Maintain 100% feature parity with WPF version
- Improve performance and responsiveness
- Better Windows 11 integration
- Enhanced accessibility support

## ??? Architecture

```
Winhance.WinUI3 (UI Layer - WinUI 3)
??? Features/
?   ??? Common/          # Shared UI components
?   ??? SoftwareApps/    # App management
?   ??? Optimize/        # System optimizations
?   ??? Customize/       # Windows customization
?   ??? AdvancedTools/   # Advanced utilities
?
Winhance.Core (Domain Layer - Unchanged)
??? Business logic, models, interfaces
?
Winhance.Infrastructure (Data Layer - Unchanged)
??? Services, repositories, external integrations
```

## ?? Getting Started

### Prerequisites

- **Visual Studio 2022** 17.8 or later
  - Workload: ".NET Desktop Development"
  - Workload: "Universal Windows Platform development"
  - Individual Component: "Windows App SDK C# Templates"
- **Windows 10 SDK** 10.0.19041.0 or higher
- **.NET 9 SDK**
- **Windows 10** version 1809 (build 17763) or later

### Building

```powershell
# Clone the repository
git clone https://github.com/porkmagus/Winhamce.git
cd Winhamce

# Restore dependencies
dotnet restore src/Winhance.WinUI3/Winhance.WinUI3.csproj

# Build
dotnet build src/Winhance.WinUI3/Winhance.WinUI3.csproj

# Run (requires admin privileges)
dotnet run --project src/Winhance.WinUI3/Winhance.WinUI3.csproj
```

### Running in Visual Studio

1. Open `Winhamce.sln`
2. Set `Winhance.WinUI3` as startup project
3. Press F5 to run

**Note**: The application requires administrator privileges.

## ?? Project Structure

### Key Files

| File | Purpose |
|------|---------|
| `App.xaml/cs` | Application entry point and lifecycle |
| `MainWindow.xaml/cs` | Main application window with navigation |
| `LoadingWindow.xaml/cs` | Startup loading screen |
| `CompositionRoot.cs` | Dependency injection setup |
| `Winhance.WinUI3.csproj` | Project configuration |
| `Package.appxmanifest` | MSIX packaging manifest |

### Feature Organization

Each feature follows this structure:
```
Features/{FeatureName}/
??? Views/           # XAML pages and custom views
??? ViewModels/      # MVVM view models
??? Models/          # UI-specific models
??? Converters/      # Value converters
??? Resources/       # Feature-specific resources
```

## ?? UI Framework

### WinUI 3 Controls Used

- **NavigationView** - Main navigation
- **SettingsExpander** - Settings categories
- **SettingsCard** - Individual settings
- **ContentDialog** - Modal dialogs
- **InfoBar** - Notifications
- **TeachingTip** - Contextual help
- **ItemsRepeater** - Efficient virtualized lists

### Community Toolkit

Leverages [CommunityToolkit.WinUI](https://learn.microsoft.com/windows/communitytoolkit/winui-3/):
- SettingsControls
- Converters
- Behaviors
- Extensions

## ?? Migration Status

See [MIGRATION_CHECKLIST.md](MIGRATION_CHECKLIST.md) for detailed progress.

### Completed ?
- Project setup and configuration
- Application infrastructure (DI, lifecycle, services)
- Main window with navigation
- Loading window
- Basic theme management
- Dialog service
- Localization service

### In Progress ??
- ViewModels migration
- Views migration
- Custom controls
- Converters
- Behaviors

### Pending ?
- Complete styling
- All dialogs
- Advanced features
- Testing
- Documentation

## ?? Testing

```powershell
# Run unit tests (when available)
dotnet test

# Run with diagnostics
dotnet run --project src/Winhance.WinUI3/Winhance.WinUI3.csproj --verbosity detailed
```

## ?? Packaging

### MSIX Package

```powershell
# Create MSIX package
msbuild src/Winhance.WinUI3/Winhance.WinUI3.csproj /p:Configuration=Release /p:Platform=x64 /p:UapAppxPackageBuildMode=SideloadOnly
```

### Self-Contained Deployment

The application uses `WindowsAppSDKSelfContained=true` for easier distribution without requiring users to install the Windows App SDK runtime.

## ?? Key Differences from WPF Version

### Technology Stack

| Aspect | WPF | WinUI 3 |
|--------|-----|---------|
| UI Framework | WPF | WinUI 3 + Windows App SDK |
| Icons | MahApps IconPacks | Segoe Fluent Icons |
| Dialogs | MessageBox, Window | ContentDialog |
| Navigation | Manual | NavigationView |
| Settings UI | Custom | SettingsExpander |
| Themes | Custom | Built-in + Mica/Acrylic |

### Benefits

- ? Modern Fluent Design
- ?? Mica and Acrylic backdrop effects
- ? Better performance with compiled bindings (x:Bind)
- ?? Touch-friendly controls
- ? Improved accessibility
- ?? Better dark mode support
- ?? Windows 11 integration

## ?? Contributing

Currently focused on migration. Contributions welcome after completion.

### Migration Help Needed

See [MIGRATION_GUIDE.md](MIGRATION_GUIDE.md) for guidelines on:
- Porting views
- Migrating ViewModels
- Converting converters
- Replacing behaviors

## ?? Resources

- [WinUI 3 Documentation](https://learn.microsoft.com/windows/apps/winui/winui3/)
- [Windows App SDK](https://learn.microsoft.com/windows/apps/windows-app-sdk/)
- [WinUI 3 Gallery Sample App](https://apps.microsoft.com/store/detail/winui-3-gallery/9P3JFPWWDZRC)
- [Community Toolkit](https://learn.microsoft.com/windows/communitytoolkit/winui-3/)
- [Fluent Design System](https://fluent2.microsoft.design/)

## ?? License

Same as the main Winhance project.

## ?? Known Issues

1. **Migration incomplete** - Many features are placeholders
2. **Custom controls** - Need to be recreated
3. **Styling** - Default WinUI 3 styles, custom styling in progress
4. **Testing** - Comprehensive testing pending

## ??? Roadmap

### Phase 1 (Current) - Foundation
- [x] Project setup
- [x] Basic infrastructure
- [ ] Complete all services
- [ ] Core windows complete

### Phase 2 - Features
- [ ] Migrate all ViewModels
- [ ] Migrate all Views
- [ ] Custom controls
- [ ] Dialogs

### Phase 3 - Polish
- [ ] Complete styling
- [ ] Animations
- [ ] Accessibility
- [ ] Localization

### Phase 4 - Release
- [ ] Testing
- [ ] Documentation
- [ ] Packaging
- [ ] Deployment

## ?? Support

For issues and questions, please use the main repository's issue tracker.

---

**Migration Started**: December 2024  
**Target Framework**: .NET 9 + WinUI 3  
**Windows App SDK**: 1.6+  
**Minimum Windows Version**: Windows 10 1809 (Build 17763)

