namespace FB98.Modules.Cinemas.Application.HallManagement.CheckSeats
{
	internal sealed class CheckSeatsValidation : AbstractValidator<CheckSeatsDto>
	{
		public CheckSeatsValidation(ILocalizedMessageService message)
		{
			RuleFor(x => x.SeatIds)
				.NotNull().WithMessage(message.GetLocalizedMessage("NotNull"))
				.NotEmpty().WithMessage(message.GetLocalizedMessage("NotEmpty"))
				.Must(BeUniqueSeatIds).WithMessage(message.GetLocalizedMessage("DuplicateData"));
		}

		private bool BeUniqueSeatIds(List<Guid> seatIds)
		{
			return seatIds.Distinct().Count() == seatIds.Count();
		}
	}
}