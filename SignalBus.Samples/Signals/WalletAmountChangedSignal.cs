using SignalBus.Contracts;

namespace SignalBus.Samples.Signals;

public record WalletAmountChangedSignal(int CoinsAmount) : ISignal;