using System;
using Avalonia;
using AvaloniaProject.ViewModels.Pages;
using Splat;

namespace AvaloniaProject.Utils;

public static class PageExtensions
{
    public static AppBuilder UsePages(this AppBuilder builder)
    {
        Register(() => new HomePageViewModel());
        Register(() => new BindingPageViewModel());
        return builder;
    }

    private static void Register<T>(Func<T> factory)
        where T : PageViewModel
    {
        Locator.CurrentMutable.Register<IPageViewModel>(() => factory());
    }
}
