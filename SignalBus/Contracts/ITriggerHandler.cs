namespace SignalBus.Contracts;

internal interface ITriggerHandler
{
    void Handle<TSignal>(TSignal signal, Delegate signalHandler)
        where TSignal : ISignal;

    Task HandleAsync<TSignal>(TSignal signal, Delegate signalHandler, CancellationToken cancellationToken = default)
        where TSignal : ISignal;
}
