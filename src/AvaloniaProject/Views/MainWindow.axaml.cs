using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using AvaloniaProject.ViewModels;
using ReactiveUI;
using Ursa.ReactiveUIExtension;

namespace AvaloniaProject.Views;

public partial class MainWindow : ReactiveUrsaWindow<MainWindowViewModel>
{
    public MainWindow()
    {
        InitializeComponent();

        this.WhenActivated(OnWhenActivated);
    }

    private void OnWhenActivated(CompositeDisposable disposable)
    {
        this.OneWayBind(
                ViewModel,
                vm => vm.MainView,
                v => v.ViewModelViewHost.ViewModel)
            .DisposeWith(disposable);
    }
}
