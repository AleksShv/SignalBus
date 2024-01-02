using SignalBus.Contracts;

namespace SignalBus.Samples.Models;

public class DistributedModel
{
    protected readonly ISignalBus Bus;

    public DistributedModel(ISignalBus bus)
    {
        Bus = bus;
        RegisterSignalHandlers();
    }

    protected virtual void RegisterSignalHandlers() { }
    protected virtual void UnregisterSignalHandlers() { }

    ~DistributedModel() 
    {
        UnregisterSignalHandlers();
    }

}
