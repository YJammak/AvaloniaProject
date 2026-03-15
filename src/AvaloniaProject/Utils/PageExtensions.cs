using Avalonia;
using AvaloniaProject.ViewModels.Pages;
using Splat;

namespace AvaloniaProject.Utils;

public static class PageExtensions
{
    public static AppBuilder UsePages(this AppBuilder builder)
    {
        Register<HomePageViewModel>();
        Register<BindingPageViewModel>();
        return builder;
    }

    private static void Register<T>()
        where T : PageViewModel, new()
    {
        Locator.CurrentMutable.Register<PageViewModel, T>();
    }
}
