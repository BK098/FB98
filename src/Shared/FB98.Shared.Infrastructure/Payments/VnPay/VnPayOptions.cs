namespace FB98.Shared.Infrastructure.Payments.VnPay
{
	public class VnPayOptions
	{
		public string Vnp_ReturnUrl { get; set; }
		public string Vnp_Url { get; set; }
		public string Vnp_TmnCode { get; set; }
		public string Vnp_HashSecret { get; set; }
		public string Vnp_Version { get; set; }
		public string Vnp_Command { get; set; }
	}
}