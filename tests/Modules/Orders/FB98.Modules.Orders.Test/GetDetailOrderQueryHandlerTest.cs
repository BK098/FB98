using AutoMapper;
using FB98.Modules.Orders.Application.Abstractions;
using FB98.Modules.Orders.Application.OrderManagement.GetDetail;
using FB98.Modules.Orders.Domain.Entities;
using FB98.Shared.Infrastructure.Localization;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit.Abstractions;

namespace FB98.Modules.Orders.Test
{
	public class GetDetailOrderQueryHandlerTest
	{
		private readonly GetDetailOrderQueryHandler _handler;
		private readonly Mock<IOrderRepository> _orderRepoMock;
		private readonly Mock<IMapper> _mapperMock;
		private readonly Mock<ILogger<GetDetailOrderQueryHandler>> _loggerMock;
		private readonly ILocalizedMessageService _localizer;
		private readonly ITestOutputHelper _output;

		private class StubLocalizer : ILocalizedMessageService
		{
			public string GetLocalizedMessage(string key) => key;
			public string GetLocalizedMessage(string key, string? culture) => key;
		}

		public GetDetailOrderQueryHandlerTest(ITestOutputHelper output)
		{
			_output = output;
			_orderRepoMock = new Mock<IOrderRepository>();
			_mapperMock = new Mock<IMapper>();
			_loggerMock = new Mock<ILogger<GetDetailOrderQueryHandler>>();
			_localizer = new StubLocalizer();

			_handler = new GetDetailOrderQueryHandler(
				_orderRepoMock.Object,
				_mapperMock.Object,
				_loggerMock.Object,
				_localizer
			);
		}

		// Test case id: TC_Booking_GetDetail_001
		[Fact]
		public async Task Handle_ShouldReturnNotFound_WhenOrderDoesNotExist()
		{
			// Arrange
			var orderId = Guid.NewGuid();
			var query = new GetDetailOrderQuery(orderId);
			_orderRepoMock.Setup(r => r.GetByIdAsync(orderId)).ReturnsAsync((Order?)null);

			// Act
			var result = await _handler.Handle(query, CancellationToken.None);

			// Assert
			_output.WriteLine("Test case name - ID: TC_Booking_GetDetail_001");
			_output.WriteLine($"Test Data: OrderId = {orderId}");
			_output.WriteLine("Expected result: ApiResult.IsSuccess=false, Message='NotFound', StatusCode=404");
			_output.WriteLine($"Actual result: IsSuccess={result.IsSuccess}, Message={result.Message}, StatusCode={result.StatusCode}");

			Assert.False(result.IsSuccess);
			Assert.Equal(404, result.StatusCode);
			Assert.Equal("NotFound", result.Message);
			_mapperMock.Verify(m => m.Map<GetDetailOrderResponse>(It.IsAny<Order>()), Times.Never);
		}

		// Test case id: TC_Booking_GetDetail_002
		[Fact]
		public async Task Handle_ShouldReturnSuccess_WhenOrderExists()
		{
			// Arrange
			var orderId = Guid.NewGuid();
			var order = new Order
			{
				Id = orderId,
				Amount = 100m,
				OrderStatusId = Guid.NewGuid(),
				// Assume domain has Items property; content irrelevant for mapping stub
			};
			var expectedResponse = new GetDetailOrderResponse
			{
				Id = orderId,
				Amount = 100m,
				StatusId = order.OrderStatusId,
				Items = new List<GetDetailOrderItemResponse>
				{
					new GetDetailOrderItemResponse
					{
						ProductId = Guid.NewGuid(),
						ProductName = "Prod",
						Quantity = 2,
						UnitPrice = 10m,
						FinalPrice = 9m,
						SubTotalPrice = 18m,
						TotalPrice = 18m,
						IsCombo = false
					}
				}
			};
			_orderRepoMock.Setup(r => r.GetByIdAsync(orderId)).ReturnsAsync(order);
			_mapperMock.Setup(m => m.Map<GetDetailOrderResponse>(order)).Returns(expectedResponse);
			var query = new GetDetailOrderQuery(orderId);

			// Act
			var result = await _handler.Handle(query, CancellationToken.None);

			// Assert
			_output.WriteLine("Test case name - ID: TC_Booking_GetDetail_002");
			_output.WriteLine($"Test Data: OrderId = {orderId}");
			_output.WriteLine("Expected result: ApiResult.IsSuccess=true, Data mapped correctly, StatusCode=200");
			_output.WriteLine($"Actual result: IsSuccess={result.IsSuccess}, Data.Id={result.Data?.Id}, Amount={result.Data?.Amount}, StatusId={result.Data?.StatusId}, ItemsCount={result.Data?.Items?.Count()}");

			Assert.True(result.IsSuccess);
			Assert.Equal(200, result.StatusCode);
			Assert.NotNull(result.Data);
			Assert.Equal(expectedResponse.Id, result.Data.Id);
			Assert.Equal(expectedResponse.Amount, result.Data.Amount);
			Assert.Equal(expectedResponse.StatusId, result.Data.StatusId);
			//Assert.Equal(expectedResponse.Items.Count, result.Data.Items.Count());


		}

		// Test case id: TC_Booking_GetDetail_003
		[Fact]
		public async Task Handle_ShouldReturnError_WhenExceptionThrown()
		{
			// Arrange
			var orderId = Guid.NewGuid();
			var query = new GetDetailOrderQuery(orderId);
			_orderRepoMock.Setup(r => r.GetByIdAsync(orderId)).ThrowsAsync(new InvalidOperationException("DB error"));

			// Act
			var result = await _handler.Handle(query, CancellationToken.None);

			// Assert
			_output.WriteLine("Test case name - ID: TC_Booking_GetDetail_003");
			_output.WriteLine("Test Data: repository.GetByIdAsync throws InvalidOperationException");
			_output.WriteLine("Expected result: ApiResult.IsSuccess=false, Message='An unexpected error occurred', StatusCode=500");
			_output.WriteLine($"Actual result: IsSuccess={result.IsSuccess}, Message={result.Message}, StatusCode={result.StatusCode}");

			Assert.False(result.IsSuccess);
			Assert.Equal(500, result.StatusCode);
			Assert.Equal("An unexpected error occurred", result.Message);
			_loggerMock.Verify(l => l.LogError(
				It.IsAny<InvalidOperationException>(),
				It.Is<string>(s => s.Contains("Error occurred while get detail order"))),
				Times.Once);
		}
	}
}
