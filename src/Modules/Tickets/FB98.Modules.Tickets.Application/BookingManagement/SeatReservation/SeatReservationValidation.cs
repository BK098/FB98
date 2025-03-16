namespace FB98.Modules.Tickets.Application.BookingManagement.SeatReservation
{
	internal sealed class SeatReservationValidation : AbstractValidator<SeatReservationDto>
	{
		public SeatReservationValidation(ILocalizedMessageService message)
		{
			RuleFor(x => x.CustomerId)
				.NotNull().WithMessage(message.GetLocalizedMessage("NotNull"))
				.NotEmpty().WithMessage(message.GetLocalizedMessage("NotEmpty"));
			RuleFor(x => x.ShowId)
				.NotNull().WithMessage(message.GetLocalizedMessage("NotNull"))
				.NotEmpty().WithMessage(message.GetLocalizedMessage("NotEmpty"));
		}
	}
}