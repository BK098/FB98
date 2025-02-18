using AutoMapper;
using FB98.Modules.Orders.Application.Abstractions;
using FB98.Modules.Orders.Domain.Entities;
using FB98.Shared.Abstractions.Refits;
using MassTransit;
using Refit;

namespace FB98.Modules.Orders.Application.OrderManagement.Create
{
	internal sealed class CreateOrderCommandHandler : ICommandHandler<CreateOrderCommand, ApiResult<object>>
	{
		private readonly IOrderRepository _orderRepository;
		private readonly ILocalizedMessageService _localizedMessageService;
		private readonly IMapper _mapper;
		private readonly ILogger<CreateOrderCommandHandler> _logger;
		private readonly IValidator<CreateOrderDto> _validator;
		private readonly IWarehouseApi _warehouseApi;
		private readonly ICatalogApi _catalogApi;
		private readonly IBus _bus;

		public CreateOrderCommandHandler(
			IOrderRepository orderRepository,
			ILocalizedMessageService localizedMessageService,
			IMapper mapper,
			ILogger<CreateOrderCommandHandler> logger,
			IValidator<CreateOrderDto> validator,
			IWarehouseApi warehouseApi,
			ICatalogApi catalogApi,
			IBus bus)
		{
			_orderRepository = orderRepository;
			_localizedMessageService = localizedMessageService;
			_mapper = mapper;
			_logger = logger;
			_validator = validator;
			_warehouseApi = warehouseApi;
			_catalogApi = catalogApi;
			_bus = bus;
		}
		public async Task<ApiResult<object>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
		{
			var model = request.Model;
			try
			{
				var validationResult = await _validator.ValidateAsync(model, cancellationToken);
				if (!validationResult.IsValid)
				{
					return ApiResponseBuilder.ValidationError<object>(validationResult.Errors);
				}

				var order = _mapper.Map<Order>(model);
				order.OrderStatusId = OrderStatusConstants.Requested;
				order.Amount = 0;
				order.SetCreatedAt();

				var productTasks = new List<Task<ApiResult<ProductDto>>>();
				var comboTasks = new List<Task<ApiResult<ComboDto>>>();
				var stockTasks = new Dictionary<Guid, Task<ApiResult<StockResponse>>>();

				foreach (var item in order.OrderItems)
				{
					if (item.IsCombo)
					{
						comboTasks.Add(_catalogApi.GetComboById(item.ProductId));
					}
					else
					{
						productTasks.Add(_catalogApi.GetProductById(item.ProductId));
						stockTasks.Add(item.ProductId, _warehouseApi.GetStock(item.ProductId));
					}
				}

				var productResponses = await Task.WhenAll(productTasks);
				var comboResponses = await Task.WhenAll(comboTasks);
				// Duyệt lại order để cập nhật thông tin sản phẩm được lấy từ catalog và warehouse
				foreach (var item in order.OrderItems)
				{
					if (item.IsCombo)
					{
						var comboResponse = comboResponses.FirstOrDefault(x => x.Data!.Id == item.ProductId);
						if (comboResponse is null || !comboResponse.IsSuccess)
						{
							return ApiResponseBuilder.Error<object>(_localizedMessageService.GetLocalizedMessage("NotFound") + $"combo{item.ProductId}", statusCode: 404);
						}

						item.ProductName = comboResponse.Data!.Name;
						item.Price = comboResponse.Data!.Price;
						item.TotalPrice = comboResponse.Data!.Price * item.Quantity;
						order.Amount += item.TotalPrice;

						foreach (var product in comboResponse.Data.Products)
						{
							if (!stockTasks.ContainsKey(product.Id))
							{
								stockTasks[product.Id] = _warehouseApi.GetStock(product.Id);
							}
						}
					}
				}
				await Task.WhenAll(stockTasks.Values);

				foreach (var item in order.OrderItems)
				{
					if (item.IsCombo)
					{
						var combo = comboResponses.FirstOrDefault(x => x.Data!.Id == item.ProductId);
						if (combo is null || !combo.IsSuccess)
						{
							return ApiResponseBuilder.Error<object>(_localizedMessageService.GetLocalizedMessage("NotFound") + $"combo{item.ProductId}", statusCode: 404);
						}
						foreach (var product in combo.Data!.Products)
						{
							var stockResponse = await stockTasks[product.Id];
							if (!stockResponse.IsSuccess)
							{
								return ApiResponseBuilder.Error<object>(_localizedMessageService.GetLocalizedMessage("WarehouseError"), statusCode: 500);
							}
							if (stockResponse.Data!.IsLimited && stockResponse.Data.Quantity < product.Quantity)
							{
								return ApiResponseBuilder.Error<object>(_localizedMessageService.GetLocalizedMessage("NotEnoughStock"), statusCode: 400);
							}
						}
					}
					else
					{
						var product = productResponses.FirstOrDefault(x => x.Data!.Id == item.ProductId);
						if (product is null || !product.IsSuccess)
						{
							return ApiResponseBuilder.Error<object>(_localizedMessageService.GetLocalizedMessage("NotFound") + $"{item.ProductId}", statusCode: 404);
						}
						var stockResponse = await stockTasks[item.ProductId];
						if (!stockResponse.IsSuccess)
						{
							return ApiResponseBuilder.Error<object>(_localizedMessageService.GetLocalizedMessage("WarehouseError"), statusCode: 500);
						}
						if (stockResponse.Data!.IsLimited && stockResponse.Data.Quantity < item.Quantity)
						{
							return ApiResponseBuilder.Error<object>(_localizedMessageService.GetLocalizedMessage("NotEnoughStock"), statusCode: 400);
						}
						item.ProductName = product.Data!.Name;
						item.Price = product.Data!.Price;
						item.TotalPrice = product.Data!.Price * item.Quantity;
						order.Amount += item.TotalPrice;
					}
					item.SetCreatedAt();
				}

				await _orderRepository.CreateAsync(order);
				await _orderRepository.SaveChangesAsync();

				return ApiResponseBuilder.Success<object>(_localizedMessageService.GetLocalizedMessage("Created"), statusCode: 201);
			}
			catch (ApiException ex)
			{
				_logger.LogError($"API error: {ex.StatusCode} - {ex.Content}");
				return ApiResponseBuilder.Error<object>("Internal Server Error", 500);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while get create order");
				return ApiResponseBuilder.Error<object>("An unexpected error occurred", statusCode: 500);
			}
		}
	}
}
