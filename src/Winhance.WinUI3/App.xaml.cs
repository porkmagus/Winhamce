using Microsoft.UI.Xaml;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using Winhance.Core.Features.Common.Enums;
using Winhance.Core.Features.Common.Interfaces;
using Winhance.Core.Features.Common.Models;
using Winhance.WinUI3.Features.Common.Extensions.DI;
using Winhance.WinUI3.Features.Common.Services;
using Winhance.WinUI3.Features.Common.ViewModels;
using Winhance.WinUI3.Features.Common.Views;
using Windows.ApplicationModel.Activation;
using WinRT.Interop;

namespace Winhance.WinUI3;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App : Application
{
    private readonly IHost _host;
    private BackupResult? _backupResult;
    private ScriptMigrationResult? _migrationResult;
    private Mutex? _singleInstanceMutex;
    private Window? _mainWindow;
    private const string MutexName = "Winhance_SingleInstance_Mutex_{B8F3E4D1-9A7C-4F2E-8D6B-1C3A5E7F9B2D}";

    public IServiceProvider Services => _host.Services;

    public static new App Current => (App)Application.Current;

    public App()
    {
        InitializeComponent();

        // Check for single instance FIRST
        try
        {
            _singleInstanceMutex = new Mutex(true, MutexName, out bool createdNew);

            if (!createdNew)
            {
                LogStartupError("Another instance of Winhance is already running");
                ActivateExistingInstance();
                Environment.Exit(0);
                return;
            }

            LogStartupError("Single instance check passed - this is the first instance");
        }
        catch (Exception ex)
        {
            LogStartupError($"Error during single instance check: {ex.Message}");
        }

        // Check admin privileges
        try
        {
            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);

            if (!principal.IsInRole(WindowsBuiltInRole.Administrator))
            {
                LogStartupError("Not admin - starting elevated process");

                _singleInstanceMutex?.ReleaseMutex();
                _singleInstanceMutex?.Dispose();
                _singleInstanceMutex = null;

                var startInfo = new ProcessStartInfo
                {
                    UseShellExecute = true,
                    WorkingDirectory = Environment.CurrentDirectory,
                    FileName = Process.GetCurrentProcess().MainModule?.FileName ?? throw new InvalidOperationException("MainModule is null"),
                    Verb = "runas"
                };

                try
                {
                    Process.Start(startInfo);
                    Environment.Exit(0);
                }
                catch (System.ComponentModel.Win32Exception w32Ex) when (w32Ex.NativeErrorCode == 1223)
                {
                    LogStartupError($"User cancelled UAC: {w32Ex.Message}");
                    Environment.Exit(1);
                }
                catch (Exception ex)
                {
                    LogStartupError($"Error starting elevated process: {ex.Message}");
                    Environment.Exit(1);
                }

                return;
            }
        }
        catch
        {
            // If admin check completely fails, continue (failsafe)
        }

        // Add global unhandled exception handlers
        UnhandledException += OnUnhandledException;

        try
        {
            LogStartupMessage("Creating host using composition root");

            _host = CompositionRoot
                .CreateWinhanceHost()
                .Build();

            LogStartupMessage("Application constructor completed");
        }
        catch (Exception ex)
        {
            LogStartupError("Error creating host in constructor", ex);
            _host = null!;
        }
    }

    protected override async void OnLaunched(LaunchActivatedEventArgs args)
    {
        LogStartupMessage("OnLaunched method beginning");
        LoadingWindow? loadingWindow = null;

        try
        {
            if (_host == null)
            {
                LogStartupError("Host is null - constructor may have failed");
                Exit();
                return;
            }

            LogStartupMessage("Starting host");
            await _host.StartAsync();
            LogStartupMessage("Host started successfully");

            await InitializeLoggingService();
            await InitializeLocalizationService();

            loadingWindow = await CreateAndShowLoadingWindow();

            await InitializeEventHandlers();

            var (mainWindow, mainViewModel) = CreateMainWindow();
            _mainWindow = mainWindow;

            await PreloadApplicationData(loadingWindow);

            ShowMainWindow(mainWindow, mainViewModel);
            CloseLoadingWindow(loadingWindow);
            loadingWindow = null;

            var startupNotifications = _host.Services.GetRequiredService<IStartupNotificationService>();
            await startupNotifications.ShowBackupNotificationAsync(_backupResult);
            startupNotifications.ShowMigrationNotification(_migrationResult);

            await CheckForUpdatesAsync();

            LogStartupMessage("OnLaunched method completed successfully");
        }
        catch (Exception ex)
        {
            LogStartupError("Error during startup", ex);
            ShowStartupErrorMessage(ex);
            CloseLoadingWindow(loadingWindow);
            Exit();
        }
    }

    #region Windows API Interop for Window Activation

    [DllImport("user32.dll")]
    private static extern bool SetForegroundWindow(IntPtr hWnd);

    [DllImport("user32.dll")]
    private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    [DllImport("user32.dll")]
    private static extern bool IsIconic(IntPtr hWnd);

    private const int SW_RESTORE = 9;

    private void ActivateExistingInstance()
    {
        try
        {
            var currentProcess = Process.GetCurrentProcess();
            var processes = Process.GetProcessesByName(currentProcess.ProcessName);

            foreach (var process in processes)
            {
                if (process.Id == currentProcess.Id)
                    continue;

                if (process.MainWindowHandle != IntPtr.Zero)
                {
                    IntPtr handle = process.MainWindowHandle;

                    if (IsIconic(handle))
                    {
                        ShowWindow(handle, SW_RESTORE);
                    }

                    SetForegroundWindow(handle);
                    LogStartupError($"Activated existing Winhance window (PID: {process.Id})");
                    return;
                }
            }

            LogStartupError("Could not find existing Winhance window to activate");
        }
        catch (Exception ex)
        {
            LogStartupError($"Error activating existing instance: {ex.Message}");
        }
    }

    #endregion

    #region Private Methods

    private async Task InitializeEventHandlers()
    {
        try
        {
            LogStartupMessage("Initializing domain event handlers");
            var tooltipHandler = _host.Services.GetRequiredService<Infrastructure.Features.Common.EventHandlers.TooltipRefreshEventHandler>();
            LogStartupMessage("TooltipRefreshEventHandler initialized");
        }
        catch (Exception ex)
        {
            LogStartupError("Error initializing event handlers", ex);
        }
    }

    private async Task InitializeLoggingService()
    {
        try
        {
            var logService = _host.Services.GetService<ILogService>();
            var versionService = _host.Services.GetService<IWindowsVersionService>();

            if (logService is Winhance.Core.Features.Common.Services.LogService concreteLogService && versionService != null)
            {
                concreteLogService.Initialize(versionService);
                concreteLogService.StartLog();
                LogStartupMessage("LogService initialized and logging started");
            }
        }
        catch (Exception initEx)
        {
            LogStartupError("Error initializing LogService", initEx);
        }
    }

    private async Task InitializeLocalizationService()
    {
        try
        {
            var localizationService = _host.Services.GetRequiredService<ILocalizationService>();
            var preferencesService = _host.Services.GetRequiredService<IUserPreferencesService>();

            WinUI3.Features.Common.Services.LocalizationManager.Instance.Initialize(localizationService);

            var savedLanguage = await preferencesService.GetPreferenceAsync("Language", "en");
            localizationService.SetLanguage(savedLanguage);

            LogStartupMessage($"Localization initialized with language: {savedLanguage}");
        }
        catch (Exception ex)
        {
            LogStartupError("Error initializing LocalizationService", ex);
        }
    }

    private async Task<LoadingWindow> CreateAndShowLoadingWindow()
    {
        LogStartupMessage("Creating loading window");
        var themeManager = _host.Services.GetRequiredService<IThemeManager>();
        var progressService = _host.Services.GetRequiredService<ITaskProgressService>();
        var localizationService = _host.Services.GetRequiredService<ILocalizationService>();

        var loadingWindow = new LoadingWindow(themeManager, progressService, localizationService);
        loadingWindow.Activate();
        LogStartupMessage("Loading window shown");

        return loadingWindow;
    }

    private (MainWindow mainWindow, MainViewModel mainViewModel) CreateMainWindow()
    {
        LogStartupMessage("Getting main window and view model");
        var mainWindow = _host.Services.GetRequiredService<MainWindow>();
        var mainViewModel = _host.Services.GetRequiredService<MainViewModel>();

        LogStartupMessage("Main window and view model initialized");
        return (mainWindow, mainViewModel);
    }

    private async Task PreloadApplicationData(LoadingWindow? loadingWindow)
    {
        try
        {
            LogStartupMessage("Initializing compatible settings registry");
            var settingsRegistry = _host.Services.GetRequiredService<ICompatibleSettingsRegistry>();
            await settingsRegistry.InitializeAsync();
            LogStartupMessage("Compatible settings registry initialized");

            LogStartupMessage("Preloading global settings registry");
            var settingsPreloader = _host.Services.GetRequiredService<IGlobalSettingsPreloader>();
            await settingsPreloader.PreloadAllSettingsAsync();
            LogStartupMessage("Global settings registry preloaded");

            LogStartupMessage("Checking system backup preferences");
            var prefsService = _host.Services.GetRequiredService<IUserPreferencesService>();
            var skipBackup = await prefsService.GetPreferenceAsync("SkipSystemBackup", false);
            var registryBackupCompleted = await prefsService.GetPreferenceAsync("RegistryBackupCompleted", false);

            if (!skipBackup || !registryBackupCompleted)
            {
                LogStartupMessage("Creating initial system backups");
                var backupService = _host.Services.GetRequiredService<ISystemBackupService>();
                _backupResult = await backupService.EnsureInitialBackupsAsync();
                LogStartupMessage($"Backup operation completed. Success: {_backupResult.Success}");
            }
            else
            {
                LogStartupMessage("System backup skipped");
            }

            LogStartupMessage("Checking for legacy script paths");
            var migrationService = _host.Services.GetRequiredService<IScriptMigrationService>();
            _migrationResult = await migrationService.MigrateFromOldPathsAsync();

            var mainViewModel = _host.Services.GetRequiredService<MainViewModel>();

            LogStartupMessage("Preloading SoftwareAppsViewModel data");
            var softwareAppsViewModel = _host.Services.GetRequiredService<SoftwareApps.ViewModels.SoftwareAppsViewModel>();
            await softwareAppsViewModel.InitializeCommand.ExecuteAsync(null);
            LogStartupMessage("SoftwareAppsViewModel fully preloaded");

            LogStartupMessage("Navigating to default view");
            await mainViewModel.InitializeApplicationAsync();
            LogStartupMessage("Navigation completed and UI ready");
        }
        catch (Exception ex)
        {
            LogStartupError("Error preloading application data", ex);
            throw;
        }
    }

    private void ShowMainWindow(MainWindow mainWindow, MainViewModel mainViewModel)
    {
        LogStartupMessage("Initializing and showing main window");
        var windowInitService = _host.Services.GetRequiredService<WindowInitializationService>();
        windowInitService.InitializeWindow(mainWindow);
        mainWindow.Activate();
        LogStartupMessage("Main window shown");
    }

    private static void CloseLoadingWindow(LoadingWindow? loadingWindow)
    {
        if (loadingWindow != null)
        {
            loadingWindow.Close();
            LogStartupMessage("Loading window closed");
        }
    }

    private async Task CheckForUpdatesAsync()
    {
        try
        {
            LogStartupMessage("Checking for updates...");
            var versionService = _host.Services.GetRequiredService<IVersionService>();
            var latestVersion = await versionService.CheckForUpdateAsync();

            if (latestVersion.IsUpdateAvailable)
            {
                LogStartupMessage($"Update available: {latestVersion.Version}");
                await ShowUpdateDialog(versionService, latestVersion);
            }
            else
            {
                LogStartupMessage("No updates available");
            }
        }
        catch (Exception ex)
        {
            LogStartupError($"Error checking for updates", ex);
        }
    }

    private async Task ShowUpdateDialog(IVersionService versionService, VersionInfo latestVersion)
    {
        var currentVersion = versionService.GetCurrentVersion();

        Func<Task> downloadAndInstallAction = async () =>
        {
            await versionService.DownloadAndInstallUpdateAsync();
            Exit();
        };

        bool installNow = await UpdateDialog.ShowAsync(
            currentVersion,
            latestVersion,
            downloadAndInstallAction
        );

        LogStartupMessage(installNow
            ? "User chose to download and install the update"
            : "User chose to be reminded later");
    }

    #endregion

    #region Exception Handlers

    private void OnUnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
    {
        LogStartupError($"Unhandled exception: {e.Exception.Message}", e.Exception);
        e.Handled = true;
    }

    #endregion

    #region Logging Methods

    private static void LogStartupMessage(string message)
    {
        LogStartupError(message);
    }

    private static void LogStartupError(string message, Exception? ex = null)
    {
        string fullMessage = $"[{DateTime.Now}] {message}";
        if (ex != null)
        {
            fullMessage += $"\nException: {ex.Message}\nStack Trace: {ex.StackTrace}";
            if (ex.InnerException != null)
            {
                fullMessage += $"\nInner Exception: {ex.InnerException.Message}";
            }
        }

        try
        {
            string logPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                "Winhance",
                "Logs",
                "WinhanceStartupLog.txt"
            );

            Directory.CreateDirectory(Path.GetDirectoryName(logPath)!);
            File.AppendAllText(logPath, $"{fullMessage}\n");
        }
        catch
        {
            // Silently fail if logging is not possible
        }
    }

    private static async void ShowStartupErrorMessage(Exception ex)
    {
        var dialog = new Microsoft.UI.Xaml.Controls.ContentDialog
        {
            Title = "Startup Error",
            Content = $"Error during startup: {ex.Message}\n\nStack Trace:\n{ex.StackTrace}",
            CloseButtonText = "OK",
            XamlRoot = Current._mainWindow?.Content?.XamlRoot
        };

        await dialog.ShowAsync();
    }

    #endregion
}
