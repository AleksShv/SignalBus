using SignalBus.Contracts;
using SignalBus.Samples.Data;

namespace SignalBus.Samples.Signals;

public record ProductSuccessfullyBuyedSignal(Product Product) : ISignal;