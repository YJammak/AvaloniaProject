using System;
using System.Globalization;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using AvaloniaProject.Services;
using AvaloniaProject.ViewModels;
using AvaloniaProject.Views;
using Splat;
using Ursa.Themes.Semi;
using SemiTheme = Semi.Avalonia.SemiTheme;

namespace AvaloniaProject;

public class App : Application
{
    public override void Initialize()
    {
        var localization = Locator.Current.GetService<ILocalizationService>()
                          ?? throw new InvalidOperationException(
                              "ILocalizationService is not registered. Ensure RegisterServices() is called first.");

        // Force LocalizationSource creation before SetCulture so it can subscribe to CultureChanged
        _ = LocalizationSource.Instance;

        localization.SetCulture(localization.ResolveStartupCulture());
        AvaloniaXamlLoader.Load(this);

#if DEBUG
        this.AttachDeveloperTools();
#endif

        ApplyThemeLocale(LocalizationSource.Instance.ThemeCulture);
        localization.CultureChanged += (_, _) =>
            ApplyThemeLocale(LocalizationSource.Instance.ThemeCulture);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var mainWindowViewModel = Locator.Current.GetService<MainWindowViewModel>()!;
            desktop.MainWindow = new MainWindow
            {
                DataContext = mainWindowViewModel
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void ApplyThemeLocale(CultureInfo culture)
    {
        foreach (var style in Styles)
        {
            if (style is SemiTheme semiTheme)
                semiTheme.Locale = culture;

            if (style is UrsaSemiTheme ursaSemiTheme)
                ursaSemiTheme.Locale = culture;
        }
    }
}
