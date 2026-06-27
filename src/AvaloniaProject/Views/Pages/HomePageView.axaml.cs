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
using Splat;
using Ursa.ReactiveUIExtension;

namespace AvaloniaProject.Views.Pages;

public partial class HomePageView : ReactiveUrsaView<HomePageViewModel>
{
    private readonly ILocalizationService _localization;
    private readonly ObservableStringBuilder _markdownBuilder;

    public HomePageView()
    {
        _localization = Locator.Current.GetService<ILocalizationService>()
                        ?? throw new InvalidOperationException(
                            "ILocalizationService is not registered. Ensure RegisterServices() is called first.");
        InitializeComponent();

        _markdownBuilder = new ObservableStringBuilder();
        MarkdownRenderer.MarkdownBuilder = _markdownBuilder;
        UpdateContent();

        this.WhenActivated(OnWhenActivated);
    }

    private void OnWhenActivated(CompositeDisposable disposable)
    {
        EventHandler handler = (_, _) => UpdateContent();
        _localization.CultureChanged += handler;
        Disposable.Create(() => _localization.CultureChanged -= handler)
            .DisposeWith(disposable);

        Observable.FromEventPattern<LinkClickedEventArgs>(
                h => MarkdownRenderer.LinkClick += h,
                h => MarkdownRenderer.LinkClick -= h)
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
        _markdownBuilder.AppendLine(_localization["HomePage_Content"]);
    }
}
