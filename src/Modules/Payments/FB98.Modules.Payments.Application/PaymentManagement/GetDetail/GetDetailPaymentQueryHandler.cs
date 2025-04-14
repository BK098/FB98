using AutoMapper;
using FB98.Modules.Payments.Application.Abstractions;
using FB98.Shared.Abstractions.Refits;
using Refit;

namespace FB98.Modules.Payments.Application.PaymentManagement.GetDetail
{
	internal sealed class GetDetailPaymentQueryHandler : IQueryHandler<GetDetailPaymentQuery, ApiResult<GetDetailPaymentResponse>>
	{
		private readonly IBookingApi _bookingApi;
		private readonly ILocalizedMessageService _localizedMessageService;
		private readonly ILogger<GetDetailPaymentQueryHandler> _logger;
		private readonly IMapper _mapper;
		private readonly IOrderApi _orderApi;
		private readonly IPaymentRepository _paymentRepository;

		public GetDetailPaymentQueryHandler(
			IBookingApi bookingApi,
			ILocalizedMessageService localizedMessageService,
			ILogger<GetDetailPaymentQueryHandler> logger,
			IMapper mapper,
			IOrderApi orderApi,
			IPaymentRepository paymentRepository)
		{
			_bookingApi = bookingApi;
			_localizedMessageService = localizedMessageService;
			_logger = logger;
			_mapper = mapper;
			_orderApi = orderApi;
			_paymentRepository = paymentRepository;
		}

		public async Task<ApiResult<GetDetailPaymentResponse>> Handle(GetDetailPaymentQuery request, CancellationToken cancellationToken)
		{
			try
			{
				var payment = await _paymentRepository.GetByIdAsync(request.PaymentId);
				if (payment == null)
				{
					return ApiResponseBuilder.Error<GetDetailPaymentResponse>("Payment " + _localizedMessageService.GetLocalizedMessage("NotFound"), 404);
				}

				var response = new GetDetailPaymentResponse
				{
					UserId = payment.UserId,
					Amount = payment.Amount,
					PaymentMethodId = payment.PaymentMethodId,
					PaymentMethodName = payment.PaymentMethod!.Name,
					PaymentStatusId = payment.PaymentStatusId,
					PaymentStatusName = payment.PaymentStatus!.Name,
					CreateAt = payment.CreateAt
				};

				if (payment.BookingId != null)
				{
					response.BookingId = payment.BookingId;
					try
					{
						var bookingResult = await _bookingApi.GetDetailBooking(payment.BookingId.Value);
						if (bookingResult.IsSuccess)
						{
							response.Tickets = new List<GetDeteailBookingPaymentResponse>
							{
								_mapper.Map<GetDeteailBookingPaymentResponse>(bookingResult.Data)
							};
						}
					}
					catch (ApiException)
					{
						return ApiResponseBuilder.Error<GetDetailPaymentResponse>("Booking " + _localizedMessageService.GetLocalizedMessage("NotFound"), 404);
					}
				}

				if (payment.OrderId != null)
				{
					response.OrderId = payment.OrderId;
					try
					{
						var orderResult = await _orderApi.GetOrderDetailById(payment.OrderId.Value);
						if (orderResult.IsSuccess)
						{
							response.Orders = new List<GetDeteailOrderPaymentResponse>
							{
								_mapper.Map<GetDeteailOrderPaymentResponse>(orderResult.Data)
							};
						}
					}
					catch (ApiException)
					{
						return ApiResponseBuilder.Error<GetDetailPaymentResponse>("Order " + _localizedMessageService.GetLocalizedMessage("NotFound"), 404);
					}
				}

				return ApiResponseBuilder.Success(response);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while get payment detail");
				return ApiResponseBuilder.Error<GetDetailPaymentResponse>("An unexpected error occurred", 500);
			}
		}
	}
}