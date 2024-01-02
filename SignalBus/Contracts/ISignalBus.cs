namespace SignalBus.Contracts;

public interface ISignalBus : IDisposable
{
    void Register<TSignal>(Action<TSignal> signalHandler)
        where TSignal : ISignal;

    void Register<TSignal>(Func<TSignal, Task> asyncSignalHandler)
        where TSignal : ISignal;

    void Register<TSignal>(Func<TSignal, CancellationToken, Task> asyncSignalHandler)
        where TSignal : ISignal;

    void Unregister<TSignal>(Action<TSignal> signalHandler)
        where TSignal : ISignal;

    void Unregister<TSignal>(Func<TSignal, Task> asyncSignalHandler)
        where TSignal : ISignal;

    void Unregister<TSignal>(Func<TSignal, CancellationToken, Task> asyncSignalHandler)
        where TSignal : ISignal;

    void Trigger<TSignal>(TSignal signal)
        where TSignal : ISignal;

    Task TriggerAsync<TSignal>(TSignal signal, CancellationToken cancellationToken = default)
        where TSignal : ISignal;
}