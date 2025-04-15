using System.Globalization;
using FB98.Modules.Identity.Application.Abtractions;
using FB98.Modules.Identity.Application.Authentication.Login;
using FB98.Modules.Identity.Application.Services;
using FB98.Modules.Identity.Domain.Entities;
using FB98.Shared.Infrastructure.Localization;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using Xunit.Abstractions;

namespace FB98.Modules.Identity.Test.Authentication
{
	public class LoginCommandHandlerTest
	{
		private readonly LoginCommandHandler _handler;
		private readonly Mock<UserManager<AppUser>> _userManagerMock;
		private readonly Mock<ILogger<LoginCommandHandler>> _loggerMock;
		private readonly Mock<IValidator<LoginDto>> _validatorMock;
		private readonly Mock<ILocalizedMessageService> _localizedMessageServiceMock;
		private readonly Mock<ITokenService> _tokenServiceMock;
		private readonly Mock<ITokenStoreRepository> _tokenStoreRepositoryMock;
		private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
		private readonly ITestOutputHelper _output;

		public LoginCommandHandlerTest(ITestOutputHelper output)
		{
			_output = output;
			_loggerMock = new Mock<ILogger<LoginCommandHandler>>();
			_validatorMock = new Mock<IValidator<LoginDto>>();
			_localizedMessageServiceMock = new Mock<ILocalizedMessageService>();
			_tokenServiceMock = new Mock<ITokenService>();
			_tokenStoreRepositoryMock = new Mock<ITokenStoreRepository>();
			_httpContextAccessorMock = new Mock<IHttpContextAccessor>();

			var store = new Mock<IUserStore<AppUser>>();
			_userManagerMock = new Mock<UserManager<AppUser>>(
				store.Object,   
				null,           
				null,           
				null,          
				null,           
				null,          
				null,          
				null,           
				null           
			);

			var httpContext = new DefaultHttpContext();
			var requestCultureFeature = new Mock<IRequestCultureFeature>();
			requestCultureFeature.Setup(f => f.RequestCulture)
				.Returns(new RequestCulture(new CultureInfo("en")));
			_httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

			_handler = new LoginCommandHandler(
				_userManagerMock.Object,
				_loggerMock.Object,
				_validatorMock.Object,
				_localizedMessageServiceMock.Object,
				_tokenServiceMock.Object,
				_tokenStoreRepositoryMock.Object,
				_httpContextAccessorMock.Object
			);
		}

		[Fact]
		public async Task Login_ShouldSucceed_WhenCredentialsAreValid()
		{
			// Arrange
			var model = new LoginDto
			{
				Email = "user@gmail.com",
				Password = "Password123"
			};
			var command = new LoginCommand(model);

			var user = new AppUser
			{
				Id = Guid.NewGuid(),
				Email = model.Email
			};

			_validatorMock
				.Setup(v => v.ValidateAsync(model, It.IsAny<CancellationToken>()))
				.ReturnsAsync(new ValidationResult(new List<ValidationFailure>()));

			_userManagerMock
				.Setup(u => u.FindByEmailAsync(model.Email))
				.ReturnsAsync(user);

			_userManagerMock
				.Setup(u => u.CheckPasswordAsync(user, model.Password))
				.ReturnsAsync(true);

			_tokenServiceMock
				.Setup(t => t.GenerateAccessToken(user))
				.ReturnsAsync("fake-access-token");
			_tokenServiceMock
				.Setup(t => t.GenerateRefreshToken())
				.Returns("fake-refresh-token");

			// Act
			var result = await _handler.Handle(command, CancellationToken.None);

			// Print Expected vs. Actual
			_output.WriteLine("Expected: IsSuccess = true, Token = fake-access-token, RefreshToken = fake-refresh-token, StatusCode = 200");
			_output.WriteLine($"Actual:   IsSuccess = {result.IsSuccess}, Token = {result.Data?.Token}, RefreshToken = {result.Data?.RefreshToken}, StatusCode = {result.StatusCode}");

			// Assert
			Assert.True(result.IsSuccess);
			Assert.NotNull(result.Data);
			Assert.Equal("fake-access-token", result.Data.Token);
			Assert.Equal("fake-refresh-token", result.Data.RefreshToken);
			Assert.Equal(200, result.StatusCode);
		}

		[Fact]
		public async Task Login_ShouldFail_WhenPasswordIsIncorrect()
		{
			// Arrange
			var model = new LoginDto
			{
				Email = "user@gmail.com",
				Password = "Password123"
			};
			var command = new LoginCommand(model);

			var user = new AppUser
			{
				Id = Guid.NewGuid(),
				Email = model.Email,
				PasswordHash = "hashed-password"
			};

			_validatorMock
				.Setup(v => v.ValidateAsync(model, It.IsAny<CancellationToken>()))
				.ReturnsAsync(new ValidationResult(new List<ValidationFailure>()));

			_userManagerMock
				.Setup(u => u.FindByEmailAsync(model.Email))
				.ReturnsAsync(user);

			_userManagerMock
				.Setup(u => u.CheckPasswordAsync(user, model.Password))
				.ReturnsAsync(false);

			_tokenServiceMock
				.Setup(t => t.GenerateAccessToken(user))
				.ReturnsAsync("fake-access-token");
			_tokenServiceMock
				.Setup(t => t.GenerateRefreshToken())
				.Returns("fake-refresh-token");

			// Act
			var result = await _handler.Handle(command, CancellationToken.None);

			_output.WriteLine("Expected: IsSuccess = false, Data = null, StatusCode = 401");
			_output.WriteLine($"Actual:   IsSuccess = {result.IsSuccess}, Data = {result.Data}, StatusCode = {result.StatusCode}");

			// Assert
			Assert.False(result.IsSuccess);
			Assert.Null(result.Data);
			Assert.Equal(401, result.StatusCode);
		}

		[Fact]
		public async Task Login_ShouldFail_WhenEmailIsInInvalidFormat()
		{
			// Arrange
			var model = new LoginDto
			{
				Email = "invalid_email_format",
				Password = "Password123"
			};
			var command = new LoginCommand(model);

			var failures = new List<ValidationFailure>
			{
				new ValidationFailure("Email", "EmailInvalid")
			};
			var validationResult = new ValidationResult(failures);

			_validatorMock
				.Setup(v => v.ValidateAsync(model, It.IsAny<CancellationToken>()))
				.ReturnsAsync(validationResult);

			// Act
			var result = await _handler.Handle(command, CancellationToken.None);

			_output.WriteLine("Expected: IsSuccess = false, Data = null, StatusCode = 400");
			_output.WriteLine($"Actual:   IsSuccess = {result.IsSuccess}, Data = {result.Data}, StatusCode = {result.StatusCode}");

			// Assert
			Assert.False(result.IsSuccess, "Kết quả phải thất bại do email sai định dạng");
			Assert.Null(result.Data);
			Assert.Equal(400, result.StatusCode);
		}

		[Fact]
		public async Task Login_ShouldFail_WhenPasswordIsEmpty()
		{
			// Arrange
			var model = new LoginDto
			{
				Email = "user@gmail.com",
				Password = ""
			};
			var command = new LoginCommand(model);

			var failures = new List<ValidationFailure>
			{
				new ValidationFailure("Password", "PasswordRequired")
			};
			var validationResult = new ValidationResult(failures);

			_validatorMock
				.Setup(v => v.ValidateAsync(model, It.IsAny<CancellationToken>()))
				.ReturnsAsync(validationResult);

			// Act
			var result = await _handler.Handle(command, CancellationToken.None);

			_output.WriteLine("Expected: IsSuccess = false, Data = null, Errors not null, StatusCode = 400");
			_output.WriteLine($"Actual:   IsSuccess = {result.IsSuccess}, Data = {result.Data}, StatusCode = {result.StatusCode}");

			// Assert
			Assert.False(result.IsSuccess, "Kết quả phải thất bại do password trống");
			Assert.Null(result.Data);
			Assert.NotNull(result.Errors);
			Assert.Equal(400, result.StatusCode);
		}

		[Fact]
		public async Task Login_ShouldFail_WhenEmailIsEmpty()
		{
			// Arrange
			var model = new LoginDto
			{
				Email = "",
				Password = "SomePassword123"
			};
			var command = new LoginCommand(model);

			var failures = new List<ValidationFailure>
			{
				new ValidationFailure("Email", "EmailRequired")
			};
			var validationResult = new ValidationResult(failures);

			_validatorMock
				.Setup(v => v.ValidateAsync(model, It.IsAny<CancellationToken>()))
				.ReturnsAsync(validationResult);

			// Act
			var result = await _handler.Handle(command, CancellationToken.None);

			_output.WriteLine("Expected: IsSuccess = false, Data = null, Errors not null, StatusCode = 400");
			_output.WriteLine($"Actual:   IsSuccess = {result.IsSuccess}, Data = {result.Data}, StatusCode = {result.StatusCode}");

			// Assert
			Assert.False(result.IsSuccess, "Kết quả phải thất bại do email trống");
			Assert.Null(result.Data);
			Assert.NotNull(result.Errors);
			Assert.Equal(400, result.StatusCode);
		}

		[Fact]
		public async Task Login_ShouldFail_WhenEmailAndPasswordIsEmpty()
		{
			// Arrange
			var model = new LoginDto
			{
				Email = "",
				Password = ""
			};
			var command = new LoginCommand(model);

			var failures = new List<ValidationFailure>
			{
				new ValidationFailure("Email", "EmailRequired"),
				new ValidationFailure("Password", "PasswordRequired")
			};
			var validationResult = new ValidationResult(failures);

			_validatorMock
				.Setup(v => v.ValidateAsync(model, It.IsAny<CancellationToken>()))
				.ReturnsAsync(validationResult);

			// Act
			var result = await _handler.Handle(command, CancellationToken.None);

			_output.WriteLine("Expected: IsSuccess = false, Data = null, Errors not null, StatusCode = 400");
			_output.WriteLine($"Actual:   IsSuccess = {result.IsSuccess}, Data = {result.Data}, StatusCode = {result.StatusCode}");

			// Assert
			Assert.False(result.IsSuccess, "Kết quả phải thất bại do email và Password trống");
			Assert.Null(result.Data);
			Assert.NotNull(result.Errors);
			Assert.Equal(400, result.StatusCode);
		}

		[Fact]
		public async Task Login_ShouldFail_WhenUserDoesNotExist()
		{
			// Arrange
			var model = new LoginDto
			{
				Email = "nonexistentuser@example.com",
				Password = "AnyPass"
			};
			var command = new LoginCommand(model);

			_validatorMock
				.Setup(v => v.ValidateAsync(model, It.IsAny<CancellationToken>()))
				.ReturnsAsync(new ValidationResult(new List<ValidationFailure>()));

			_userManagerMock
				.Setup(u => u.FindByEmailAsync(model.Email))
				.ReturnsAsync((AppUser?)null);

			// Act
			var result = await _handler.Handle(command, CancellationToken.None);

			_output.WriteLine("Expected: IsSuccess = false, Data = null, StatusCode = 401");
			_output.WriteLine($"Actual:   IsSuccess = {result.IsSuccess}, Data = {result.Data}, StatusCode = {result.StatusCode}");

			// Assert
			Assert.False(result.IsSuccess, "Phải thất bại do user không tồn tại");
			Assert.Null(result.Data);
			Assert.Equal(401, result.StatusCode);
		}

		[Fact]
		public async Task Login_ShouldFail_WhenSQLInjectionIsAttempted()
		{
			// Arrange
			var model = new LoginDto
			{
				Email = "admin@example.com' OR '1'='1",
				Password = "AnyPass"
			};
			var command = new LoginCommand(model);

			var failures = new List<ValidationFailure>
			{
				new ValidationFailure("Email", "EmailInvalid")
			};
			var validationResult = new ValidationResult(failures);

			_validatorMock
				.Setup(v => v.ValidateAsync(model, It.IsAny<CancellationToken>()))
				.ReturnsAsync(validationResult);

			// Act
			var result = await _handler.Handle(command, CancellationToken.None);

			_output.WriteLine("Expected: IsSuccess = false, Data = null, Errors not null, StatusCode = 400");
			_output.WriteLine($"Actual:   IsSuccess = {result.IsSuccess}, Data = {result.Data}, StatusCode = {result.StatusCode}");

			// Assert
			Assert.False(result.IsSuccess, "Phải thất bại do định dạng email sai (SQL Injection)");
			Assert.Null(result.Data);
			Assert.NotNull(result.Errors);
			Assert.Equal(400, result.StatusCode);
		}

		[Fact]
		public async Task Login_ShouldSucceed_WhenPasswordHasMinimumAllowedLength()
		{
			// Arrange
			var model = new LoginDto
			{
				Email = "user@example.com",
				Password = "Abc123!@" // 8 ký tự: thoả điều kiện min length
			};
			var command = new LoginCommand(model);

			var user = new AppUser
			{
				Id = Guid.NewGuid(),
				Email = model.Email
			};

			_validatorMock
				.Setup(v => v.ValidateAsync(model, It.IsAny<CancellationToken>()))
				.ReturnsAsync(new ValidationResult());

			_userManagerMock
				.Setup(u => u.FindByEmailAsync(model.Email))
				.ReturnsAsync(user);

			_userManagerMock
				.Setup(u => u.CheckPasswordAsync(user, model.Password))
				.ReturnsAsync(true);

			_tokenServiceMock
				.Setup(t => t.GenerateAccessToken(user))
				.ReturnsAsync("fake-access-token-minlength");
			_tokenServiceMock
				.Setup(t => t.GenerateRefreshToken())
				.Returns("fake-refresh-token-minlength");

			// Act
			var result = await _handler.Handle(command, CancellationToken.None);

			_output.WriteLine("Expected: IsSuccess = true, Token = fake-access-token-minlength, RefreshToken = fake-refresh-token-minlength, StatusCode = 200");
			_output.WriteLine($"Actual:   IsSuccess = {result.IsSuccess}, Token = {result.Data?.Token}, RefreshToken = {result.Data?.RefreshToken}, StatusCode = {result.StatusCode}");

			// Assert
			Assert.True(result.IsSuccess, "Login phải thành công với password tối thiểu 8 ký tự");
			Assert.NotNull(result.Data);
			Assert.Equal("fake-access-token-minlength", result.Data.Token);
			Assert.Equal("fake-refresh-token-minlength", result.Data.RefreshToken);
			Assert.Equal(200, result.StatusCode);
		}

		[Fact]
		public async Task Login_ShouldFail_WhenPasswordExceedsMaximumAllowedLength()
		{
			// Arrange
			var overlyLongPassword = new string('A', 30);
			var model = new LoginDto
			{
				Email = "user@example.com",
				Password = overlyLongPassword
			};
			var command = new LoginCommand(model);

			var failures = new List<ValidationFailure>
			{
				new ValidationFailure("Password", "PasswordTooLong")
			};
			var validationResult = new ValidationResult(failures);

			_validatorMock
				.Setup(v => v.ValidateAsync(model, It.IsAny<CancellationToken>()))
				.ReturnsAsync(validationResult);

			// Act
			var result = await _handler.Handle(command, CancellationToken.None);

			_output.WriteLine("Expected: IsSuccess = false, Data = null, Errors not null, StatusCode = 400");
			_output.WriteLine($"Actual:   IsSuccess = {result.IsSuccess}, Data = {result.Data}, StatusCode = {result.StatusCode}");

			// Assert
			Assert.False(result.IsSuccess, "Phải thất bại do password vượt độ dài tối đa");
			Assert.Null(result.Data);
			Assert.NotNull(result.Errors);
			Assert.Equal(400, result.StatusCode);
		}

		[Fact]
		public async Task Login_ShouldFail_WhenPasswordCaseDoesNotMatch()
		{
			// Arrange
			var model = new LoginDto
			{
				Email = "user@example.com",
				Password = "PASSWORD123" // Sai case so với password chính xác "Password123"
			};
			var command = new LoginCommand(model);

			var user = new AppUser
			{
				Id = Guid.NewGuid(),
				Email = model.Email
			};

			_validatorMock
				.Setup(v => v.ValidateAsync(model, It.IsAny<CancellationToken>()))
				.ReturnsAsync(new ValidationResult());

			_userManagerMock
				.Setup(u => u.FindByEmailAsync(model.Email))
				.ReturnsAsync(user);

			_userManagerMock
				.Setup(u => u.CheckPasswordAsync(user, model.Password))
				.ReturnsAsync(false);

			// Act
			var result = await _handler.Handle(command, CancellationToken.None);

			_output.WriteLine("Expected: IsSuccess = false, Data = null, StatusCode = 401");
			_output.WriteLine($"Actual:   IsSuccess = {result.IsSuccess}, Data = {result.Data}, StatusCode = {result.StatusCode}");

			// Assert
			Assert.False(result.IsSuccess, "Phải thất bại do password sai case");
			Assert.Null(result.Data);
			Assert.Equal(401, result.StatusCode);
		}
	}
}
