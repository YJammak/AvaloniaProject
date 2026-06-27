using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using AvaloniaProject.ViewModels.Pages;
using ReactiveUI.SourceGenerators;

namespace AvaloniaProject.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private readonly List<IPageViewModel> _pages;

    public IEnumerable<IPageViewModel> Pages => _pages;

    [Reactive]
    public partial IPageViewModel? SelectedPage { get; set; }

    public MainViewModel(IEnumerable<IPageViewModel> pages)
    {
        _pages = pages.OrderBy(p => p.Index).ToList();
    }

    protected override async Task OnWhenActivatedAsync(CompositeDisposable disposable)
    {
        await base.OnWhenActivatedAsync(disposable);
        SelectedPage = _pages.FirstOrDefault();
    }
}
