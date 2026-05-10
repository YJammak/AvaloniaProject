using System.Globalization;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using AvaloniaProject.Services;
using AvaloniaProject.ViewModels;
using AvaloniaProject.Views;
using Ursa.Themes.Semi;
using SemiTheme = Semi.Avalonia.SemiTheme;

namespace AvaloniaProject;

public class App : Application
{
    public override void Initialize()
    {
        LocalizationService.Instance.SetCulture(LocalizationService.Instance.ResolveStartupCulture());
        AvaloniaXamlLoader.Load(this);

#if DEBUG
        this.AttachDeveloperTools();
#endif

        ApplyThemeLocale(LocalizationSource.Instance.ThemeCulture);
        LocalizationService.Instance.CultureChanged += (_, _) =>
            ApplyThemeLocale(LocalizationSource.Instance.ThemeCulture);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel()
            };

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
