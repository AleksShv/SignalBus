using SignalBus.Contracts;

namespace SignalBus.Core;

internal class AsyncWithTokenTriggerHandler : ITriggerHandler
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
        return ((Func<TSignal, CancellationToken, Task>)signalHandler)(signal, cancellationToken);
    }
}
