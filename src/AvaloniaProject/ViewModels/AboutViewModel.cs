using System.Reactive.Disposables;
using System.Reflection;
using ReactiveUI.SourceGenerators;

namespace AvaloniaProject.ViewModels;

public partial class AboutViewModel : ViewModelBase
{
    [Reactive]
    public partial string Version { get; private set; }

    protected override void OnWhenActivated(CompositeDisposable disposable)
    {
        base.OnWhenActivated(disposable);

        Version = $"V {Assembly.GetExecutingAssembly().GetName().Version!.ToString(3)}";
    }
}
