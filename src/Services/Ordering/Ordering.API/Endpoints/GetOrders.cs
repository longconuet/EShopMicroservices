using BuildingBlocks.Pagination;
using Ordering.Application.Orders.Queries.GetOrders;

namespace Ordering.API.Endpoints;

public class GetOrders : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/orders", async ([AsParameters] PaginationRequest request, ISender sender) =>
        {
            var response = await sender.Send(new GetOrdersQuery(request));
            var result = response.Orders;

            return Results.Ok(result);
        })
       .WithName("GetOrders")
       .Produces<PaginationResult<OrderDto>>(StatusCodes.Status200OK)
       .ProducesProblem(StatusCodes.Status400BadRequest)
       .WithSummary("Get Orders")
       .WithDescription("Get Orders");
    }
}