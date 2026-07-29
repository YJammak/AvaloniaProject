using Avalonia;
using Avalonia.Controls;
using ReactiveUI;
using Ursa.Controls;

namespace AvaloniaProject.Views;

/// <summary>
/// Base class for ReactiveUI views that extend UserControl, providing IViewFor{TViewModel}
/// without depending on the binary-incompatible Irihi.Ursa.ReactiveUIExtension package.
/// </summary>
/// <remarks>
/// ViewModel is synced with DataContext so that setting either property updates the other.
/// This matches the behavior of the official ReactiveUI.Avalonia ReactiveUserControl{T}.
/// </remarks>
public class ReactiveUrsaView<TViewModel> : UserControl, IViewFor<TViewModel>
    where TViewModel : class
{
#pragma warning disable AVP1002 // Generic owner is intentional here; property is set only from code, not XAML
    public static readonly StyledProperty<TViewModel?> ViewModelProperty =
        AvaloniaProperty.Register<ReactiveUrsaView<TViewModel>, TViewModel?>(nameof(ViewModel));
#pragma warning restore AVP1002

    /// <summary>
    /// Gets or sets the ViewModel. This property is synced with <see cref="StyledElement.DataContext"/>:
    /// the getter reads from DataContext and the setter writes to DataContext, so the two are always
    /// in agreement regardless of which one was set.
    /// </summary>
    public TViewModel? ViewModel
    {
        get => (TViewModel?)DataContext;
        set => DataContext = value;
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
/// <remarks>
/// ViewModel is synced with DataContext so that setting either property updates the other.
/// This matches the behavior of the official ReactiveUI.Avalonia ReactiveWindow{T}.
/// </remarks>
public class ReactiveUrsaWindow<TViewModel> : UrsaWindow, IViewFor<TViewModel>
    where TViewModel : class
{
#pragma warning disable AVP1002 // Generic owner is intentional here; property is set only from code, not XAML
    public static readonly StyledProperty<TViewModel?> ViewModelProperty =
        AvaloniaProperty.Register<ReactiveUrsaWindow<TViewModel>, TViewModel?>(nameof(ViewModel));
#pragma warning restore AVP1002

    /// <summary>
    /// Gets or sets the ViewModel. This property is synced with <see cref="StyledElement.DataContext"/>:
    /// the getter reads from DataContext and the setter writes to DataContext, so the two are always
    /// in agreement regardless of which one was set.
    /// </summary>
    public TViewModel? ViewModel
    {
        get => (TViewModel?)DataContext;
        set => DataContext = value;
    }

    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = value as TViewModel;
    }
}
