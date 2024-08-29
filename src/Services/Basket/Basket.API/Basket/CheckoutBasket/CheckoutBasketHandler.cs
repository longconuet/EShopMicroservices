using Basket.API.Dtos;
using BuildingBlocks.Messaging.Events;
using FluentValidation;
using Mapster;
using MassTransit;

namespace Basket.API.Basket.CheckoutBasket;

public record CheckoutBasketCommand(BasketCheckoutDto BasketCheckoutDto) : ICommand<CheckoutBasketResult>;

public record CheckoutBasketResult(bool IsSuccess);

public class CheckoutBasketValidator : AbstractValidator<CheckoutBasketCommand>
{
    public CheckoutBasketValidator()
    {
        RuleFor(x => x.BasketCheckoutDto).NotNull().WithMessage("BasketCheckoutDto is not be null");
        RuleFor(x => x.BasketCheckoutDto.Username).NotEmpty().WithMessage("Username is required");
    }
}

public class CheckoutBasketCommandHandler (IBasketRepository basketRepository, IPublishEndpoint publishEndpoint)
    : ICommandHandler<CheckoutBasketCommand, CheckoutBasketResult>
{
    public async Task<CheckoutBasketResult> Handle(CheckoutBasketCommand command, CancellationToken cancellationToken)
    {
        var basket = await basketRepository.GetBasketAsync(command.BasketCheckoutDto.Username, cancellationToken);
        if (basket == null)
        {
            return new CheckoutBasketResult(false);
        }

        // publish event message
        var eventMessage = command.BasketCheckoutDto.Adapt<BasketCheckoutEvent>();
        eventMessage.TotalPrice = basket.TotalPrice;

        await publishEndpoint.Publish(eventMessage, cancellationToken);

        // delete basket
        await basketRepository.DeleteBasketAsync(command.BasketCheckoutDto.Username, cancellationToken);

        return new CheckoutBasketResult(true);
    }
}
