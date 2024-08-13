using Basket.API.Data;
using FluentValidation;

namespace Basket.API.Basket.StoreBasket;

public record StoreBasketCommand(ShoppingCart Cart) : ICommand<StoreBasketResult>;
public record StoreBasketResult(string Username);

public class StoreBasketCommandValidator : AbstractValidator<StoreBasketCommand>
{
    public StoreBasketCommandValidator()
    {
        RuleFor(x => x.Cart).NotNull().WithMessage("Cart is not null");
        RuleFor(x => x.Cart.Username).NotEmpty().WithMessage("Username is required");
    }
}

public class StoreBasketCommandHandler : ICommandHandler<StoreBasketCommand, StoreBasketResult>
{
    private readonly IBasketRepository _basketRepository;

    public StoreBasketCommandHandler(IBasketRepository basketRepository)
    {
        _basketRepository = basketRepository;
    }

    public async Task<StoreBasketResult> Handle(StoreBasketCommand command, CancellationToken cancellationToken)
    {
        await _basketRepository.StoreBasketAsync(command.Cart, cancellationToken);
        return new StoreBasketResult(command.Cart.Username);
    }
}