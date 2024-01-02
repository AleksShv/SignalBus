namespace SignalBus.Samples.Contracts;

public interface ISignalHandlersHolder
{
    void RegisterSignalHandlers();
    void UnregisterSignalHandlers();
}
