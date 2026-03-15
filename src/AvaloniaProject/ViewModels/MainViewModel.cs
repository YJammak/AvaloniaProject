using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using AvaloniaProject.ViewModels.Pages;
using ReactiveUI.SourceGenerators;
using Splat;

namespace AvaloniaProject.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    public IEnumerable<PageViewModel> Pages =>
        AppLocator.Current.GetServices<PageViewModel>().OrderBy(p => p.Index);

    [Reactive]
    public partial PageViewModel? SelectedPage { get; set; }

    protected override void OnWhenActivated(CompositeDisposable disposable)
    {
        base.OnWhenActivated(disposable);
        SelectedPage = Pages.FirstOrDefault();
    }
}
