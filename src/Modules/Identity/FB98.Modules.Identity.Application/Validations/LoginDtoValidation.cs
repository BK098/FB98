using FB98.Modules.Identity.Application.Models;
using FB98.Shared.Infrastructure.Localization;
using FluentValidation;

namespace FB98.Modules.Identity.Application.Validations
{
	public class LoginDtoValidation : AbstractValidator<LoginDto>
	{
		public LoginDtoValidation(ILocalizedMessageService message)
		{
			RuleFor(x => x.Email)
				.Cascade(CascadeMode.Stop)
				.NotEmpty().WithMessage(message.GetLocalizedMessage("EmailRequired"))
				.EmailAddress().WithMessage(message.GetLocalizedMessage("EmailInvalid"));

			RuleFor(x => x.Password)
				.Cascade(CascadeMode.Stop)
				.NotEmpty().WithMessage(message.GetLocalizedMessage("PasswordRequired"));
		}
	}
}
