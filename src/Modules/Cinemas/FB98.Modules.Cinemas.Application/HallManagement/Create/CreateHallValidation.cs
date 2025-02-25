namespace FB98.Modules.Cinemas.Application.HallManagement.Create
{
	internal sealed class CreateHallValidation : AbstractValidator<CreateHallDto>
	{
		public CreateHallValidation(ILocalizedMessageService message)
		{
			RuleFor(x => x.Name)
				.NotNull().WithMessage(message.GetLocalizedMessage("NotNull"))
				.NotEmpty().WithMessage(message.GetLocalizedMessage("NotEmpty"));

			RuleFor(x => x.CinemaId)
				.NotNull().WithMessage(message.GetLocalizedMessage("NotNull"))
				.NotEmpty().WithMessage(message.GetLocalizedMessage("NotEmpty"));

			RuleFor(x => x.RangeSeatColumn)
				.GreaterThanOrEqualTo(1).WithMessage(message.GetLocalizedMessage("GreaterThanOrEqualTo1"))
				.LessThanOrEqualTo(15).WithMessage(message.GetLocalizedMessage("LessThanOrEqualTo15"))
				.NotEmpty().WithMessage(message.GetLocalizedMessage("NotEmpty"));

			RuleFor(x => x.RangeSeatRow)
				.GreaterThanOrEqualTo(1).WithMessage(message.GetLocalizedMessage("GreaterThanOrEqualTo1"))
				.LessThanOrEqualTo(15).WithMessage(message.GetLocalizedMessage("LessThanOrEqualTo15"))
				.NotEmpty().WithMessage(message.GetLocalizedMessage("NotEmpty"));
		}
	}
}