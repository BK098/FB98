using FluentValidation;

namespace FB98.Modules.ShoppingList.Application.TodoManagement.CreateTodoItem
{
	internal sealed class CreateTodoItemValidation : AbstractValidator<CreateTodoItemDto>
	{
		public CreateTodoItemValidation()
		{
			RuleFor(x => x.TodoId).NotEmpty().WithMessage("TodoId is required");

			RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("Quantity must be greater than zero");

			// Nếu có ProductId => Không được nhập Name, Unit
			When(x => x.ProductId.HasValue, () =>
			{
				RuleFor(x => x.Name).Must(name => string.IsNullOrWhiteSpace(name))
					.WithMessage("Do not provide Name when ProductId is present");

				RuleFor(x => x.Unit).Must(unit => string.IsNullOrWhiteSpace(unit))
					.WithMessage("Do not provide Unit when ProductId is present");
			});

			// Nếu không có ProductId => Name và Unit là bắt buộc
			When(x => !x.ProductId.HasValue, () =>
			{
				RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required when ProductId is not provided");

				RuleFor(x => x.Unit).NotEmpty().WithMessage("Unit is required when ProductId is not provided");
			});
		}
	}
}