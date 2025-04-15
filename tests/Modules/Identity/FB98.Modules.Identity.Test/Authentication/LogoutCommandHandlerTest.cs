using System;
using System.Threading;
using System.Threading.Tasks;
using FB98.Modules.Identity.Application.Authentication.Logout;
using FB98.Modules.Identity.Domain.Entities;
using FB98.Shared.Infrastructure.Localization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using Xunit.Abstractions;

namespace FB98.Modules.Identity.Test.Authentication
{
	public class LogoutCommandHandlerTest
	{
		private readonly LogoutCommandHandler _handler;
		private readonly Mock<UserManager<AppUser>> _userManagerMock;
		private readonly Mock<ILocalizedMessageService> _localizedMessageServiceMock;
		private readonly Mock<ILogger<LogoutCommandHandler>> _loggerMock;
		private readonly ITestOutputHelper _output;

		public LogoutCommandHandlerTest(ITestOutputHelper output)
		{
			_output = output;
			_loggerMock = new Mock<ILogger<LogoutCommandHandler>>();
			_localizedMessageServiceMock = new Mock<ILocalizedMessageService>();


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

			_handler = new LogoutCommandHandler(_userManagerMock.Object, _localizedMessageServiceMock.Object);
		}

		/// TC_Logout_Success
		/// Logout Success – UserId: valid Guid (đã tồn tại trong DB)
		[Fact]
		public async Task Logout_ShouldSucceed_WhenUserExists()
		{
			_output.WriteLine("Test Case: TC_Logout_Success");
			var validGuid = Guid.NewGuid();
			string validUserId = validGuid.ToString();
			var command = new LogoutCommand(validUserId);

			// Setup: tìm thấy user với Id hợp lệ
			var user = new AppUser { Id = validGuid };
			_userManagerMock.Setup(u => u.FindByIdAsync(validUserId))
				.ReturnsAsync(user);
			// Setup UpdateAsync thành công
			_userManagerMock.Setup(u => u.UpdateAsync(user))
				.ReturnsAsync(IdentityResult.Success);

			var result = await _handler.Handle(command, CancellationToken.None);

			_output.WriteLine($"Input UserId: {validUserId}");
			_output.WriteLine("Expected: IsSuccess = true, StatusCode = 200");
			_output.WriteLine($"Actual:   IsSuccess = {result.IsSuccess}, StatusCode = {result.StatusCode}");

			Assert.True(result.IsSuccess, "Logout should succeed when user exists.");
			Assert.Equal(200, result.StatusCode);
		}


		/// TC_Logout_UserNotFound
		/// Logout – User Not Found – UserId: valid Guid không tồn tại
		[Fact]
		public async Task Logout_ShouldFail_WhenUserNotFound()
		{
			_output.WriteLine("Test Case: TC_Logout_UserNotFound");
			string nonexistentUserId = Guid.NewGuid().ToString();
			var command = new LogoutCommand(nonexistentUserId);

			// Setup: không tìm thấy user
			_userManagerMock.Setup(u => u.FindByIdAsync(nonexistentUserId))
				.ReturnsAsync((AppUser)null);

			var result = await _handler.Handle(command, CancellationToken.None);

			_output.WriteLine($"Input UserId: {nonexistentUserId}");
			_output.WriteLine("Expected: IsSuccess = false, StatusCode = 404");
			_output.WriteLine($"Actual:   IsSuccess = {result.IsSuccess}, StatusCode = {result.StatusCode}");

			Assert.False(result.IsSuccess, "Logout should fail when user is not found.");
			Assert.Equal(404, result.StatusCode);
		}

		/// TC_Logout_WithEmptyUserId
		/// Logout with Empty UserId – UserId: ""
		[Fact]
		public async Task Logout_ShouldFail_WhenUserIdIsEmpty()
		{
			_output.WriteLine("Test Case: TC_Logout_WithEmptyUserId");
			string emptyUserId = "";
			var command = new LogoutCommand(emptyUserId);


			_userManagerMock.Setup(u => u.FindByIdAsync(emptyUserId))
				.ReturnsAsync((AppUser)null);

			var result = await _handler.Handle(command, CancellationToken.None);

			_output.WriteLine($"Input UserId: {emptyUserId}");
			_output.WriteLine("Expected: IsSuccess = false, StatusCode = 404");
			_output.WriteLine($"Actual:   IsSuccess = {result.IsSuccess}, StatusCode = {result.StatusCode}");

			Assert.False(result.IsSuccess, "Logout should fail when UserId is empty.");
			Assert.Equal(404, result.StatusCode);
		}


		/// TC_Logout_ExceptionOccurrence
		/// Logout – Exception Occurrence – UserId: valid Guid (user tồn tại) và _userManager.UpdateAsync ném Exception
		[Fact]
		public async Task Logout_ShouldFail_WhenExceptionOccurs()
		{
			_output.WriteLine("Test Case: TC_Logout_ExceptionOccurrence");
			var validGuid = Guid.NewGuid();
			string validUserId = validGuid.ToString();
			var command = new LogoutCommand(validUserId);


			var user = new AppUser { Id = validGuid };
			_userManagerMock.Setup(u => u.FindByIdAsync(validUserId))
				.ReturnsAsync(user);

			_userManagerMock.Setup(u => u.UpdateAsync(user))
				.ThrowsAsync(new Exception("Simulated exception"));

			var result = await _handler.Handle(command, CancellationToken.None);

			_output.WriteLine($"Input UserId: {validUserId}");
			_output.WriteLine("Expected: IsSuccess = false, StatusCode = 500");
			_output.WriteLine($"Actual:   IsSuccess = {result.IsSuccess}, StatusCode = {result.StatusCode}");

			Assert.False(result.IsSuccess, "Logout should fail when an exception occurs.");
			Assert.Equal(500, result.StatusCode);
		}
	}
}
