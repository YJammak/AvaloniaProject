using System;
using System.Text;
using Avalonia;
using AvaloniaProject.Services;
using AvaloniaProject.Utils;
using AvaloniaProject.ViewModels;
using AvaloniaProject.ViewModels.Pages;
using AvaloniaProject.Views;
using AvaloniaProject.Views.Pages;
using NLog;
using NLog.Targets;
using Optris.Icons.Avalonia;
using Optris.Icons.Avalonia.MaterialDesign;
using ReactiveUI.Avalonia;
using Splat;
using Splat.NLog;
using LogLevel = NLog.LogLevel;

namespace AvaloniaProject;

internal sealed class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        SatelliteAssemblyResolver.Register();
        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    public static AppBuilder BuildAvaloniaApp()
    {
        // Register NLog
        AppLocator.CurrentMutable.UseNLogWithWrappingFullLogger();
        NLogConfigure();

        // Register Icon
        IconProvider.Current.Register<MaterialDesignIconProvider>();

        // --- Composition root: register all services and ViewModels ---
        RegisterServices();

        return AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace()
            .UseReactiveUI(builder =>
            {
                builder.ConfigureViewLocator(locator =>
                {
                    locator.Map<MainWindowViewModel, MainWindow>(() => new MainWindow());
                    locator.Map<MainViewModel, MainView>(() => new MainView());
                    locator.Map<AboutViewModel, AboutView>(() => new AboutView());
                    locator.Map<HomePageViewModel, HomePageView>(() => new HomePageView());
                    locator.Map<BindingPageViewModel, BindingPageView>(() => new BindingPageView());
                    locator.Map<ValidationPageViewModel, ValidationPageView>(() => new ValidationPageView());
                });
            })
            .UsePages();
    }

    private static void RegisterServices()
    {
        // Services
        var localizationService = new LocalizationService();
        Locator.CurrentMutable.RegisterConstant<ILocalizationService>(localizationService);

        // ViewModels — resolved via Splat, dependencies injected via constructor
        Locator.CurrentMutable.RegisterLazySingleton(() =>
            new MainViewModel(Locator.Current.GetServices<IPageViewModel>()));

        Locator.CurrentMutable.RegisterLazySingleton(() =>
            new MainWindowViewModel(
                Locator.Current.GetService<MainViewModel>()
                ?? throw new InvalidOperationException(
                    "MainViewModel is not registered. Ensure RegisterServices() is called first.")
            ));
    }

    private static void NLogConfigure()
    {
        // Global log level
#if DEBUG
        LogManager.GlobalThreshold = LogLevel.Trace;
#else
        LogManager.GlobalThreshold = LogLevel.Info;
#endif
        LogManager.Setup().LoadConfiguration(builder =>
        {
            // Set log levels for ReactiveUI and Avalonia
            builder.ForLogger("ReactiveUI.*").WriteToNil(LogLevel.Warn);
            builder.ForLogger("Avalonia.*").WriteToNil(LogLevel.Warn);
            // Ignore Splat initialization logs
            builder.ForLogger("Splat.*")
                .FilterDynamicIgnore(info => info.Message.StartsWith("Initializing to"), true)
                .WriteToNil();
            // Write all logs to console
            builder.ForLogger().WriteToConsole(encoding: Encoding.UTF8);
            // Write all logs to file, ignoring ReactiveUI logs
            builder.ForLogger().FilterDynamicIgnore(info =>
                    info.LoggerName.StartsWith("ReactiveUI"))
                .WriteTo(
                    new FileTarget
                    {
                        Encoding = Encoding.UTF8,
                        FileName = "${basedir}/logs/programs/${shortdate}.log",
                        Layout =
                            "${longdate} ${uppercase:${level}} ${logger} ${message} ${exception:format=Message} ${exception:format=StackTrace}"
                    });
        });
    }
}
