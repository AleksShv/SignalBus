using SignalBus.Contracts;

namespace SignalBus.Core;

internal class AsyncTriggerHandler : ITriggerHandler
{
    public void Handle<TSignal>(TSignal signal, Delegate signalHandler) 
        where TSignal : ISignal
    {
        HandleAsync(signal, signalHandler)
            .GetAwaiter()
            .GetResult();
    }

    public Task HandleAsync<TSignal>(TSignal signal, Delegate signalHandler, CancellationToken cancellationToken = default) 
        where TSignal : ISignal
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return Task.FromCanceled(cancellationToken);
        }

        return ((Func<TSignal, Task>)signalHandler)(signal);
    }
}
