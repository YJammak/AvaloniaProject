using System;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using ReactiveUI;
using ReactiveUI.Validation.Abstractions;
using ReactiveUI.Validation.Contexts;

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

    public void Dispose()
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
        catch
        {
            //
        }
    }

    protected virtual Task OnWhenActivatedAsync(CompositeDisposable disposable)
    {
        return Task.CompletedTask;
    }
}
