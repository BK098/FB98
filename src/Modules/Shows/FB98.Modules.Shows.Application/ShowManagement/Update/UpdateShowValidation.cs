namespace FB98.Modules.Shows.Application.ShowManagement.Update
{
	internal sealed class UpdateShowValidation : AbstractValidator<UpdateShowDto>
	{
		public UpdateShowValidation(ILocalizedMessageService message)
		{
			RuleFor(x => x.MovieId)
				.NotNull().WithMessage(message.GetLocalizedMessage("NotNull"))
				.NotEmpty().WithMessage(message.GetLocalizedMessage("NotEmpty"));

			RuleFor(x => x.CinemaHallId)
				.NotNull().WithMessage(message.GetLocalizedMessage("NotNull"))
				.NotEmpty().WithMessage(message.GetLocalizedMessage("NotEmpty"));

			RuleFor(x => x.StartTime)
				.NotNull().WithMessage(message.GetLocalizedMessage("NotNull"))
				.NotEmpty().WithMessage(message.GetLocalizedMessage("NotEmpty"));

			RuleFor(x => x.EndTime)
				.NotNull().WithMessage(message.GetLocalizedMessage("NotNull"))
				.NotEmpty().WithMessage(message.GetLocalizedMessage("NotEmpty"));

			RuleFor(x => x.Features)
				.NotNull().WithMessage(message.GetLocalizedMessage("NotNull"))
				.NotEmpty().WithMessage(message.GetLocalizedMessage("NotEmpty"))
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