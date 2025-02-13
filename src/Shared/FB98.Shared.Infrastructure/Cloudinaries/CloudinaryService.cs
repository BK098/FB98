using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;

namespace FB98.Shared.Infrastructure.Cloudinaries
{
	public class CloudinaryService : ICloudinaryService
	{
		private readonly Cloudinary _cloudinary;

		public CloudinaryService(Cloudinary cloudinary)
		{
			_cloudinary = cloudinary;
		}

		public async Task<string?> UploadImageAsync(IFormFile file, string folderStorage)
		{
			if (file.Length <= 0)
			{
				return null;
			}

			await using var stream = file.OpenReadStream();
			var uploadParams = new ImageUploadParams
			{
				File = new FileDescription(file.FileName, stream),
				Folder = folderStorage
			};

			var uploadResult = await _cloudinary.UploadAsync(uploadParams);
			return uploadResult.SecureUrl.AbsoluteUri;
		}

		public async Task<string?> ReplaceImageAsync(IFormFile? newImage, string folderStorage, string? existingImageUrl = null)
		{
			if (newImage is null || newImage.Length <= 0)
			{
				Console.WriteLine("[Cloudinary] No new image provided. Keeping existing image.");
				return existingImageUrl; // Không có ảnh mới, giữ nguyên ảnh cũ
			}

			await using var stream = newImage.OpenReadStream();
			var uploadParams = new ImageUploadParams()
			{
				File = new FileDescription(newImage.FileName, stream),
				PublicId = GetPublicIdFromUrl(existingImageUrl!),
				Invalidate = true,
				Folder = folderStorage
			};
			var uploadResult = _cloudinary.Upload(uploadParams);

			return uploadResult.SecureUrl.AbsoluteUri;
		}

		private static string GetPublicIdFromUrl(string imageUrl)
		{
			var uri = new Uri(imageUrl);
			var segments = uri.Segments;
			var fileName = segments[^1].Split('.')[0];
			return fileName;
		}
		private static string GetPublicIdFromUrl2(string imageUrl)
		{
			var uri = new Uri(imageUrl);
			var path = uri.AbsolutePath.Substring(1);
			var publicId = path.Substring(0, path.LastIndexOf('.'));
			return publicId;
		}

		public bool DeleteImage(string? imageUrl)
		{
			if (string.IsNullOrWhiteSpace(imageUrl)) return false;

			try
			{
				var publicId = GetPublicIdFromUrl2(imageUrl);
				if (string.IsNullOrWhiteSpace(publicId))
				{
					Console.WriteLine("[Cloudinary] Invalid public ID extracted. Skipping deletion.");
					return false;
				}

				var deletionParams = new DeletionParams(publicId)
				{
					Invalidate = true,
					Type = "upload",
					ResourceType = ResourceType.Image
				};
				var resource = _cloudinary.GetResource(new GetResourceParams(publicId));
				if (resource == null)
				{
					Console.WriteLine($"[Cloudinary] Image not found before deletion: {publicId}");
					return false;
				}

				var result = _cloudinary.Destroy(deletionParams);
				return result.Result == "ok";
			}
			catch (Exception ex)
			{
				Console.WriteLine($"[Cloudinary] Error deleting image: {ex.Message}");
				return false;
			}
		}
	}
}
