namespace FB98.Modules.Cinemas.Application.HallManagement.Update
{
	internal sealed class UpdateHallValidation : AbstractValidator<UpdateHallDto>
	{
		public UpdateHallValidation(ILocalizedMessageService message)
		{
			RuleFor(x => x.Name)
				.NotNull().WithMessage(message.GetLocalizedMessage("NotNull"))
				.NotEmpty().WithMessage(message.GetLocalizedMessage("NotEmpty"));

			When(x => x.Seats != null, () =>
			{
				RuleFor(x => x.Seats)
					.Must(BeUniqueSeatIds).WithMessage(message.GetLocalizedMessage("DuplicateData"));

				RuleForEach(x => x.Seats).ChildRules(seat =>
				{
					seat.RuleFor(s => s.SeatId)
						.NotNull().WithMessage(message.GetLocalizedMessage("NotNull"))
						.NotEmpty().WithMessage(message.GetLocalizedMessage("NotEmpty"));

					seat.RuleFor(s => s.SeatTypeId)
						.NotNull().WithMessage(message.GetLocalizedMessage("NotNull"))
						.NotEmpty().WithMessage(message.GetLocalizedMessage("NotEmpty"));
				});
			});
		}

		private static bool BeUniqueSeatIds(ICollection<UpdateSeatDto>? seats)
		{
			if (seats == null)
			{
				return true;
			}

			var seatIds = seats.Where(s => s.SeatId.HasValue).Select(s => s.SeatId.Value).ToList();
			return seatIds.Distinct().Count() == seatIds.Count();
		}
	}
}