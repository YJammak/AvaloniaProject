using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using AvaloniaProject.ViewModels;
using ReactiveUI;
using Ursa.ReactiveUIExtension;

namespace AvaloniaProject.Views;

public partial class AboutView : ReactiveUrsaView<AboutViewModel>
{
    public AboutView()
    {
        InitializeComponent();

        this.WhenActivated(OnWhenActivated);
    }

    private void OnWhenActivated(CompositeDisposable disposable)
    {
        this.OneWayBind(
                ViewModel,
                vm => vm.Version,
                v => v.AppVersionTextBlock.Text)
            .DisposeWith(disposable);
    }
}
