using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using Xunit.Abstractions;
using FB98.Modules.Orders.Application.Abstractions;
using FB98.Modules.Orders.Domain.Entities;
using FB98.Shared.Abstractions.Events;
using FB98.Shared.Abstractions.StatusConstants;
using FB98.Modules.Orders.Application.OrderManagement.Events;

namespace FB98.Modules.Orders.Test.Events
{
	public class VnPayPaymentCreatedEventHandlerTest
	{
		private readonly VnPayPaymentCreatedEventHandler _handler;
		private readonly Mock<IOrderRepository> _orderRepoMock;
		private readonly Mock<IUnitOfWork> _unitOfWorkMock;
		private readonly Mock<ILogger<VnPayPaymentCreatedEventHandler>> _loggerMock;
		private readonly ITestOutputHelper _output;

		public VnPayPaymentCreatedEventHandlerTest(ITestOutputHelper output)
		{
			_output = output;
			_orderRepoMock = new Mock<IOrderRepository>();
			_unitOfWorkMock = new Mock<IUnitOfWork>();
			_loggerMock = new Mock<ILogger<VnPayPaymentCreatedEventHandler>>();

			_handler = new VnPayPaymentCreatedEventHandler(
				_orderRepoMock.Object,
				_unitOfWorkMock.Object,
				_loggerMock.Object
			);
		}

		// Test case id: TC_Booking_Events_006
		[Fact]
		public async Task Booking_ShouldSkip_WhenOrderIdIsNull()
		{
			// Arrange
			var evt = new VnPayPaymentCreatedEvent(Guid.NewGuid(), null, Guid.NewGuid());
			var ctx = new Mock<ConsumeContext<VnPayPaymentCreatedEvent>>();
			ctx.Setup(c => c.Message).Returns(evt);
			ctx.Setup(c => c.ConsumeCompleted).Returns(Task.CompletedTask);

			// Act
			await _handler.Consume(ctx.Object);

			// Assert
			_output.WriteLine("Test case name - ID: TC_Booking_Events_006");
			_output.WriteLine("Test Data: OrderId = null");
			_output.WriteLine("Expected result: Log Information 'OrderId is null, skipping order processing.'; no repository or unitOfWork calls");
			_output.WriteLine($"Actual: repo.GetByIdAsync calls = {_orderRepoMock.Invocations.Count}, unitOfWork.SaveChangesAsync calls = {_unitOfWorkMock.Invocations.Count}");

			_loggerMock.Verify(l => l.Log(
				LogLevel.Information,
				It.IsAny<EventId>(),
				It.Is<object>(v => v != null && v.ToString()!.Contains("OrderId is null, skipping order processing.")),
				null,
				It.IsAny<Func<object, Exception?, string>>()), Times.Once);

			_orderRepoMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>()), Times.Never);
			_unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
		}


		// Test case id: TC_Booking_Events_007
		[Fact]
		public async Task Booking_ShouldError_WhenOrderNotFound()
		{
			// Arrange
			var orderId = Guid.NewGuid();
			var evt = new VnPayPaymentCreatedEvent(Guid.NewGuid(), orderId, Guid.NewGuid());
			var ctx = new Mock<ConsumeContext<VnPayPaymentCreatedEvent>>();
			ctx.Setup(c => c.Message).Returns(evt);
			ctx.Setup(c => c.ConsumeCompleted).Returns(Task.CompletedTask);

			_orderRepoMock.Setup(r => r.GetByIdAsync(orderId)).ReturnsAsync((Order)null);

			// Act
			await _handler.Consume(ctx.Object);

			// Assert
			_output.WriteLine("Test case name - ID: TC_Booking_Events_007");
			_output.WriteLine($"Test Data: OrderId = {orderId}");
			_output.WriteLine("Expected result: Log Error 'Order not found: {OrderId}'; no SaveChangesAsync");
			_output.WriteLine($"Actual: unitOfWork.SaveChangesAsync calls = {_unitOfWorkMock.Invocations.Count}");

			_loggerMock.Verify(l => l.Log(
				LogLevel.Error,
				It.IsAny<EventId>(),
				It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Order not found: ")),
				null,
				It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);
			_unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
		}

		// Test case id: TC_Booking_Events_008
		[Fact]
		public async Task Booking_ShouldError_WhenStatusNotCreated()
		{
			// Arrange
			var orderId = Guid.NewGuid();
			var order = new Order { Id = orderId, OrderStatusId = OrderStatusConstants.Pending };
			var evt = new VnPayPaymentCreatedEvent(Guid.NewGuid(), orderId, Guid.NewGuid());
			var ctx = new Mock<ConsumeContext<VnPayPaymentCreatedEvent>>();
			ctx.Setup(c => c.Message).Returns(evt);
			ctx.Setup(c => c.ConsumeCompleted).Returns(Task.CompletedTask);

			_orderRepoMock.Setup(r => r.GetByIdAsync(orderId)).ReturnsAsync(order);

			// Act
			await _handler.Consume(ctx.Object);

			// Assert
			_output.WriteLine("Test case name - ID: TC_Booking_Events_008");
			_output.WriteLine($"Test Data: OrderId = {orderId}, StatusId = {order.OrderStatusId}");
			_output.WriteLine("Expected result: Log Error 'Order is not in Created state: {OrderId}'; no SaveChangesAsync");
			_output.WriteLine($"Actual: unitOfWork.SaveChangesAsync calls = {_unitOfWorkMock.Invocations.Count}");

			_loggerMock.Verify(l => l.Log(
				LogLevel.Error,
				It.IsAny<EventId>(),
				It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Order is not in Created state: ")),
				null,
				It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);
			_unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
		}

		// Test case id: TC_Booking_Events_009
		[Fact]
		public async Task Booking_ShouldUpdateStatusToPending_WhenOrderCreated()
		{
			// Arrange
			var orderId = Guid.NewGuid();
			var order = new Order { Id = orderId, OrderStatusId = OrderStatusConstants.Created };
			var evt = new VnPayPaymentCreatedEvent(Guid.NewGuid(), orderId, Guid.NewGuid());
			var ctx = new Mock<ConsumeContext<VnPayPaymentCreatedEvent>>();
			ctx.Setup(c => c.Message).Returns(evt);
			ctx.Setup(c => c.ConsumeCompleted).Returns(Task.CompletedTask);

			_orderRepoMock.Setup(r => r.GetByIdAsync(orderId)).ReturnsAsync(order);
			_unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

			// Act
			await _handler.Consume(ctx.Object);

			// Assert
			_output.WriteLine("Test case name - ID: TC_Booking_Events_009");
			_output.WriteLine($"Test Data: OrderId = {orderId}, InitialStatus = Created");
			_output.WriteLine("Expected result: OrderStatusId = Pending; SaveChangesAsync called; Log Information");
			_output.WriteLine($"Actual: OrderStatusId = {order.OrderStatusId}; SaveChangesAsync calls = {_unitOfWorkMock.Invocations.Count}");

			Assert.Equal(OrderStatusConstants.Pending, order.OrderStatusId);
			_unitOfWorkMock.Verify(u => u.Entry(order, EntityState.Modified), Times.Once);
			_unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
			_loggerMock.Verify(l => l.Log(
				LogLevel.Information,
				It.IsAny<EventId>(),
				It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Order status updated to Pending")),
				null,
				It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);
		}

		// Test case id: TC_Booking_Events_010
		[Fact]
		public async Task Booking_ShouldHandleException_Gracefully()
		{
			// Arrange
			var orderId = Guid.NewGuid();
			var order = new Order { Id = orderId, OrderStatusId = OrderStatusConstants.Created };
			var evt = new VnPayPaymentCreatedEvent(Guid.NewGuid(), orderId, Guid.NewGuid());
			var ctx = new Mock<ConsumeContext<VnPayPaymentCreatedEvent>>();
			ctx.Setup(c => c.Message).Returns(evt);
			ctx.Setup(c => c.ConsumeCompleted).Returns(Task.CompletedTask);

			_orderRepoMock.Setup(r => r.GetByIdAsync(orderId)).ReturnsAsync(order);
			_unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ThrowsAsync(new InvalidOperationException("DB error"));

			// Act
			var ex = await Record.ExceptionAsync(() => _handler.Consume(ctx.Object));

			// Assert
			_output.WriteLine("Test case name - ID: TC_Booking_Events_010");
			_output.WriteLine("Test Data: SaveChangesAsync throws InvalidOperationException");
			_output.WriteLine("Expected result: No exception thrown; Log Error with exception message");
			_output.WriteLine($"Actual: exception thrown = {(ex != null)}; Logger.LogError calls = {_loggerMock.Invocations.Count}");

			Assert.Null(ex);
			_loggerMock.Verify(l => l.Log(
				LogLevel.Error,
				It.IsAny<EventId>(),
				It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Error occurred while processing VnPayPaymentCreatedEvent")),
				It.IsAny<InvalidOperationException>(),
				It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);
		}
	}
}
