using System;
using System.Reactive;
using System.Reactive.Disposables;
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

    public ReactiveCommand<Unit, Unit> IncrementCommand { get; }
    public ReactiveCommand<Unit, Unit> DecrementCommand { get; }
    public ReactiveCommand<Unit, Unit> ResetCommand { get; }

    public BindingPageViewModel() : base("Page_Binding", "mdi-link-variant", 1, 16)
    {
        InputText = string.Empty;
        StatusText = string.Empty;

        IncrementCommand = ReactiveCommand.Create(() => { Counter++; });
        DecrementCommand = ReactiveCommand.Create(() => { Counter--; });
        ResetCommand = ReactiveCommand.Create(() => { Counter = 0; });
    }

    protected override void OnWhenActivated(CompositeDisposable disposable)
    {
        base.OnWhenActivated(disposable);
        UpdateStatusText();

        disposable.Add(
            this.WhenAnyValue(x => x.IsToggled)
                .Do(_ => UpdateStatusText())
                .Subscribe());

        EventHandler onCultureChanged = (_, _) => UpdateStatusText();
        LocalizationService.Instance.CultureChanged += onCultureChanged;
        disposable.Add(Disposable.Create(() => LocalizationService.Instance.CultureChanged -= onCultureChanged));
    }

    private void UpdateStatusText()
    {
        StatusText = IsToggled
            ? LocalizationService.Instance["BindingPage_Status_Enabled"]
            : LocalizationService.Instance["BindingPage_Status_Disabled"];
    }
}
