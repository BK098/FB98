using Swashbuckle.AspNetCore.Annotations;

namespace FB98.Modules.Catalog.Application.DiscountManagement.UpdateDiscountRule
{
	public class UpdateDiscountRuleDto
	{
		/// <summary>
		///     default is Product
		/// </summary>
		[SwaggerSchema(ReadOnly = true, WriteOnly = true)]
		public bool? IsCombo { get; private set; } = false;
		public string? Name { get; set; }
		public string? Description { get; set; }
		public decimal? Value { get; set; }
		public bool? IsDiscountPercentage { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }

		public void SetAtCombo()
		{
			IsCombo = true;
		}
	}
}
