using Microsoft.Extensions.Logging;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace FB98.Shared.Infrastructure.Payments.VnPay
{
	public class VnPayService : IVnPayService
	{
		private readonly ILogger<VnPayService> _logger;
		private readonly VnPayOptions _options;

		public VnPayService(
			VnPayOptions options,
			ILogger<VnPayService> logger)
		{
			_options = options;
			_logger = logger;
		}

		public string GeneratePaymentUrl(Guid? orderId, Guid? bookingId, decimal amount, string ipAddress)
		{
			var vnpayData = new SortedDictionary<string, string>
			{
				{ "vnp_Version", _options.Vnp_Version },
				//{ "vnp_Command", _options.Vnp_Command },
				{ "vnp_Command", "querydr" },
				{ "vnp_TmnCode", _options.Vnp_TmnCode },
				{ "vnp_Amount", ((int)(amount * 100)).ToString() }, // VNPay yêu cầu nhân 100
				{ "vnp_CreateDate", DateTime.UtcNow.AddHours(7).ToString("yyyyMMddHHmmss") },
				{ "vnp_CurrCode", "VND" },
				{ "vnp_IpAddr", ipAddress },
				{ "vnp_Locale", "vn" },
				{ "vnp_OrderInfo", $"Thanh toan don hang {orderId}" },
				{ "vnp_OrderType", "other" },
				{ "vnp_BankCode", "VNBANK" },
				{ "vnp_ReturnUrl", _options.Vnp_ReturnUrl },
				{ "vnp_ExpireDate", DateTime.UtcNow.AddHours(7).AddMinutes(15).ToString("yyyyMMddHHmmss") },
				{ "vnp_TxnRef", orderId.ToString() }
			};

			var paymentUrl = CreateRequestUrl(vnpayData);
			_logger.LogInformation($"VNPay URL generated: {paymentUrl}");
			return paymentUrl;
		}

		public bool ValidateVnPayResponse(SortedDictionary<string, string> queryParams)
		{
			if (!queryParams.ContainsKey("vnp_SecureHash"))
			{
				return false;
			}

			var secureHash = queryParams["vnp_SecureHash"];
			queryParams.Remove("vnp_SecureHash");

			var calculatedHash = CreateRequestUrl(queryParams);
			return secureHash.Equals(calculatedHash, StringComparison.OrdinalIgnoreCase);
		}

		private string CreateRequestUrl(SortedDictionary<string, string> requestData)
		{
			var data = new StringBuilder();
			foreach (var kv in requestData.Where(kv => !string.IsNullOrEmpty(kv.Value)))
			{
				data.Append(WebUtility.UrlEncode(kv.Key) + "=" + WebUtility.UrlEncode(kv.Value) + "&");
			}

			var queryString = data.ToString();
			var baseUrl = _options.Vnp_Url;
			baseUrl += "?" + queryString;
			var signData = queryString;
			if (signData.Length > 0)
			{
				signData = signData.Remove(data.Length - 1, 1);
			}

			var vnp_SecureHash = GenerateSecureHash(signData);
			baseUrl += "vnp_SecureHash=" + vnp_SecureHash;

			return baseUrl;
		}

		private string GenerateSecureHash(string inputData)
		{
			var hash = new StringBuilder();
			var keyBytes = Encoding.UTF8.GetBytes(_options.Vnp_HashSecret);
			var inputBytes = Encoding.UTF8.GetBytes(inputData);
			using (var hmac = new HMACSHA512(keyBytes))
			{
				var hashValue = hmac.ComputeHash(inputBytes);
				foreach (var theByte in hashValue)
				{
					hash.Append(theByte.ToString("x2"));
				}
			}

			return hash.ToString();
		}
	}
}