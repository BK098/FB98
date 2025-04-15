namespace FB98.Modules.Shows.Application.ShowManagement.CreateRange
{
	public  sealed class CreateRangeShowValidation : AbstractValidator<CreateRangeShowDto>
	{
		public CreateRangeShowValidation(ILocalizedMessageService message)
		{
			RuleFor(x => x.MovieId)
				.NotNull().WithMessage(message.GetLocalizedMessage("NotNull"))
				.NotEmpty().WithMessage(message.GetLocalizedMessage("NotEmpty"));

			RuleFor(x => x.CinemaHallId)
				.NotNull().WithMessage(message.GetLocalizedMessage("NotNull"))
				.NotEmpty().WithMessage(message.GetLocalizedMessage("NotEmpty"));

			RuleFor(x => x.StartDate)
				.LessThan(x => x.EndDate).WithMessage(message.GetLocalizedMessage("StartDateBeforeEndDate"));

			RuleFor(x => x.TimeRest)
				.InclusiveBetween(1, 30).WithMessage(message.GetLocalizedMessage("TimeRestRange"));

			RuleFor(x => x.Features)
				.NotNull().WithMessage(message.GetLocalizedMessage("NotNull"))
				.NotEmpty().WithMessage(message.GetLocalizedMessage("NotEmpty"));

			RuleFor(x => x.Features)
				.Must(features => features.Select(f => f.FeatureId).Distinct().Count() == features.Count)
				.WithMessage(message.GetLocalizedMessage("DuplicateData"));

			RuleForEach(x => x.Features).ChildRules(features =>
			{
				features.RuleFor(f => f.FeatureId)
					.NotNull().WithMessage(message.GetLocalizedMessage("NotNull"))
					.NotEmpty().WithMessage(message.GetLocalizedMessage("NotEmpty"));
			});
		}
	}
}