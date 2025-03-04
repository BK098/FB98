namespace FB98.Modules.Movies.Application.MovieManagement.Create
{
	internal sealed class CreateMovieValidation : AbstractValidator<CreateMovieDto>
	{
		public CreateMovieValidation(ILocalizedMessageService message)
		{
			RuleFor(x => x.Title)
				.NotNull().WithMessage(message.GetLocalizedMessage("NotNull"))
				.NotEmpty().WithMessage(message.GetLocalizedMessage("NotEmpty"));

			RuleFor(x => x.Country)
				.NotNull().WithMessage(message.GetLocalizedMessage("NotNull"))
				.NotEmpty().WithMessage(message.GetLocalizedMessage("NotEmpty"));

			RuleFor(x => x.AgeRating).IsInEnum().WithMessage("test cái enum");

			RuleFor(x => x.ReleaseDate)
				.NotNull().WithMessage(message.GetLocalizedMessage("NotNull"))
				.NotEmpty().WithMessage(message.GetLocalizedMessage("NotEmpty"));

			RuleFor(x => x.IsPublished)
				.NotNull().WithMessage(message.GetLocalizedMessage("NotNull"))
				.NotEmpty().WithMessage(message.GetLocalizedMessage("NotEmpty"));

			RuleFor(x => x.TrailerLink)
				.NotNull().WithMessage(message.GetLocalizedMessage("NotNull"))
				.NotEmpty().WithMessage(message.GetLocalizedMessage("NotEmpty"));

			RuleFor(x => x.RuntimeMinutes)
				.GreaterThanOrEqualTo(30).WithMessage(message.GetLocalizedMessage("GreaterThanOrEqualTo30"))
				.LessThanOrEqualTo(250).WithMessage(message.GetLocalizedMessage("LessThanOrEqualTo250"))
				.NotNull().WithMessage(message.GetLocalizedMessage("NotNull"))
				.NotEmpty().WithMessage(message.GetLocalizedMessage("NotEmpty"));


		}
	}
}