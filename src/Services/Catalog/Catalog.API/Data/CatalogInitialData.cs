using Marten.Schema;

namespace Catalog.API.Data;

public class CatalogInitialData : IInitialData
{
    public async Task Populate(IDocumentStore store, CancellationToken cancellation)
    {
        using var session = store.LightweightSession();

        if (await session.Query<Product>().AnyAsync())
        {
            return;
        }

        session.Store<Product>(GetPreconfiguredProducts());
        await session.SaveChangesAsync();
    }

    private static IEnumerable<Product> GetPreconfiguredProducts()
    {
        return new List<Product>()
        {
            new Product
            {
                Id = Guid.NewGuid(),
                Name = "iPhone 14 Plus",
                Description = "iPhone 14 Plus 256Gb Black",
                ImageFile = "",
                Price = 699,
                Category = new List<string> { "Smartphone", "iPhone" }
            },
            new Product
            {
                Id = Guid.NewGuid(),
                Name = "Samsung Galaxy S24 Ultra",
                Description = "Samsung Galaxy S24 Ultra 256Gb Black",
                ImageFile = "",
                Price = 719,
                Category = new List<string> { "Smartphone", "Samsung" }
            },
            new Product
            {
                Id = Guid.NewGuid(),
                Name = "iPhone 15",
                Description = "iPhone 15",
                ImageFile = "",
                Price = 729,
                Category = new List<string> { "Smartphone", "iPhone" }
            },
        };
    }
}