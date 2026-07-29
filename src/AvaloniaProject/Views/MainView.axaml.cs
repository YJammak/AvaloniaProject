using AvaloniaProject.ViewModels;
using AvaloniaProject.ViewModels.Pages;
using ReactiveUI;
using ReactiveUI.Primitives;
using ReactiveUI.Primitives.Disposables;

namespace AvaloniaProject.Views;

public partial class MainView : ReactiveUrsaView<MainViewModel>
{
    public MainView()
    {
        InitializeComponent();

        this.WhenActivated(OnWhenActivated);
    }

    private void OnWhenActivated(MultipleDisposable disposable)
    {
        this.WhenAnyValue(x => x.ViewModel!.Pages)
            .Subscribe(pages => NavMenu.ItemsSource = pages)
            .DisposeWith(disposable);

        // Bind and OneWayBind below trigger CS8714 because Avalonia framework properties
        // (SelectedItem, ViewModelViewHost.ViewModel) are typed as nullable.
#pragma warning disable CS8714
        this.Bind(
                ViewModel,
                vm => vm.SelectedPage,
                v => v.NavMenu.SelectedItem,
                page => page,
                item => item as IPageViewModel)
            .DisposeWith(disposable);

        this.OneWayBind(
                ViewModel,
                vm => vm.SelectedPage,
                v => v.ViewModelViewHost.ViewModel)
            .DisposeWith(disposable);
#pragma warning restore CS8714
    }
}
