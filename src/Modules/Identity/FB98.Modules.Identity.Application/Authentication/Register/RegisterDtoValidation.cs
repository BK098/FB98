namespace FB98.Modules.Identity.Application.Authentication.Register
{
	internal sealed class RegisterDtoValidation : AbstractValidator<RegisterDto>
	{
		public RegisterDtoValidation(ILocalizedMessageService message)
		{
			RuleFor(x => x.Email)
				.Cascade(CascadeMode.Stop)
				.NotEmpty().WithMessage("EmailRequired")
				.EmailAddress().WithMessage("EmailInvalid");

			RuleFor(x => x.Password)
				.NotEmpty().WithMessage(message.GetLocalizedMessage("PasswordRequired"))
				.MinimumLength(6).WithMessage(message.GetLocalizedMessage("PasswordTooShort"))
				.Matches(@"[A-Z]").WithMessage(message.GetLocalizedMessage("PasswordMustContainUppercase"))
				.Matches(@"[0-9]").WithMessage(message.GetLocalizedMessage("PasswordMustContainNumber"))
				.Matches(@"[\W]").WithMessage(message.GetLocalizedMessage("PasswordMustContainSpecialCharacter"));

			RuleFor(x => x.PhoneNumber)
				.NotEmpty().WithMessage(message.GetLocalizedMessage("PhoneNumberRequired"))
				.Matches(@"^((\+84)|0)(3|5|7|8|9)[0-9]{8}$").WithMessage(message.GetLocalizedMessage("PhoneNumberInvalid"));

			RuleFor(x => x.Firstname)
				.NotEmpty().WithMessage(message.GetLocalizedMessage("FirstnameRequired"));

			RuleFor(x => x.Lastname)
				.NotEmpty().WithMessage(message.GetLocalizedMessage("LastnameRequired"));

			RuleFor(x => x.BirthOfDate)
				.NotEmpty().WithMessage(message.GetLocalizedMessage("BirthOfDateRequired"))
				.Must(date => date <= DateOnly.FromDateTime(DateTime.Today) && date >= DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-99)))
				.WithMessage(message.GetLocalizedMessage("BirthOfDateInvalid"));
		}
	}
}
