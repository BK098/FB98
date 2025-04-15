using FB98.Modules.Identity.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FB98.Modules.Identity.Application.Authentication.Register
{
	public sealed class RegisterCommandHandler : ICommandHandler<RegisterCommand, ApiResult<object>>
	{
		private readonly ILocalizedMessageService _localizedMessageService;
		private readonly ILogger<RegisterCommandHandler> _logger;
		private readonly UserManager<AppUser> _userManager;
		private readonly IValidator<RegisterDto> _validator;

		public RegisterCommandHandler(
			UserManager<AppUser> userManager,
			ILogger<RegisterCommandHandler> logger,
			IValidator<RegisterDto> validator,
			ILocalizedMessageService localizedMessageService)
		{
			_userManager = userManager;
			_logger = logger;
			_validator = validator;
			_localizedMessageService = localizedMessageService;
		}

		public async Task<ApiResult<object>> Handle(RegisterCommand request, CancellationToken cancellationToken)
		{
			var model = request.Model;
			try
			{
				var validationResult = await _validator.ValidateAsync(model, cancellationToken);
				if (!validationResult.IsValid)
				{
					return ApiResponseBuilder.ValidationError<object>(validationResult.Errors, _localizedMessageService.GetLocalizedMessage("ValidationFailed"));
				}

				var checkUser = await _userManager.FindByEmailAsync(model.Email!);
				if (checkUser != null)
				{
					return ApiResponseBuilder.Error<object>(_localizedMessageService.GetLocalizedMessage("UserAlreadyExists"), 409);
				}

				var checkPhoneNumber = await _userManager.Users.FirstOrDefaultAsync(x => x.PhoneNumber == model.PhoneNumber, cancellationToken);
				if (checkPhoneNumber != null)
				{
					return ApiResponseBuilder.Error<object>(_localizedMessageService.GetLocalizedMessage("PhoneNumberExists"), 409);
				}

				var user = new AppUser
				{
					UserName = model.Email,
					Email = model.Email,
					PhoneNumber = model.PhoneNumber,
					Firstname = model.Firstname!,
					Lastname = model.Lastname!,
					Age = CaculatorAge(model.BirthOfDate),
					BirthOfDate = model.BirthOfDate
				};
				var result = await _userManager.CreateAsync(user, model.Password!);

				if (!result.Succeeded)
				{
					return ApiResponseBuilder.Error<object>(_localizedMessageService.GetLocalizedMessage("UserCreationFailed"), 400);
				}

				return ApiResponseBuilder.Success<object>("", _localizedMessageService.GetLocalizedMessage("AccountCreatedSuccessfully"), 201);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An error occurred: Register");
				return ApiResponseBuilder.Error<object>("An unexpected error occurred", 500);
			}
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