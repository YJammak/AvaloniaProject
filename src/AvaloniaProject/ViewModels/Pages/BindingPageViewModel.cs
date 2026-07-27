using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using DynamicData;
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

    private ISourceList<string> RecordList { get; }
    public ReadOnlyObservableCollection<string> Records => _records;
    private readonly ReadOnlyObservableCollection<string> _records;

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
        StatusText = string.Empty;

        IncrementCommand = ReactiveCommand.Create(Increment);
        DecrementCommand = ReactiveCommand.Create(Decrement);
        ResetCommand = ReactiveCommand.Create(Reset);
        AddRecordCommand = ReactiveCommand.Create(AddRecord);
        RemoveRecordCommand = ReactiveCommand.Create(RemoveRecord);
        ResetRecordCommand = ReactiveCommand.Create(ResetRecord);

        RecordList = new SourceList<string>();
        RecordList
            .AsObservableList()
            .Connect()
            .ObserveOn(RxSchedulers.MainThreadScheduler)
            .Bind(out _records)
            .SubscribeSafe(ex => this.Log().Error(ex, "Error binding records list"));
    }

    protected override async Task OnWhenActivatedAsync(MultipleDisposable disposable)
    {
        await base.OnWhenActivatedAsync(disposable);

        UpdateStatusText();

        this.WhenAnyValue(x => x.IsToggled)
            .Do(_ => UpdateStatusText())
            .SubscribeSafe(ex => this.Log().Error(ex, "Error updating status text"))
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
    private void AddRecord() => RecordList.Insert(0, $"Record {DateTime.Now:HH:mm:ss.fff}");
    private void RemoveRecord() { if (RecordList.Count > 0) RecordList.RemoveAt(RecordList.Count - 1); }
    private void ResetRecord() => RecordList.Clear();
}
