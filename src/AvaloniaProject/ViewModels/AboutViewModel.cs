using System.Reactive.Disposables;
using System.Reflection;
using System.Threading.Tasks;
using ReactiveUI.SourceGenerators;

namespace AvaloniaProject.ViewModels;

public partial class AboutViewModel : ViewModelBase
{
    [Reactive]
    public partial string Version { get; private set; }

    protected override async Task OnWhenActivatedAsync(CompositeDisposable disposable)
    {
        await base.OnWhenActivatedAsync(disposable);

        Version = $"V {Assembly.GetExecutingAssembly().GetName().Version!.ToString(3)}";
    }
}
