using System;
using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using AvaloniaProject.Services;
using DynamicData;
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

    private ISourceList<string> RecordList { get; }
    public ReadOnlyObservableCollection<string> Records => _records;
    private readonly ReadOnlyObservableCollection<string> _records;

    public BindingPageViewModel() : base("Page_Binding", "mdi-link-variant", 1, 16)
    {
        InputText = string.Empty;
        StatusText = string.Empty;

        RecordList = new SourceList<string>();
        RecordList
            .AsObservableList()
            .Connect()
            .ObserveOn(RxSchedulers.MainThreadScheduler)
            .Bind(out _records)
            .Subscribe();
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
    private void Increment()
    {
        Counter++;
    }

    [ReactiveCommand]
    private void Decrement()
    {
        Counter--;
    }

    [ReactiveCommand]
    private void Reset()
    {
        Counter = 0;
    }

    [ReactiveCommand]
    private void AddRecord()
    {
        RecordList.Insert(0, $"Record {DateTime.Now:HH:mm:ss.fff}");
    }

    [ReactiveCommand]
    private void RemoveRecord()
    {
        if (RecordList.Count > 0)
            RecordList.RemoveAt(RecordList.Count - 1);
    }

    [ReactiveCommand]
    private void ResetRecord()
    {
        RecordList.Clear();
    }
}
