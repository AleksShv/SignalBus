using SignalBus.Contracts;
using SignalBus.Samples.Contracts;
using SignalBus.Samples.Data;
using SignalBus.Samples.Signals;

namespace SignalBus.Samples.Models;

public partial class Player
{
    private readonly ISignalBus _bus;

    private readonly Wallet _wallet;
    private readonly List<Item> _items = new();

    public Player(ISignalBus bus, Wallet wallet)
    {
        _bus = bus;
        _wallet = wallet;
        RegisterSignalHandlers();
    }

    public IReadOnlyList<Item> Items => _items.AsReadOnly();

    public IReadOnlyWallet Wallet => _wallet;

    public void CollectCoins(int amount)
        => _wallet.AddCoins(amount);

    ~Player()
    {
        UnregisterSignalHandlers();
    }
}

public partial class Player : ISignalHandlersHolder
{
    public void RegisterSignalHandlers()
    {
        _bus.Register<ProductSuccessfullyBuyedSignal>(OnProductSuccessfullyBuyed);
    }

    public void UnregisterSignalHandlers()
    {
        _bus.Unregister<ProductSuccessfullyBuyedSignal>(OnProductSuccessfullyBuyed);
    }

    private void OnProductSuccessfullyBuyed(ProductSuccessfullyBuyedSignal signal)
    {
        _items.Add(signal.Product);
    }
}