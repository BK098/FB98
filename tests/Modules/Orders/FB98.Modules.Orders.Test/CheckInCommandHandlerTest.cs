using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using Xunit.Abstractions;
using FB98.Modules.Orders.Application.Abstractions;
using FB98.Modules.Orders.Application.OrderManagement.CheckIn;
using FB98.Modules.Orders.Domain.Entities;
using FB98.Shared.Abstractions.StatusConstants;
using FB98.Shared.Infrastructure.Localization;

namespace FB98.Modules.Orders.Test
{
	public class CheckInCommandHandlerTest
	{
		private readonly Mock<IOrderRepository> _orderRepoMock;
		private readonly Mock<IUnitOfWork> _unitOfWorkMock;
		private readonly Mock<ILogger<CheckInCommandHandler>> _loggerMock;
		private readonly Mock<ILocalizedMessageService> _localizerMock;
		private readonly CheckInCommandHandler _handler;
		private readonly ITestOutputHelper _output;

		public CheckInCommandHandlerTest(ITestOutputHelper output)
		{
			_output = output;
			_orderRepoMock = new Mock<IOrderRepository>();
			_unitOfWorkMock = new Mock<IUnitOfWork>();
			_loggerMock = new Mock<ILogger<CheckInCommandHandler>>();
			_localizerMock = new Mock<ILocalizedMessageService>();


			_localizerMock
			.Setup(x => x.GetLocalizedMessage(It.IsAny<string>(), It.IsAny<string?>()))
			.Returns((string key, string? culture) => key);

			_localizerMock
				.Setup(x => x.GetLocalizedMessage(It.IsAny<string>(), It.IsAny<string?>()))
				.Returns((string key, string? culture) => key);

			_handler = new CheckInCommandHandler(
				_loggerMock.Object,
				_localizerMock.Object,
				_orderRepoMock.Object,
				_unitOfWorkMock.Object
			);
		}

		// TC_Booking_CheckIn_001 – Order Not Found
		[Fact]
		public async Task CheckIn_OrderNotFound_ShouldReturnNotFound()
		{
			var orderId = Guid.NewGuid();
			var command = new CheckInCommand(orderId);
			_orderRepoMock.Setup(r => r.GetByIdAsync(orderId))
						  .ReturnsAsync((Order?)null);

			_output.WriteLine("Test case name - ID: TC_Booking_CheckIn_001");
			_output.WriteLine($"Test Data: OrderId = {orderId}");
			_output.WriteLine("Expected result: IsSuccess=false, StatusCode=404, Message='NotFound'");
			_output.WriteLine("-----------------------");

			var result = await _handler.Handle(command, CancellationToken.None);

			_output.WriteLine($"Actual result: IsSuccess={result.IsSuccess}, StatusCode={result.StatusCode}, Message={result.Message}");
			result.IsSuccess.Should().BeFalse();
			result.StatusCode.Should().Be(404);
			result.Message.Should().Be("NotFound");
		}

		// TC_Booking_CheckIn_002 – Invalid Order Status
		[Fact]
		public async Task CheckIn_InvalidOrderStatus_ShouldReturnInvalidState()
		{
			var orderId = Guid.NewGuid();
			var command = new CheckInCommand(orderId);
			var order = new Order { Id = orderId, OrderStatusId = OrderStatusConstants.Created };

			_orderRepoMock.Setup(r => r.GetByIdAsync(orderId))
						  .ReturnsAsync(order);

			_output.WriteLine("Test case name - ID: TC_Booking_CheckIn_002");
			_output.WriteLine($"Test Data: OrderId = {orderId}, StatusId = {order.OrderStatusId}");
			_output.WriteLine("Expected result: IsSuccess=false, StatusCode=400, Message='InvalidState'");
			_output.WriteLine("-----------------------");

			var result = await _handler.Handle(command, CancellationToken.None);

			_output.WriteLine($"Actual result: IsSuccess={result.IsSuccess}, StatusCode={result.StatusCode}, Message={result.Message}");
			result.IsSuccess.Should().BeFalse();
			result.StatusCode.Should().Be(400);
			result.Message.Should().Be("InvalidState");
		}

		// TC_Booking_CheckIn_003 – Successful Check In
		[Fact]
		public async Task CheckIn_SuccessfulCheckIn_ShouldReturnUpdated()
		{
			var orderId = Guid.NewGuid();
			var command = new CheckInCommand(orderId);
			var order = new Order { Id = orderId, OrderStatusId = OrderStatusConstants.Confirmed };

			_orderRepoMock.Setup(r => r.GetByIdAsync(orderId))
						  .ReturnsAsync(order);
			_unitOfWorkMock.Setup(u => u.SaveChangesAsync())
						   .ReturnsAsync(1);

			_output.WriteLine("Test case name - ID: TC_Booking_CheckIn_003");
			_output.WriteLine($"Test Data: OrderId = {orderId}, InitialStatusId = Confirmed");
			_output.WriteLine("Expected result: IsSuccess=true, StatusCode=200, Message='Updated', OrderStatusId=CheckedIn");
			_output.WriteLine("-----------------------");

			var result = await _handler.Handle(command, CancellationToken.None);

			_output.WriteLine($"Actual result: IsSuccess={result.IsSuccess}, StatusCode={result.StatusCode}, Message={result.Message}");
			result.IsSuccess.Should().BeTrue();
			result.StatusCode.Should().Be(200);
			result.Message.Should().Be("Updated");
			order.OrderStatusId.Should().Be(OrderStatusConstants.CheckedIn);

			_unitOfWorkMock.Verify(u => u.Entry(order, EntityState.Modified), Times.Once);
			_unitOfWorkMock.Verify(u => u.Entry(
				It.Is<OrderStatusHistory>(h =>
					h.OrderId == orderId &&
					h.OldStatusId == OrderStatusConstants.Confirmed &&
					h.NewStatusId == OrderStatusConstants.CheckedIn),
				EntityState.Added), Times.Once);
			_unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
		}

		// TC_Booking_CheckIn_004 – Exception Handling
		[Fact]
		public async Task CheckIn_ExceptionHandling_ShouldReturnError500()
		{
			var orderId = Guid.NewGuid();
			var command = new CheckInCommand(orderId);

			_orderRepoMock.Setup(r => r.GetByIdAsync(orderId))
						  .ThrowsAsync(new InvalidOperationException("DB error"));

			_output.WriteLine("Test case name - ID: TC_Booking_CheckIn_004");
			_output.WriteLine($"Test Data: OrderId = {orderId}");
			_output.WriteLine("Expected result: IsSuccess=false, StatusCode=500, Message='An unexpected error occurred'");
			_output.WriteLine("-----------------------");

			var result = await _handler.Handle(command, CancellationToken.None);

			_output.WriteLine($"Actual result: IsSuccess={result.IsSuccess}, StatusCode={result.StatusCode}, Message={result.Message}");
			result.IsSuccess.Should().BeFalse();
			result.StatusCode.Should().Be(500);
			result.Message.Should().Be("An unexpected error occurred");
			_loggerMock.Verify(l => l.LogError(
				It.IsAny<Exception>(),
				"Error occurred while checking in order"), Times.Once);
		}
	}
}
