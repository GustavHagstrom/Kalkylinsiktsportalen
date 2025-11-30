using MudBlazor;

namespace BlazorServer;

public static class CustomThemes
{
    private static MudTheme _mudDefault = new MudTheme();
    public static MudTheme PlatformStandard { get; private set; } = new()
    {

        PaletteLight = new PaletteLight
        {
            AppbarBackground = _mudDefault.PaletteLight.White,
            AppbarText = _mudDefault.PaletteLight.Primary,
            //AppbarBackground = "#ffffff",
            //AppbarText = "#594ae2",
            //Secondary = "#1ec8a5",
            //SecondaryLighten = "#2adfbb",
            //SecondaryDarken = "#19a98c",
            //Tertiary = "#00FFFF ",
            //TertiaryLighten = "#E0FFFF ",
            //TertiaryDarken = "#008B8B",
        },
        PaletteDark = new PaletteDark
        {

            AppbarText = _mudDefault.PaletteDark.Primary,
            //AppbarBackground = _mudDefault.PaletteDark.Surface,
            //DrawerBackground = _mudDefault.PaletteDark.Surface,
            //AppbarText = "#594ae2",
            //Secondary = "#1ec8a5",
            //SecondaryLighten = "#2adfbb",
            //SecondaryDarken = "#19a98c",
            //Tertiary = "#00FFFF ",
            //TertiaryLighten = "#E0FFFF ",
            //TertiaryDarken = "#008B8B",
        },
        LayoutProperties = new LayoutProperties
        {
            DefaultBorderRadius = "0"
        },
        PseudoCss = new PseudoCss
        {

        },
        Shadows = new Shadow
        {

        },
        Typography = new Typography
        {
            //Button = new Button { TextTransform = "none" },
            //H5 = new H5 { FontWeight = 600 },
            //H6 = new H6 { FontWeight = 600 },
            //Subtitle1 = new Subtitle1 { FontWeight = 500 },
            //Default = new Default { LineHeight = 1 },
        },
        ZIndex = new ZIndex
        {

        },

    };

}
