using System;
using AvaloniaProject.Services;
using ReactiveUI.SourceGenerators;
using Splat;

namespace AvaloniaProject.ViewModels.Pages;

public abstract partial class PageViewModel : ViewModelBase
{
    protected readonly ILocalizationService Localization;
    private readonly string _nameKey;
    private readonly EventHandler _cultureChangedHandler;

    [Reactive]
    public partial string Name { get; private set; }

    [Reactive]
    public partial string Icon { get; private set; }

    [Reactive]
    public partial int IconSize { get; set; }

    [Reactive]
    public partial int Index { get; private set; }

    protected PageViewModel(
        string nameKey,
        string icon,
        int index,
        int iconSize = 18)
    {
        Localization = Locator.Current.GetService<ILocalizationService>()
                       ?? throw new InvalidOperationException(
                           "ILocalizationService is not registered. Ensure RegisterServices() runs before creating PageViewModels.");
        _nameKey = nameKey;
        Name = Localization[nameKey];
        Icon = icon;
        Index = index;
        IconSize = iconSize;

        _cultureChangedHandler = (_, _) => Name = Localization[_nameKey];
        Localization.CultureChanged += _cultureChangedHandler;
    }

    public override void Dispose()
    {
        Localization.CultureChanged -= _cultureChangedHandler;
        base.Dispose();
    }
}
