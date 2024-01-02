using SignalBus.Samples;

// Init
var player = ModelsFactory.CreatePlayer();
var store = ModelsFactory.CreateStore();

void PrintStoreProducts()
{
    foreach (var product in store.Products)
    {
        Console.WriteLine($"{product.Name}, Cost: {product.Cost:C2}");
    }
    Console.WriteLine();
}

void PrintPlayerItems()
{
    foreach (var item in player.Items)
    {
        Console.WriteLine($"{item.Name}");
    }
    Console.WriteLine();
}

// Act
PrintStoreProducts();

var amount = 17;
player.CollectCoins(amount);
Console.WriteLine($"After adding {amount} coins:");
PrintStoreProducts();

Console.WriteLine($"Coins before buying: {player.Wallet.CoinsAmount:C2}\n");
var product = store.Products[0];
Console.WriteLine($"Product for buying: {product.Name}\n");

store.BuyProduct(product);
Console.WriteLine($"Coins after buying: {player.Wallet.CoinsAmount:C2}\n");

Console.WriteLine("Store products after buying");
PrintStoreProducts();

Console.WriteLine("Player items after buying");
PrintPlayerItems();