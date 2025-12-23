using Microsoft.UI.Xaml;

namespace Winhance.WinUI3.Features.Common.Helpers;

/// <summary>
/// Helper class for managing window backdrop effects (Mica, Acrylic)
/// </summary>
public static class WindowBackdropHelper
{
    public static void SetBackdrop(Window window, BackdropType backdropType)
    {
        // WinUI 3 backdrop implementation
        // This requires Windows App SDK 1.2+
        
        if (Microsoft.UI.Composition.SystemBackdrops.MicaController.IsSupported())
        {
            var backdropController = new Microsoft.UI.Composition.SystemBackdrops.MicaController();
            backdropController.Kind = backdropType == BackdropType.Mica 
                ? Microsoft.UI.Composition.SystemBackdrops.MicaKind.Base
                : Microsoft.UI.Composition.SystemBackdrops.MicaKind.BaseAlt;
            
            backdropController.SetTarget(window);
        }
    }
}

public enum BackdropType
{
    None,
    Mica,
    Acrylic
}
