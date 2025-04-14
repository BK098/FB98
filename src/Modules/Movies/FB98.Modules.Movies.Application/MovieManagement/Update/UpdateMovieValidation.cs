namespace FB98.Modules.Movies.Application.MovieManagement.Update
{
	internal sealed class UpdateMovieValidation : AbstractValidator<UpdateMovieDto>
	{
		public UpdateMovieValidation(ILocalizedMessageService message)
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

			RuleFor(x => x.Casts)
				.NotNull().WithMessage(message.GetLocalizedMessage("NotNull"))
				.NotEmpty().WithMessage(message.GetLocalizedMessage("NotEmpty"))
				.Must(BeUniqueCastIds).WithMessage(message.GetLocalizedMessage("DuplicateData"));

			RuleFor(x => x.Genres)
				.NotNull().WithMessage(message.GetLocalizedMessage("NotNull"))
				.NotEmpty().WithMessage(message.GetLocalizedMessage("NotEmpty"))
				.Must(BeUniqueGenreIds).WithMessage(message.GetLocalizedMessage("DuplicateData"));

			RuleFor(x => x.Directors)
				.NotNull().WithMessage(message.GetLocalizedMessage("NotNull"))
				.NotEmpty().WithMessage(message.GetLocalizedMessage("NotEmpty"))
				.Must(BeUniqueDirectorIds).WithMessage(message.GetLocalizedMessage("DuplicateData"));
		}

		private bool BeUniqueCastIds(ICollection<UpdateMovieCastDto>? casts)
		{
			if (casts == null)
			{
				return true;
			}

			var productIds = casts.Where(s => s.Id.HasValue).Select(s => s.Id!.Value).ToList();
			return productIds.Distinct().Count() == productIds.Count();
		}

		private bool BeUniqueGenreIds(ICollection<UpdateMovieGenreDto>? genres)
		{
			if (genres == null)
			{
				return true;
			}

			var productIds = genres.Where(s => s.Id.HasValue).Select(s => s.Id!.Value).ToList();
			return productIds.Distinct().Count() == productIds.Count();
		}

		private bool BeUniqueDirectorIds(ICollection<UpdateMovieDirectorDto>? directors)
		{
			if (directors == null)
			{
				return true;
			}

			var productIds = directors.Where(s => s.Id.HasValue).Select(s => s.Id!.Value).ToList();
			return productIds.Distinct().Count() == productIds.Count();
		}
	}
}