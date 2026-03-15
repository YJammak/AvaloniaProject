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

    protected PageViewModel(string name, string icon, int index, int iconSize = 14)
    {
        Name = name;
        Icon = icon;
        Index = index;
        IconSize = iconSize;
    }
}
