namespace FB98.Modules.Tickets.Application.SeatManagement.UnlockSeat
{
	public  sealed class UnlockSeatsValidation : AbstractValidator<UnlockSeatsDto>
	{
		public UnlockSeatsValidation(ILocalizedMessageService message)
		{
			RuleFor(x => x.CustomerId)
				.NotNull().WithMessage(message.GetLocalizedMessage("NotNull"))
				.NotEmpty().WithMessage(message.GetLocalizedMessage("NotEmpty"));
			RuleFor(x => x.ShowId)
				.NotNull().WithMessage(message.GetLocalizedMessage("NotNull"))
				.NotEmpty().WithMessage(message.GetLocalizedMessage("NotEmpty"));
			RuleFor(x => x.SeatIds)
				.NotNull().WithMessage(message.GetLocalizedMessage("NotNull"))
				.NotEmpty().WithMessage(message.GetLocalizedMessage("NotEmpty"));
			RuleFor(x => x.SeatIds)
				.NotNull().WithMessage(message.GetLocalizedMessage("NotNull"))
				.NotEmpty().WithMessage(message.GetLocalizedMessage("NotEmpty"))
				.Must(seats => seats!.Count == seats.Distinct().Count())
				.WithMessage(message.GetLocalizedMessage("DuplicateData"));
		}
	}
}