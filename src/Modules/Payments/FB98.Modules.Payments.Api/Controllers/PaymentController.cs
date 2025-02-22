using FB98.Modules.Payments.Application.PaymentManagement.CreateCashPayment;
using FB98.Modules.Payments.Application.PaymentManagement.CreateVnPayPayment;
using FB98.Modules.Payments.Application.PaymentManagement.ProcessVNPayReturn;
using Microsoft.AspNetCore.Mvc;

namespace FB98.Modules.Payments.Api.Controllers
{
	internal class PaymentController : BaseController
	{
		public PaymentController(IMediator mediator) : base(mediator)
		{
		}

		[HttpPost("cash")]
		public async Task<IActionResult> CreateCashPayment(CreateCashPaymentDto model)
		{
			var request = new CreateCashPaymentCommand(model);
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
		}

		[HttpPost("vnpay")]
		public async Task<IActionResult> CreateVnPayPayment(CreateVnPayPaymentDto model)
		{
			var request = new CreateVnPayPaymentCommand(model);
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
		}

		[HttpGet("vnpay-return")]
		public async Task<IActionResult> ProcessVnPayReturn()
		{
			var queryParams = new SortedDictionary<string, string>(Request.Query.ToDictionary(q => q.Key, q => q.Value.ToString()));

			var request = new ProcessVnPayReturnCommand(queryParams);
			var result = await _mediator.Send(request);

			return StatusCode(result.StatusCode, result);
		}
	}
}