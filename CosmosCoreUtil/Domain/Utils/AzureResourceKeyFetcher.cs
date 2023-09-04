namespace CosmosCoreUtil.Domain.Utils
{
    using Azure.Core;
    using Azure.Identity;
    using Newtonsoft.Json;
    using System.Net.Http.Headers;

    internal class DatabaseAccountListKeysResult
    {
        public string PrimaryMasterKey { get; set; } = string.Empty;
        public string PrimaryReadonlyMasterKey { get; set; } = string.Empty;
        public string SecondaryMasterKey { get; set; } = string.Empty;
        public string SecondaryReadonlyMasterKey { get; set; } = string.Empty;
    }


    internal class AzureResourceKeyFetcher
    {
        public const string AzureResourceManagerScope = "https://management.azure.com";
        private const string RequestParameterListKeys = "/listKeys?api-version={0}";
        private const string DefaultCosmosListKeysVersion = "2019-12-12";
        private const string DefaultStorageListKeysVersion = "2023-01-01";

        private const string Scheme = "Bearer";




        public static async Task<string> GetCosmosDbPrimaryMasterKeyAsync(string cosmosDbResourceId, string? listKeyVersion = null)
        {
            var endpoint = GetKeysRequestUri(
                cosmosDbResourceId,
                string.Format(AzureResourceKeyFetcher.RequestParameterListKeys,
                    listKeyVersion ?? AzureResourceKeyFetcher.DefaultCosmosListKeysVersion));

            string payload = await GetPayload(endpoint, "Cosmos Primary Key");

            var databaseAccountListKeyResult =
                JsonConvert.DeserializeObject<DatabaseAccountListKeysResult>(payload);

            return databaseAccountListKeyResult != null ? databaseAccountListKeyResult.PrimaryMasterKey : string.Empty;
        }

        private static string GetKeysRequestUri(string resourceId, string version)
        {
            var slashDelimiter = resourceId.StartsWith("/", StringComparison.Ordinal) ? string.Empty : "/";
            return $"{AzureResourceKeyFetcher.AzureResourceManagerScope}{slashDelimiter}{resourceId}{version}";
        }

        private static async Task<string> GetPayload(string endpoint, string keyType)
        {
            string accessToken = string.Empty;
            try
            {
                accessToken = GetAccessToken();
            }
            catch (Exception ex)
            {
                throw;
            }

            string payload = string.Empty;
            try
            {
                payload = await GetRequestBodyAsString(endpoint, accessToken);
            }
            catch (Exception ex)
            {
                throw;
            }

            return payload;
        }

        private static string GetAccessToken()
        {
            var azureServiceTokenProvider = new DefaultAzureCredential();

            TokenRequestContext context = new TokenRequestContext(new string[] { AzureResourceKeyFetcher.AzureResourceManagerScope });
            AccessToken token = azureServiceTokenProvider.GetToken(context);

            return token.Token;
        }

        private static async Task<string> GetRequestBodyAsString(string endpoint, string accessToken)
        {
            using (var httpClient = new HttpClient())
            using (var postContent = new StringContent(string.Empty))
            using (var request = new HttpRequestMessage(HttpMethod.Post, new Uri(endpoint)))
            {
                request.Content = postContent;
                request.Headers.Authorization = new AuthenticationHeaderValue(Scheme, accessToken);

                var result = await httpClient.SendAsync(request);

                return await result.Content.ReadAsStringAsync();
            }
        }
    }
}
