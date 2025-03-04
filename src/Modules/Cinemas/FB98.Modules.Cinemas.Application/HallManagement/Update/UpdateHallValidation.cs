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
				RuleForEach(x => x.Seats).SetValidator(new UpdateHallSeatValidation(message));
			});
		}

		private bool BeUniqueSeatIds(ICollection<UpdateSeatDto>? seats)
		{
			if (seats == null)
			{
				return true;
			}

			var seatIds = seats.Where(s => s.SeatId.HasValue).Select(s => s.SeatId.Value).ToList();
			return seatIds.Distinct().Count() == seatIds.Count();
		}
	}

	internal sealed class UpdateHallSeatValidation : AbstractValidator<UpdateSeatDto>
	{
		public UpdateHallSeatValidation(ILocalizedMessageService message)
		{
			RuleFor(x => x.SeatId)
				.NotNull().WithMessage(message.GetLocalizedMessage("NotNull"))
				.NotEmpty().WithMessage(message.GetLocalizedMessage("NotEmpty"));

			RuleFor(x => x.SeatTypeId)
				.NotNull().WithMessage(message.GetLocalizedMessage("NotNull"))
				.NotEmpty().WithMessage(message.GetLocalizedMessage("NotEmpty"));
		}
	}
}