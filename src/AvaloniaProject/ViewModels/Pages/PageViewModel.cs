using System;
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

        // Permanent subscription — pages live for the app lifetime (held by MainViewModel),
        // and Name must stay in sync for the NavMenu regardless of activation state.
        Localization.CultureChanged += (_, _) => Name = Localization[_nameKey];
    }
}
