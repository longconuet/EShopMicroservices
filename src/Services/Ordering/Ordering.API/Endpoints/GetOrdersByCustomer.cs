using Ordering.Application.Orders.Queries.GetOrdersByCustomer;

namespace Ordering.API.Endpoints;

public class GetOrdersByCustomer : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/orders/customer/{customerId}", async (Guid customerId, ISender sender) =>
        {
            var response = await sender.Send(new GetOrdersByCustomerQuery(customerId));

            var result = response.Orders.Adapt<List<OrderDto>>();

            return Results.Ok(result);
        })
        .WithName("GetOrdersByCustomer")
        .Produces<List<OrderDto>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Get Order By Customer")
        .WithDescription("Get Order By Customer");
    }
}
