using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using System.Threading.Tasks;
using AvaloniaProject.ViewModels;
using ReactiveUI;
using Ursa.Controls;
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

    protected override async Task<bool> CanClose()
    {
        var result = await MessageBox.ShowAsync(
            "Are you sure you want to close?",
            "Close",
            MessageBoxIcon.Question,
            MessageBoxButton.OKCancel);
        return result == MessageBoxResult.OK;
    }
}
