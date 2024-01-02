using SignalBus.Contracts;
using SignalBus.Samples.Models;

namespace SignalBus.Samples;

public static class ModelsFactory
{
    private static readonly ISignalBus _bus = Core.SignalBus.Instance;

    public static Player CreatePlayer()
    {
        var wallet = new Wallet(_bus);
        var player = new Player(_bus, wallet);
        return player;
    }

    public static Store CreateStore()
    {
        return new Store(_bus);
    }
}