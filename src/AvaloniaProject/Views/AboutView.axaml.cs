using AvaloniaProject.ViewModels;
using ReactiveUI;
using ReactiveUI.Primitives.Disposables;
using static ReactiveUI.Primitives.LinqExtensions;

namespace AvaloniaProject.Views;

public partial class AboutView : ReactiveUrsaView<AboutViewModel>
{
    public AboutView()
    {
        InitializeComponent();

        this.WhenActivated(OnWhenActivated);
    }

    private void OnWhenActivated(MultipleDisposable disposable)
    {
        this.OneWayBind(
                ViewModel,
                vm => vm.Version,
                v => v.AppVersionTextBlock.Text)
            .DisposeWith(disposable);
    }
}
