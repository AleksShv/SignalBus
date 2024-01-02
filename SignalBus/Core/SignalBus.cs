using SignalBus.Contracts;

namespace SignalBus.Core;

public class SignalBus : ISignalBus
{
    private static SignalBus? _instance;
    private static readonly object _sync = new();

    private readonly Dictionary<Type, List<WeakReference<Delegate>>> _handlers = new();
    private readonly Dictionary<int, WeakReference<Delegate>> _refs = new();

    public static ISignalBus Instance
    {
        get
        {
            lock (_sync)
            {
                return _instance ??= new SignalBus();
            }
        }
    }

    #region Register

    public void Register<TSignal>(Action<TSignal> signalHandler) 
        where TSignal : ISignal
    {
        RegisterInternal(typeof(TSignal), signalHandler);
    }

    public void Register<TSignal>(Func<TSignal, Task> asyncSignalHandler) 
        where TSignal : ISignal
    {
        RegisterInternal(typeof(TSignal), asyncSignalHandler);
    }

    public void Register<TSignal>(Func<TSignal, CancellationToken, Task> asyncSignalHandler) 
        where TSignal : ISignal
    {
        RegisterInternal(typeof(TSignal), asyncSignalHandler);
    }

    private void RegisterInternal(Type signalType, Delegate signalHandler)
    {
        if (!_handlers.ContainsKey(signalType))
        {
            _handlers[signalType] = new();
        }

        var handlerRef = new WeakReference<Delegate>(signalHandler);
        var handlerHash = signalHandler.GetHashCode();

        _handlers[signalType].Add(handlerRef);
        _refs[handlerHash] = handlerRef;
    }

    #endregion

    #region Unregister

    public void Unregister<TSignal>(Action<TSignal> signalHandler)
        where TSignal : ISignal
    {
        UnregisterInternal(typeof(TSignal), signalHandler);
    }

    public void Unregister<TSignal>(Func<TSignal, Task> asyncSignalHandler)
        where TSignal : ISignal
    {
        UnregisterInternal(typeof(TSignal), asyncSignalHandler);
    }

    public void Unregister<TSignal>(Func<TSignal, CancellationToken, Task> asyncSignalHandler) 
        where TSignal : ISignal
    {
        UnregisterInternal(typeof(TSignal), asyncSignalHandler);
    }

    private void UnregisterInternal(Type signalType, Delegate signalHandler)
    {
        var handlerHash = signalHandler.GetHashCode();

        if (
            !_handlers.TryGetValue(signalType, out var handlers) ||
            !_refs.TryGetValue(handlerHash, out var handlerRef))
        {
            return;
        }

        handlers.Remove(handlerRef);
        _refs.Remove(handlerHash);
    }

    #endregion

    #region Trigger

    public void Trigger<TSignal>(TSignal signal) 
        where TSignal : ISignal
    {
        if (!_handlers.TryGetValue(typeof(TSignal), out var handlerRefs))
        {
            return;
        }

        foreach (var handlerRef in handlerRefs)
        {
            if (handlerRef.TryGetTarget(out var handler))
            {
                TriggerHandlersManager<TSignal>.Handle(signal, handler);
            }
        }
    }

    public Task TriggerAsync<TSignal>(TSignal signal, CancellationToken cancellationToken = default) 
        where TSignal : ISignal
    {
        if (!_handlers.TryGetValue(typeof(TSignal), out var handlerRefs))
        {
            return Task.CompletedTask;
        }

        var tasks = new List<Task>();
        foreach (var handlerRef in handlerRefs)
        {
            if (handlerRef.TryGetTarget(out var handler))
            {
                tasks.Add(
                    TriggerHandlersManager<TSignal>.HandleAsync(signal, handler, cancellationToken));
            }
        }

        return Task.WhenAll(tasks);
    }

    #endregion

    public void Dispose()
    {
        _handlers.Clear();
        _refs.Clear();
        _instance = null;
    }

}
