using System;
using System.Diagnostics;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using AvaloniaProject.Services;
using AvaloniaProject.ViewModels.Pages;
using LiveMarkdown.Avalonia;
using ReactiveUI;
using Ursa.ReactiveUIExtension;

namespace AvaloniaProject.Views.Pages;

public partial class HomePageView : ReactiveUrsaView<HomePageViewModel>
{
    private readonly ObservableStringBuilder _markdownBuilder;

    public HomePageView()
    {
        InitializeComponent();

        _markdownBuilder = new ObservableStringBuilder();
        MarkdownRenderer.MarkdownBuilder = _markdownBuilder;
        UpdateContent();
        LocalizationService.Instance.CultureChanged += (_, _) => UpdateContent();

        this.WhenActivated(OnWhenActivated);
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
