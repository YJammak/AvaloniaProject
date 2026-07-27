using AvaloniaProject.ViewModels;
using AvaloniaProject.ViewModels.Pages;
using ReactiveUI;
using ReactiveUI.Primitives.Disposables;
using static ReactiveUI.Primitives.LinqExtensions;

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
        this.OneWayBind(
                ViewModel,
                vm => vm.Pages,
                v => v.NavMenu.ItemsSource)
            .DisposeWith(disposable);

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
    }
}
