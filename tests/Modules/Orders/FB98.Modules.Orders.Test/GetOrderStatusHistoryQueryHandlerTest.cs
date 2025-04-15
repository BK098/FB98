using AutoMapper;
using FB98.Modules.Orders.Application.Abstractions;
using FB98.Modules.Orders.Application.OrderManagement.GetOrderStatusHistory;
using FB98.Modules.Orders.Domain.Entities;
using FB98.Shared.Infrastructure.Localization;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit.Abstractions;

namespace FB98.Modules.Orders.Test
{
	public class GetOrderStatusHistoryQueryHandlerTest
	{
		private readonly GetOrderStatusHistoryQueryHandler _handler;
		private readonly Mock<IOrderRepository> _orderRepoMock;
		private readonly Mock<IMapper> _mapperMock;
		private readonly Mock<ILogger<GetOrderStatusHistoryQueryHandler>> _loggerMock;
		private readonly ILocalizedMessageService _localizer;
		private readonly ITestOutputHelper _output;

		private class StubLocalizer : ILocalizedMessageService
		{
			public string GetLocalizedMessage(string key) => key;
			public string GetLocalizedMessage(string key, string? culture) => key;
		}

		public GetOrderStatusHistoryQueryHandlerTest(ITestOutputHelper output)
		{
			_output = output;
			_orderRepoMock = new Mock<IOrderRepository>();
			_mapperMock = new Mock<IMapper>();
			_loggerMock = new Mock<ILogger<GetOrderStatusHistoryQueryHandler>>();
			_localizer = new StubLocalizer();

			_handler = new GetOrderStatusHistoryQueryHandler(
				_orderRepoMock.Object,
				_localizer,
				_loggerMock.Object,
				_mapperMock.Object
			);
		}

		// Test case id: TC_Booking_GetOrderStatusHistory_001
		[Fact]
		public async Task Handle_ShouldReturnNotFound_WhenHistoryIsNullOrEmpty()
		{
			// Arrange
			var orderId = Guid.NewGuid();
			var query = new GetOrderStatusHistoryQuery(orderId);

			_orderRepoMock
				.Setup(r => r.GetOrderStatusHistoryAsync(orderId))
				.ReturnsAsync((IEnumerable<OrderStatusHistory>?)null);

			// Act
			var result = await _handler.Handle(query, CancellationToken.None);

			// Assert
			_output.WriteLine("Test case ID: TC_Booking_GetOrderStatusHistory_001");
			_output.WriteLine("Test Data: OrderId = {0}", orderId);
			_output.WriteLine("Expected result: NotFound (404)");
			_output.WriteLine("Actual result: {0} ({1})", result.Message, result.StatusCode);
			if (!result.IsSuccess)
			{
				_output.WriteLine("Error: {0}", result.Message);
			}

			Assert.False(result.IsSuccess);
			Assert.Equal(404, result.StatusCode);
			Assert.Equal("NotFound", result.Message);
			_mapperMock.Verify(m => m.Map<IEnumerable<GetOrderStatusHistoryResponse>>(It.IsAny<IEnumerable<OrderStatusHistory>>()), Times.Never);
		}

		// Test case id: TC_Booking_GetOrderStatusHistory_002
		[Fact]
		public async Task Handle_ShouldReturnSuccess_WhenHistoryExists()
		{
			// Arrange
			var orderId = Guid.NewGuid();
			var domainHistory = new List<OrderStatusHistory>
			{
				new OrderStatusHistory
				{
					OrderId = orderId,
					OldStatusId = Guid.NewGuid(),
					NewStatusId = Guid.NewGuid(),
					ChangedBy = "System"
				}
			};
			var expectedResponse = new List<GetOrderStatusHistoryResponse>
			{
				new GetOrderStatusHistoryResponse
				{
					OldStatusId = domainHistory[0].OldStatusId,
					NewStatusId = domainHistory[0].NewStatusId,
					OldStatus = "Created",
					NewStatus = "Pending",
					ChangedAt = DateTime.UtcNow
				}
			};

			_orderRepoMock
				.Setup(r => r.GetOrderStatusHistoryAsync(orderId))
				.ReturnsAsync(domainHistory);
			_mapperMock
				.Setup(m => m.Map<IEnumerable<GetOrderStatusHistoryResponse>>(domainHistory))
				.Returns(expectedResponse);

			var query = new GetOrderStatusHistoryQuery(orderId);

			// Act
			var result = await _handler.Handle(query, CancellationToken.None);

			// Assert
			_output.WriteLine("Test case ID: TC_Booking_GetOrderStatusHistory_002");
			_output.WriteLine("Test Data: OrderId = {0}", orderId);
			_output.WriteLine("Expected result: Success (200) with history data");
			_output.WriteLine("Actual result: {0} ({1})", result.Message, result.StatusCode);
			if (result.IsSuccess)
			{
				_output.WriteLine("Data count: {0}", result.Data?.Count() ?? 0);
			}

			Assert.True(result.IsSuccess);
			Assert.Equal(200, result.StatusCode);
			Assert.Equal("DataRetrieved", result.Message);
			Assert.NotNull(result.Data);
			Assert.Equal(expectedResponse.Count, result.Data.Count());
		}

		// Test case id: TC_Booking_GetOrderStatusHistory_003
		[Fact]
		public async Task Handle_ShouldReturnError_WhenExceptionThrown()
		{
			// Arrange
			var orderId = Guid.NewGuid();
			var query = new GetOrderStatusHistoryQuery(orderId);
			_orderRepoMock
				.Setup(r => r.GetOrderStatusHistoryAsync(orderId))
				.ThrowsAsync(new Exception("Test error"));

			// Act
			var result = await _handler.Handle(query, CancellationToken.None);

			// Assert
			_output.WriteLine("Test case ID: TC_Booking_GetOrderStatusHistory_003");
			_output.WriteLine("Test Data: OrderId = {0}", orderId);
			_output.WriteLine("Expected result: Error (500)");
			_output.WriteLine("Actual result: {0} ({1})", result.Message, result.StatusCode);
			if (!result.IsSuccess)
			{
				_output.WriteLine("Error: {0}", result.Message);
			}

			Assert.False(result.IsSuccess);
			Assert.Equal(500, result.StatusCode);
			Assert.Equal("An unexpected error occurred", result.Message);
			_loggerMock.Verify(l => l.LogError(
				It.IsAny<Exception>(),
				It.Is<string>(s => s.Contains("Error occurred while get order status history"))),
				Times.Once);
		}
	}
}