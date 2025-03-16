using FB98.Modules.Orders.Application.Abstractions;
using FB98.Modules.Orders.Application.OrderManagement.CheckIn;
using FB98.Modules.Orders.Domain.Entities;
using FB98.Shared.Abstractions.StatusConstants;
using Microsoft.EntityFrameworkCore;

namespace FB98.Modules.Orders.Application.OrderManagement.Update
{
	public sealed class CheckInCommandHandler : ICommandHandler<CheckInCommand, ApiResult<object>>
	{
		private readonly ILocalizedMessageService _localizedMessageService;
		private readonly ILogger<CheckInCommandHandler> _logger;
		private readonly IOrderRepository _orderRepository;
		private readonly IUnitOfWork _unitOfWork;

		public CheckInCommandHandler(
			ILogger<CheckInCommandHandler> logger,
			ILocalizedMessageService localizedMessageService,
			IOrderRepository orderRepository,
			IUnitOfWork unitOfWork)
		{
			_logger = logger;
			_localizedMessageService = localizedMessageService;
			_orderRepository = orderRepository;
			_unitOfWork = unitOfWork;
		}

		public async Task<ApiResult<object>> Handle(CheckInCommand request, CancellationToken cancellationToken)
		{
			try
			{
				var order = await _orderRepository.GetByIdAsync(request.OrderId);
				if (order == null)
				{
					return ApiResponseBuilder.Error<object>(_localizedMessageService.GetLocalizedMessage("NotFound"), 404);
				}

				if (order.OrderStatusId != OrderStatusConstants.Confirmed)
				{
					return ApiResponseBuilder.Error<object>(_localizedMessageService.GetLocalizedMessage("InvalidState"));
				}

				var previousStatus = order.OrderStatusId;
				order.OrderStatusId = OrderStatusConstants.CheckedIn;
				order.SetCreatedAt();
				var orderStatusHistory = new OrderStatusHistory
				{
					OrderId = order.Id,
					OldStatusId = previousStatus,
					NewStatusId = OrderStatusConstants.CheckedIn
				};
				orderStatusHistory.SetCreatedAt();
				_unitOfWork.Entry(order, EntityState.Modified);
				_unitOfWork.Entry(orderStatusHistory, EntityState.Added);

				await _unitOfWork.SaveChangesAsync();

				return ApiResponseBuilder.Success<object>("", _localizedMessageService.GetLocalizedMessage("Updated"));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while checking in order");
				return ApiResponseBuilder.Error<object>("An unexpected error occurred", 500);
			}
		}
	}
}