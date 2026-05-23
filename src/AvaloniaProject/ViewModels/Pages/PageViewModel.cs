using System;
using AvaloniaProject.Services;
using ReactiveUI.SourceGenerators;

namespace AvaloniaProject.ViewModels.Pages;

public abstract partial class PageViewModel : ViewModelBase
{
    private readonly EventHandler? _cultureChangedHandler;

    [Reactive]
    public partial string Name { get; private set; }

    [Reactive]
    public partial string Icon { get; private set; }

    [Reactive]
    public partial int IconSize { get; set; }

    [Reactive]
    public partial int Index { get; private set; }

    protected PageViewModel(string nameKey, string icon, int index, int iconSize = 18)
    {
        var nameKey1 = nameKey;
        Name = LocalizationService.Instance[nameKey];
        Icon = icon;
        Index = index;
        IconSize = iconSize;

        _cultureChangedHandler = (_, _) => { Name = LocalizationService.Instance[nameKey1]; };
        LocalizationService.Instance.CultureChanged += _cultureChangedHandler;
    }

    public override void Dispose()
    {
        if (_cultureChangedHandler is not null)
            LocalizationService.Instance.CultureChanged -= _cultureChangedHandler;

        base.Dispose();
    }
}
