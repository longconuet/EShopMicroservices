using Ordering.Application.Orders.Commands.DeleteOrder;

namespace Ordering.API.Endpoints;

public class DeleteOrder : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/orders", async (Guid id, ISender sender) =>
        {
            var command = new DeleteOrderCommand(id);
            var result = await sender.Send(command);

            return Results.Ok(result.IsSuccess);
        })
        .WithName("DeleteOrder")
        .Produces<bool>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Delete Order")
        .WithDescription("Delete Order");
    }
}
