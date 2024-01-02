using SignalBus.Contracts;
using SignalBus.Samples.Contracts;
using SignalBus.Samples.Signals;

namespace SignalBus.Samples.Models;

public sealed class Wallet : DistributedModel, IReadOnlyWallet
{
    public Wallet(ISignalBus bus)
        : base(bus)
    { }

    public int CoinsAmount { get; private set; }

    public void AddCoins(int amount)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(amount);
        CoinsAmount += amount;
        Bus.Trigger(new WalletAmountChangedSignal(CoinsAmount));
    }

    protected override void RegisterSignalHandlers()
    {
        Bus.Register<ProductPrepaedForBuyingSignal>(OnProductPrepaedForBuying);
    }

    protected override void UnregisterSignalHandlers()
    {
        Bus.Unregister<ProductPrepaedForBuyingSignal>(OnProductPrepaedForBuying);
    }

    private void OnProductPrepaedForBuying(ProductPrepaedForBuyingSignal signal)
    {
        var residual = CoinsAmount - signal.Product.Cost;

        if (residual >= 0)
        {
            CoinsAmount = residual;
            Bus.Trigger(new ProductSuccessfullyBuyedSignal(signal.Product));
        }
    }
}