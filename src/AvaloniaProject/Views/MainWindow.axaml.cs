using System;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using AvaloniaProject.Services;
using AvaloniaProject.ViewModels;
using ReactiveUI;
using Ursa.Controls;
using Ursa.ReactiveUIExtension;

namespace AvaloniaProject.Views;

public partial class MainWindow : ReactiveUrsaWindow<MainWindowViewModel>
{
    public MainWindow()
    {
        InitializeComponent();

        SetLanguageSelector(LocalizationService.Instance.CurrentCulture.Name);
        LocalizationService.Instance.CultureChanged += (_, _) =>
            SetLanguageSelector(LocalizationService.Instance.CurrentCulture.Name);

        this.WhenActivated(OnWhenActivated);
    }

    private void OnWhenActivated(CompositeDisposable disposable)
    {
        this.OneWayBind(
                ViewModel,
                vm => vm.MainView,
                v => v.ViewModelViewHost.ViewModel)
            .DisposeWith(disposable);
    }

    protected override async Task<bool> CanClose()
    {
        var result = await MessageBox.ShowAsync(
            LocalizationService.Instance["MainWindow_CloseConfirm_Message"],
            LocalizationService.Instance["MainWindow_CloseConfirm_Title"],
            MessageBoxIcon.Question,
            MessageBoxButton.OKCancel);
        return result == MessageBoxResult.OK;
    }

    private void SetLanguageSelector(string cultureName)
    {
        var isChinese = cultureName.StartsWith("zh", StringComparison.OrdinalIgnoreCase);
        ChineseMenuItem.IsChecked = isChinese;
        EnglishMenuItem.IsChecked = !isChinese;
        ToolTip.SetTip(LanguageButton, LocalizationSource.Instance["MainWindow_LanguageTooltip"]);
        ToolTip.SetTip(AboutButton, LocalizationSource.Instance["MainWindow_AboutTooltip"]);
    }

    private void OnLanguageMenuItemClick(object? sender, RoutedEventArgs e)
    {
        if (sender is not MenuItem menuItem)
            return;

        LocalizationService.Instance.SetCulture(menuItem == ChineseMenuItem ? "zh-Hans" : "en-US");
    }

    private void OnAboutButtonClick(object? sender, RoutedEventArgs e)
    {
        Dialog.ShowCustomModal<AboutView, AboutViewModel, bool>(new AboutViewModel());
    }
}
