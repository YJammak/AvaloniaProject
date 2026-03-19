using System;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using AvaloniaProject.Services;
using ReactiveUI;
using ReactiveUI.SourceGenerators;

namespace AvaloniaProject.ViewModels.Pages;

public partial class BindingPageViewModel : PageViewModel
{
    [Reactive]
    public partial string InputText { get; set; }

    [Reactive]
    public partial int Counter { get; set; }

    [Reactive]
    public partial bool IsToggled { get; set; }

    [Reactive]
    public partial string StatusText { get; private set; }

    public BindingPageViewModel() : base("Page_Binding", "mdi-link-variant", 1, 16)
    {
        InputText = string.Empty;
        StatusText = string.Empty;
    }

    protected override void OnWhenActivated(CompositeDisposable disposable)
    {
        base.OnWhenActivated(disposable);
        UpdateStatusText();

        this.WhenAnyValue(x => x.IsToggled)
            .Do(_ => UpdateStatusText())
            .Subscribe()
            .DisposeWith(disposable);

        Observable.FromEventPattern(
                handler => LocalizationService.Instance.CultureChanged += handler,
                handler => LocalizationService.Instance.CultureChanged -= handler)
            .Do(_ => UpdateStatusText())
            .Subscribe()
            .DisposeWith(disposable);
    }

    private void UpdateStatusText()
    {
        StatusText = IsToggled
            ? LocalizationService.Instance["BindingPage_Status_Enabled"]
            : LocalizationService.Instance["BindingPage_Status_Disabled"];
    }

    [ReactiveCommand]
    private void Increment() => Counter++;

    [ReactiveCommand]
    private void Decrement() => Counter--;

    [ReactiveCommand]
    private void Reset() => Counter = 0;
}
