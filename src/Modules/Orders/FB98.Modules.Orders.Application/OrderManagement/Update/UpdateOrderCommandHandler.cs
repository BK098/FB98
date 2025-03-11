using FB98.Modules.Orders.Application.Abstractions;
using FB98.Modules.Orders.Domain.Entities;
using FB98.Shared.Abstractions.StatusConstants;
using Microsoft.EntityFrameworkCore;

namespace FB98.Modules.Orders.Application.OrderManagement.Update
{
	public sealed class UpdateOrderCommandHandler : ICommandHandler<UpdateOrderCommand, ApiResult<object>>
	{
		private readonly ILocalizedMessageService _localizedMessageService;
		private readonly ILogger<UpdateOrderCommandHandler> _logger;
		private readonly IOrderRepository _orderRepository;
		private readonly IUnitOfWork _unitOfWork;

		public UpdateOrderCommandHandler(
			ILogger<UpdateOrderCommandHandler> logger,
			ILocalizedMessageService localizedMessageService,
			IOrderRepository orderRepository,
			IUnitOfWork unitOfWork)
		{
			_logger = logger;
			_localizedMessageService = localizedMessageService;
			_orderRepository = orderRepository;
			_unitOfWork = unitOfWork;
		}

		public async Task<ApiResult<object>> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
		{
			try
			{
				var statusUpdates = new Dictionary<Guid, Guid>
				{
					{ OrderStatusConstants.Created, OrderStatusConstants.Confirmed },
					{ OrderStatusConstants.Confirmed, OrderStatusConstants.CheckedIn }
				};

				if (!statusUpdates.TryGetValue(request.OrderStatusId, out var expectedNewStatus))
				{
					return ApiResponseBuilder.Error<object>(_localizedMessageService.GetLocalizedMessage("InvalidState"));
				}

				var order = await _orderRepository.GetByIdAsync(request.OrderId);
				if (order == null)
				{
					return ApiResponseBuilder.Error<object>(_localizedMessageService.GetLocalizedMessage("NotFound"), 404);
				}

				if (order.OrderStatusId != expectedNewStatus)
				{
					return ApiResponseBuilder.Error<object>(_localizedMessageService.GetLocalizedMessage("InvalidState"));
				}

				var previousStatus = order.OrderStatusId;
				order.OrderStatusId = request.OrderStatusId;
				order.SetCreatedAt();
				var orderStatusHistory = new OrderStatusHistory
				{
					OrderId = order.Id,
					OldStatusId = previousStatus,
					NewStatusId = request.OrderStatusId
				};
				orderStatusHistory.SetCreatedAt();
				_unitOfWork.Entry(order, EntityState.Modified);
				_unitOfWork.Entry(orderStatusHistory, EntityState.Added);

				await _unitOfWork.SaveChangesAsync();

				return ApiResponseBuilder.Success<object>("", _localizedMessageService.GetLocalizedMessage("Updated"));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while update order");
				return ApiResponseBuilder.Error<object>("An unexpected error occurred", 500);
			}
		}
	}
}