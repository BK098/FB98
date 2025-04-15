using System;
using System.Threading.Tasks;
using FB98.Modules.Orders.Application.Abstractions;
using FB98.Modules.Orders.Application.OrderManagement.Events;
using FB98.Modules.Orders.Domain.Entities;
using FB98.Shared.Abstractions.Events;
using FB98.Shared.Abstractions.StatusConstants;
using FluentAssertions;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using Xunit.Abstractions;

namespace FB98.Modules.Orders.Test.Events
{
	public class PaymentSuccessEventHandlerTest
	{
		private readonly Mock<IOrderRepository> _orderRepoMock;
		private readonly Mock<IUnitOfWork> _unitOfWorkMock;
		private readonly Mock<ILogger<PaymentSuccessEventHandler>> _loggerMock;
		private readonly PaymentSuccessEventHandler _handler;
		private readonly ITestOutputHelper _output;

		public PaymentSuccessEventHandlerTest(ITestOutputHelper output)
		{
			_output = output;
			_orderRepoMock = new Mock<IOrderRepository>();
			_unitOfWorkMock = new Mock<IUnitOfWork>();
			_loggerMock = new Mock<ILogger<PaymentSuccessEventHandler>>();
			_handler = new PaymentSuccessEventHandler(
				_orderRepoMock.Object,
				_loggerMock.Object,
				_unitOfWorkMock.Object
			);
		}

		// Test case ID: TC_Order_Events_001
		[Fact]
		public async Task OrderIdNull_ShouldSkipProcessingAndNotThrow()
		{
			// Arrange: PaymentSuccessEvent with OrderId = null
			var evt = new PaymentSuccessEvent(
			OrderId: null,
			BookingId: null,
			UserId: Guid.NewGuid(),
			Amount: 0m,
			Email: "test@example.com"
		);
			var ctx = new Mock<ConsumeContext<PaymentSuccessEvent>>();
			ctx.Setup(c => c.Message).Returns(evt);
			ctx.Setup(c => c.ConsumeCompleted).Returns(Task.CompletedTask);

			// Act
			await _handler.Consume(ctx.Object);

			// Log
			_output.WriteLine("Test case name – ID: TC_Order_Events_001");
			_output.WriteLine($"Test Data: OrderId={evt.OrderId}, BookingId={evt.BookingId}, UserId={evt.UserId}");
			_output.WriteLine("Expected result: Skip processing, no exception");
			_output.WriteLine("-----------------------");
			_output.WriteLine("Actual result: Completed without error");

			// Verify log information
			_loggerMock.Verify(l => l.Log(
				LogLevel.Information,
				It.IsAny<EventId>(),
				It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("OrderId is null, skipping order processing.")),
				null,
				It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
				Times.Once);

			// Verify no repository or unit-of-work calls
			_orderRepoMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>()), Times.Never);
			_unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
		}

		// Test case ID: TC_Order_Events_002
		[Fact]
		public async Task OrderNotFound_ShouldLogErrorNotfound()
		{
			// Arrange: PaymentSuccessEvent with valid OrderId, repository returns null
			var orderId = Guid.NewGuid();
			var evt = new PaymentSuccessEvent(
			OrderId: orderId,
			BookingId: null,
			UserId: Guid.NewGuid(),
			Amount: 0m,
			Email: "test@example.com" // Removed Currency property
		);
			var ctx = new Mock<ConsumeContext<PaymentSuccessEvent>>();
			ctx.Setup(c => c.Message).Returns(evt);

			_orderRepoMock
				.Setup(r => r.GetByIdAsync(orderId))
				.ReturnsAsync((Order)null);

			// Act
			await _handler.Consume(ctx.Object);

			// Log
			_output.WriteLine("Test case name – ID: TC_Order_Events_002");
			_output.WriteLine($"Test Data: OrderId={orderId}");
			_output.WriteLine("Expected result: Log error 'Notfound'");
			_output.WriteLine("-----------------------");
			_output.WriteLine("Actual result: Error logged");

			// Verify error log
			_loggerMock.Verify(l => l.Log(
				LogLevel.Error,
				It.IsAny<EventId>(),
				It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Notfound")),
				null,
				It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
				Times.Once);

			// Verify no SaveChanges
			_unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
		}

		// Test case ID: TC_Order_Events_003
		[Fact]
		public async Task InvalidOrderStatus_ShouldLogErrorInvalidState()
		{
			// Arrange: PaymentSuccessEvent with OrderId, repository returns Created status
			var orderId = Guid.NewGuid();
			var order = new Order { Id = orderId, OrderStatusId = OrderStatusConstants.Created };
			var evt = new PaymentSuccessEvent(
			OrderId: orderId,
			BookingId: null,
			UserId: Guid.NewGuid(),
			Amount: 0m,
			Email: "test@example.com" // Removed Currency property
		);
			var ctx = new Mock<ConsumeContext<PaymentSuccessEvent>>();
			ctx.Setup(c => c.Message).Returns(evt);

			_orderRepoMock
				.Setup(r => r.GetByIdAsync(orderId))
				.ReturnsAsync(order);

			// Act
			await _handler.Consume(ctx.Object);

			// Log
			_output.WriteLine("Test case name – ID: TC_Order_Events_003");
			_output.WriteLine($"Test Data: OrderId={orderId}, Status={order.OrderStatusId}");
			_output.WriteLine("Expected result: Log error 'InvalidState'");
			_output.WriteLine("-----------------------");
			_output.WriteLine("Actual result: Error logged");

			// Verify error log
			_loggerMock.Verify(l => l.Log(
				LogLevel.Error,
				It.IsAny<EventId>(),
				It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("InvalidState")),
				null,
				It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
				Times.Once);

			// Verify no SaveChanges
			_unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
		}

		// Test case ID: TC_Order_Events_004
		[Fact]
		public async Task SuccessfulConfirmUpdate_ShouldUpdateStatusAndSave()
		{
			// Arrange: PaymentSuccessEvent with OrderId, repository returns Pending status
			var orderId = Guid.NewGuid();
			var order = new Order { Id = orderId, OrderStatusId = OrderStatusConstants.Pending };
			var evt = new PaymentSuccessEvent(
			OrderId: orderId,
			BookingId: null,
			UserId: Guid.NewGuid(),
			Amount: 0m,
			Email: "test@example.com" // Removed Currency property
		);
			var ctx = new Mock<ConsumeContext<PaymentSuccessEvent>>();
			ctx.Setup(c => c.Message).Returns(evt);

			_orderRepoMock
				.Setup(r => r.GetByIdAsync(orderId))
				.ReturnsAsync(order);
			_unitOfWorkMock
				.Setup(u => u.SaveChangesAsync())
				.ReturnsAsync(1);

			// Act
			await _handler.Consume(ctx.Object);

			// Log
			_output.WriteLine("Test case name – ID: TC_Order_Events_004");
			_output.WriteLine($"Test Data: OrderId={orderId}, InitialStatus=Pending");
			_output.WriteLine("Expected result: Status changed to Confirmed, entries added, SaveChanges called");
			_output.WriteLine("-----------------------");
			_output.WriteLine($"Actual result: Status={order.OrderStatusId}");

			// Assert status updated
			Assert.Equal(OrderStatusConstants.Confirmed, order.OrderStatusId);

			// Verify unitOfWork entries and save
			_unitOfWorkMock.Verify(u => u.Entry(order, EntityState.Modified), Times.Once);
			_unitOfWorkMock.Verify(u => u.Entry(
				It.Is<OrderStatusHistory>(h =>
					h.OrderId == orderId &&
					h.OldStatusId == OrderStatusConstants.Pending &&
					h.NewStatusId == OrderStatusConstants.Confirmed),
				EntityState.Added), Times.Once);
			_unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
		}

		// Test case ID: TC_Order_Events_005
		[Fact]
		public async Task ExceptionDuringProcessing_ShouldCatchAndLog()
		{
			// Arrange: PaymentSuccessEvent with OrderId, SaveChangesAsync throws
			var orderId = Guid.NewGuid();
			var evt = new PaymentSuccessEvent(
			OrderId: orderId,
			BookingId: null,
			UserId: Guid.NewGuid(),
			Amount: 0m,
			Email: "test@example.com" // Removed Currency property
		);
			var ctx = new Mock<ConsumeContext<PaymentSuccessEvent>>();
			ctx.Setup(c => c.Message).Returns(evt);

			_orderRepoMock
				.Setup(r => r.GetByIdAsync(orderId))
				.ReturnsAsync(new Order { Id = orderId, OrderStatusId = OrderStatusConstants.Pending });
			_unitOfWorkMock
				.Setup(u => u.SaveChangesAsync())
				.ThrowsAsync(new InvalidOperationException("DB error"));

			// Act
			await _handler.Consume(ctx.Object);

			// Log
			_output.WriteLine("Test case name – ID: TC_Order_Events_005");
			_output.WriteLine($"Test Data: OrderId={orderId}, SaveChanges throws InvalidOperationException");
			_output.WriteLine("Expected result: Exception caught and logged");
			_output.WriteLine("-----------------------");
			_output.WriteLine("Actual result: No exception thrown");

			// Verify error log contains exception
			_loggerMock.Verify(l => l.Log(
				LogLevel.Error,
				It.IsAny<EventId>(),
				It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("System.InvalidOperationException")),
				It.IsAny<Exception?>(),
				It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
				Times.Once);
		}
	}
}
