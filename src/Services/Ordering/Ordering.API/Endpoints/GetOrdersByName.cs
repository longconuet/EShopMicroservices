using Ordering.Application.Orders.Queries.GetOrdersByName;

namespace Ordering.API.Endpoints;

public class GetOrdersByName : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/orders/{orderName}", async (string orderName, ISender sender) =>
        {
            var response = await sender.Send(new GetOrdersByNameQuery(orderName));

            var result = response.Orders.Adapt<List<OrderDto>>();

            return Results.Ok(result);
        })
        .WithName("GetOrdersByName")
        .Produces<List<OrderDto>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Get Order By Name")
        .WithDescription("Get Order By Name");
    }
}
