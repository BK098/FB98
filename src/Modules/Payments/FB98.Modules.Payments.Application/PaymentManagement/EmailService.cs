using FB98.Shared.Abstractions.Refits;
using FB98.Shared.Infrastructure.Email;
using System.Text.Json;

namespace FB98.Modules.Payments.Application.PaymentManagement
{
	public static class EmailService
	{
		private static readonly IEmailSender? _emailSender;

		public static async Task SendMailAsync(string email, string phoneNumber, OrderDetailDto? order)
		{
			var emailbody = GetOrderOnlyTemplate(email, phoneNumber, order!);

			var attachments = new List<(byte[], string)>();

			await _emailSender.SendEmailWithMultipleInlineAttachmentsAsync(email, "Xác nhận đơn hàng của bạn", emailbody, attachments);
		}

		private static string GetOrderOnlyTemplate(string email, string phone, OrderDetailDto order)
		{
			var orderData = new
			{
				orderId = order.Id
			};
			var orderJson = JsonSerializer.Serialize(orderData);
			var orderUrl = $"https://api.qrserver.com/v1/create-qr-code/?data={Uri.EscapeDataString(orderJson)}&size=200x200";

			var emailBody = $$"""
			                  <!DOCTYPE html>
			                  <html lang='vi'>
			                  <head>
			                      <meta charset='UTF-8'>
			                      <title>Xác nhận đặt vé xem phim</title>
			                      <style>
			                          body {
			                              font-family: Arial, sans-serif;
			                              background-color: #f4f4f4;
			                              color: #333;
			                          }
			                          .container {
			                              width: 600px;
			                              margin: auto;
			                              background-color: white;
			                              border-radius: 10px;
			                              overflow: hidden;
			                              box-shadow: 0 4px 8px rgba(0,0,0,0.2);
			                          }
			                          .header {
			                              background-color: #007bff;
			                              color: white;
			                              padding: 20px;
			                              text-align: center;
			                          }
			                          .content {
			                              padding: 20px;
			                              line-height: 1.6;
			                              margin-bottom: 20px;
			                          }
			                          .qr-code {
			                              text-align: center;
			                              margin-top: 20px;
			                          }
			                          table {
			                              width: 100%;
			                              border-collapse: collapse;
			                              margin-top: 15px;
			                          }
			                          table, th, td {
			                              border: 1px solid #ddd;
			                          }
			                          th, td {
			                              padding: 8px;
			                              text-align: left;
			                          }
			                          th {
			                              background-color: #f8f8f8;
			                          }
			                          .footer {
			                              background-color: #007bff;
			                              color: white;
			                              text-align: center;
			                              padding: 10px;
			                          }
			                      </style>
			                  </head>
			                  <body>
			                      <div class='container'>
			                          <div class='header'>
			                              <h2>Thông tin vé xem phim 🎬</h2>
			                          </div>
			                          <div class='content'>
			                              <p><strong>Email:</strong> {{email}}</p>
			                              <p><strong>Số điện thoại:</strong> {{phone}}</p>
			                              <p><strong>Tổng tiền thanh toán:</strong> {{order.Amount:N0}} VNĐ</p>
			                  
			                              <table>
			                                  <thead>
			                                      <tr>
			                                          <th>Sản phẩm</th>
			                                          <th>Số lượng</th>
			                                          <th>Tổng tiền</th>
			                                      </tr>
			                                  </thead>
			                                  <tbody>
			                                      {{string.Join("", order.Items.Select(item =>
													  $"<tr><td>{item.ProductName}</td>" +
													  $"<td>{item.Quantity}</td>" +
													  $"<td>{item.TotalPrice:N0} VNĐ</td></tr>"))}}
			                                  </tbody>
			                              </table>
			                              <p>Hãy đưa mã QR này cho nhân viên khi bạn check-in tại rạp:</p>
			                  
			                              <div class='qr-code'>
			                                  <div style='display: flex; justify-content: center; align-items: center;'>
			                                      <div style='text-align: center;'>
			                                          <p>Vé lấy đồ ăn:</p>
			                                          <img src='{{orderUrl}}' alt='QR Code' width='200' height='200' />
			                                      </div>
			                                  </div>
			                              </div>
			                          </div>
			                          <div class='footer'>
			                              <p>Địa chỉ: Tòa nhà xxx, Công Viên Phần Mềm Quang Trung, Tầng 3</p>
			                          </div>
			                      </div>
			                  </body>
			                  </html>
			                  """;
			return emailBody;
		}
	}
}