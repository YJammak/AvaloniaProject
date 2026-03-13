using ReactiveUI;
using ReactiveUI.Validation.Abstractions;
using ReactiveUI.Validation.Contexts;

namespace AvaloniaProject.ViewModels;

public abstract class ViewModelBase : ReactiveObject, IActivatableViewModel, IValidatableViewModel
{
    public ViewModelActivator Activator { get; } = new();

    public IValidationContext ValidationContext { get; } = new ValidationContext();
}