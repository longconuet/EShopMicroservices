namespace Catalog.API.Products.CreateProduct;

public record CreateProductCommand(
    string Name, 
    List<string> Category, 
    string Description, 
    string ImageFile, 
    decimal Price
) : ICommand<CreateProductResult>;

public record CreateProductResult(Guid Id);

internal class CreateProductCommandHandler 
    : ICommandHandler<CreateProductCommand, CreateProductResult>
{
    private readonly IDocumentSession _documentSession;

    public CreateProductCommandHandler(IDocumentSession documentSession)
    {
        _documentSession = documentSession;
    }

    public async Task<CreateProductResult> Handle(CreateProductCommand command, CancellationToken cancellationToken)
    {
        var product = new Product
        {
            Name = command.Name,
            Category = command.Category,
            Description = command.Description,
            ImageFile = command.ImageFile,
            Price = command.Price,
        };

        _documentSession.Store(product);
        await _documentSession.SaveChangesAsync(cancellationToken);

        return new CreateProductResult(product.Id);
    }
}
