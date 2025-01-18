namespace FB98.Shared.Infrastructure.Email
{
	public class OAuthTokenResponse
	{
		[System.Text.Json.Serialization.JsonPropertyName("access_token")]
		public string AccessToken { get; set; } = default!;

		[System.Text.Json.Serialization.JsonPropertyName("token_type")]
		public string TokenType { get; set; } = default!;

		[System.Text.Json.Serialization.JsonPropertyName("expires_in")]
		public int ExpiresIn { get; set; }
	}
}
