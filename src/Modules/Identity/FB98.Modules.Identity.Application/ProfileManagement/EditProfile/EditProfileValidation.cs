namespace FB98.Modules.Identity.Application.ProfileManagement.EditProfile
{
	internal sealed class EditProfileValidation : AbstractValidator<EditProfileDto>
	{
		public EditProfileValidation(ILocalizedMessageService message)
		{
			RuleFor(x => x.FirstName)
				.NotNull().WithMessage(message.GetLocalizedMessage("NotNull"))
				.NotEmpty().WithMessage(message.GetLocalizedMessage("NotEmpty"));

			RuleFor(x => x.LastName)
				.NotNull().WithMessage(message.GetLocalizedMessage("NotNull"))
				.NotEmpty().WithMessage(message.GetLocalizedMessage("NotEmpty"));

			RuleFor(x => x.Email)
				.NotNull().WithMessage(message.GetLocalizedMessage("NotNull"))
				.NotEmpty().WithMessage(message.GetLocalizedMessage("NotEmpty"));

			RuleFor(x => x.PhoneNumber)
				.NotNull().WithMessage(message.GetLocalizedMessage("NotNull"))
				.NotEmpty().WithMessage(message.GetLocalizedMessage("NotEmpty"));

			RuleFor(x => x.BirthOfDate)
				.NotNull().WithMessage(message.GetLocalizedMessage("NotNull"))
				.NotEmpty().WithMessage(message.GetLocalizedMessage("NotEmpty"))
				.Must(birthDate => birthDate != null && CaculatorAge(birthDate.Value) >= 13)
				.WithMessage(message.GetLocalizedMessage("AgeRestriction"));
		}

		private static byte CaculatorAge(DateOnly birthOfDate)
		{
			var currentDate = DateOnly.FromDateTime(DateTime.Today);
			var age = currentDate.Year - birthOfDate.Year;

			if (currentDate < birthOfDate.AddYears(age))
			{
				--age;
			}

			return (byte)age;
		}
	}
}