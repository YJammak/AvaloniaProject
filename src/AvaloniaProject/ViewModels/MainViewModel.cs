using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using AvaloniaProject.ViewModels.Pages;
using ReactiveUI.SourceGenerators;

namespace AvaloniaProject.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private readonly List<PageViewModel> _pages;

    public IEnumerable<PageViewModel> Pages => _pages;

    [Reactive]
    public partial PageViewModel? SelectedPage { get; set; }

    public MainViewModel(IEnumerable<PageViewModel> pages)
    {
        _pages = pages.OrderBy(p => p.Index).ToList();
    }

    protected override async Task OnWhenActivatedAsync(CompositeDisposable disposable)
    {
        await base.OnWhenActivatedAsync(disposable);
        SelectedPage = _pages.FirstOrDefault();
    }
}
