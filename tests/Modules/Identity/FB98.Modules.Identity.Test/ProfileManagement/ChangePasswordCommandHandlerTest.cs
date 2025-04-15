using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using FB98.Modules.Identity.Application.ProfileManagement.ChangePassword;
using FB98.Modules.Identity.Domain.Entities;
using FB98.Shared.Infrastructure.Localization;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using Xunit.Abstractions;

namespace FB98.Modules.Identity.Test.ProfileManagement
{
	public class ChangePasswordCommandHandlerTest
	{
		private readonly ChangePasswordCommandHandler _handler;
		private readonly Mock<UserManager<AppUser>> _userManagerMock;
		private readonly Mock<ILocalizedMessageService> _localizedMessageServiceMock;
		private readonly Mock<IValidator<ChangePasswordDto>> _validatorMock;
		private readonly Mock<ILogger<ChangePasswordCommandHandler>> _loggerMock;
		private readonly ITestOutputHelper _output;

		public ChangePasswordCommandHandlerTest(ITestOutputHelper output)
		{
			_output = output;
			_loggerMock = new Mock<ILogger<ChangePasswordCommandHandler>>();
			_validatorMock = new Mock<IValidator<ChangePasswordDto>>();
			_localizedMessageServiceMock = new Mock<ILocalizedMessageService>();

			// Setup localized message trả về key làm message
			_localizedMessageServiceMock
				.Setup(m => m.GetLocalizedMessage(It.IsAny<string>(), It.IsAny<string>()))
				.Returns((string key, string culture) => key);
			_localizedMessageServiceMock
				.Setup(m => m.GetLocalizedMessage(It.IsAny<string>(), null))
				.Returns((string key, string? culture) => key);

			var store = new Mock<IUserStore<AppUser>>();
			_userManagerMock = new Mock<UserManager<AppUser>>(
				store.Object, null, null, null, null, null, null, null, null
			);

			_handler = new ChangePasswordCommandHandler(
				_userManagerMock.Object,
				_loggerMock.Object,
				_localizedMessageServiceMock.Object,
				_validatorMock.Object
			);
		}


		/// TC_CP_001 Change Password Success
		[Fact]
		public async Task ChangePassword_ShouldSucceed_WhenDataIsValid()
		{
			_output.WriteLine("Test Case: TC_CP_001 Change Password Success");
			string validUserId = Guid.NewGuid().ToString();
			var changeDto = new ChangePasswordDto
			{
				CurrentPassword = "CurrentPass1!",
				NewPassword = "NewPass1!"
			};
			var command = new ChangePasswordCommand(validUserId, changeDto);

			// Validator passes
			_validatorMock.Setup(v => v.ValidateAsync(changeDto, It.IsAny<CancellationToken>()))
						  .ReturnsAsync(new ValidationResult());
			// Setup: tìm thấy user
			var userGuid = new Guid(validUserId);
			var user = new AppUser { Id = userGuid };
			_userManagerMock.Setup(u => u.FindByIdAsync(validUserId))
							.ReturnsAsync(user);
			// Setup: check current password thành công
			_userManagerMock.Setup(u => u.CheckPasswordAsync(user, changeDto.CurrentPassword))
							.ReturnsAsync(true);
			// Setup: ChangePasswordAsync thành công
			_userManagerMock.Setup(u => u.ChangePasswordAsync(user, changeDto.CurrentPassword, changeDto.NewPassword))
							.ReturnsAsync(IdentityResult.Success);

			var result = await _handler.Handle(command, CancellationToken.None);

			_output.WriteLine("Input Data: " + JsonSerializer.Serialize(changeDto));
			_output.WriteLine("Expected: IsSuccess = true, StatusCode = 200");
			_output.WriteLine($"Actual:   IsSuccess = {result.IsSuccess}, StatusCode = {result.StatusCode}");

			Assert.True(result.IsSuccess, "Change password should succeed with valid data.");
			Assert.Equal(200, result.StatusCode);
		}

		/// TC_CP_002 Change Password – Empty Current Password
		[Fact]
		public async Task ChangePassword_ShouldFail_WhenCurrentPasswordIsEmpty()
		{
			_output.WriteLine("Test Case: TC_CP_002 Change Password – Empty Current Password");
			string validUserId = Guid.NewGuid().ToString();
			var changeDto = new ChangePasswordDto
			{
				CurrentPassword = "",
				NewPassword = "NewPass1!"
			};
			var command = new ChangePasswordCommand(validUserId, changeDto);

			// Validator returns failure for empty current password
			var failures = new List<ValidationFailure> { new ValidationFailure("CurrentPassword", "CurrentPasswordRequired") };
			_validatorMock.Setup(v => v.ValidateAsync(changeDto, It.IsAny<CancellationToken>()))
						  .ReturnsAsync(new ValidationResult(failures));

			var result = await _handler.Handle(command, CancellationToken.None);

			_output.WriteLine("Input Data: " + JsonSerializer.Serialize(changeDto));
			_output.WriteLine("Expected: IsSuccess = false, StatusCode = 400");
			_output.WriteLine($"Actual:   IsSuccess = {result.IsSuccess}, StatusCode = {result.StatusCode}");

			Assert.False(result.IsSuccess, "Change password should fail when current password is empty.");
			Assert.Equal(400, result.StatusCode);
		}

		/// TC_CP_003 Change Password – Empty New Password
		[Fact]
		public async Task ChangePassword_ShouldFail_WhenNewPasswordIsEmpty()
		{
			_output.WriteLine("Test Case: TC_CP_003 Change Password – Empty New Password");
			string validUserId = Guid.NewGuid().ToString();
			var changeDto = new ChangePasswordDto
			{
				CurrentPassword = "CurrentPass1!",
				NewPassword = ""
			};
			var command = new ChangePasswordCommand(validUserId, changeDto);

			var failures = new List<ValidationFailure> { new ValidationFailure("NewPassword", "PasswordRequired") };
			_validatorMock.Setup(v => v.ValidateAsync(changeDto, It.IsAny<CancellationToken>()))
						  .ReturnsAsync(new ValidationResult(failures));

			var result = await _handler.Handle(command, CancellationToken.None);

			_output.WriteLine("Input Data: " + JsonSerializer.Serialize(changeDto));
			_output.WriteLine("Expected: IsSuccess = false, StatusCode = 400");
			_output.WriteLine($"Actual:   IsSuccess = {result.IsSuccess}, StatusCode = {result.StatusCode}");

			Assert.False(result.IsSuccess, "Change password should fail when new password is empty.");
			Assert.Equal(400, result.StatusCode);
		}


		/// TC_CP_004 Change Password – New Password Too Short
		[Fact]
		public async Task ChangePassword_ShouldFail_WhenNewPasswordIsTooShort()
		{
			_output.WriteLine("Test Case: TC_CP_004 Change Password – New Password Too Short");
			string validUserId = Guid.NewGuid().ToString();
			var changeDto = new ChangePasswordDto
			{
				CurrentPassword = "CurrentPass1!",
				NewPassword = "P1!" // too short
			};
			var command = new ChangePasswordCommand(validUserId, changeDto);

			var failures = new List<ValidationFailure> { new ValidationFailure("NewPassword", "PasswordTooShort") };
			_validatorMock.Setup(v => v.ValidateAsync(changeDto, It.IsAny<CancellationToken>()))
						  .ReturnsAsync(new ValidationResult(failures));

			var result = await _handler.Handle(command, CancellationToken.None);

			_output.WriteLine("Input Data: " + JsonSerializer.Serialize(changeDto));
			_output.WriteLine("Expected: IsSuccess = false, StatusCode = 400");
			_output.WriteLine($"Actual:   IsSuccess = {result.IsSuccess}, StatusCode = {result.StatusCode}");

			Assert.False(result.IsSuccess, "Change password should fail when new password is too short.");
			Assert.Equal(400, result.StatusCode);
		}

		/// TC_CP_005 Change Password – New Password Lacks Uppercase
		[Fact]
		public async Task ChangePassword_ShouldFail_WhenNewPasswordMissingUppercase()
		{
			_output.WriteLine("Test Case: TC_CP_005 Change Password – New Password Lacks Uppercase");
			string validUserId = Guid.NewGuid().ToString();
			var changeDto = new ChangePasswordDto
			{
				CurrentPassword = "CurrentPass1!",
				NewPassword = "newpass1!" // no uppercase
			};
			var command = new ChangePasswordCommand(validUserId, changeDto);

			var failures = new List<ValidationFailure> { new ValidationFailure("NewPassword", "PasswordMustContainUppercase") };
			_validatorMock.Setup(v => v.ValidateAsync(changeDto, It.IsAny<CancellationToken>()))
						  .ReturnsAsync(new ValidationResult(failures));

			var result = await _handler.Handle(command, CancellationToken.None);

			_output.WriteLine("Input Data: " + JsonSerializer.Serialize(changeDto));
			_output.WriteLine("Expected: IsSuccess = false, StatusCode = 400");
			_output.WriteLine($"Actual:   IsSuccess = {result.IsSuccess}, StatusCode = {result.StatusCode}");

			Assert.False(result.IsSuccess, "Change password should fail when new password is missing uppercase.");
			Assert.Equal(400, result.StatusCode);
		}

		/// TC_CP_006 Change Password – New Password Lacks Number
		[Fact]
		public async Task ChangePassword_ShouldFail_WhenNewPasswordMissingNumber()
		{
			_output.WriteLine("Test Case: TC_CP_006 Change Password – New Password Lacks Number");
			string validUserId = Guid.NewGuid().ToString();
			var changeDto = new ChangePasswordDto
			{
				CurrentPassword = "CurrentPass1!",
				NewPassword = "NewPass!" // no number
			};
			var command = new ChangePasswordCommand(validUserId, changeDto);

			var failures = new List<ValidationFailure> { new ValidationFailure("NewPassword", "PasswordMustContainNumber") };
			_validatorMock.Setup(v => v.ValidateAsync(changeDto, It.IsAny<CancellationToken>()))
						  .ReturnsAsync(new ValidationResult(failures));

			var result = await _handler.Handle(command, CancellationToken.None);

			_output.WriteLine("Input Data: " + JsonSerializer.Serialize(changeDto));
			_output.WriteLine("Expected: IsSuccess = false, StatusCode = 400");
			_output.WriteLine($"Actual:   IsSuccess = {result.IsSuccess}, StatusCode = {result.StatusCode}");

			Assert.False(result.IsSuccess, "Change password should fail when new password is missing a number.");
			Assert.Equal(400, result.StatusCode);
		}


		/// TC_CP_007 Change Password – New Password Lacks Special Character
		[Fact]
		public async Task ChangePassword_ShouldFail_WhenNewPasswordMissingSpecialCharacter()
		{
			_output.WriteLine("Test Case: TC_CP_007 Change Password – New Password Lacks Special Character");
			string validUserId = Guid.NewGuid().ToString();
			var changeDto = new ChangePasswordDto
			{
				CurrentPassword = "CurrentPass1!",
				NewPassword = "NewPass1" // no special character
			};
			var command = new ChangePasswordCommand(validUserId, changeDto);

			var failures = new List<ValidationFailure> { new ValidationFailure("NewPassword", "PasswordMustContainSpecialCharacter") };
			_validatorMock.Setup(v => v.ValidateAsync(changeDto, It.IsAny<CancellationToken>()))
						  .ReturnsAsync(new ValidationResult(failures));

			var result = await _handler.Handle(command, CancellationToken.None);

			_output.WriteLine("Input Data: " + JsonSerializer.Serialize(changeDto));
			_output.WriteLine("Expected: IsSuccess = false, StatusCode = 400");
			_output.WriteLine($"Actual:   IsSuccess = {result.IsSuccess}, StatusCode = {result.StatusCode}");

			Assert.False(result.IsSuccess, "Change password should fail when new password is missing a special character.");
			Assert.Equal(400, result.StatusCode);
		}


		/// TC_CP_008 Change Password – New Password Same as Current
		[Fact]
		public async Task ChangePassword_ShouldFail_WhenNewPasswordIsSameAsCurrent()
		{
			_output.WriteLine("Test Case: TC_CP_008 Change Password – New Password Same as Current");
			string validUserId = Guid.NewGuid().ToString();
			var changeDto = new ChangePasswordDto
			{
				CurrentPassword = "CurrentPass1!",
				NewPassword = "CurrentPass1!" // same as current
			};
			var command = new ChangePasswordCommand(validUserId, changeDto);

			var failures = new List<ValidationFailure> { new ValidationFailure("NewPassword", "PasswordMustBeDifferent") };
			_validatorMock.Setup(v => v.ValidateAsync(changeDto, It.IsAny<CancellationToken>()))
						  .ReturnsAsync(new ValidationResult(failures));

			var result = await _handler.Handle(command, CancellationToken.None);

			_output.WriteLine("Input Data: " + JsonSerializer.Serialize(changeDto));
			_output.WriteLine("Expected: IsSuccess = false, StatusCode = 400");
			_output.WriteLine($"Actual:   IsSuccess = {result.IsSuccess}, StatusCode = {result.StatusCode}");

			Assert.False(result.IsSuccess, "Change password should fail when new password is the same as the current password.");
			Assert.Equal(400, result.StatusCode);
		}


		/// TC_CP_009 Change Password – Invalid Current Password
		[Fact]
		public async Task ChangePassword_ShouldFail_WhenCurrentPasswordIsInvalid()
		{
			_output.WriteLine("Test Case: TC_CP_009 Change Password – Invalid Current Password");
			string validUserId = Guid.NewGuid().ToString();
			var changeDto = new ChangePasswordDto
			{
				CurrentPassword = "WrongPass!",
				NewPassword = "NewPass1!"
			};
			var command = new ChangePasswordCommand(validUserId, changeDto);

			// Validator passes
			_validatorMock.Setup(v => v.ValidateAsync(changeDto, It.IsAny<CancellationToken>()))
						  .ReturnsAsync(new ValidationResult());
			// Setup: tìm thấy user
			var user = new AppUser { Id = new Guid(validUserId) };
			_userManagerMock.Setup(u => u.FindByIdAsync(validUserId))
							.ReturnsAsync(user);
			// Setup: CheckPasswordAsync trả về false để mô phỏng current password không đúng
			_userManagerMock.Setup(u => u.CheckPasswordAsync(user, changeDto.CurrentPassword))
							.ReturnsAsync(false);

			var result = await _handler.Handle(command, CancellationToken.None);

			_output.WriteLine("Input Data: " + JsonSerializer.Serialize(changeDto));
			_output.WriteLine("Expected: IsSuccess = false, StatusCode = 400");
			_output.WriteLine($"Actual:   IsSuccess = {result.IsSuccess}, StatusCode = {result.StatusCode}");

			Assert.False(result.IsSuccess, "Change password should fail when current password is invalid.");
			Assert.Equal(400, result.StatusCode);
		}


		/// TC_CP_010 Change Password – User Not Found
		[Fact]
		public async Task ChangePassword_ShouldFail_WhenUserNotFound()
		{
			_output.WriteLine("Test Case: TC_CP_010 Change Password – User Not Found");
			string nonexistentUserId = Guid.NewGuid().ToString();
			var changeDto = new ChangePasswordDto
			{
				CurrentPassword = "CurrentPass1!",
				NewPassword = "NewPass1!"
			};
			var command = new ChangePasswordCommand(nonexistentUserId, changeDto);

			_validatorMock.Setup(v => v.ValidateAsync(changeDto, It.IsAny<CancellationToken>()))
						  .ReturnsAsync(new ValidationResult());
			// Setup: không tìm thấy user
			_userManagerMock.Setup(u => u.FindByIdAsync(nonexistentUserId))
							.ReturnsAsync((AppUser)null);

			var result = await _handler.Handle(command, CancellationToken.None);

			_output.WriteLine($"Input UserId: {nonexistentUserId}");
			_output.WriteLine("Expected: IsSuccess = false, StatusCode = 404");
			_output.WriteLine($"Actual:   IsSuccess = {result.IsSuccess}, StatusCode = {result.StatusCode}");

			Assert.False(result.IsSuccess, "Change password should fail when user is not found.");
			Assert.Equal(404, result.StatusCode);
		}


		/// TC_CP_011 Change Password – Exception Occurrence
		[Fact]
		public async Task ChangePassword_ShouldFail_WhenExceptionOccurs()
		{
			_output.WriteLine("Test Case: TC_CP_011 Change Password – Exception Occurrence");
			string validUserId = Guid.NewGuid().ToString();
			var changeDto = new ChangePasswordDto
			{
				CurrentPassword = "CurrentPass1!",
				NewPassword = "NewPass1!"
			};
			var command = new ChangePasswordCommand(validUserId, changeDto);

			_validatorMock.Setup(v => v.ValidateAsync(changeDto, It.IsAny<CancellationToken>()))
						  .ReturnsAsync(new ValidationResult());
			var user = new AppUser { Id = new Guid(validUserId) };
			_userManagerMock.Setup(u => u.FindByIdAsync(validUserId))
							.ReturnsAsync(user);
			_userManagerMock.Setup(u => u.CheckPasswordAsync(user, changeDto.CurrentPassword))
							.ReturnsAsync(true);
			_userManagerMock.Setup(u => u.ChangePasswordAsync(user, changeDto.CurrentPassword, changeDto.NewPassword))
							.ThrowsAsync(new Exception("Simulated exception"));

			var result = await _handler.Handle(command, CancellationToken.None);

			_output.WriteLine($"Input UserId: {validUserId}");
			_output.WriteLine("Expected: IsSuccess = false, StatusCode = 500");
			_output.WriteLine($"Actual:   IsSuccess = {result.IsSuccess}, StatusCode = {result.StatusCode}");

			Assert.False(result.IsSuccess, "Change password should fail when an exception occurs.");
			Assert.Equal(500, result.StatusCode);
		}
	}
}
