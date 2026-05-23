using System;
using System.Diagnostics;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using Avalonia;
using AvaloniaProject.Services;
using AvaloniaProject.ViewModels.Pages;
using LiveMarkdown.Avalonia;
using ReactiveUI;
using Ursa.ReactiveUIExtension;

namespace AvaloniaProject.Views.Pages;

public partial class HomePageView : ReactiveUrsaView<HomePageViewModel>
{
    private readonly ObservableStringBuilder _markdownBuilder;
    private readonly EventHandler? _cultureChangedHandler;

    public HomePageView()
    {
        InitializeComponent();

        _markdownBuilder = new ObservableStringBuilder();
        MarkdownRenderer.MarkdownBuilder = _markdownBuilder;
        UpdateContent();
        _cultureChangedHandler = (_, _) => UpdateContent();
        LocalizationService.Instance.CultureChanged += _cultureChangedHandler;

        this.WhenActivated(OnWhenActivated);
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);

        if (_cultureChangedHandler is not null)
            LocalizationService.Instance.CultureChanged -= _cultureChangedHandler;
    }

    private void OnWhenActivated(CompositeDisposable disposable)
    {
        Observable.FromEventPattern<LinkClickedEventArgs>(
                handler => MarkdownRenderer.LinkClick += handler,
                handler => MarkdownRenderer.LinkClick -= handler)
            .ObserveOn(RxSchedulers.MainThreadScheduler)
            .Do(args =>
            {
                var uri = args.EventArgs.HRef;
                if (uri == null)
                    return;
                if (uri.Scheme is "http" or "https")
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = uri.ToString(),
                        UseShellExecute = true
                    });
            })
            .Subscribe()
            .DisposeWith(disposable);
    }

    private void UpdateContent()
    {
        _markdownBuilder.Clear();
        _markdownBuilder.AppendLine(LocalizationService.Instance["HomePage_Content"]);
    }
}
