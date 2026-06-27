using System;
using System.Diagnostics;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
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

        // Run after layout completes so the markdown visual tree is built
        Dispatcher.UIThread.Post(FixCodeBlockStyling, DispatcherPriority.Background);
    }

    /// <summary>
    /// Walk the visual tree and strip hardcoded dark backgrounds from code elements
    /// so inline code and code blocks render correctly in light theme.
    /// </summary>
    private void FixCodeBlockStyling()
    {
        foreach (var child in MarkdownRenderer.GetVisualDescendants())
        {
            switch (child)
            {
                case Border border when border.Background is ISolidColorBrush bg:
                {
                    var luminance = (0.299 * bg.Color.R + 0.587 * bg.Color.G + 0.114 * bg.Color.B) / 255.0;
                    if (luminance < 0.3 && bg.Color.A > 0)
                        border.Background = Brushes.Transparent;
                    break;
                }
                case TextBlock textBlock when textBlock.Background is ISolidColorBrush fg:
                {
                    var luminance = (0.299 * fg.Color.R + 0.587 * fg.Color.G + 0.114 * fg.Color.B) / 255.0;
                    if (luminance < 0.3 && fg.Color.A > 0)
                        textBlock.Background = Brushes.Transparent;
                    break;
                }
            }
        }
    }
}
