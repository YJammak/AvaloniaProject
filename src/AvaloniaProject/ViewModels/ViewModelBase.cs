using System;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using ReactiveUI;
using ReactiveUI.Validation.Abstractions;
using ReactiveUI.Validation.Contexts;
using Splat;

namespace AvaloniaProject.ViewModels;

public abstract class ViewModelBase :
    ReactiveObject,
    IActivatableViewModel,
    IValidatableViewModel,
    IDisposable
{
    protected ViewModelBase()
    {
        this.WhenActivated(OnWhenActivated);
    }

    public ViewModelActivator Activator { get; } = new();

    public virtual void Dispose()
    {
        Activator.Dispose();
        ValidationContext.Dispose();
    }

    public IValidationContext ValidationContext { get; } = new ValidationContext();

    protected virtual async void OnWhenActivated(CompositeDisposable disposable)
    {
        try
        {
            await OnWhenActivatedAsync(disposable);
        }
        catch (Exception ex)
        {
            this.Log().Error(ex, "Error during ViewModel activation");
        }
    }

    protected virtual Task OnWhenActivatedAsync(CompositeDisposable disposable)
    {
        return Task.CompletedTask;
    }
}
