using SignalBus.Contracts;
using SignalBus.Samples.Contracts;
using SignalBus.Samples.Data;
using SignalBus.Samples.Signals;

namespace SignalBus.Samples.Models;

public partial class Store
{
    private readonly ISignalBus _bus;
    private readonly List<Product> _products = new()
    {
        new Product
        {
            Id = 1,
            Name = "Product 1",
            Cost = 10
        },
        new Product
        {
            Id = 2,
            Name = "Product 2",
            Cost = 7
        }
    };

    public Store(ISignalBus bus)
    {
        _bus = bus;
        RegisterSignalHandlers();
    }

    public IReadOnlyList<Product> Products => _products.AsReadOnly();

    public void BuyProduct(Product product)
    {
        _bus.Trigger(new ProductPrepaedForBuyingSignal(product));
    }

    ~Store()
    {
        UnregisterSignalHandlers();
    }
}

public partial class Store : ISignalHandlersHolder
{
    public void RegisterSignalHandlers()
    {
        _bus.Register<WalletAmountChangedSignal>(OnWalletAmountChanged);
        _bus.Register<ProductSuccessfullyBuyedSignal>(OnProductSuccessfullyBued);
    }

    public void UnregisterSignalHandlers()
    {
        _bus.Unregister<WalletAmountChangedSignal>(OnWalletAmountChanged);
        _bus.Unregister<ProductSuccessfullyBuyedSignal>(OnProductSuccessfullyBued);
    }

    private void OnWalletAmountChanged(WalletAmountChangedSignal signal)
    {
        if (signal.CoinsAmount is >= 15 and < 20)
        {
            _products.Add(new()
            {
                Id = 3,
                Name = "Product 3",
                Cost = 7
            });
        }
        else if (signal.CoinsAmount > 20)
        {
            _products.Add(new()
            {
                Id = 4,
                Name = "Product 4",
                Cost = 2
            });
        }
    }

    private void OnProductSuccessfullyBued(ProductSuccessfullyBuyedSignal signal)
    {
        _products.Remove(signal.Product);
    }
}