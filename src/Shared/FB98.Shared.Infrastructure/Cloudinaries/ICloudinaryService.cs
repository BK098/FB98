using Microsoft.AspNetCore.Http;

namespace FB98.Shared.Infrastructure.Cloudinaries
{
	public interface ICloudinaryService
	{
		Task<string?> UploadImageAsync(IFormFile file, string folderStorage);
		Task<string?> ReplaceImageAsync(IFormFile? newImage, string folderStorage, string? existingImageUrl = null);
		bool DeleteImage(string? imageUrl);
	}
}