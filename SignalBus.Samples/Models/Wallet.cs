using SignalBus.Contracts;
using SignalBus.Samples.Contracts;
using SignalBus.Samples.Signals;

namespace SignalBus.Samples.Models;

public partial class Wallet : IReadOnlyWallet
{
    private readonly ISignalBus _bus;

    public Wallet(ISignalBus bus)
    {
        _bus = bus;
        RegisterSignalHandlers();
    }

    public int CoinsAmount { get; private set; }

    public void AddCoins(int amount)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(amount);
        CoinsAmount += amount;
        _bus.Trigger(new WalletAmountChangedSignal(CoinsAmount));
    }

    ~Wallet()
    {
        UnregisterSignalHandlers();
    }
}

public partial class Wallet : ISignalHandlersHolder
{
    public void RegisterSignalHandlers()
    {
        _bus.Register<ProductPrepaedForBuyingSignal>(OnProductPrepaedForBuying);
    }

    public void UnregisterSignalHandlers()
    {
        _bus.Unregister<ProductPrepaedForBuyingSignal>(OnProductPrepaedForBuying);
    }

    private void OnProductPrepaedForBuying(ProductPrepaedForBuyingSignal signal)
    {
        var residual = CoinsAmount - signal.Product.Cost;

        if (residual >= 0)
        {
            CoinsAmount = residual;
            _bus.Trigger(new ProductSuccessfullyBuyedSignal(signal.Product));
        }
    }
}