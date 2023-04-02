using FluentValidation;

namespace Ordering.Application.Features.Orders.Commands.CheckoutOrder
{
	public class CheckoutOrderCommandValidator : AbstractValidator<CheckoutOrderCommand>
	{
		public CheckoutOrderCommandValidator()
		{
			RuleFor(c => c.UserName)
				.NotEmpty().WithMessage("{UserName} is required")
				.NotNull()
				.MaximumLength(50).WithMessage("{UserName} must not exceed 50 characters.");
		}
	}
}
