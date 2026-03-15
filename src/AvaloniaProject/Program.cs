using System;
using System.Text;
using Avalonia;
using AvaloniaProject.Utils;
using NLog;
using NLog.Targets;
using Projektanker.Icons.Avalonia;
using Projektanker.Icons.Avalonia.MaterialDesign;
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
        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    public static AppBuilder BuildAvaloniaApp()
    {
        // Register NLog
        AppLocator.CurrentMutable.UseNLogWithWrappingFullLogger();
        NLogConfigure();

        // Register Icon
        IconProvider.Current.Register<MaterialDesignIconProvider>();

        return AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace()
            .UseReactiveUI(_ => { })
            .RegisterReactiveUIViewsFromEntryAssembly()
            .UsePages();
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
