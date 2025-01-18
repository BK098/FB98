using FB98.Modules.Identity.Application.Models;
using FB98.Shared.Infrastructure.Localization;
using FluentValidation;

namespace FB98.Modules.Identity.Application.Validations
{
	public class ChangePasswordDtoValidation : AbstractValidator<ChangePasswordDto>
	{
		public ChangePasswordDtoValidation(ILocalizedMessageService message)
		{
			RuleFor(x => x.CurrentPassword)
				.NotEmpty().WithMessage(message.GetLocalizedMessage("CurrentPasswordRequired"));

			RuleFor(x => x.NewPassword)
				.NotEmpty().WithMessage(message.GetLocalizedMessage("PasswordRequired"))
				.MinimumLength(6).WithMessage(message.GetLocalizedMessage("PasswordTooShort"))
				.Matches(@"[A-Z]").WithMessage(message.GetLocalizedMessage("PasswordMustContainUppercase"))
				.Matches(@"[0-9]").WithMessage(message.GetLocalizedMessage("PasswordMustContainNumber"))
				.Matches(@"[\W]").WithMessage(message.GetLocalizedMessage("PasswordMustContainSpecialCharacter"))
				.NotEqual(x => x.CurrentPassword).WithMessage(message.GetLocalizedMessage("PasswordMustBeDifferent"));
		}
	}
}
