using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using FB98.Modules.Identity.Application.Authentication.ResetPassword;
using FB98.Modules.Identity.Domain.Entities;
using FB98.Shared.Infrastructure.Localization;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using System.Web;
using Xunit;
using Xunit.Abstractions;

namespace FB98.Modules.Identity.Test.Authentication
{
	public class ResetPasswordCommandHandlerTest
	{
		private readonly ResetPasswordCommandHandler _handler;
		private readonly Mock<UserManager<AppUser>> _userManagerMock;
		private readonly Mock<ILocalizedMessageService> _localizedMessageServiceMock;
		private readonly Mock<IValidator<ResetPasswordDto>> _validatorMock;
		private readonly Mock<ILogger<ResetPasswordCommandHandler>> _loggerMock;
		private readonly ITestOutputHelper _output;

		public ResetPasswordCommandHandlerTest(ITestOutputHelper output)
		{
			_output = output;
			_loggerMock = new Mock<ILogger<ResetPasswordCommandHandler>>();
			_validatorMock = new Mock<IValidator<ResetPasswordDto>>();
			_localizedMessageServiceMock = new Mock<ILocalizedMessageService>();

			var store = new Mock<IUserStore<AppUser>>();
			_userManagerMock = new Mock<UserManager<AppUser>>(
				store.Object, null, null, null, null, null, null, null, null
			);

			// Setup: _localizedMessageServiceMock trả về key
			_localizedMessageServiceMock
				.Setup(m => m.GetLocalizedMessage(It.IsAny<string>(), It.IsAny<string>()))
				.Returns((string key, string culture) => key);
			_localizedMessageServiceMock
				.Setup(m => m.GetLocalizedMessage(It.IsAny<string>(), null))
				.Returns((string key, string? culture) => key);

			_handler = new ResetPasswordCommandHandler(
				_validatorMock.Object,
				_localizedMessageServiceMock.Object,
				_userManagerMock.Object,
				_loggerMock.Object
			);
		}


		/// TC_FP_001 Reset Password Success
		/// Email: "user@example.com"
		[Fact]
		public async Task ResetPassword_ShouldSucceed_WhenDataIsValid()
		{
			_output.WriteLine("Test Case: TC_FP_001 Reset Password Success");

			var resetDto = new ResetPasswordDto
			{
				Email = "user@example.com",
				Token = "validTokenEncoded", // giả lập token đã được URL-encode
				Password = "Password1!"
			};
			var command = new ResetPasswordCommand(resetDto);

			// Setup validator: trả về kết quả hợp lệ
			_validatorMock.Setup(v => v.ValidateAsync(resetDto, It.IsAny<CancellationToken>()))
						  .ReturnsAsync(new ValidationResult());

			// Setup user manager: tìm thấy user
			var user = new AppUser { Email = resetDto.Email };
			_userManagerMock.Setup(u => u.FindByEmailAsync(resetDto.Email))
							.ReturnsAsync(user);

			// Giả lập URL decode token (HttpUtility.UrlDecode)
			// Nếu token là "validTokenEncoded", ta giả sử decode trả về "validTokenDecoded"
			// Nhưng trong test, chúng ta có thể setup ResetPasswordAsync với bất kỳ token nào
			_userManagerMock.Setup(u => u.ResetPasswordAsync(user, It.IsAny<string>(), resetDto.Password))
							.ReturnsAsync(IdentityResult.Success);

			var result = await _handler.Handle(command, CancellationToken.None);

			_output.WriteLine("Input Data: " + JsonSerializer.Serialize(resetDto));
			_output.WriteLine("Expected: IsSuccess = true, StatusCode = 200");
			_output.WriteLine($"Actual:   IsSuccess = {result.IsSuccess}, StatusCode = {result.StatusCode}");

			Assert.True(result.IsSuccess, "Reset password should succeed with valid data.");
			Assert.Equal(200, result.StatusCode);
		}


		/// TC_FP_002 Reset Password – Empty Email
		[Fact]
		public async Task ResetPassword_ShouldFail_WhenEmailIsEmpty()
		{
			_output.WriteLine("Test Case: TC_FP_002 Reset Password – Empty Email");
			var resetDto = new ResetPasswordDto
			{
				Email = "",
				Token = "anyToken",
				Password = "Password1!"
			};
			var command = new ResetPasswordCommand(resetDto);

			var failures = new List<ValidationFailure> { new ValidationFailure("Email", "EmailRequired") };
			_validatorMock.Setup(v => v.ValidateAsync(resetDto, It.IsAny<CancellationToken>()))
						  .ReturnsAsync(new ValidationResult(failures));

			var result = await _handler.Handle(command, CancellationToken.None);

			_output.WriteLine("Input Data: " + JsonSerializer.Serialize(resetDto));
			_output.WriteLine("Expected: IsSuccess = false, StatusCode = 400");
			_output.WriteLine($"Actual:   IsSuccess = {result.IsSuccess}, StatusCode = {result.StatusCode}");

			Assert.False(result.IsSuccess, "Reset password should fail when email is empty.");
			Assert.Equal(400, result.StatusCode);
		}


		/// TC_FP_003 Reset Password – Invalid Email Format
		[Fact]
		public async Task ResetPassword_ShouldFail_WhenEmailFormatIsInvalid()
		{
			_output.WriteLine("Test Case: TC_FP_003 Reset Password – Invalid Email Format");
			var resetDto = new ResetPasswordDto
			{
				Email = "invalidemail",
				Token = "anyToken",
				Password = "Password1!"
			};
			var command = new ResetPasswordCommand(resetDto);

			var failures = new List<ValidationFailure> { new ValidationFailure("Email", "EmailInvalid") };
			_validatorMock.Setup(v => v.ValidateAsync(resetDto, It.IsAny<CancellationToken>()))
						  .ReturnsAsync(new ValidationResult(failures));

			var result = await _handler.Handle(command, CancellationToken.None);

			_output.WriteLine("Input Data: " + JsonSerializer.Serialize(resetDto));
			_output.WriteLine("Expected: IsSuccess = false, StatusCode = 400");
			_output.WriteLine($"Actual:   IsSuccess = {result.IsSuccess}, StatusCode = {result.StatusCode}");

			Assert.False(result.IsSuccess, "Reset password should fail when email format is invalid.");
			Assert.Equal(400, result.StatusCode);
		}


		/// TC_FP_004 Reset Password – Empty Token
		[Fact]
		public async Task ResetPassword_ShouldFail_WhenTokenIsEmpty()
		{
			_output.WriteLine("Test Case: TC_FP_004 Reset Password – Empty Token");
			var resetDto = new ResetPasswordDto
			{
				Email = "user@example.com",
				Token = "", // empty token
				Password = "Password1!"
			};
			var command = new ResetPasswordCommand(resetDto);

			// Validator pass (không kiểm tra token)
			_validatorMock.Setup(v => v.ValidateAsync(resetDto, It.IsAny<CancellationToken>()))
						  .ReturnsAsync(new ValidationResult());
			// Tìm thấy user
			var user = new AppUser { Email = resetDto.Email };
			_userManagerMock.Setup(u => u.FindByEmailAsync(resetDto.Email))
							.ReturnsAsync(user);
			// Setup ResetPasswordAsync: nếu token rỗng, giả lập trả về thất bại
			_userManagerMock.Setup(u => u.ResetPasswordAsync(user, It.IsAny<string>(), resetDto.Password))
							.ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "PasswordResetFailed" }));

			var result = await _handler.Handle(command, CancellationToken.None);

			_output.WriteLine("Input Data: " + JsonSerializer.Serialize(resetDto));
			_output.WriteLine("Expected: IsSuccess = false, StatusCode = 400");
			_output.WriteLine($"Actual:   IsSuccess = {result.IsSuccess}, StatusCode = {result.StatusCode}");

			Assert.False(result.IsSuccess, "Reset password should fail when token is empty.");
			Assert.Equal(400, result.StatusCode);
		}


		/// TC_FP_005 Reset Password – Invalid Password Format
		[Fact]
		public async Task ResetPassword_ShouldFail_WhenPasswordFormatIsInvalid()
		{
			_output.WriteLine("Test Case: TC_FP_005 Reset Password – Invalid Password Format");
			var resetDto = new ResetPasswordDto
			{
				Email = "user@example.com",
				Token = "validTokenEncoded",
				Password = "password"
			};
			var command = new ResetPasswordCommand(resetDto);

			var failures = new List<ValidationFailure> { new ValidationFailure("Password", "PasswordMustContainUppercase") };
			_validatorMock.Setup(v => v.ValidateAsync(resetDto, It.IsAny<CancellationToken>()))
						  .ReturnsAsync(new ValidationResult(failures));

			var result = await _handler.Handle(command, CancellationToken.None);

			_output.WriteLine("Input Data: " + JsonSerializer.Serialize(resetDto));
			_output.WriteLine("Expected: IsSuccess = false, StatusCode = 400");
			_output.WriteLine($"Actual:   IsSuccess = {result.IsSuccess}, StatusCode = {result.StatusCode}");

			Assert.False(result.IsSuccess, "Reset password should fail when password format is invalid.");
			Assert.Equal(400, result.StatusCode);
		}

		/// TC_FP_006 Reset Password – User Not Found
		[Fact]
		public async Task ResetPassword_ShouldFail_WhenUserNotFound()
		{
			_output.WriteLine("Test Case: TC_FP_006 Reset Password – User Not Found");
			var resetDto = new ResetPasswordDto
			{
				Email = "nonexistent@example.com",
				Token = "validTokenEncoded",
				Password = "Password1!"
			};
			var command = new ResetPasswordCommand(resetDto);

			_validatorMock.Setup(v => v.ValidateAsync(resetDto, It.IsAny<CancellationToken>()))
						  .ReturnsAsync(new ValidationResult());
			// Setup user manager: không tìm thấy user
			_userManagerMock.Setup(u => u.FindByEmailAsync(resetDto.Email))
							.ReturnsAsync((AppUser)null);

			var result = await _handler.Handle(command, CancellationToken.None);

			_output.WriteLine("Input Data: " + JsonSerializer.Serialize(resetDto));
			_output.WriteLine("Expected: IsSuccess = false, StatusCode = 404");
			_output.WriteLine($"Actual:   IsSuccess = {result.IsSuccess}, StatusCode = {result.StatusCode}");

			Assert.False(result.IsSuccess, "Reset password should fail when user is not found.");
			Assert.Equal(404, result.StatusCode);
		}
	}
}
