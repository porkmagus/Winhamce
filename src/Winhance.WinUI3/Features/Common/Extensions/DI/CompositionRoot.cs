using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Winhance.Core.Features.Common.Interfaces;
using Winhance.WinUI3.Features.Common.Services;
using Winhance.WinUI3.Features.Common.ViewModels;
using Winhance.WinUI3.Features.Common.Views;

namespace Winhance.WinUI3.Features.Common.Extensions.DI;

/// <summary>
/// Composition root for dependency injection configuration.
/// This class centralizes all DI registrations following the Composition Root pattern.
/// </summary>
public static class CompositionRoot
{
    /// <summary>
    /// Creates and configures the application host with all required services.
    /// </summary>
    public static IHostBuilder CreateWinhanceHost()
    {
        return Host.CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
            {
                // Register all service groups
                services.AddCoreServices();
                services.AddInfrastructureServices();
                services.AddDomainServices();
                services.AddUIServices();
                services.AddViewModels();
                services.AddViews();
            });
    }

    private static IServiceCollection AddUIServices(this IServiceCollection services)
    {
        // Theme Management
        services.AddSingleton<IThemeManager, ThemeManager>();
        
        // Window Services
        services.AddSingleton<WindowManagementService>();
        services.AddSingleton<WindowInitializationService>();
        services.AddSingleton<IApplicationCloseService, ApplicationCloseService>();
        
        // Dialog Services
        services.AddTransient<IDialogService, DialogService>();
        services.AddTransient<ISettingsConfirmationService, SettingsConfirmationService>();
        
        // Localization
        services.AddSingleton<ILocalizationService, LocalizationService>();
        services.AddSingleton<SettingLocalizationService>();
        
        // Other UI Services
        services.AddSingleton<IFlyoutManagementService, FlyoutManagementService>();
        services.AddSingleton<IFilterUpdateService, FilterUpdateService>();
        services.AddSingleton<ISettingsLoadingService, SettingsLoadingService>();
        services.AddSingleton<FeatureRegistry>();
        services.AddSingleton<FeatureViewModelFactory>();
        services.AddSingleton<IStartupNotificationService, StartupNotificationService>();
        services.AddSingleton<IUserPreferencesService, UserPreferencesService>();
        services.AddSingleton<ConfigurationService>();
        
        return services;
    }

    private static IServiceCollection AddViewModels(this IServiceCollection services)
    {
        // Main ViewModels
        services.AddSingleton<MainViewModel>();
        services.AddSingleton<WinhanceSettingsViewModel>();
        services.AddSingleton<MoreMenuViewModel>();
        
        // Feature ViewModels
        services.AddTransient<SoftwareApps.ViewModels.SoftwareAppsViewModel>();
        services.AddTransient<SoftwareApps.ViewModels.WindowsAppsViewModel>();
        services.AddTransient<SoftwareApps.ViewModels.ExternalAppsViewModel>();
        
        services.AddTransient<Optimize.ViewModels.OptimizeViewModel>();
        services.AddTransient<Optimize.ViewModels.PrivacyOptimizationsViewModel>();
        services.AddTransient<Optimize.ViewModels.PowerOptimizationsViewModel>();
        services.AddTransient<Optimize.ViewModels.GamingandPerformanceOptimizationsViewModel>();
        services.AddTransient<Optimize.ViewModels.SoundOptimizationsViewModel>();
        services.AddTransient<Optimize.ViewModels.UpdateOptimizationsViewModel>();
        services.AddTransient<Optimize.ViewModels.NotificationOptimizationsViewModel>();
        
        services.AddTransient<Customize.ViewModels.CustomizeViewModel>();
        services.AddTransient<Customize.ViewModels.TaskbarCustomizationsViewModel>();
        services.AddTransient<Customize.ViewModels.StartMenuCustomizationsViewModel>();
        services.AddTransient<Customize.ViewModels.ExplorerCustomizationsViewModel>();
        services.AddTransient<Customize.ViewModels.WindowsThemeCustomizationsViewModel>();
        
        services.AddTransient<AdvancedTools.ViewModels.WIMUtilViewModel>();
        services.AddTransient<AdvancedTools.ViewModels.AdvancedToolsMenuViewModel>();
        
        return services;
    }

    private static IServiceCollection AddViews(this IServiceCollection services)
    {
        services.AddSingleton<MainWindow>();
        services.AddTransient<LoadingWindow>();
        
        return services;
    }

    private static IServiceCollection AddCoreServices(this IServiceCollection services)
    {
        // Delegate to Core layer's service registration
        Winhance.Core.Features.Common.Extensions.DI.CoreServicesExtensions.AddCoreServices(services);
        return services;
    }

    private static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        // Delegate to Infrastructure layer's service registration
        Winhance.Infrastructure.Features.Common.Extensions.DI.InfrastructureServicesExtensions.AddInfrastructureServices(services);
        return services;
    }

    private static IServiceCollection AddDomainServices(this IServiceCollection services)
    {
        // Delegate to domain service registration from WPF project
        // This would be migrated from DomainServicesExtensions
        return services;
    }
}
