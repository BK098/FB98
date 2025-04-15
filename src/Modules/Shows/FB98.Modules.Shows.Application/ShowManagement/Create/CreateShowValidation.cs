namespace FB98.Modules.Shows.Application.ShowManagement.Create
{
	public sealed class CreateShowValidation : AbstractValidator<CreateShowDto>
	{
		public CreateShowValidation(ILocalizedMessageService message)
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

			RuleFor(x => x.Features)
				.NotNull().WithMessage(message.GetLocalizedMessage("NotNull"))
				.NotEmpty().WithMessage(message.GetLocalizedMessage("NotEmpty"))
				.Must(features => features.Select(f => f.FeatureId).Distinct().Count() == features.Count)
				.WithMessage(message.GetLocalizedMessage("DuplicateData"));
		}
	}
}