using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using Xunit.Abstractions;
using FB98.Modules.Orders.Application.Abstractions;
using FB98.Modules.Orders.Application.OrderManagement.Create;
using FB98.Modules.Orders.Domain.Entities;
using FB98.Shared.Abstractions.Refits;
using FB98.Shared.Abstractions.Events;
using FB98.Shared.Abstractions.StatusConstants;
using FB98.Shared.Infrastructure.Localization;
using MassTransit;
using FB98.Shared.Abstractions.Responses;
using FluentValidation;

namespace FB98.Modules.Orders.Test
{
	public class CreateOrderCommandHandlerTest
	{
		private readonly ITestOutputHelper _output;
		private readonly IValidator<CreateOrderDto> _validator;
		private readonly Mock<ILocalizedMessageService> _localizerMock;

		private readonly Mock<IOrderRepository> _orderRepoMock;
		private readonly Mock<IUnitOfWork> _unitOfWorkMock;
		private readonly Mock<IMapper> _mapperMock;
		private readonly Mock<ICatalogApi> _catalogApiMock;
		private readonly Mock<IWarehouseApi> _warehouseApiMock;
		private readonly Mock<IBus> _busMock;
		private readonly Mock<ILogger<CreateOrderCommandHandler>> _loggerMock;
		private readonly CreateOrderCommandHandler _handler;

		public CreateOrderCommandHandlerTest(ITestOutputHelper output)
		{
			_output = output;

			// Khởi tạo mock
			_localizerMock = new Mock<ILocalizedMessageService>();

			
			_localizerMock
				.Setup(x => x.GetLocalizedMessage(It.IsAny<string>(), It.IsAny<string?>()))
				.Returns((string key, string? defaultMessage) => key);

			// Validator dùng CreateOrderValidation :contentReference[oaicite:0]{index=0}
			_validator = new CreateOrderValidation(_localizerMock.Object);

			// Các dependency khác
			_orderRepoMock = new Mock<IOrderRepository>();
			_unitOfWorkMock = new Mock<IUnitOfWork>();
			_mapperMock = new Mock<IMapper>();
			_catalogApiMock = new Mock<ICatalogApi>();
			_warehouseApiMock = new Mock<IWarehouseApi>();
			_busMock = new Mock<IBus>();
			_loggerMock = new Mock<ILogger<CreateOrderCommandHandler>>();

			// Khởi tạo handler :contentReference[oaicite:1]{index=1}
			_handler = new CreateOrderCommandHandler(
				_orderRepoMock.Object,
				_localizerMock.Object,
				_mapperMock.Object,
				_loggerMock.Object,
				_validator,
				_warehouseApiMock.Object,
				_catalogApiMock.Object,
				_busMock.Object,
				_unitOfWorkMock.Object
			);
		}





		// TC_Booking_Create_001
		[Fact]
		public async Task MissingItems_ShouldFailValidation()
		{
			var userId = Guid.NewGuid();
			var dtos = new[]
			{
				new CreateOrderDto { UserId = userId, Items = null },
				new CreateOrderDto { UserId = userId, Items = new List<CreateOrderItemDto>() }
			};

			foreach (var dto in dtos)
			{
				_output.WriteLine("Test case name - ID: TC_Booking_Create_001");
				_output.WriteLine($"Test Data: UserId={dto.UserId}, Items={(dto.Items == null ? "null" : "empty")}");
				_output.WriteLine("Expected result: validation fails with NotNull, NotEmpty for Items");
				_output.WriteLine("-----------------------");

				var result = await _validator.ValidateAsync(dto, CancellationToken.None);
				var messages = result.Errors.Select(e => e.ErrorMessage).ToList();

				_output.WriteLine("Actual result: " + string.Join(", ", messages));
				result.IsValid.Should().BeFalse();
				messages.Should().Contain("NotNull");
				messages.Should().Contain("NotEmpty");
			}
		}

		// TC_Booking_Create_002
		[Fact]
		public async Task InvalidOrderItemFields_ShouldFailValidation()
		{
			var userId = Guid.NewGuid();
			var invalidItems = new[]
			{
				new CreateOrderItemDto { ProductId = null, IsCombo = true,  Quantity = 1 },
				new CreateOrderItemDto { ProductId = Guid.NewGuid(), IsCombo = null, Quantity = 1 },
				new CreateOrderItemDto { ProductId = Guid.NewGuid(), IsCombo = false, Quantity = 0 },
				new CreateOrderItemDto { ProductId = Guid.NewGuid(), IsCombo = false, Quantity = 11 }
			};

			foreach (var item in invalidItems)
			{
				var dto = new CreateOrderDto { UserId = userId, Items = new List<CreateOrderItemDto> { item } };
				_output.WriteLine("Test case name - ID: TC_Booking_Create_002");
				_output.WriteLine($"Test Data: ProductId={item.ProductId}, IsCombo={item.IsCombo}, Quantity={item.Quantity}");
				_output.WriteLine("Expected result: validation fails with appropriate messages");
				_output.WriteLine("-----------------------");

				var result = await _validator.ValidateAsync(dto, CancellationToken.None);
				var messages = result.Errors.Select(e => e.ErrorMessage).ToList();
				_output.WriteLine("Actual result: " + string.Join(", ", messages));

				result.IsValid.Should().BeFalse();
				if (item.ProductId == null) messages.Should().Contain("NotNull");
				if (item.IsCombo == null) messages.Should().Contain("NotNull");
				if (item.Quantity < 1 || item.Quantity > 10) messages.Should().Contain("QuantityRange");
			}
			
		}

		// TC_Booking_Create_003
		[Fact]
		public async Task ProductLookupFailure_ShouldReturnNotFound()
		{
			var userId = Guid.NewGuid();
			var prodId = Guid.NewGuid();
			var dto = new CreateOrderDto
			{
				UserId = userId,
				Items = new List<CreateOrderItemDto>
				{
					new CreateOrderItemDto { ProductId = prodId, IsCombo = false, Quantity = 1 }
				}
			};
			var command = new CreateOrderCommand(dto); // :contentReference[oaicite:2]{index=2}

			// Stub mapping
			var order = new Order
			{
				Id = Guid.NewGuid(),
				UserId = userId,
				OrderItems = new List<OrderItem>
			{
				new OrderItem { ProductId = prodId, IsCombo = false, Quantity = 1 }
			}
			};
			_mapperMock.Setup(m => m.Map<Order>(dto)).Returns(order);

			// Catalog returns failure
			_catalogApiMock.Setup(c => c.GetProductById(prodId))
						   .ReturnsAsync(new ApiResult<ProductDto> { IsSuccess = false, Data = null });
			// Warehouse stub (not reached)
			_warehouseApiMock.Setup(w => w.GetStock(prodId))
							.ReturnsAsync(new ApiResult<StockResponse> { IsSuccess = true, Data = new StockResponse(prodId, 10, false) });

			_output.WriteLine("Test case name - ID: TC_Booking_Create_003");
			_output.WriteLine($"Test Data: ProductId={prodId}, IsCombo=false, Quantity=1");
			_output.WriteLine("Expected result: handler returns NotFound + 404");
			_output.WriteLine("-----------------------");

			var result = await _handler.Handle(command, CancellationToken.None);

			_output.WriteLine($"Actual result: IsSuccess={result.IsSuccess}, StatusCode={result.StatusCode}, Message={result.Message}");
			result.IsSuccess.Should().BeFalse();
			result.StatusCode.Should().Be(404);
			result.Message.Should().StartWith("NotFound");
		}

		// TC_Booking_Create_004
		[Fact]
		public async Task InsufficientStock_ShouldReturnNotEnoughStock()
		{
			var userId = Guid.NewGuid();
			var prodId = Guid.NewGuid();
			var dto = new CreateOrderDto
			{
				UserId = userId,
				Items = new List<CreateOrderItemDto>
				{
					new CreateOrderItemDto { ProductId = prodId, IsCombo = false, Quantity = 5 }
				}
			};
			var command = new CreateOrderCommand(dto);

			var order = new Order
			{
				Id = Guid.NewGuid(),
				UserId = userId,
				OrderItems = new List<OrderItem>
			{
				new OrderItem { ProductId = prodId, IsCombo = false, Quantity = 5 }
			}
			};
			_mapperMock.Setup(m => m.Map<Order>(dto)).Returns(order);

			_catalogApiMock.Setup(c => c.GetProductById(prodId))
						   .ReturnsAsync(new ApiResult<ProductDto> { IsSuccess = true, Data = new ProductDto(prodId, "P", 100, 0) });
			_warehouseApiMock.Setup(w => w.GetStock(prodId))
							.ReturnsAsync(new ApiResult<StockResponse> { IsSuccess = true, Data = new StockResponse(prodId, 3, true) });

			_output.WriteLine("Test case name - ID: TC_Booking_Create_004");
			_output.WriteLine($"Test Data: ProductId={prodId}, Quantity=5, Stock=3");
			_output.WriteLine("Expected result: handler returns NotEnoughStock");
			_output.WriteLine("-----------------------");

			var result = await _handler.Handle(command, CancellationToken.None);

			_output.WriteLine($"Actual result: IsSuccess={result.IsSuccess}, Message={result.Message}");
			result.IsSuccess.Should().BeFalse();
			result.Message.Should().Be("NotEnoughStock");
		}

		// TC_Booking_Create_005
		[Fact]
		public async Task SuccessfulOrderCreation_ShouldReturnCreated()
		{
			var userId = Guid.NewGuid();
			var p1 = Guid.NewGuid();
			var c1 = Guid.NewGuid();
			var dto = new CreateOrderDto
			{
				UserId = userId,
				Items = new List<CreateOrderItemDto>
				{
					new CreateOrderItemDto { ProductId = p1, IsCombo = false, Quantity = 2 },
					new CreateOrderItemDto { ProductId = c1, IsCombo = true,  Quantity = 1 }
				}
			};
			var command = new CreateOrderCommand(dto);

			// Prepare order and items
			var order = new Order
			{
				Id = Guid.NewGuid(),
				UserId = userId,
				OrderItems = new List<OrderItem>
			{
				new OrderItem { ProductId = p1, IsCombo = false, Quantity = 2 },
				new OrderItem { ProductId = c1, IsCombo = true,  Quantity = 1 }
			}
			};
			_mapperMock.Setup(m => m.Map<Order>(dto)).Returns(order);

			// Catalog: product
			_catalogApiMock.Setup(c => c.GetProductById(p1))
						   .ReturnsAsync(new ApiResult<ProductDto> { IsSuccess = true, Data = new ProductDto(p1, "Prod1", 50, 0) });
			// Catalog: combo
			var comboDto = new ComboDto(c1, "Combo1", 100, 80, new List<ComboProductDto>
			{
				new ComboProductDto(p1, "Prod1", 50, 1)
			});
			_catalogApiMock.Setup(c => c.GetComboById(c1))
						   .ReturnsAsync(new ApiResult<ComboDto> { IsSuccess = true, Data = comboDto });

			// Warehouse: stock for product and combo's inner product
			_warehouseApiMock.Setup(w => w.GetStock(p1))
							.ReturnsAsync(new ApiResult<StockResponse> { IsSuccess = true, Data = new StockResponse(p1, 10, true) });

			// Repository create & save
			_orderRepoMock.Setup(r => r.CreateAsync(order)).ReturnsAsync(true);
			_unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

			// Bus publish stub
			_busMock.Setup(b => b.Publish(It.IsAny<OrderCreatedEvent>(), It.IsAny<CancellationToken>()))
					.Returns(Task.CompletedTask);

			_output.WriteLine("Test case name - ID: TC_Booking_Create_005");
			_output.WriteLine("Test Data: mixed product and combo, sufficient stock");
			_output.WriteLine("Expected result: IsSuccess=true, StatusCode=201, Message='Created'");
			_output.WriteLine("-----------------------");

			var result = await _handler.Handle(command, CancellationToken.None);

			_output.WriteLine($"Actual result: IsSuccess={result.IsSuccess}, StatusCode={result.StatusCode}, Message={result.Message}");
			result.IsSuccess.Should().BeTrue();
			result.StatusCode.Should().Be(201);
			result.Message.Should().Be("Created");
			result.Data.Should().Be(order.Id);

			_orderRepoMock.Verify(r => r.CreateAsync(order), Times.Once);
			_unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
			_busMock.Verify(b => b.Publish(
				It.Is<OrderCreatedEvent>(e => e.OrderId == order.Id), It.IsAny<CancellationToken>()), Times.Once);
		}

		// TC_Booking_Create_006
		[Fact]
		public async Task ExceptionDuringCreate_ShouldReturnError500()
		{
			var userId = Guid.NewGuid();
			var p1 = Guid.NewGuid();
			var dto = new CreateOrderDto
			{
				UserId = userId,
				Items = new List<CreateOrderItemDto>
				{
					new CreateOrderItemDto { ProductId = p1, IsCombo = false, Quantity = 1 }
				}
			};
			var command = new CreateOrderCommand(dto);

			var order = new Order
			{
				Id = Guid.NewGuid(),
				UserId = userId,
				OrderItems = new List<OrderItem>
			{
				new OrderItem { ProductId = p1, IsCombo = false, Quantity = 1 }
			}
			};
			_mapperMock.Setup(m => m.Map<Order>(dto)).Returns(order);

			_catalogApiMock.Setup(c => c.GetProductById(p1))
						   .ReturnsAsync(new ApiResult<ProductDto> { IsSuccess = true, Data = new ProductDto(p1, "Prod1", 50, 0) });
			_warehouseApiMock.Setup(w => w.GetStock(p1))
							.ReturnsAsync(new ApiResult<StockResponse> { IsSuccess = true, Data = new StockResponse(p1, 10, false) });

			// Throw on save
			_orderRepoMock.Setup(r => r.CreateAsync(order)).ThrowsAsync(new Exception("Test error"));

			_output.WriteLine("Test case name - ID: TC_Booking_Create_006");
			_output.WriteLine("Test Data: valid order but repository throws");
			_output.WriteLine("Expected result: IsSuccess=false, StatusCode=500, Message='An unexpected error occurred'");
			_output.WriteLine("-----------------------");

			var result = await _handler.Handle(command, CancellationToken.None);

			_output.WriteLine($"Actual result: IsSuccess={result.IsSuccess}, StatusCode={result.StatusCode}, Message={result.Message}");
			result.IsSuccess.Should().BeFalse();
			result.StatusCode.Should().Be(500);
			result.Message.Should().Be("An unexpected error occurred");
			_loggerMock.Verify(l => l.LogError(It.IsAny<Exception>(), "Error occurred while get create order"), Times.Once);
		}
	}
}
