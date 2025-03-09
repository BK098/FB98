using Microsoft.AspNetCore.Http;

namespace FB98.Modules.Systems.Api.Dtos
{
	public record ReplaceImageDto(IFormFile newImage, string folder, string existingImageUrl);
}