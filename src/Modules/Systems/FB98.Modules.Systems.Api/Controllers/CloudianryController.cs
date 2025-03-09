using FB98.Modules.Systems.Api.Dtos;
using FB98.Shared.Abstractions.Responses;
using FB98.Shared.Infrastructure.Cloudinaries;
using Microsoft.AspNetCore.Mvc;

namespace FB98.Modules.Systems.Api.Controllers
{
	internal class CloudinaryController : BaseController
	{
		private readonly ICloudinaryService _cloudinaryService;

		public CloudinaryController(ICloudinaryService cloudinaryService)
		{
			_cloudinaryService = cloudinaryService;
		}

		[HttpPost("upload")]
		public async Task<IActionResult> UploadImage(UploadImageDto model)
		{
			if (model.file == null || model.file.Length == 0)
			{
				return StatusCode(400, ApiResponseBuilder.Error<string>("File không hợp lệ."));
			}

			var imageUrl = await _cloudinaryService.UploadImageAsync(model.file, model.folder);

			if (imageUrl == null)
			{
				return StatusCode(400, ApiResponseBuilder.Error<string>("Không thể upload hình ảnh."));
			}

			return StatusCode(200, ApiResponseBuilder.Success(imageUrl, "Upload hình ảnh thành công."));
		}

		[HttpPut("replace")]
		public async Task<IActionResult> ReplaceImage(ReplaceImageDto model)
		{
			var imageUrl = await _cloudinaryService.ReplaceImageAsync(model.newImage, model.folder, model.existingImageUrl);

			if (imageUrl == null)
			{
				return StatusCode(400, ApiResponseBuilder.Error<string>("Không thể thay thế hình ảnh."));
			}

			return StatusCode(200, ApiResponseBuilder.Success(imageUrl, "Thay thế hình ảnh thành công."));
		}

		[HttpDelete("delete")]
		public IActionResult DeleteImage([FromQuery] string imageUrl)
		{
			var success = _cloudinaryService.DeleteImage(imageUrl);

			if (!success)
			{
				return StatusCode(400, ApiResponseBuilder.Error<string>("Không thể xóa hình ảnh."));
			}

			return StatusCode(200, ApiResponseBuilder.Success("", "Hình ảnh đã được xóa thành công."));
		}
	}
}