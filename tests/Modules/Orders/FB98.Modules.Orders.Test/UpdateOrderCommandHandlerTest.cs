using FB98.Modules.Orders.Application.Abstractions;
using FB98.Modules.Orders.Application.OrderManagement.Update;
using FB98.Modules.Orders.Domain.Entities;
using FB98.Shared.Infrastructure.Localization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Logging;
using Moq;
using System.Globalization;

namespace FB98.Modules.Orders.Test
{
	public class UpdateOrderCommandHandlerTest
	{
		private readonly UpdateOrderCommandHandler _handler;
		private readonly Mock<IHttpContextAccessor> _httpContextMock;
		private readonly Mock<ILocalizedMessageService> _localizedMessageServiceMock;
		private readonly Mock<ILogger<UpdateOrderCommandHandler>> _loggerMock;
		private readonly Mock<IOrderRepository> _orderRepositoryMock;

		public UpdateOrderCommandHandlerTest()
		{
			_orderRepositoryMock = new Mock<IOrderRepository>();
			_localizedMessageServiceMock = new Mock<ILocalizedMessageService>();
			_loggerMock = new Mock<ILogger<UpdateOrderCommandHandler>>();
			_httpContextMock = new Mock<IHttpContextAccessor>();

			var httpContext = new DefaultHttpContext();
			var requestCultureFeature = new Mock<IRequestCultureFeature>();
			requestCultureFeature.Setup(f => f.RequestCulture)
				.Returns(new RequestCulture(new CultureInfo("en")));

			_handler = new UpdateOrderCommandHandler(
				_loggerMock.Object,
				_localizedMessageServiceMock.Object,
				_orderRepositoryMock.Object
			);
		}

		[Fact]
		public async Task Handle_Should_Return_InvalidState_When_OrderStatus_Is_Not_Valid()
		{
			// Arrange
			var order = new Order { Id = Guid.NewGuid(), OrderStatusId = OrderStatusConstants.CheckedIn };
			var command = new UpdateOrderCommand(order.Id, OrderStatusConstants.Created);

			_orderRepositoryMock.Setup(repo => repo.GetByIdAsync(command.OrderId))
				.ReturnsAsync(order);

			_localizedMessageServiceMock
				.Setup(l => l.GetLocalizedMessage(It.Is<string>(s => s == "InvalidState"), It.IsAny<string>()))
				.Returns("Invalid state transition");

			// Act
			var result = await _handler.Handle(command, CancellationToken.None);

			// Assert
			Assert.False(result.IsSuccess);
			Assert.Equal(400, result.StatusCode);
			Assert.Equal("Invalid state transition", result.Message);
		}

		[Fact]
		public async Task Handle_Should_Return_NotFound_When_Order_Does_Not_Exist()
		{
			//Arrange
			var command = new UpdateOrderCommand(Guid.NewGuid(), OrderStatusConstants.Confirmed);
			_orderRepositoryMock.Setup(r => r.GetByIdAsync(command.OrderId))
				.ReturnsAsync((Order)null);

			_localizedMessageServiceMock
				.Setup(l => l.GetLocalizedMessage(It.Is<string>(s => s == "NotFound"), It.IsAny<string>()))
				.Returns("Not found");

			//Act
			var result = await _handler.Handle(command, CancellationToken.None);

			//Assert
			Assert.False(result.IsSuccess);
			Assert.Equal(404, result.StatusCode);
			Assert.Equal("Not found", result.Message);
		}

		[Fact]
		public async Task Handle_Should_Update_OrderStatus_When_Valid_Transition()
		{
			// Arrange
			var order = new Order { Id = Guid.NewGuid(), OrderStatusId = OrderStatusConstants.CheckedIn };
			var command = new UpdateOrderCommand(order.Id, OrderStatusConstants.Confirmed);

			_orderRepositoryMock.Setup(repo => repo.GetByIdAsync(command.OrderId))
				.ReturnsAsync(order);

			_orderRepositoryMock.Setup(repo => repo.SaveChangesAsync())
				.ReturnsAsync(1);

			_localizedMessageServiceMock
				.Setup(l => l.GetLocalizedMessage(It.Is<string>(s => s == "Updated"), It.IsAny<string>()))
				.Returns("Updated successfully");

			// Act
			var result = await _handler.Handle(command, CancellationToken.None);

			// Assert
			Assert.True(result.IsSuccess);
			Assert.Equal(200, result.StatusCode);
			Assert.Equal("Updated successfully", result.Message);
			Assert.Equal(OrderStatusConstants.Confirmed, order.OrderStatusId);

			// Kiểm tra xem SaveChangesAsync có được gọi không
			_orderRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Once);
		}

		[Fact]
		public async Task Handle_Should_Log_Error_When_Exception_Occurs()
		{
			// Arrange
			var command = new UpdateOrderCommand(Guid.NewGuid(), OrderStatusConstants.Confirmed);
			_orderRepositoryMock.Setup(repo => repo.GetByIdAsync(command.OrderId))
				.ThrowsAsync(new Exception("Database failure"));
			// Act
			var result = await _handler.Handle(command, CancellationToken.None);

			// Assert
			Assert.False(result.IsSuccess);
			Assert.Equal(500, result.StatusCode);
			Assert.Equal("An unexpected error occurred", result.Message);

			// Kiểm tra xem logger có ghi log lỗi không
			_loggerMock.Verify(
				logger => logger.Log(
					LogLevel.Error,
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((o, t) => o.ToString().Contains("Error occurred while update order")),
					It.IsAny<Exception>(),
					(Func<It.IsAnyType, Exception, string>)It.IsAny<object>()
				),
				Times.Once);
		}
	}
}