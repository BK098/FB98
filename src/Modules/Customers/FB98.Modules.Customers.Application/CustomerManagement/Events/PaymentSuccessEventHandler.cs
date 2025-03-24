using FB98.Modules.Customers.Application.Abstractions;
using FB98.Modules.Customers.Domain.Entities;
using FB98.Shared.Abstractions.Events;
using FB98.Shared.Abstractions.StatusConstants;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FB98.Modules.Customers.Application.CustomerManagement.Events
{
	/// <summary>
	///     Xử lí sự kiện cập nhật điểm thưởng khi thanh toán thành công
	/// </summary>
	public sealed class PaymentSuccessEventHandler : IConsumer<PaymentSuccessEvent>
	{
		private const decimal PointRate = 0.001m;
		private readonly ICustomerRepository _customerRepository;
		private readonly ILogger<PaymentSuccessEventHandler> _logger;
		private readonly IMemberShipRepository _memberShipRepository;
		private readonly IUnitOfWork _unitOfWork;

		public PaymentSuccessEventHandler(
			ILogger<PaymentSuccessEventHandler> logger,
			ICustomerRepository customerRepository,
			IUnitOfWork unitOfWork,
			IMemberShipRepository memberShipRepository)
		{
			_logger = logger;
			_customerRepository = customerRepository;
			_unitOfWork = unitOfWork;
			_memberShipRepository = memberShipRepository;
		}

		public async Task Consume(ConsumeContext<PaymentSuccessEvent> context)
		{
			try
			{
				var userId = context.Message.UserId;
				var amount = context.Message.Amount / 100;
				var orderId = context.Message.OrderId;
				var bookingId = context.Message.BookingId;

				var customer = await _customerRepository.GetByIdAsync(userId);

				if (customer == null)
				{
					customer = new Customer(userId, 0, 0, MembershipConstants.Silver);
					_unitOfWork.Entry(customer, EntityState.Added);
					await _unitOfWork.SaveChangesAsync();
					_logger.LogInformation("Created new customer for user: {UserId}", userId);
				}

				var points = CalculatePoints(amount);
				AddPoints(customer, amount, orderId, bookingId);

				_unitOfWork.Entry(customer, EntityState.Modified);

				await _unitOfWork.SaveChangesAsync();

				_logger.LogInformation("Added {Points} points to customer {CustomerId} for {TransactionType}", points, customer.Id, "Add");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while processing payment success event");
				throw;
			}
		}

		public static int CalculatePoints(decimal amount)
		{
			return (int)(amount * PointRate);
		}

		public void AddPoints(Customer customer, decimal amount, Guid? orderId = null, Guid? bookingId = null)
		{
			var points = (int)(amount * PointRate);
			customer.LoyaltyPoints += points;
			customer.TotalSpent += points / PointRate;

			var transaction = new PointTransaction
			{
				CustomerId = customer.Id,
				PointChange = points,
				TransactionType = "add",
				OrderId = orderId,
				BookingId = bookingId
			};
			//customer.PointTransactions.Add(transaction);
			_unitOfWork.Entry(transaction, EntityState.Added);

			UpdateMembership(customer);
		}

		private void UpdateMembership(Customer customer)
		{
			var newMembership = _memberShipRepository
				.GetAll()
				.Where(m => customer.TotalSpent >= m.TotalAmountForUpgrade)
				.OrderByDescending(m => m.TotalAmountForUpgrade)
				.FirstOrDefault();

			// Chỉ cập nhật nếu rank mới cao hơn rank hiện tại
			if (newMembership != null && newMembership.LevelName != MembershipConstants.GetStatusName(customer.MembershipId))
			{
				var oldMembership = MembershipConstants.GetStatusName(customer.MembershipId);
				customer.MembershipId = MembershipConstants.GetStatusId(newMembership.LevelName);
				_unitOfWork.Entry(customer, EntityState.Modified);

				var transaction = new PointTransaction
				{
					CustomerId = customer.Id,
					PointChange = 0,
					TransactionType = $"UPGRADE {oldMembership} TO {newMembership.LevelName}"
				};
				_unitOfWork.Entry(transaction, EntityState.Added);
			}
		}
	}
}