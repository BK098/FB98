using FB98.Modules.Payments.Application.PaymentManagement.CreateCashPayment;
using FB98.Modules.Payments.Application.PaymentManagement.CreateVnPayPayment;
using FB98.Modules.Payments.Application.PaymentManagement.GetDetail;
using FB98.Modules.Payments.Application.PaymentManagement.GetPaymentHisotry;
using FB98.Modules.Payments.Application.PaymentManagement.ProcessVNPayReturn;
using FB98.Shared.Abstractions.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;

namespace FB98.Modules.Payments.Api.Controllers
{
	internal class PaymentController : BaseController
	{
		private readonly string? _frontEnd;

		public PaymentController(
			IMediator mediator,
			IConfiguration configuration) : base(mediator)
		{
			_frontEnd = configuration["FrontendBaseUrl"];
		}

		[Authorize(Roles = "Administrator")]
		[HttpPost("pay-cash")]
		public async Task<IActionResult> CreateCashPayment(CreateCashPaymentDto model)
		{
			var request = new CreateCashPaymentCommand(model);
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
		}

		[Authorize]
		[HttpPost("pay-vnpay")]
		public async Task<IActionResult> CreateVnPayPayment(Guid? userId, CreateVnPayPaymentDto model)
		{
			if (userId == null)
			{
				var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
				var phoneNumberClaim = User.FindFirst(ClaimTypes.MobilePhone);
				var emailClaim = User.FindFirst(ClaimTypes.Email);
				if (userIdClaim == null)
				{
					return Unauthorized();
				}

				model.UserId = Guid.Parse(userIdClaim.Value);
				model.PhoneNumber = phoneNumberClaim?.Value;
				model.Email = emailClaim?.Value;
			}
			else
			{
				model.UserId = userId;
			}

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
			if (!result.IsSuccess)
			{
				return Redirect($"{_frontEnd}/payment-error");
			}
			return Redirect($"{_frontEnd}/payment-success?paymentId={queryParams["vnp_TxnRef"]}");
		}

		[Authorize]
		[HttpGet]
		public async Task<IActionResult> GetAllPaymentByUserId([FromQuery] Guid? userId, [FromQuery] Filter filter)
		{
			var currentUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

			if (userId == null || userId == currentUserId)
			{
				userId = currentUserId;
			}
			else if (!User.IsInRole("Administrator"))
			{
				return Forbid();
			}

			var request = new GetPaymentHisotryQuery(userId!.Value, filter);
			var result = await _mediator.Send(request);

			return StatusCode(result.StatusCode, result);
		}

		[Authorize]
		[HttpGet("{paymentId:guid}")]
		public async Task<IActionResult> PaymentHistoryByUserId(Guid paymentId)
		{
			var request = new GetDetailPaymentQuery(paymentId);
			var result = await _mediator.Send(request);

			return StatusCode(result.StatusCode, result);
		}
	}
}