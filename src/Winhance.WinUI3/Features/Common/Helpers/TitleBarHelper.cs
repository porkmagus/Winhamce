using Microsoft.UI.Xaml;
using Microsoft.UI.Windowing;
using WinRT.Interop;

namespace Winhance.WinUI3.Features.Common.Helpers;

/// <summary>
/// Helper for customizing window title bar in WinUI 3
/// </summary>
public static class TitleBarHelper
{
    public static void CustomizeTitleBar(Window window)
    {
        // Get the AppWindow
        var hWnd = WindowNative.GetWindowHandle(window);
        var windowId = Win32Interop.GetWindowIdFromWindow(hWnd);
        var appWindow = AppWindow.GetFromWindowId(windowId);

        if (appWindow is not null)
        {
            // Customize title bar
            var titleBar = appWindow.TitleBar;
            titleBar.ExtendsContentIntoTitleBar = true;
            
            // Set button colors
            titleBar.ButtonBackgroundColor = Microsoft.UI.Colors.Transparent;
            titleBar.ButtonInactiveBackgroundColor = Microsoft.UI.Colors.Transparent;
        }
    }

    public static void SetDragRegion(Window window, UIElement dragRegion)
    {
        // Set the drag region for custom title bar
        var hWnd = WindowNative.GetWindowHandle(window);
        var windowId = Win32Interop.GetWindowIdFromWindow(hWnd);
        var appWindow = AppWindow.GetFromWindowId(windowId);

        if (appWindow is not null && dragRegion is FrameworkElement element)
        {
            // This would need additional implementation for drag regions
            // Using InputNonClientPointerSource
        }
    }
}
