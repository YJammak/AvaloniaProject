using System;
using System.Threading.Tasks;
using ReactiveUI;
using ReactiveUI.Primitives.Disposables;
using Splat;

namespace AvaloniaProject.ViewModels;

public abstract class ViewModelBase :
    ReactiveObject,
    IActivatableViewModel,
    IEnableLogger,
    IDisposable
{
    protected ViewModelBase()
    {
        this.WhenActivated((MultipleDisposable disposable) =>
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
    }

    protected virtual Task OnWhenActivatedAsync(MultipleDisposable disposable)
    {
        return Task.CompletedTask;
    }
}
