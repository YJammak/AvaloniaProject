using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using AvaloniaProject.ViewModels;
using AvaloniaProject.ViewModels.Pages;
using AvaloniaProject.Views;
using AvaloniaProject.Views.Pages;

namespace AvaloniaProject;

public class ViewLocator : IDataTemplate
{
    private static readonly Dictionary<Type, Func<Control>> ViewFactories = new()
    {
        [typeof(MainViewModel)] = () => new MainView(),
        [typeof(MainWindowViewModel)] = () => new MainWindow(),
        [typeof(AboutViewModel)] = () => new AboutView(),
        [typeof(HomePageViewModel)] = () => new HomePageView(),
        [typeof(BindingPageViewModel)] = () => new BindingPageView(),
    };

    public Control? Build(object? param)
    {
        if (param is null)
            return null;

        var viewModelType = param.GetType();

        if (ViewFactories.TryGetValue(viewModelType, out var factory))
            return factory();

        return new TextBlock { Text = "Not Found: " + viewModelType.FullName };
    }

    public bool Match(object? data)
    {
        return data is ViewModelBase;
    }
}
