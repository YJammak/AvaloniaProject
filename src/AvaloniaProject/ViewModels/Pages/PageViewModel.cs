using AvaloniaProject.Services;
using ReactiveUI.SourceGenerators;

namespace AvaloniaProject.ViewModels.Pages;

public abstract partial class PageViewModel : ViewModelBase
{
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
        Name = LocalizationService.Instance[nameKey];
        Icon = icon;
        Index = index;
        IconSize = iconSize;

        LocalizationService.Instance.CultureChanged += (_, _) => { Name = LocalizationService.Instance[nameKey]; };
    }
}
