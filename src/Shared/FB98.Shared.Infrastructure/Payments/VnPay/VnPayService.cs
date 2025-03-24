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

		public string GeneratePaymentUrl(Guid paymentId, decimal amount, string ipAddress)
		{
			var vnpayData = PrepareVnPayData(paymentId, amount, ipAddress);
			var paymentUrl = CreateRequestUrl(vnpayData);
			_logger.LogInformation($"VNPay URL generated: {paymentUrl}");
			return paymentUrl;
		}

		public bool ValidateVnPayResponse(SortedDictionary<string, string> queryParams, decimal expectedAmount, string expectedTxnRef)
		{
			expectedAmount *= 100;
			if (!queryParams.ContainsKey("vnp_SecureHash"))
			{
				return false;
			}

			var secureHash = queryParams["vnp_SecureHash"];
			queryParams.Remove("vnp_SecureHash");

			// Tính toán lại hash từ các tham số
			var calculatedHash = GenerateSecureHash(CreateRequestUrl(queryParams));
			if (secureHash.Equals(calculatedHash, StringComparison.OrdinalIgnoreCase))
			{
				return false;
			}

			// Kiểm tra các tham số quan trọng
			if (!queryParams.TryGetValue("vnp_Amount", out var amountStr) ||
				!queryParams.TryGetValue("vnp_TxnRef", out var txnRef) ||
				!queryParams.TryGetValue("vnp_ResponseCode", out var responseCode))
			{
				return false;
			}

			// Kiểm tra số tiền và mã giao dịch
			if (!decimal.TryParse(amountStr, out var amount) || amount != expectedAmount)
			{
				return false;
			}

			if (txnRef != expectedTxnRef || responseCode != "00")
			{
				return false;
			}

			return true;
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
		private SortedDictionary<string, string> PrepareVnPayData(Guid paymentId, decimal amount, string ipAddress)
		{
			return new SortedDictionary<string, string>
			{
				{ "vnp_Version", _options.Vnp_Version },
				{ "vnp_Command", _options.Vnp_Command },
				{ "vnp_TmnCode", _options.Vnp_TmnCode },
				{ "vnp_Amount", ((int)(amount * 100)).ToString() }, // VNPay yêu cầu nhân 100
				{ "vnp_BankCode", "VNBANK" },
				{ "vnp_CreateDate", DateTime.UtcNow.AddHours(7).ToString("yyyyMMddHHmmss") },
				{ "vnp_CurrCode", "VND" },
				{ "vnp_IpAddr", ipAddress },
				{ "vnp_Locale", "vn" },
				{ "vnp_OrderInfo", $"Thanh toan don hang {paymentId}" },
				{ "vnp_OrderType", "other" },
				{ "vnp_ExpireDate", DateTime.UtcNow.AddHours(7).AddMinutes(15).ToString("yyyyMMddHHmmss") },
				{ "vnp_ReturnUrl", _options.Vnp_ReturnUrl },
				{ "vnp_TxnRef", $"{paymentId}"}
			};
		}
	}
}