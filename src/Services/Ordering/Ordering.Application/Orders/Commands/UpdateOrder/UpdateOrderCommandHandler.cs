using BuildingBlocks.CQRS;
using Ordering.Application.Data;
using Ordering.Application.Dtos;
using Ordering.Application.Exceptions;
using Ordering.Domain.Models;
using Ordering.Domain.ValueObjects;

namespace Ordering.Application.Orders.Commands.UpdateOrder;

public class UpdateOrderCommandHandler
    : ICommandHandler<UpdateOrderCommand, UpdateOrderResult>
{
    private readonly IApplicationDbContext _context;

    public UpdateOrderCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<UpdateOrderResult> Handle(UpdateOrderCommand command, CancellationToken cancellationToken)
    {
        var orderId = OrderId.Of(command.Order.Id);
        var order = await _context.Orders.FindAsync([orderId], cancellationToken);
        if (order is null)
        {
            throw new OrderNotFoundException(command.Order.Id);
        }
       
        UpdateOrder(order, command.Order);

        _context.Orders.Update(order);
        await _context.SaveChangesAsync(cancellationToken);

        return new UpdateOrderResult(true);
    }

    private void UpdateOrder(Order order, OrderDto orderDto)
    {
        var updateShippingAddress = Address.Of(
            orderDto.ShippingAddress.FirstName,
            orderDto.ShippingAddress.LastName,
            orderDto.ShippingAddress.EmailAddress,
            orderDto.ShippingAddress.AddressLine,
            orderDto.ShippingAddress.Country,
            orderDto.ShippingAddress.State,
            orderDto.ShippingAddress.ZipCode
            );
        var updateBillingAddress = Address.Of(
            orderDto.BillingAddress.FirstName,
            orderDto.BillingAddress.LastName,
            orderDto.BillingAddress.EmailAddress,
            orderDto.BillingAddress.AddressLine,
            orderDto.BillingAddress.Country,
            orderDto.BillingAddress.State,
            orderDto.BillingAddress.ZipCode
            );
        var updatePayment = Payment.Of(
            orderDto.Payment.CardName, 
            orderDto.Payment.CardNumber, 
            orderDto.Payment.Expiration, 
            orderDto.Payment.Cvv, 
            orderDto.Payment.PaymentMethod
            );

        order.Update(OrderName.Of(orderDto.OrderName), updateShippingAddress, updateBillingAddress, updatePayment, orderDto.Status);
    }
}
