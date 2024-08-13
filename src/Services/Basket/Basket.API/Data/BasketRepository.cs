using Marten;

namespace Basket.API.Data;

public class BasketRepository : IBasketRepository
{
    private readonly IDocumentSession _session;

    public BasketRepository(IDocumentSession session)
    {
        _session = session;
    }

    public async Task<ShoppingCart> GetBasketAsync(string username, CancellationToken cancellationToken = default)
    {
        var basket = await _session.LoadAsync<ShoppingCart>(username, cancellationToken);
        return basket is null ? throw new BasketNotFoundException(username) : basket;
    }

    public async Task<ShoppingCart> StoreBasketAsync(ShoppingCart cart, CancellationToken cancellationToken = default)
    {
        var basket = await _session.LoadAsync<ShoppingCart>(cart.Username, cancellationToken);
        if (basket is null)
        {
            _session.Store(cart);
            await _session.SaveChangesAsync();
        }

        return basket;
    }

    public async Task<bool> DeleteBasketAsync(string username, CancellationToken cancellationToken = default)
    {
        _session.Delete<ShoppingCart>(username);
        await _session.SaveChangesAsync();
        return true;
    }
}
