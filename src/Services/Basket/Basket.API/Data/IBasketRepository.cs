namespace Basket.API.Data;

public interface IBasketRepository
{
    Task<ShoppingCart> GetBasketAsync(string username, CancellationToken cancellationToken = default);
    Task<ShoppingCart> StoreBasketAsync(ShoppingCart cart, CancellationToken cancellationToken = default);
    Task<bool> DeleteBasketAsync(string username, CancellationToken cancellationToken = default);
}
