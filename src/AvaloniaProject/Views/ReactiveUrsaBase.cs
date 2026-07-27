using Avalonia;
using Avalonia.Controls;
using ReactiveUI;
using Ursa.Controls;

namespace AvaloniaProject.Views;

/// <summary>
/// Base class for ReactiveUI views that extend UserControl, providing IViewFor{TViewModel}
/// without depending on the binary-incompatible Irihi.Ursa.ReactiveUIExtension package.
/// </summary>
public class ReactiveUrsaView<TViewModel> : UserControl, IViewFor<TViewModel>
    where TViewModel : class
{
#pragma warning disable AVP1002 // Generic owner is intentional here; property is set only from code, not XAML
    public static readonly StyledProperty<TViewModel?> ViewModelProperty =
        AvaloniaProperty.Register<ReactiveUrsaView<TViewModel>, TViewModel?>(nameof(ViewModel));
#pragma warning restore AVP1002

    public TViewModel? ViewModel
    {
        get => GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = value as TViewModel;
    }
}

/// <summary>
/// Base class for ReactiveUI windows that extend UrsaWindow, providing IViewFor{TViewModel}
/// without depending on the binary-incompatible Irihi.Ursa.ReactiveUIExtension package.
/// </summary>
public class ReactiveUrsaWindow<TViewModel> : UrsaWindow, IViewFor<TViewModel>
    where TViewModel : class
{
#pragma warning disable AVP1002 // Generic owner is intentional here; property is set only from code, not XAML
    public static readonly StyledProperty<TViewModel?> ViewModelProperty =
        AvaloniaProperty.Register<ReactiveUrsaWindow<TViewModel>, TViewModel?>(nameof(ViewModel));
#pragma warning restore AVP1002

    public TViewModel? ViewModel
    {
        get => GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = value as TViewModel;
    }
}
