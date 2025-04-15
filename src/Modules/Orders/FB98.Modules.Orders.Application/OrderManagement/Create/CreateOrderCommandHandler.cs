using AutoMapper;
using FB98.Modules.Orders.Application.Abstractions;
using FB98.Modules.Orders.Domain.Entities;
using FB98.Shared.Abstractions.Events;
using FB98.Shared.Abstractions.Refits;
using FB98.Shared.Abstractions.StatusConstants;
using MassTransit;
using Refit;

namespace FB98.Modules.Orders.Application.OrderManagement.Create
{
	public sealed class CreateOrderCommandHandler : ICommandHandler<CreateOrderCommand, ApiResult<object>>
	{
		private readonly IBus _bus;
		private readonly ICatalogApi _catalogApi;
		private readonly ILocalizedMessageService _localizedMessageService;
		private readonly ILogger<CreateOrderCommandHandler> _logger;
		private readonly IMapper _mapper;
		private readonly IOrderRepository _orderRepository;
		private readonly IValidator<CreateOrderDto> _validator;
		private readonly IWarehouseApi _warehouseApi;
		private readonly IUnitOfWork _unitOfWork;

		public CreateOrderCommandHandler(
			IOrderRepository orderRepository,
			ILocalizedMessageService localizedMessageService,
			IMapper mapper,
			ILogger<CreateOrderCommandHandler> logger,
			IValidator<CreateOrderDto> validator,
			IWarehouseApi warehouseApi,
			ICatalogApi catalogApi,
			IBus bus,
			IUnitOfWork unitOfWork)
		{
			_orderRepository = orderRepository;
			_localizedMessageService = localizedMessageService;
			_mapper = mapper;
			_logger = logger;
			_validator = validator;
			_warehouseApi = warehouseApi;
			_catalogApi = catalogApi;
			_bus = bus;
			_unitOfWork = unitOfWork;
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
				order.OrderStatusId = OrderStatusConstants.Created;
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
					if (!item.IsCombo)
					{
						continue;
					}

					var combo = comboResponses.FirstOrDefault(x => x.Data!.Id == item.ProductId);
					if (combo is null || !combo.IsSuccess)
					{
						return ApiResponseBuilder.Error<object>(_localizedMessageService.GetLocalizedMessage("NotFound") + $"combo{item.ProductId}", 404);
					}

					item.ProductName = combo.Data!.Name;
					item.UnitPrice = combo.Data!.Price;
					item.FinalPrice = combo.Data!.DiscountPrice > 0 ? combo.Data!.DiscountPrice : item.UnitPrice;
					item.SubTotalPrice = combo.Data!.Price * item.Quantity;
					item.TotalPrice = item.FinalPrice * item.Quantity;
					order.SubAmount += item.SubTotalPrice;
					order.Amount += item.TotalPrice;

					foreach (var product in combo.Data.Products.Where(product => !stockTasks.ContainsKey(product.Id)))
					{
						stockTasks[product.Id] = _warehouseApi.GetStock(product.Id);
					}
				}

				await Task.WhenAll(stockTasks.Values);

				var stockItems = new List<StockItem>();

				foreach (var item in order.OrderItems)
				{
					if (item.IsCombo)
					{
						var combo = comboResponses.FirstOrDefault(x => x.Data!.Id == item.ProductId);
						if (combo is null || !combo.IsSuccess)
						{
							return ApiResponseBuilder.Error<object>(_localizedMessageService.GetLocalizedMessage("NotFound") + $"combo: {item.ProductId}", 404);
						}

						foreach (var product in combo.Data!.Products)
						{
							var stockResponse = await stockTasks[product.Id];
							if (!stockResponse.IsSuccess)
							{
								return ApiResponseBuilder.Error<object>(_localizedMessageService.GetLocalizedMessage("WarehouseError"), 500);
							}

							if (stockResponse.Data!.IsLimited && stockResponse.Data.Quantity < product.Quantity)
							{
								return ApiResponseBuilder.Error<object>(_localizedMessageService.GetLocalizedMessage("NotEnoughStock"));
							}

							stockItems.Add(new StockItem(product.Id, product.Quantity));
						}
					}
					else
					{
						var product = productResponses.FirstOrDefault(x => x.Data!.Id == item.ProductId);
						if (product is null || !product.IsSuccess)
						{
							return ApiResponseBuilder.Error<object>(_localizedMessageService.GetLocalizedMessage("NotFound") + $"{item.ProductId}", 404);
						}

						var stockResponse = await stockTasks[item.ProductId];
						if (!stockResponse.IsSuccess)
						{
							return ApiResponseBuilder.Error<object>(_localizedMessageService.GetLocalizedMessage("WarehouseError"), 500);
						}

						if (stockResponse.Data!.IsLimited && stockResponse.Data.Quantity < item.Quantity)
						{
							return ApiResponseBuilder.Error<object>(_localizedMessageService.GetLocalizedMessage("NotEnoughStock"));
						}

						stockItems.Add(new StockItem(item.ProductId, item.Quantity));

						item.ProductName = product.Data!.Name;
						item.UnitPrice = product.Data!.Price;
						item.FinalPrice = product.Data!.DiscountPrice > 0 ? product.Data!.DiscountPrice : item.UnitPrice;
						item.SubTotalPrice = product.Data!.Price * item.Quantity;
						item.TotalPrice = item.FinalPrice * item.Quantity;
						order.SubAmount += item.SubTotalPrice;
						order.Amount += item.TotalPrice;
					}

					item.SetCreatedAt();
				}

				order.SetDiscountPercentage();
				await _orderRepository.CreateAsync(order);
				await _unitOfWork.SaveChangesAsync();

				var discountItems = order.OrderItems.Select(x => new DiscountItem(x.ProductId, x.IsCombo)).ToList();
				await _bus.Publish(new OrderCreatedEvent(order.Id, stockItems, discountItems), cancellationToken);
				return ApiResponseBuilder.Success<object>(order.Id, _localizedMessageService.GetLocalizedMessage("Created"), 201);
			}
			catch (ApiException ex)
			{
				_logger.LogError($"API error: {ex.StatusCode} - {ex.Content}");
				return ApiResponseBuilder.Error<object>("I", 404);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while get create order");
				return ApiResponseBuilder.Error<object>("An unexpected error occurred", 500);
			}
		}
	}
}