using AvaloniaProject.ViewModels.Pages;
using ReactiveUI;
using ReactiveUI.Primitives.Disposables;

namespace AvaloniaProject.Views.Pages;

public partial class ValidationPageView : ReactiveUrsaView<ValidationPageViewModel>
{
    public ValidationPageView()
    {
        InitializeComponent();
        this.WhenActivated(OnWhenActivated);
    }

    private void OnWhenActivated(MultipleDisposable disposable)
    {
        // All bindings are handled by Avalonia compiled bindings; no code-behind setup needed.
    }
}
