namespace Basket.API.Basket.GetBasket;

public class GetBasketEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/basket/{username}", async (string username, ISender sender) =>
        {
            var result = await sender.Send(new GetBasketQuery(username));

            return Results.Ok(result.Cart);
        })
        .WithName("GetCarts")
        .Produces<IEnumerable<ShoppingCart>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Get Carts")
        .WithDescription("Get Carts");
    }
}
