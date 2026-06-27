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
        this.WhenActivated(disposable =>
        {
            OnWhenActivatedAsync(disposable)
                .ContinueWith(t =>
                {
                    if (t.IsFaulted && t.Exception is not null)
                        this.Log().Error(t.Exception, "Error during ViewModel activation");
                }, TaskContinuationOptions.OnlyOnFaulted);
        });
    }

    public ViewModelActivator Activator { get; } = new();

    public virtual void Dispose()
    {
        Activator.Dispose();
        ValidationContext.Dispose();
    }

    public IValidationContext ValidationContext { get; } = new ValidationContext();

    protected virtual Task OnWhenActivatedAsync(CompositeDisposable disposable)
    {
        return Task.CompletedTask;
    }
}
