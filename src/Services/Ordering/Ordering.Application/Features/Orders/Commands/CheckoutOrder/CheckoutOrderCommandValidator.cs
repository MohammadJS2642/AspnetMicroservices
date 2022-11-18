using FluentValidation;

namespace Ordering.Application.Features.Orders.Commands.CheckoutOrder
{
    public class CheckoutOrderCommandValidator : AbstractValidator<CheckoutOrderCommand>
    {
        public CheckoutOrderCommandValidator()
        {
            RuleFor(p => p.UserName)
                .NotEmpty()
                .WithMessage("{UserName} is Required")
                .NotNull()
                .MaximumLength(50)
                .WithMessage("{UserName} must not be exceed 50 characters.");

            RuleFor(p => p.EmailAddress)
                .NotEmpty()
                .WithMessage("{EmailAddress} is Required");

            RuleFor(p => p.TotalPrice)
                .NotEmpty()
                .WithMessage("{TotalPrice} is Required")
                .GreaterThan(0)
                .WithMessage("{TotalPrice} should be greater than zero.");
        }
    }
}
