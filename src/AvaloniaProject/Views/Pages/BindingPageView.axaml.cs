using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using AvaloniaProject.ViewModels.Pages;
using ReactiveUI;
using Ursa.ReactiveUIExtension;

namespace AvaloniaProject.Views.Pages;

public partial class BindingPageView : ReactiveUrsaView<BindingPageViewModel>
{
    public BindingPageView()
    {
        InitializeComponent();

        this.WhenActivated(OnWhenActivated);
    }

    private void OnWhenActivated(CompositeDisposable disposable)
    {
        // Two-way binding: TextBox.Text ↔ ViewModel.InputText
        this.Bind(
                ViewModel,
                vm => vm.InputText,
                v => v.InputTextBox.Text)
            .DisposeWith(disposable);

        // One-way binding: ViewModel.InputText → TextBlock.Text
        this.OneWayBind(
                ViewModel,
                vm => vm.InputText,
                v => v.InputDisplayText.Text)
            .DisposeWith(disposable);

        // One-way binding: ViewModel.Counter → TextBlock.Text
        this.OneWayBind(
                ViewModel,
                vm => vm.Counter,
                v => v.CounterValueText.Text,
                counter => counter.ToString())
            .DisposeWith(disposable);

        // Command binding: Button → ViewModel.IncrementCommand
        this.BindCommand(
                ViewModel,
                vm => vm.IncrementCommand,
                v => v.IncrementButton)
            .DisposeWith(disposable);

        // Command binding: Button → ViewModel.DecrementCommand
        this.BindCommand(
                ViewModel,
                vm => vm.DecrementCommand,
                v => v.DecrementButton)
            .DisposeWith(disposable);

        // Command binding: Button → ViewModel.ResetCommand
        this.BindCommand(
                ViewModel,
                vm => vm.ResetCommand,
                v => v.ResetButton)
            .DisposeWith(disposable);

        // Two-way binding: CheckBox.IsChecked ↔ ViewModel.IsToggled
        this.Bind(
                ViewModel,
                vm => vm.IsToggled,
                v => v.ToggledCheckBox.IsChecked)
            .DisposeWith(disposable);

        // One-way binding: ViewModel.StatusText → TextBlock.Text
        this.OneWayBind(
                ViewModel,
                vm => vm.StatusText,
                v => v.ToggledStatusText.Text)
            .DisposeWith(disposable);

        // Command binding: Button → ViewModel.AddRecordCommand
        this.BindCommand(
                ViewModel,
                vm => vm.AddRecordCommand,
                v => v.AddRecordButton)
            .DisposeWith(disposable);

        // Command binding: Button → ViewModel.RemoveRecordCommand
        this.BindCommand(
                ViewModel,
                vm => vm.RemoveRecordCommand,
                v => v.RemoveRecordButton)
            .DisposeWith(disposable);

        // Command binding: Button → ViewModel.ResetRecordCommand
        this.BindCommand(
                ViewModel,
                vm => vm.ResetRecordCommand,
                v => v.ResetRecordButton)
            .DisposeWith(disposable);

        // One-way binding: ViewModel.Records → ListBox.ItemsSource
        this.OneWayBind(
                ViewModel,
                vm => vm.Records,
                v => v.RecordsListBox.ItemsSource)
            .DisposeWith(disposable);
    }
}
