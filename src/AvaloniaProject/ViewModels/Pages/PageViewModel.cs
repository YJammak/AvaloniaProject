using System;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using System.Threading.Tasks;
using AvaloniaProject.Services;
using ReactiveUI.SourceGenerators;
using Splat;

namespace AvaloniaProject.ViewModels.Pages;

public abstract partial class PageViewModel : ViewModelBase, IPageViewModel
{
    protected readonly ILocalizationService Localization;
    private readonly string _nameKey;

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
    }

    protected override async Task OnWhenActivatedAsync(CompositeDisposable disposable)
    {
        await base.OnWhenActivatedAsync(disposable);

        var nameKey = _nameKey;
        EventHandler handler = (_, _) => Name = Localization[nameKey];
        Localization.CultureChanged += handler;
        Disposable.Create(() => Localization.CultureChanged -= handler)
            .DisposeWith(disposable);
    }
}
