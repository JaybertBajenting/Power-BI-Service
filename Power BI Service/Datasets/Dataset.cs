using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Power_BI_Service.Response;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Json;




namespace Power_BI_Service.Datasets
{
    

    public class Dataset
    {

        private readonly ILogger<Dataset> _logger;
        private static readonly HttpClient _httpClient = new HttpClient();
        private static readonly string baseUrl = "https://api.powerbi.com/v1.0/myorg/groups";

        private static readonly string clientId = Environment.GetEnvironmentVariable("FBDEV_AzureClientID");
        private static readonly string tenantId = Environment.GetEnvironmentVariable("FBDEV_AzureTenantID");
        private static readonly string clientSecret = Environment.GetEnvironmentVariable("FBDEV_AzureClientSecret");

        public Dataset(ILogger<Dataset> logger)
        {
            _logger = logger;
        }




        private static async Task<string> GetAccessTokenAsync()
        {
            string tokenUrl = $"https://login.microsoftonline.com/{tenantId}/oauth2/v2.0/token";
            var data = new Dictionary<string, string>
            {
                { "grant_type", "client_credentials" },
                { "scope", "https://analysis.windows.net/powerbi/api/.default" },
                { "client_id", clientId },
                { "client_secret", clientSecret }
            };

            using var content = new FormUrlEncodedContent(data);
            HttpResponseMessage response = await _httpClient.PostAsync(tokenUrl, content);
            string responseJson = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to retrieve token: {responseJson}");
            }

            var tokenObj = JsonSerializer.Deserialize<TokenResponse>(responseJson);
            return tokenObj?.AccessToken ?? throw new Exception("Access token not found in response.");
        }



        [Function("GetAllDatasets")]
        public async Task<HttpResponseData> GetAllDatasets(
    [HttpTrigger(AuthorizationLevel.Function, "get", Route = "workspace/{workspaceId}/datasets")] HttpRequestData req,
    string workspaceId)
        {
            _logger.LogInformation($"Retrieving all datasets in workspace: {workspaceId}");

            string accessToken;
            try
            {
                accessToken = await GetAccessTokenAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting access token: {ex.Message}");
                var errorResponse = req.CreateResponse(System.Net.HttpStatusCode.InternalServerError);
                await errorResponse.WriteStringAsync($"Error retrieving access token: {ex.Message}");
                return errorResponse;
            }

            try
            {
                string datasetsUrl = $"{baseUrl}/{workspaceId}/datasets";
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    HttpResponseMessage response = await client.GetAsync(datasetsUrl);
                    string responseJson = await response.Content.ReadAsStringAsync();

                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception($"Error retrieving datasets: {responseJson}");
                    }

                    var successResponse = req.CreateResponse(System.Net.HttpStatusCode.OK);
                    await successResponse.WriteStringAsync(responseJson);
                    return successResponse;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving datasets: {ex.Message}");
                var errorResponse = req.CreateResponse(System.Net.HttpStatusCode.InternalServerError);
                await errorResponse.WriteStringAsync($"Error retrieving datasets: {ex.Message}");
                return errorResponse;
            }
        }

    }
}
