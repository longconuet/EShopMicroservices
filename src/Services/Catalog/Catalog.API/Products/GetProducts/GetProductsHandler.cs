namespace Catalog.API.Products.GetProducts;

public record GetProductsQuery() : IQuery<GetProductsResult>;

public record GetProductsResult(IEnumerable<Product> Products);

internal class GetProductsQuaryHandler : IQueryHandler<GetProductsQuery, GetProductsResult>
{
    private readonly IDocumentSession _documentSession;
    private readonly ILogger<GetProductsQuaryHandler> _logger;

    public GetProductsQuaryHandler(IDocumentSession documentSession, ILogger<GetProductsQuaryHandler> logger)
    {
        _documentSession = documentSession;
        _logger = logger;
    }

    public async Task<GetProductsResult> Handle(GetProductsQuery query, CancellationToken cancellationToken)
    {
        _logger.LogInformation("GetProductsQueryHandler.Handle called with {@Query}", query);

        var products = await _documentSession.Query<Product>().ToListAsync(cancellationToken);

        return new GetProductsResult(products);
    }
}
