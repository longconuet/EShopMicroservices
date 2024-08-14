using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Basket.API.Data;

public class CachedBasketRepository : IBasketRepository
{
    private readonly IBasketRepository _repository;
    private readonly IDistributedCache _cache;

    public CachedBasketRepository(IBasketRepository repository, IDistributedCache cache)
    {
        _repository = repository;
        _cache = cache;
    }

    public async Task<ShoppingCart> GetBasketAsync(string username, CancellationToken cancellationToken = default)
    {
        var cachedBasket = await _cache.GetStringAsync(username, cancellationToken);
        if (!string.IsNullOrEmpty(cachedBasket))
        {
            return JsonSerializer.Deserialize<ShoppingCart>(cachedBasket)!;
        }

        var basket = await _repository.GetBasketAsync(username, cancellationToken);
        await _cache.SetStringAsync(username, JsonSerializer.Serialize(basket), cancellationToken);
        return basket;
    }

    public async Task<ShoppingCart> StoreBasketAsync(ShoppingCart basket, CancellationToken cancellationToken = default)
    {
        await _cache.SetStringAsync(basket.Username, JsonSerializer.Serialize(basket), cancellationToken);
        return await _repository.StoreBasketAsync(basket, cancellationToken);
    }

    public async Task<bool> DeleteBasketAsync(string username, CancellationToken cancellationToken = default)
    {
        await _cache.RemoveAsync(username, cancellationToken);
        return await _repository.DeleteBasketAsync(username, cancellationToken);
    }
}
