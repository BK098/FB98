using AutoMapper;
using FB98.Modules.Customers.Application.Abstractions;
using FB98.Shared.Abstractions.CQRS;
using FB98.Shared.Abstractions.Responses;
using FB98.Shared.Infrastructure.Localization;
using Microsoft.Extensions.Logging;

namespace FB98.Modules.Customers.Application.CustomerManagement.GetDetail
{
	public class GetDetailCustomerQueryHandler : IQueryHandler<GetDetailCustomerQuery, ApiResult<GetDetailCustomerResponse>>
	{
		private readonly ICustomerRepository _customerRepository;
		private readonly ILocalizedMessageService _localizedMessageService;
		private readonly ILogger<GetDetailCustomerQueryHandler> _logger;
		private readonly IMapper _mapper;

		public GetDetailCustomerQueryHandler(
			ICustomerRepository customerRepository,
			ILogger<GetDetailCustomerQueryHandler> logger,
			ILocalizedMessageService localizedMessageService, IMapper mapper)
		{
			_customerRepository = customerRepository;
			_logger = logger;
			_localizedMessageService = localizedMessageService;
			_mapper = mapper;
		}

		public async Task<ApiResult<GetDetailCustomerResponse>> Handle(GetDetailCustomerQuery request, CancellationToken cancellationToken)
		{
			var userId = request.UserId;
			try
			{
				var customer = await _customerRepository.GetByIdAsync(userId);
				if (customer == null)
				{
					return ApiResponseBuilder.Error<GetDetailCustomerResponse>(_localizedMessageService.GetLocalizedMessage("NotFound"), 404);
				}

				var response = _mapper.Map<GetDetailCustomerResponse>(customer);

				return ApiResponseBuilder.Success(response);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while get user");
				return ApiResponseBuilder.Error<GetDetailCustomerResponse>("An unexpected error occurred", 500);
			}
		}
	}
}