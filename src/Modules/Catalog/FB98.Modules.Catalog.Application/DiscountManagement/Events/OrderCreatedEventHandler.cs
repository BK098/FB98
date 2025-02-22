using FB98.Modules.Catalog.Application.Abstractions;
using FB98.Modules.Catalog.Domain.Entities;
using FB98.Modules.Catalog.Domain.Services;
using FB98.Shared.Abstractions.Events;
using MassTransit;

namespace FB98.Modules.Catalog.Application.DiscountManagement.Events
{
	public class OrderCreatedEventHandler : IConsumer<OrderCreatedEvent>
	{
		private readonly IProductRepository _productRepository;
		private readonly ProductDiscountDomainService _discountService;
		private readonly ILogger<OrderCreatedEventHandler> _logger;
		private readonly IComboRepository _comboRepository;
		private readonly IProductDiscountApplicationRepository _discountRepository;
		private readonly IUnitOfWork _unitOfWork;

		public OrderCreatedEventHandler(
			ProductDiscountDomainService discountService,
			ILogger<OrderCreatedEventHandler> logger,
			IProductRepository productRepository,
			IComboRepository comboRepository,
			IProductDiscountApplicationRepository discountRepository,
			IUnitOfWork unitOfWork)
		{
			_discountService = discountService;
			_logger = logger;
			_productRepository = productRepository;
			_comboRepository = comboRepository;
			_discountRepository = discountRepository;
			_unitOfWork = unitOfWork;
		}

		public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
		{
			var model = context.Message.DiscountItems;
			try
			{
				var productIds = model.Where(x => x.IsCombo == false).Select(item => item.ProductId).ToList();
				var comboIds = model.Where(x => x.IsCombo == true).Select(item => item.ProductId).ToList();

				var products = (await _productRepository.GetByIdsAsync(productIds)).ToDictionary(p => p.Id, BaseProduct (p) => p);
				var combos = (await _comboRepository.GetByIdsAsync(comboIds)).ToDictionary(c => c.Id, BaseProduct (c) => c);

				foreach (var item in model)
				{
					if (!products.TryGetValue(item.ProductId, out var product))
					{
						if (!combos.TryGetValue(item.ProductId, out product))
						{
							_logger.LogWarning($@"Combo or Product {item.ProductId} not found.");
							continue;
						}
					}

					var discountApplication = _discountService.ApplyDiscount(product, context.Message.OrderId);
					if (discountApplication == null)
					{
						continue;
					}

					await _discountRepository.CreateAsync(discountApplication);
					await _unitOfWork.SaveChangesAsync();
				}
			}
			catch (Exception)
			{
				throw;
			}
		}
	}
}