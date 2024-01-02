using SignalBus.Contracts;

namespace SignalBus.Core;

internal static class TriggerHandlersManager<TSignal>
    where TSignal : ISignal
{
    private static readonly Dictionary<Type, Func<ITriggerHandler>> _handlers = new()
    {
        { typeof(Action<TSignal>), () => new SyncTriggerHandler() },
        { typeof(Func<TSignal, Task>), () => new AsyncTriggerHandler() },
        { typeof(Func<TSignal, CancellationToken, Task>), () => new AsyncWithTokenTriggerHandler() },
    };

    public static void Handle(TSignal signal, Delegate signalHandler)
    {
        GetTriggerHandler(signalHandler.GetType()).Handle(signal, signalHandler);
    }

    public static Task HandleAsync(TSignal signal, Delegate signalHandler, CancellationToken cancellationToken = default)
    {
        return GetTriggerHandler(signalHandler.GetType()).HandleAsync(signal, signalHandler, cancellationToken);
    }

    private static ITriggerHandler GetTriggerHandler(Type handlerType)
    {
        if (!_handlers.TryGetValue(handlerType, out var triggerhandlerFactory))
        {
            throw new ArgumentOutOfRangeException(nameof(handlerType));
        }

        return triggerhandlerFactory();
    }
}