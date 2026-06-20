using MudBlazor;

namespace hydash.WebApp.Theme;

/// <summary>
/// Central MudBlazor theme for Hydash. Brand identity: magenta (pink) → purple,
/// clean modern surfaces, rounded shapes and the Inter/Outfit type family.
/// Use <see cref="Default"/> on the single <c>MudThemeProvider</c>.
/// </summary>
public static class HydashTheme
{
    public static readonly MudTheme Default = new()
    {
        PaletteDark = new PaletteDark
        {
            Primary = "#E5247E",
            PrimaryContrastText = "#FFFFFF",
            Secondary = "#9B51E0",
            SecondaryContrastText = "#FFFFFF",
            Tertiary = "#FF5FA2",

            Background = "#13101A",
            BackgroundGray = "#0E0B14",
            Surface = "#1C1726",

            AppbarBackground = "rgba(20, 16, 26, 0.65)",
            AppbarText = "#F3EFFA",

            DrawerBackground = "#171221",
            DrawerText = "#E6E0EE",
            DrawerIcon = "#C9BFE0",

            TextPrimary = "#F3EFFA",
            TextSecondary = "rgba(243, 239, 250, 0.62)",
            TextDisabled = "rgba(243, 239, 250, 0.32)",

            ActionDefault = "#C9BFE0",
            ActionDisabled = "rgba(243, 239, 250, 0.28)",
            ActionDisabledBackground = "rgba(255, 255, 255, 0.06)",

            Divider = "rgba(255, 255, 255, 0.08)",
            DividerLight = "rgba(255, 255, 255, 0.04)",
            LinesDefault = "rgba(255, 255, 255, 0.10)",
            LinesInputs = "rgba(255, 255, 255, 0.18)",
            TableLines = "rgba(255, 255, 255, 0.08)",

            Success = "#37C871",
            Info = "#3E9BFF",
            Warning = "#FFB13C",
            Error = "#FF5470",
            Dark = "#0E0B14",
        },

        PaletteLight = new PaletteLight
        {
            Primary = "#D81B7A",
            PrimaryContrastText = "#FFFFFF",
            Secondary = "#7C2FE0",
            SecondaryContrastText = "#FFFFFF",
            Tertiary = "#FF5FA2",

            Background = "#FAF7FD",
            BackgroundGray = "#F2ECF8",
            Surface = "#FFFFFF",

            AppbarBackground = "rgba(255, 255, 255, 0.75)",
            AppbarText = "#1E1726",

            DrawerBackground = "#FFFFFF",
            DrawerText = "#3A3147",
            DrawerIcon = "#6A5E7E",

            TextPrimary = "#1E1726",
            TextSecondary = "rgba(30, 23, 38, 0.62)",

            Divider = "rgba(30, 23, 38, 0.10)",
            LinesDefault = "rgba(30, 23, 38, 0.10)",

            Success = "#2BB673",
            Info = "#2E86FF",
            Warning = "#F39C12",
            Error = "#E5436A",
        },

        LayoutProperties = new LayoutProperties
        {
            DefaultBorderRadius = "14px",
            AppbarHeight = "68px",
            DrawerWidthLeft = "260px",
        },
    };
}