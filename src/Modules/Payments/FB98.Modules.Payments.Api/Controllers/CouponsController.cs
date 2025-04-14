using FB98.Modules.Payments.Application.CouponManagement.Create;
using FB98.Modules.Payments.Application.CouponManagement.Delete;
using FB98.Modules.Payments.Application.CouponManagement.GetAll;
using FB98.Modules.Payments.Application.CouponManagement.GetDetail;
using FB98.Modules.Payments.Application.CouponManagement.Update;
using FB98.Shared.Abstractions.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FB98.Modules.Payments.Api.Controllers
{
	internal class CouponsController : BaseController
	{
		public CouponsController(IMediator mediator) : base(mediator)
		{
		}

		[Authorize(Roles = "Administrator")]
		[HttpPost]
		public async Task<IActionResult> CreateCoupon(CreateCouponDto model)
		{
			var request = new CreateCouponCommand(model);
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
		}

		[HttpGet("{couponId:guid}")]
		public async Task<IActionResult> GetCoupon(Guid couponId)
		{
			var request = new GetDetailCouponQuery(couponId);
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
		}

		[Authorize(Roles = "Administrator")]
		[HttpGet]
		public async Task<IActionResult> GetAllCoupons([FromQuery] Filter filter)
		{
			var result = await _mediator.Send(new GetAllCouponQuery(filter));
			return StatusCode(result.StatusCode, result);
		}

		[Authorize(Roles = "Administrator")]
		[HttpPut("{couponId:guid}")]
		public async Task<IActionResult> UpdateCoupon(Guid couponId, [FromBody] UpdateCouponDto model)
		{
			var result = await _mediator.Send(new UpdateCouponCommand(couponId, model));
			return StatusCode(result.StatusCode, result);
		}

		[Authorize(Roles = "Administrator")]
		[HttpDelete("{couponId:guid}")]
		public async Task<IActionResult> DeleteCoupon(Guid couponId)
		{
			var result = await _mediator.Send(new DeleteCouponCommand(couponId));
			return StatusCode(result.StatusCode, result);
		}
	}
}