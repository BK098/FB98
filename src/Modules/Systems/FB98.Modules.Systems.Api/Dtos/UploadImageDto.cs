using Microsoft.AspNetCore.Http;

namespace FB98.Modules.Systems.Api.Dtos
{
	public record UploadImageDto(IFormFile file, string folder);
}