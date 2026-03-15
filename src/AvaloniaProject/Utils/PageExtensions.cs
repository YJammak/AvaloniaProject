using Avalonia;
using AvaloniaProject.ViewModels.Pages;
using Splat;

namespace AvaloniaProject.Utils;

public static class PageExtensions
{
    public static AppBuilder UsePages(this AppBuilder builder)
    {
        Register<PageViewModel, HomePageViewModel>();
        return builder;
    }

    private static void Register<TService, TImplementation>()
        where TService : class
        where TImplementation : class, TService, new()
    {
        Locator.CurrentMutable.Register<TService, TImplementation>();
    }
}
