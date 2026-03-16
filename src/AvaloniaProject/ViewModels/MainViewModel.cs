using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using AvaloniaProject.Services;
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

        EventHandler onCultureChanged = (_, _) =>
        {
            var currentPage = SelectedPage;
            if (currentPage is null)
                return;

            // Force ViewModelViewHost to rebuild current page view immediately on language change.
            SelectedPage = null;
            SelectedPage = currentPage;
        };

        LocalizationService.Instance.CultureChanged += onCultureChanged;
        disposable.Add(Disposable.Create(() => LocalizationService.Instance.CultureChanged -= onCultureChanged));
    }
}
