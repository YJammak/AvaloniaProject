using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using ReactiveUI;
using ReactiveUI.Primitives.Disposables;
using ReactiveUI.SourceGenerators;
using Splat;
using static ReactiveUI.Primitives.LinqExtensions;
using RxVoid = ReactiveUI.Primitives.RxVoid;

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

    public ObservableCollection<string> Records { get; }

    public ReactiveCommand<RxVoid, RxVoid> IncrementCommand { get; }
    public ReactiveCommand<RxVoid, RxVoid> DecrementCommand { get; }
    public ReactiveCommand<RxVoid, RxVoid> ResetCommand { get; }
    public ReactiveCommand<RxVoid, RxVoid> AddRecordCommand { get; }
    public ReactiveCommand<RxVoid, RxVoid> RemoveRecordCommand { get; }
    public ReactiveCommand<RxVoid, RxVoid> ResetRecordCommand { get; }

    public BindingPageViewModel() :
        base("Page_Binding", "mdi-link-variant", 1, 16)
    {
        InputText = string.Empty;
        IsToggled = false;
        StatusText = string.Empty;
        Records = new ObservableCollection<string>();

        IncrementCommand = ReactiveCommand.Create(Increment);
        DecrementCommand = ReactiveCommand.Create(Decrement);
        ResetCommand = ReactiveCommand.Create(Reset);
        AddRecordCommand = ReactiveCommand.Create(AddRecord);
        RemoveRecordCommand = ReactiveCommand.Create(RemoveRecord);
        ResetRecordCommand = ReactiveCommand.Create(ResetRecord);
    }

    protected override async Task OnWhenActivatedAsync(MultipleDisposable disposable)
    {
        await base.OnWhenActivatedAsync(disposable);

        UpdateStatusText();

        this.WhenAnyValue(x => x.IsToggled)
            .SubscribeSafe(_ => UpdateStatusText(), ex => this.Log().Error(ex, "Error updating status text"))
            .DisposeWith(disposable);

        EventHandler cultureHandler = (_, _) => UpdateStatusText();
        Localization.CultureChanged += cultureHandler;
        new ActionDisposable(() => Localization.CultureChanged -= cultureHandler)
            .DisposeWith(disposable);
    }

    private void UpdateStatusText()
    {
        StatusText = IsToggled
            ? Localization["BindingPage_Status_Enabled"]
            : Localization["BindingPage_Status_Disabled"];
    }

    private void Increment() => Counter++;
    private void Decrement() => Counter--;
    private void Reset() => Counter = 0;
    private void AddRecord() => Records.Insert(0, $"Record {DateTime.Now:HH:mm:ss.fff}");
    private void RemoveRecord() { if (Records.Count > 0) Records.RemoveAt(Records.Count - 1); }
    private void ResetRecord() => Records.Clear();
}
