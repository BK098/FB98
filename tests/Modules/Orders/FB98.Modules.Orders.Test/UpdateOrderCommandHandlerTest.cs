using FB98.Modules.Orders.Application.Abstractions;
using FB98.Modules.Orders.Application.OrderManagement.Update;
using FB98.Modules.Orders.Domain.Entities;
using FB98.Shared.Abstractions.StatusConstants;
using FB98.Shared.Infrastructure.Localization;
using CheckInCommandHandler = FB98.Modules.Orders.Application.OrderManagement.CheckIn.CheckInCommandHandler;
using FB98.Modules.Orders.Application.OrderManagement.CheckIn;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit.Abstractions;

namespace FB98.Modules.Orders.Test
{
	public class UpdateOrderCommandHandlerTest
	{
		private readonly Mock<ILogger<CheckInCommandHandler>> _loggerMock;
		private readonly Mock<ILocalizedMessageService> _localizerMock;
		private readonly Mock<IOrderRepository> _orderRepoMock;
		private readonly Mock<IUnitOfWork> _unitOfWorkMock;
		private readonly CheckInCommandHandler _handler;
		private readonly ITestOutputHelper _output;

		public UpdateOrderCommandHandlerTest(ITestOutputHelper output)
		{
			_output = output;
			_loggerMock = new Mock<ILogger<CheckInCommandHandler>>();
			_localizerMock = new Mock<ILocalizedMessageService>();
			_orderRepoMock = new Mock<IOrderRepository>();
			_unitOfWorkMock = new Mock<IUnitOfWork>();

			// Localizer returns the key as message
			_localizerMock
				.Setup(x => x.GetLocalizedMessage(It.IsAny<string>(), It.IsAny<string?>()))
				.Returns((string key, string? _) => key);

			_handler = new CheckInCommandHandler(
				_loggerMock.Object,
				_localizerMock.Object,
				_orderRepoMock.Object,
				_unitOfWorkMock.Object
			);
		}

		// Test case id: TC_Booking_Update_001
		[Fact]
		public async Task Handle_OrderNotFound_ShouldReturnNotFound()
		{
			// Arrange
			var orderId = Guid.NewGuid();
			_orderRepoMock
				.Setup(r => r.GetByIdAsync(orderId))
				.ReturnsAsync((Order?)null);

			var command = new CheckInCommand(orderId);

			// Act
			var result = await _handler.Handle(command, CancellationToken.None);

			// Assert
			_output.WriteLine("Test case ID: TC_Booking_Update_001");
			_output.WriteLine("Test Data: OrderId = {0}", orderId);
			_output.WriteLine("Expected result: Error (404) with message 'NotFound'");
			_output.WriteLine("Actual result: {0} ({1})", result.Message, result.StatusCode);
			if (!result.IsSuccess) _output.WriteLine("Error: {0}", result.Message);

			Assert.False(result.IsSuccess);
			Assert.Equal(404, result.StatusCode);
			Assert.Equal("NotFound", result.Message);
			_unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
		}

		// Test case id: TC_Booking_Update_002
		[Fact]
		public async Task Handle_InvalidStatus_ShouldReturnInvalidState()
		{
			// Arrange
			var orderId = Guid.NewGuid();
			var order = new Order { Id = orderId, OrderStatusId = OrderStatusConstants.Created };
			_orderRepoMock
				.Setup(r => r.GetByIdAsync(orderId))
				.ReturnsAsync(order);

			var command = new CheckInCommand(orderId);

			// Act
			var result = await _handler.Handle(command, CancellationToken.None);

			// Assert
			_output.WriteLine("Test case ID: TC_Booking_Update_002");
			_output.WriteLine("Test Data: OrderId = {0}, OrderStatusId = {1}", orderId, OrderStatusConstants.Created);
			_output.WriteLine("Expected result: Error (400) with message 'InvalidState'");
			_output.WriteLine("Actual result: {0} ({1})", result.Message, result.StatusCode);
			if (!result.IsSuccess) _output.WriteLine("Error: {0}", result.Message);

			Assert.False(result.IsSuccess);
			Assert.Equal(400, result.StatusCode);
			Assert.Equal("InvalidState", result.Message);
			_unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
		}

		// Test case id: TC_Booking_Update_003
		[Fact]
		public async Task Handle_SuccessfulCheckIn_ShouldReturnUpdated()
		{
			// Arrange
			var orderId = Guid.NewGuid();
			var order = new Order { Id = orderId, OrderStatusId = OrderStatusConstants.Confirmed };
			_orderRepoMock
				.Setup(r => r.GetByIdAsync(orderId))
				.ReturnsAsync(order);
			_unitOfWorkMock
				.Setup(u => u.SaveChangesAsync())
				.ReturnsAsync(1);

			var command = new CheckInCommand(orderId);

			// Act
			var result = await _handler.Handle(command, CancellationToken.None);

			// Assert
			_output.WriteLine("Test case ID: TC_Booking_Update_003");
			_output.WriteLine("Test Data: OrderId = {0}, OrderStatusId = {1}", orderId, OrderStatusConstants.Confirmed);
			_output.WriteLine("Expected result: Success (200) with message 'Updated'");
			_output.WriteLine("Actual result: {0} ({1})", result.Message, result.StatusCode);

			Assert.True(result.IsSuccess);
			Assert.Equal(200, result.StatusCode);
			Assert.Equal("Updated", result.Message);
			Assert.Equal(OrderStatusConstants.CheckedIn, order.OrderStatusId);
			_unitOfWorkMock.Verify(u => u.Entry(order, EntityState.Modified), Times.Once);
			_unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
		}

		// Test case id: TC_Booking_Update_004
		[Fact]
		public async Task Handle_ExceptionThrown_ShouldReturnError500()
		{
			// Arrange
			var orderId = Guid.NewGuid();
			_orderRepoMock
				.Setup(r => r.GetByIdAsync(orderId))
				.ThrowsAsync(new InvalidOperationException("DB error"));

			var command = new CheckInCommand(orderId);

			// Act
			var result = await _handler.Handle(command, CancellationToken.None);

			// Assert
			_output.WriteLine("Test case ID: TC_Booking_Update_004");
			_output.WriteLine("Test Data: OrderId = {0}", orderId);
			_output.WriteLine("Expected result: Error (500) with message 'An unexpected error occurred'");
			_output.WriteLine("Actual result: {0} ({1})", result.Message, result.StatusCode);
			if (!result.IsSuccess) _output.WriteLine("Error: {0}", result.Message);

			Assert.False(result.IsSuccess);
			Assert.Equal(500, result.StatusCode);
			Assert.Equal("An unexpected error occurred", result.Message);
			_loggerMock.Verify(
				x => x.LogError(
					It.IsAny<Exception>(),
					It.Is<string>(s => s.Contains("Error occurred while checking in order"))
				), Times.Once);
			_unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
		}
	}
}