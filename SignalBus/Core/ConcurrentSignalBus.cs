using SignalBus.Contracts;
using System.Collections.Concurrent;

namespace SignalBus.Core;

public class ConcurrentSignalBus : ISignalBus
{
    private static ConcurrentSignalBus? _instance;
    private static readonly object _sync = new();

    private readonly ConcurrentDictionary<Type, List<WeakReference<Delegate>>> _handlers = new();
    private readonly ConcurrentDictionary<int, WeakReference<Delegate>> _refs = new();

    public static ISignalBus Instance
    {
        get
        {
            lock (_sync)
            {
                return _instance ??= new ConcurrentSignalBus();
            }
        }
    }

    #region Register

    public void Register<TSignal>(Action<TSignal> signalHandler) where TSignal : ISignal
    {
        RegisterInternal(typeof(TSignal), signalHandler);
    }

    public void Register<TSignal>(Func<TSignal, Task> asyncSignalHandler) where TSignal : ISignal
    {
        RegisterInternal(typeof(TSignal), asyncSignalHandler);
    }

    public void Register<TSignal>(Func<TSignal, CancellationToken, Task> asyncSignalHandler) where TSignal : ISignal
    {
        RegisterInternal(typeof(TSignal), asyncSignalHandler);
    }

    public void RegisterInternal(Type signalType, Delegate signalHandler)
    {
        var handlerRefs = _handlers.GetOrAdd(signalType, new List<WeakReference<Delegate>>());
        var handlerRef = new WeakReference<Delegate>(signalHandler);
        
        lock (_sync)
        {
            handlerRefs.Add(handlerRef);
        }

        var handlerHash = signalHandler.GetHashCode(); 
        _refs.TryAdd(handlerHash, handlerRef);
    }

    #endregion

    #region Unregister

    public void Unregister<TSignal>(Action<TSignal> signalHandler) where TSignal : ISignal
    {
        UnregisterInternal(typeof(TSignal), signalHandler);
    }

    public void Unregister<TSignal>(Func<TSignal, Task> asyncSignalHandler) where TSignal : ISignal
    {
        UnregisterInternal(typeof(TSignal), asyncSignalHandler);
    }

    public void Unregister<TSignal>(Func<TSignal, CancellationToken, Task> asyncSignalHandler) where TSignal : ISignal
    {
        UnregisterInternal(typeof(TSignal), asyncSignalHandler);
    }

    private void UnregisterInternal(Type signalType, Delegate signalHandler)
    {
        var handlerHash = signalHandler.GetHashCode();

        if (
            !_handlers.TryGetValue(signalType, out var handlerRefs) ||
            !_refs.TryGetValue(handlerHash, out var handlerRef))
        {
            return;
        }

        lock (_sync) 
        {
            handlerRefs.Remove(handlerRef);
        }

        _refs.TryRemove(new(handlerHash, handlerRef));
    }

    #endregion;

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
        lock (_sync)
        {
            _handlers.Clear();
            _refs.Clear();
            _instance = null;
        }
    }
}
