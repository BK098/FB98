using FB98.Shared.Abstractions.Entities;

namespace FB98.Shared.Abstractions.Responses
{
	public class ApiResult<T> : IResponse
	{
		public bool IsSuccess { get; set; }
		public string? Message { get; set; }
		public T? Data { get; set; }
		public Dictionary<string, List<object>>? Errors { get; set; }
		public int StatusCode { get; set; }
		public DateTime Timestamp { get; set; } = DateTime.UtcNow;
	}
}