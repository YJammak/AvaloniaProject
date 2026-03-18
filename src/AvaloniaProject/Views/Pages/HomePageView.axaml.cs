using System.Reactive.Disposables;
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

    private void OnWhenActivated(CompositeDisposable disposable) { }

    private void UpdateContent()
    {
        _markdownBuilder.Clear();
        _markdownBuilder.AppendLine(LocalizationService.Instance["HomePage_Content"]);
    }
}
