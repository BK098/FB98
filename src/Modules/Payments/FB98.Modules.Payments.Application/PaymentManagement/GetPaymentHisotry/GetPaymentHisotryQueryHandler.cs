using AutoMapper;
using FB98.Modules.Payments.Application.Abstractions;
using FB98.Modules.Payments.Domain.Entities;
using FB98.Shared.Infrastructure.Paging;
using FB98.Shared.Utils.Extensions;
using Microsoft.EntityFrameworkCore;

namespace FB98.Modules.Payments.Application.PaymentManagement.GetPaymentHisotry
{
	internal sealed class GetPaymentHisotryQueryHandler : IQueryHandler<GetPaymentHisotryQuery, ApiResult<PaginatedResult<GetPaymentHisotryResponse>>>
	{
		private readonly List<string> _allowedProperties = ["CreateAt"];
		private readonly ILocalizedMessageService _localizedMessageService;
		private readonly ILogger<GetPaymentHisotryQueryHandler> _logger;
		private readonly IMapper _mapper;
		private readonly IPaymentRepository _paymentRepository;

		public GetPaymentHisotryQueryHandler(
			IPaymentRepository paymentRepository,
			ILocalizedMessageService localizedMessageService,
			ILogger<GetPaymentHisotryQueryHandler> logger,
			IMapper mapper)
		{
			_paymentRepository = paymentRepository;
			_localizedMessageService = localizedMessageService;
			_logger = logger;
			_mapper = mapper;
		}

		public async Task<ApiResult<PaginatedResult<GetPaymentHisotryResponse>>> Handle(GetPaymentHisotryQuery request, CancellationToken cancellationToken)
		{
			var filter = request.Filter;
			var userId = request.UserId;
			try
			{
				var payments = _paymentRepository.GetAllPaymentByUser(userId);

				if (!await payments.AnyAsync(cancellationToken))
				{
					return ApiResponseBuilder.Error<PaginatedResult<GetPaymentHisotryResponse>>(_localizedMessageService.GetLocalizedMessage("NotFound"), 404);
				}

				payments = payments.SortBy(filter.SortColumn, _allowedProperties, filter.IsDescending);

				var paginatedResult = await PaginatedResult<PaymentTransaction>.CreateAsync(
					payments,
					filter.PageIndex,
					filter.PageSize,
					cancellationToken);

				var response = new PaginatedResult<GetPaymentHisotryResponse>(
					_mapper.Map<List<GetPaymentHisotryResponse>>(paginatedResult.Items),
					paginatedResult.PageIndex,
					paginatedResult.PageSize,
					paginatedResult.TotalCount);

				return ApiResponseBuilder.Success(response, _localizedMessageService.GetLocalizedMessage("DataRetrieved"));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while get all payment by user");
				return ApiResponseBuilder.Error<PaginatedResult<GetPaymentHisotryResponse>>("An unexpected error occurred", 500);
			}
		}
	}
}