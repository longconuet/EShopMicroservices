namespace Catalog.API.Products.GetProductsByCategory;

//public record GetProductsByCategoryResponse(Product Product);

public class GetProductsByCategoryEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/products/category/{category}", async (string category, ISender sender) =>
        {
            var result = await sender.Send(new GetProductsByCategoryQuery(category));
            var products = result.Products;

            return Results.Ok(products);
        })
        .WithName("GetProductsByCategory")
        .Produces<IEnumerable<Product>>(StatusCodes.Status200OK) 
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Get Products By Category")
        .WithDescription("Get Products By Category"); ;
    }
}
