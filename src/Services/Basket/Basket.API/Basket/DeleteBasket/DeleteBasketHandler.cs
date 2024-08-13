using FluentValidation;

namespace Basket.API.Basket.DeleteBasket;

public record DeleteBasketCommand(string Username) : ICommand<DeleteBasketResult>;
public record DeleteBasketResult(bool IsSuccess);

public class DeleteBasketCommandValidator : AbstractValidator<DeleteBasketCommand>
{
    public DeleteBasketCommandValidator()
    {
        RuleFor(x => x.Username).NotEmpty().WithMessage("Username is required");
    }
}

public class DeleteBasketHandler : ICommandHandler<DeleteBasketCommand, DeleteBasketResult>
{
    private readonly IBasketRepository _basketRepository;

    public DeleteBasketHandler(IBasketRepository basketRepository)
    {
        _basketRepository = basketRepository;
    }

    public async Task<DeleteBasketResult> Handle(DeleteBasketCommand command, CancellationToken cancellationToken)
    {
        await _basketRepository.DeleteBasketAsync(command.Username, cancellationToken);
        return new DeleteBasketResult(true);
    }
}
