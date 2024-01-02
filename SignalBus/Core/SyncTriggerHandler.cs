using SignalBus.Contracts;

namespace SignalBus.Core;

internal class SyncTriggerHandler : ITriggerHandler
{
    public void Handle<TSignal>(TSignal signal, Delegate signalHandler)
        where TSignal: ISignal
    {
        ((Action<TSignal>)signalHandler)(signal);
    }

    public Task HandleAsync<TSignal>(TSignal signal, Delegate signalHandler, CancellationToken cancellationToken = default)
        where TSignal : ISignal
    {
        return Task.Run(() => Handle(signal, signalHandler), cancellationToken);
    }
}
