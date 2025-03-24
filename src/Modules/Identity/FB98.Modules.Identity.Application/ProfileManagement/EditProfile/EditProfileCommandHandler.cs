using FB98.Modules.Identity.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace FB98.Modules.Identity.Application.ProfileManagement.EditProfile
{
	internal class EditProfileCommandHandler : ICommandHandler<EditProfileCommand, ApiResult<object>>
	{
		private readonly ILocalizedMessageService _localizedMessageService;
		private readonly ILogger<EditProfileCommandHandler> _logger;
		private readonly UserManager<AppUser> _userManager;
		private readonly IValidator<EditProfileDto> _validator;

		public EditProfileCommandHandler(
			UserManager<AppUser> userManager,
			ILocalizedMessageService localizedMessageService,
			ILogger<EditProfileCommandHandler> logger,
			IValidator<EditProfileDto> validator)
		{
			_userManager = userManager;
			_localizedMessageService = localizedMessageService;
			_logger = logger;
			_validator = validator;
		}

		public async Task<ApiResult<object>> Handle(EditProfileCommand request, CancellationToken cancellationToken)
		{
			var userId = request.UserId;
			var model = request.Model;
			try
			{
				var validationResult = await _validator.ValidateAsync(model, cancellationToken);
				if (!validationResult.IsValid)
				{
					return ApiResponseBuilder.ValidationError<object>(validationResult.Errors, _localizedMessageService.GetLocalizedMessage("ValidationFailed"));
				}

				var user = await _userManager.FindByIdAsync(userId.ToString());
				if (user == null)
				{
					return ApiResponseBuilder.Error<object>(_localizedMessageService.GetLocalizedMessage("NotFound"), 404);
				}

				var phoneNumber = user.PhoneNumber;
				if (model.PhoneNumber != phoneNumber)
				{
					var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, model.PhoneNumber);
					if (!setPhoneResult.Succeeded)
					{
						return ApiResponseBuilder.Error<object>(_localizedMessageService.GetLocalizedMessage(""));
					}
				}

				var email = user.Email;
				if (model.Email != email)
				{
					var emailExists = await _userManager.FindByEmailAsync(model.Email!);
					if (emailExists != null)
					{
						return ApiResponseBuilder.Error<object>(_localizedMessageService.GetLocalizedMessage("EmailExists"));
					}

					var setEmailResult = await _userManager.SetEmailAsync(user, model.Email);
					if (!setEmailResult.Succeeded)
					{
						return ApiResponseBuilder.Error<object>(_localizedMessageService.GetLocalizedMessage(""));
					}
				}

				user.Firstname = model.FirstName!;
				user.Lastname = model.LastName!;
				user.BirthOfDate = model.BirthOfDate!.Value;
				user.Age = CaculatorAge(model.BirthOfDate!.Value);
				user.Gender = model.Gender;

				var result = await _userManager.UpdateAsync(user);
				if (!result.Succeeded)
				{
					return ApiResponseBuilder.Error<object>(_localizedMessageService.GetLocalizedMessage(""));
				}

				return ApiResponseBuilder.Success<object>(userId, _localizedMessageService.GetLocalizedMessage("Updated"));
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