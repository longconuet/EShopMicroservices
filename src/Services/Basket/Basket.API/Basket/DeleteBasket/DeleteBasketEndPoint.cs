namespace Basket.API.Basket.DeleteBasket;

public class DeleteBasketEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/baseket/{username}", async (string username, ISender sender) =>
        {
            var result = await sender.Send(new DeleteBasketCommand(username));

            return Results.Ok(result.IsSuccess);
        })
        .WithName("DeleteBasket")
        .Produces<bool>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Delete Basket")
        .WithDescription("Delete Basket");
    }
}
