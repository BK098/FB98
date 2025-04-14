using FB98.Shared.Abstractions.Entities;
using System.ComponentModel.DataAnnotations;

namespace FB98.Modules.Payments.Domain.Entities
{
	public class Coupon : BaseEntity
	{
		[StringLength(50)]
		public string Code { get; set; } = null!;
		public decimal Value { get; set; }
		public string? Description { get; set; }
		public decimal? MaxDiscountAmount { get; set; }
		public decimal MinPaymentAmount { get; set; } = 0;
		public int SoftUsageCount { get; set; } = 0;
		public DateTime EndDate { get; set; }
		public DateTime StartDate { get; set; }
		public int MaxUsage { get; set; } = 0;
		public int UsageCount { get; set; } = 0;
		public bool IsLimited { get; set; } = false;
		public bool IsDiscountPercentage { get; set; }
		public bool IsActive { get; set; } = false;
		[Timestamp]
		public byte[] RowVersion { get; set; } = null!;

		public decimal CalculateDiscount(decimal orderAmount)
		{
			if (orderAmount < MinPaymentAmount)
			{
				return 0;
			}

			if (IsDiscountPercentage)
			{
				var rawDiscount = orderAmount * (Value / 100);
				return MaxDiscountAmount.HasValue ? Math.Min(rawDiscount, MaxDiscountAmount.Value) : rawDiscount;
			}

			return Value;
		}
	}
}