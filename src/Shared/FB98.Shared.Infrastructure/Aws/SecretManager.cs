using Amazon;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;

namespace FB98.Shared.Infrastructure.Aws
{
	public static class SecretManager
	{
		public static async Task<string> GetSecretAsync()
		{
			string secretName = "pgDatabase";
			string region = "us-east-1";

			IAmazonSecretsManager client = new AmazonSecretsManagerClient(RegionEndpoint.GetBySystemName(region));
			GetSecretValueRequest request = new GetSecretValueRequest
			{
				SecretId = secretName,
				VersionStage = "AWSCURRENT", // VersionStage defaults to AWSCURRENT if unspecified.
			};

			GetSecretValueResponse response;
			try
			{
				response = await client.GetSecretValueAsync(request);
			}
			catch (Exception ex)
			{
				// For a list of the exceptions thrown, see
				// https://docs.aws.amazon.com/secretsmanager/latest/apireference/API_GetSecretValue.html
				throw ex;
			}
			string secret = response.SecretString;
			return secret;
		}
	}
}
