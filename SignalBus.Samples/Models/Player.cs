using SignalBus.Contracts;
using SignalBus.Samples.Contracts;
using SignalBus.Samples.Data;
using SignalBus.Samples.Signals;

namespace SignalBus.Samples.Models;

public partial class Player : DistributedModel
{
    private readonly Wallet _wallet;
    private readonly List<Item> _items = new();

    public Player(ISignalBus bus, Wallet wallet)
        : base(bus)
    {
        _wallet = wallet;
    }

    public IReadOnlyList<Item> Items => _items.AsReadOnly();

    public IReadOnlyWallet Wallet => _wallet;

    public void CollectCoins(int amount)
        => _wallet.AddCoins(amount);

    protected override void RegisterSignalHandlers()
    {
        Bus.Register<ProductSuccessfullyBuyedSignal>(OnProductSuccessfullyBuyed);
    }

    protected override void UnregisterSignalHandlers()
    {
        Bus.Unregister<ProductSuccessfullyBuyedSignal>(OnProductSuccessfullyBuyed);
    }

    private void OnProductSuccessfullyBuyed(ProductSuccessfullyBuyedSignal signal)
    {
        _items.Add(signal.Product);
    }
}