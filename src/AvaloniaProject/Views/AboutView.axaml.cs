using AvaloniaProject.ViewModels;
using ReactiveUI;
using ReactiveUI.Primitives;
using ReactiveUI.Primitives.Disposables;

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
        this.WhenAnyValue(x => x.ViewModel!.Version)
            .Subscribe(v => AppVersionTextBlock.Text = v)
            .DisposeWith(disposable);
    }
}
