using System.Linq.Expressions;
using System.Text.Json;
using FB98.Modules.Identity.Application.Authentication.Register;
using FB98.Modules.Identity.Domain.Entities;
using FB98.Shared.Infrastructure.Localization;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit.Abstractions;

namespace FB98.Modules.Identity.Test.Authentication
{
	
	internal class TestAsyncQueryProvider<T> : IAsyncQueryProvider
	{
		private readonly IQueryProvider _inner;
		public TestAsyncQueryProvider(IQueryProvider inner) => _inner = inner;
		public IQueryable CreateQuery(Expression expression) => new TestAsyncEnumerable<T>(expression);
		public IQueryable<TElement> CreateQuery<TElement>(Expression expression) => new TestAsyncEnumerable<TElement>(expression);
		public object Execute(Expression expression) => _inner.Execute(expression);
		public TResult Execute<TResult>(Expression expression) => _inner.Execute<TResult>(expression);
		public IAsyncEnumerable<TResult> ExecuteAsync<TResult>(Expression expression) => new TestAsyncEnumerable<TResult>(expression);
		public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
		{
			// Đánh giá biểu thức thực sự
			var result = _inner.Execute(expression);
			return (TResult)(object)Task.FromResult(result);
		}
	}

	internal class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
	{
		public TestAsyncEnumerable(IEnumerable<T> enumerable) : base(enumerable) { }
		public TestAsyncEnumerable(Expression expression) : base(expression) { }
		public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default) =>
			new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
		IQueryProvider IQueryable.Provider => new TestAsyncQueryProvider<T>(this);
	}

	internal class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
	{
		private readonly IEnumerator<T> _inner;
		public TestAsyncEnumerator(IEnumerator<T> inner) => _inner = inner;
		public T Current => _inner.Current;
		public ValueTask DisposeAsync()
		{
			_inner.Dispose();
			return ValueTask.CompletedTask;
		}
		public ValueTask<bool> MoveNextAsync() => new ValueTask<bool>(_inner.MoveNext());
	}

	
	public class DummyLocalizedMessageService : ILocalizedMessageService
	{
		public string GetLocalizedMessage(string key, string? culture = null) => key;
	}

	public class RegisterCommandHandlerTest
	{
		private readonly RegisterCommandHandler _handler;
		private readonly Mock<ILogger<RegisterCommandHandler>> _loggerMock;
		private readonly Mock<UserManager<AppUser>> _userManagerMock;
		private readonly Mock<IValidator<RegisterDto>> _validatorMock;
		private readonly ITestOutputHelper _output;

		public RegisterCommandHandlerTest(ITestOutputHelper output)
		{
			_output = output;
			_loggerMock = new Mock<ILogger<RegisterCommandHandler>>();
			_validatorMock = new Mock<IValidator<RegisterDto>>();

			var store = new Mock<IUserStore<AppUser>>();
			_userManagerMock = new Mock<UserManager<AppUser>>(
				store.Object, null, null, null, null, null, null, null, null
			);

			
			var users = new List<AppUser>().AsQueryable();
			var asyncUsers = new TestAsyncEnumerable<AppUser>(users);
			_userManagerMock.Setup(u => u.Users).Returns(asyncUsers);

			
			var dummyLocalizedService = new DummyLocalizedMessageService();

			_handler = new RegisterCommandHandler(
				_userManagerMock.Object,
				_loggerMock.Object,
				_validatorMock.Object,
				dummyLocalizedService
			);
		}

		/// <summary>
		/// TC_Register_001 Valid Registration
	
		[Fact]
		public async Task Register_ShouldSucceed_WhenRegistrationIsValid()// fail
		{
			_output.WriteLine("Test Case: TC_Register_001 Valid Registration");
			var registerDto = new RegisterDto
			{
				Email = "newuser@example.com",
				Password = "Passw0rd!",
				PhoneNumber = "0912345678",
				Firstname = "Nguyen",
				Lastname = "Van A",
				BirthOfDate = DateOnly.FromDateTime(new DateTime(1990, 5, 15))
			};
			var command = new RegisterCommand(registerDto);

			_validatorMock.Setup(v => v.ValidateAsync(registerDto, It.IsAny<CancellationToken>()))
						  .ReturnsAsync(new ValidationResult());
			_userManagerMock.Setup(u => u.FindByEmailAsync(registerDto.Email))
							.ReturnsAsync((AppUser)null);
			_userManagerMock.Setup(u => u.CreateAsync(It.IsAny<AppUser>(), It.IsAny<string>()))
							.ReturnsAsync(IdentityResult.Success);
			
			_userManagerMock.Setup(u => u.UpdateAsync(It.IsAny<AppUser>()))
							.ReturnsAsync(IdentityResult.Success);

			var result = await _handler.Handle(command, CancellationToken.None);

			_output.WriteLine("Input Data: " + JsonSerializer.Serialize(registerDto));
			_output.WriteLine("Expected: IsSuccess = true, StatusCode = 201");
			_output.WriteLine($"Actual:   IsSuccess = {result.IsSuccess}, StatusCode = {result.StatusCode}");

			Assert.True(result.IsSuccess, "Registration should succeed with valid registration data.");
			Assert.Equal(201, result.StatusCode);
		}

		/// TC_Register_002 Existing Email
	
		[Fact]
		public async Task Register_ShouldFail_WhenEmailAlreadyExists()
		{
			_output.WriteLine("Test Case: TC_Register_002 Existing Email");
			var registerDto = new RegisterDto
			{
				Email = "existing@example.com",
				Password = "Passw0rd!",
				PhoneNumber = "0987654321",
				Firstname = "Le",
				Lastname = "Thi B",
				BirthOfDate = DateOnly.FromDateTime(new DateTime(1992, 3, 10))
			};
			var command = new RegisterCommand(registerDto);

			_validatorMock.Setup(v => v.ValidateAsync(registerDto, It.IsAny<CancellationToken>()))
						  .ReturnsAsync(new ValidationResult());
			var existingUser = new AppUser { Email = registerDto.Email, PhoneNumber = "0000000000" };
			_userManagerMock.Setup(u => u.FindByEmailAsync(registerDto.Email))
							.ReturnsAsync(existingUser);

			var result = await _handler.Handle(command, CancellationToken.None);

			_output.WriteLine("Input Data: " + JsonSerializer.Serialize(registerDto));
			_output.WriteLine("Expected: IsSuccess = false, StatusCode = 409");
			_output.WriteLine($"Actual:   IsSuccess = {result.IsSuccess}, StatusCode = {result.StatusCode}");

			Assert.False(result.IsSuccess, "Registration should fail when email already exists.");
			Assert.Equal(409, result.StatusCode);
		}


		/// TC_Register_003 Existing Phone Number
		
		[Fact]
		public async Task Register_ShouldFail_WhenPhoneNumberAlreadyExists()
		{
			_output.WriteLine("Test Case: TC_Register_003 Existing Phone Number");
			var registerDto = new RegisterDto
			{
				Email = "unique@example.com",
				Password = "Passw0rd!",
				PhoneNumber = "0912345678", // đã tồn tại
				Firstname = "Tran",
				Lastname = "Van C",
				BirthOfDate = DateOnly.FromDateTime(new DateTime(1988, 12, 1))
			};
			var command = new RegisterCommand(registerDto);

			_validatorMock.Setup(v => v.ValidateAsync(registerDto, It.IsAny<CancellationToken>()))
						  .ReturnsAsync(new ValidationResult());
			_userManagerMock.Setup(u => u.FindByEmailAsync(registerDto.Email))
							.ReturnsAsync((AppUser)null);
			var phoneUser = new AppUser { Email = "other@example.com", PhoneNumber = "0912345678" };
			_userManagerMock.Setup(u => u.Users)
				.Returns(new List<AppUser> { phoneUser }.AsQueryable());

			var result = await _handler.Handle(command, CancellationToken.None);

			_output.WriteLine("Input Data: " + JsonSerializer.Serialize(registerDto));
			_output.WriteLine("Expected: IsSuccess = false, StatusCode = 409");
			_output.WriteLine($"Actual:   IsSuccess = {result.IsSuccess}, StatusCode = {result.StatusCode}");

			Assert.False(result.IsSuccess, "Registration should fail when phone number already exists.");
			Assert.Equal(409, result.StatusCode);
		} //Fail


		/// TC_Register_004 Empty Email Field

		[Fact]
		public async Task Register_ShouldFail_WhenEmailIsEmpty()
		{
			_output.WriteLine("Test Case: TC_Register_004 Empty Email Field");
			var registerDto = new RegisterDto
			{
				Email = "",
				Password = "Passw0rd!",
				PhoneNumber = "0912345678",
				Firstname = "Nguyen",
				Lastname = "Van A",
				BirthOfDate = DateOnly.FromDateTime(new DateTime(1990, 5, 15))
			};
			var command = new RegisterCommand(registerDto);

			var failures = new List<ValidationFailure> { new ValidationFailure("Email", "EmailRequired") };
			_validatorMock.Setup(v => v.ValidateAsync(registerDto, It.IsAny<CancellationToken>()))
						  .ReturnsAsync(new ValidationResult(failures));

			var result = await _handler.Handle(command, CancellationToken.None);

			_output.WriteLine("Input Data: " + JsonSerializer.Serialize(registerDto));
			_output.WriteLine("Expected: IsSuccess = false, StatusCode = 400");
			_output.WriteLine($"Actual:   IsSuccess = {result.IsSuccess}, StatusCode = {result.StatusCode}");

			Assert.False(result.IsSuccess, "Registration should fail when email is empty.");
			Assert.Equal(400, result.StatusCode);
		}


		/// TC_Register_005 Empty Password Field

		[Fact]
		public async Task Register_ShouldFail_WhenPasswordIsEmpty()
		{
			_output.WriteLine("Test Case: TC_Register_005 Empty Password Field");
			var registerDto = new RegisterDto
			{
				Email = "newuser@example.com",
				Password = "",
				PhoneNumber = "0912345678",
				Firstname = "Nguyen",
				Lastname = "Van A",
				BirthOfDate = DateOnly.FromDateTime(new DateTime(1990, 5, 15))
			};
			var command = new RegisterCommand(registerDto);

			var failures = new List<ValidationFailure> { new ValidationFailure("Password", "PasswordRequired") };
			_validatorMock.Setup(v => v.ValidateAsync(registerDto, It.IsAny<CancellationToken>()))
						  .ReturnsAsync(new ValidationResult(failures));

			var result = await _handler.Handle(command, CancellationToken.None);

			_output.WriteLine("Input Data: " + JsonSerializer.Serialize(registerDto));
			_output.WriteLine("Expected: IsSuccess = false, StatusCode = 400");
			_output.WriteLine($"Actual:   IsSuccess = {result.IsSuccess}, StatusCode = {result.StatusCode}");

			Assert.False(result.IsSuccess, "Registration should fail when password is empty.");
			Assert.Equal(400, result.StatusCode);
		}


		/// TC_Register_006 Empty Phone Field

		[Fact]
		public async Task Register_ShouldFail_WhenPhoneIsEmpty()
		{
			_output.WriteLine("Test Case: TC_Register_006 Empty Phone Field");
			var registerDto = new RegisterDto
			{
				Email = "newuser@example.com",
				Password = "Passw0rd!",
				PhoneNumber = "",
				Firstname = "Nguyen",
				Lastname = "Van A",
				BirthOfDate = DateOnly.FromDateTime(new DateTime(1990, 5, 15))
			};
			var command = new RegisterCommand(registerDto);

			var failures = new List<ValidationFailure> { new ValidationFailure("PhoneNumber", "PhoneNumberRequired") };
			_validatorMock.Setup(v => v.ValidateAsync(registerDto, It.IsAny<CancellationToken>()))
						  .ReturnsAsync(new ValidationResult(failures));

			var result = await _handler.Handle(command, CancellationToken.None);

			_output.WriteLine("Input Data: " + JsonSerializer.Serialize(registerDto));
			_output.WriteLine("Expected: IsSuccess = false, StatusCode = 400");
			_output.WriteLine($"Actual:   IsSuccess = {result.IsSuccess}, StatusCode = {result.StatusCode}");

			Assert.False(result.IsSuccess, "Registration should fail when phone number is empty.");
			Assert.Equal(400, result.StatusCode);
		}


		/// TC_Register_007 Empty Firstname Field

		[Fact]
		public async Task Register_ShouldFail_WhenFirstnameIsEmpty()
		{
			_output.WriteLine("Test Case: TC_Register_007 Empty Firstname Field");
			var registerDto = new RegisterDto
			{
				Email = "newuser@example.com",
				Password = "Passw0rd!",
				PhoneNumber = "0912345678",
				Firstname = "",
				Lastname = "Van A",
				BirthOfDate = DateOnly.FromDateTime(new DateTime(1990, 5, 15))
			};
			var command = new RegisterCommand(registerDto);

			var failures = new List<ValidationFailure> { new ValidationFailure("Firstname", "FirstnameRequired") };
			_validatorMock.Setup(v => v.ValidateAsync(registerDto, It.IsAny<CancellationToken>()))
						  .ReturnsAsync(new ValidationResult(failures));

			var result = await _handler.Handle(command, CancellationToken.None);

			_output.WriteLine("Input Data: " + JsonSerializer.Serialize(registerDto));
			_output.WriteLine("Expected: IsSuccess = false, StatusCode = 400");
			_output.WriteLine($"Actual:   IsSuccess = {result.IsSuccess}, StatusCode = {result.StatusCode}");

			Assert.False(result.IsSuccess, "Registration should fail when firstname is empty.");
			Assert.Equal(400, result.StatusCode);
		}


		/// TC_Register_008 Empty Lastname Field

		[Fact]
		public async Task Register_ShouldFail_WhenLastnameIsEmpty()
		{
			_output.WriteLine("Test Case: TC_Register_008 Empty Lastname Field");
			var registerDto = new RegisterDto
			{
				Email = "newuser@example.com",
				Password = "Passw0rd!",
				PhoneNumber = "0912345678",
				Firstname = "Nguyen",
				Lastname = "",
				BirthOfDate = DateOnly.FromDateTime(new DateTime(1990, 5, 15))
			};
			var command = new RegisterCommand(registerDto);

			var failures = new List<ValidationFailure> { new ValidationFailure("Lastname", "LastnameRequired") };
			_validatorMock.Setup(v => v.ValidateAsync(registerDto, It.IsAny<CancellationToken>()))
						  .ReturnsAsync(new ValidationResult(failures));

			var result = await _handler.Handle(command, CancellationToken.None);

			_output.WriteLine("Input Data: " + JsonSerializer.Serialize(registerDto));
			_output.WriteLine("Expected: IsSuccess = false, StatusCode = 400");
			_output.WriteLine($"Actual:   IsSuccess = {result.IsSuccess}, StatusCode = {result.StatusCode}");

			Assert.False(result.IsSuccess, "Registration should fail when lastname is empty.");
			Assert.Equal(400, result.StatusCode);
		}


		/// TC_Register_009 Invalid Email Format

		[Fact]
		public async Task Register_ShouldFail_WhenEmailFormatIsInvalid()
		{
			_output.WriteLine("Test Case: TC_Register_009 Invalid Email Format");
			var registerDto = new RegisterDto
			{
				Email = "userexample.com", // thiếu '@'
				Password = "Passw0rd!",
				PhoneNumber = "0912345678",
				Firstname = "Nguyen",
				Lastname = "Van A",
				BirthOfDate = DateOnly.FromDateTime(new DateTime(1990, 5, 15))
			};
			var command = new RegisterCommand(registerDto);

			var failures = new List<ValidationFailure> { new ValidationFailure("Email", "EmailInvalid") };
			_validatorMock.Setup(v => v.ValidateAsync(registerDto, It.IsAny<CancellationToken>()))
						  .ReturnsAsync(new ValidationResult(failures));

			var result = await _handler.Handle(command, CancellationToken.None);

			_output.WriteLine("Input Data: " + JsonSerializer.Serialize(registerDto));
			_output.WriteLine("Expected: IsSuccess = false, StatusCode = 400");
			_output.WriteLine($"Actual:   IsSuccess = {result.IsSuccess}, StatusCode = {result.StatusCode}");

			Assert.False(result.IsSuccess, "Registration should fail when email format is invalid.");
			Assert.Equal(400, result.StatusCode);
		}


		/// TC_Register_010 Password Too Short

		[Fact]
		public async Task Register_ShouldFail_WhenPasswordTooShort()
		{
			_output.WriteLine("Test Case: TC_Register_010 Password Too Short");
			var registerDto = new RegisterDto
			{
				Email = "newuser@example.com",
				Password = "Pass1!", // 6 ký tự
				PhoneNumber = "0912345678",
				Firstname = "Nguyen",
				Lastname = "Van A",
				BirthOfDate = DateOnly.FromDateTime(new DateTime(1990, 5, 15))
			};
			var command = new RegisterCommand(registerDto);

			var failures = new List<ValidationFailure> { new ValidationFailure("Password", "PasswordTooShort") };
			_validatorMock.Setup(v => v.ValidateAsync(registerDto, It.IsAny<CancellationToken>()))
						  .ReturnsAsync(new ValidationResult(failures));

			var result = await _handler.Handle(command, CancellationToken.None);

			_output.WriteLine("Input Data: " + JsonSerializer.Serialize(registerDto));
			_output.WriteLine("Expected: IsSuccess = false, StatusCode = 400");
			_output.WriteLine($"Actual:   IsSuccess = {result.IsSuccess}, StatusCode = {result.StatusCode}");

			Assert.False(result.IsSuccess, "Registration should fail when password is too short.");
			Assert.Equal(400, result.StatusCode);
		}


		/// TC_Register_011 Password Missing Uppercase

		[Fact]
		public async Task Register_ShouldFail_WhenPasswordMissingUppercase()
		{
			_output.WriteLine("Test Case: TC_Register_011 Password Missing Uppercase");
			var registerDto = new RegisterDto
			{
				Email = "newuser@example.com",
				Password = "password1!", // không có chữ hoa
				PhoneNumber = "0912345678",
				Firstname = "Nguyen",
				Lastname = "Van A",
				BirthOfDate = DateOnly.FromDateTime(new DateTime(1990, 5, 15))
			};
			var command = new RegisterCommand(registerDto);

			var failures = new List<ValidationFailure> { new ValidationFailure("Password", "PasswordMustContainUppercase") };
			_validatorMock.Setup(v => v.ValidateAsync(registerDto, It.IsAny<CancellationToken>()))
						  .ReturnsAsync(new ValidationResult(failures));

			var result = await _handler.Handle(command, CancellationToken.None);
			_output.WriteLine("Input Data: " + JsonSerializer.Serialize(registerDto));
			_output.WriteLine("Expected: IsSuccess = false, StatusCode = 400");
			_output.WriteLine($"Actual:   IsSuccess = {result.IsSuccess}, StatusCode = {result.StatusCode}");

			Assert.False(result.IsSuccess, "Registration should fail when password is missing uppercase.");
			Assert.Equal(400, result.StatusCode);
		}


		/// TC_Register_012 Password Missing Number

		[Fact]
		public async Task Register_ShouldFail_WhenPasswordMissingNumber()
		{
			_output.WriteLine("Test Case: TC_Register_012 Password Missing Number");
			var registerDto = new RegisterDto
			{
				Email = "newuser@example.com",
				Password = "Password!", // không có số
				PhoneNumber = "0912345678",
				Firstname = "Nguyen",
				Lastname = "Van A",
				BirthOfDate = DateOnly.FromDateTime(new DateTime(1990, 5, 15))
			};
			var command = new RegisterCommand(registerDto);

			var failures = new List<ValidationFailure> { new ValidationFailure("Password", "PasswordMustContainNumber") };
			_validatorMock.Setup(v => v.ValidateAsync(registerDto, It.IsAny<CancellationToken>()))
						  .ReturnsAsync(new ValidationResult(failures));

			var result = await _handler.Handle(command, CancellationToken.None);
			_output.WriteLine("Input Data: " + JsonSerializer.Serialize(registerDto));
			_output.WriteLine("Expected: IsSuccess = false, StatusCode = 400");
			_output.WriteLine($"Actual:   IsSuccess = {result.IsSuccess}, StatusCode = {result.StatusCode}");

			Assert.False(result.IsSuccess, "Registration should fail when password is missing a number.");
			Assert.Equal(400, result.StatusCode);
		}


		/// TC_Register_013 Password Missing Special Character

		[Fact]
		public async Task Register_ShouldFail_WhenPasswordMissingSpecialCharacter()
		{
			_output.WriteLine("Test Case: TC_Register_013 Password Missing Special Character");
			var registerDto = new RegisterDto
			{
				Email = "newuser@example.com",
				Password = "Password1", // không có ký tự đặc biệt
				PhoneNumber = "0912345678",
				Firstname = "Nguyen",
				Lastname = "Van A",
				BirthOfDate = DateOnly.FromDateTime(new DateTime(1990, 5, 15))
			};
			var command = new RegisterCommand(registerDto);

			var failures = new List<ValidationFailure> { new ValidationFailure("Password", "PasswordMustContainSpecialCharacter") };
			_validatorMock.Setup(v => v.ValidateAsync(registerDto, It.IsAny<CancellationToken>()))
						  .ReturnsAsync(new ValidationResult(failures));

			var result = await _handler.Handle(command, CancellationToken.None);
			_output.WriteLine("Input Data: " + JsonSerializer.Serialize(registerDto));
			_output.WriteLine("Expected: IsSuccess = false, StatusCode = 400");
			_output.WriteLine($"Actual:   IsSuccess = {result.IsSuccess}, StatusCode = {result.StatusCode}");

			Assert.False(result.IsSuccess, "Registration should fail when password is missing a special character.");
			Assert.Equal(400, result.StatusCode);
		}


		/// TC_Register_014 Invalid Phone Format

		[Fact]
		public async Task Register_ShouldFail_WhenPhoneFormatIsInvalid()
		{
			_output.WriteLine("Test Case: TC_Register_014 Invalid Phone Format");
			var registerDto = new RegisterDto
			{
				Email = "newuser@example.com",
				Password = "Passw0rd!",
				PhoneNumber = "12345678", // không khớp regex
				Firstname = "Nguyen",
				Lastname = "Van A",
				BirthOfDate = DateOnly.FromDateTime(new DateTime(1990, 5, 15))
			};
			var command = new RegisterCommand(registerDto);

			var failures = new List<ValidationFailure> { new ValidationFailure("PhoneNumber", "PhoneNumberInvalid") };
			_validatorMock.Setup(v => v.ValidateAsync(registerDto, It.IsAny<CancellationToken>()))
						  .ReturnsAsync(new ValidationResult(failures));

			var result = await _handler.Handle(command, CancellationToken.None);
			_output.WriteLine("Input Data: " + JsonSerializer.Serialize(registerDto));
			_output.WriteLine("Expected: IsSuccess = false, StatusCode = 400");
			_output.WriteLine($"Actual:   IsSuccess = {result.IsSuccess}, StatusCode = {result.StatusCode}");

			Assert.False(result.IsSuccess, "Registration should fail when phone number format is invalid.");
			Assert.Equal(400, result.StatusCode);
		}


		/// TC_Register_015 Invalid BirthOfDate – Future Date

		[Fact]
		public async Task Register_ShouldFail_WhenBirthOfDateIsFutureDate()
		{
			_output.WriteLine("Test Case: TC_Register_015 Invalid BirthOfDate – Future Date");
			var futureDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
			var registerDto = new RegisterDto
			{
				Email = "newuser@example.com",
				Password = "Passw0rd!",
				PhoneNumber = "0912345678",
				Firstname = "Nguyen",
				Lastname = "Van A",
				BirthOfDate = futureDate
			};
			var command = new RegisterCommand(registerDto);

			var failures = new List<ValidationFailure> { new ValidationFailure("BirthOfDate", "BirthOfDateInvalid") };
			_validatorMock.Setup(v => v.ValidateAsync(registerDto, It.IsAny<CancellationToken>()))
						  .ReturnsAsync(new ValidationResult(failures));

			var result = await _handler.Handle(command, CancellationToken.None);
			_output.WriteLine("Input Data: " + JsonSerializer.Serialize(registerDto));
			_output.WriteLine("Expected: IsSuccess = false, StatusCode = 400");
			_output.WriteLine($"Actual:   IsSuccess = {result.IsSuccess}, StatusCode = {result.StatusCode}");

			Assert.False(result.IsSuccess, "Registration should fail when birth date is a future date.");
			Assert.Equal(400, result.StatusCode);
		}


		/// TC_Register_016 Invalid BirthOfDate – Too Old

		[Fact]
		public async Task Register_ShouldFail_WhenBirthOfDateIsTooOld()
		{
			_output.WriteLine("Test Case: TC_Register_016 Invalid BirthOfDate – Too Old");
			var registerDto = new RegisterDto
			{
				Email = "newuser@example.com",
				Password = "Passw0rd!",
				PhoneNumber = "0912345678",
				Firstname = "Nguyen",
				Lastname = "Van A",
				BirthOfDate = DateOnly.FromDateTime(new DateTime(1900, 1, 1))
			};
			var command = new RegisterCommand(registerDto);

			var failures = new List<ValidationFailure> { new ValidationFailure("BirthOfDate", "BirthOfDateInvalid") };
			_validatorMock.Setup(v => v.ValidateAsync(registerDto, It.IsAny<CancellationToken>()))
						  .ReturnsAsync(new ValidationResult(failures));

			var result = await _handler.Handle(command, CancellationToken.None);
			_output.WriteLine("Input Data: " + JsonSerializer.Serialize(registerDto));
			_output.WriteLine("Expected: IsSuccess = false, StatusCode = 400");
			_output.WriteLine($"Actual:   IsSuccess = {result.IsSuccess}, StatusCode = {result.StatusCode}");

			Assert.False(result.IsSuccess, "Registration should fail when birth date is too old.");
			Assert.Equal(400, result.StatusCode);
		}
	}
}
