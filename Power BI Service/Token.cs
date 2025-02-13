using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
namespace Power_BI_Service
{
    public class Token
    {

       
         
        private readonly ILogger<Token> _logger;
        private static readonly HttpClient _httpClient = new HttpClient();
    
        // Global Environment Variables
        //private static readonly string clientId = Environment.GetEnvironmentVariable("FBDEV_AzureClientID");
        //private static readonly string tenantId = Environment.GetEnvironmentVariable("FBDEV_AzureTenantID");
        //private static readonly string clientSecret = Environment.GetEnvironmentVariable("FBDEV_AzureClientSecret");

        public string ClientId { get; }
        public string TenantId { get; }
        public string ClientSecret { get; }

        public string TokenUrl { get; set; }


        public string BaseUrl { get; set; }

        public Token(ILogger<Token> logger)
        {
            ClientId = Environment.GetEnvironmentVariable("FBDEV_AzureClientID") ?? throw new ArgumentNullException(nameof(ClientId));
            TenantId = Environment.GetEnvironmentVariable("FBDEV_AzureTenantID") ?? throw new ArgumentNullException(nameof(TenantId));
            ClientSecret = Environment.GetEnvironmentVariable("FBDEV_AzureClientSecret") ?? throw new ArgumentNullException(nameof(ClientSecret));
            TokenUrl = "https://login.microsoftonline.com/{TENANT_ID}/oauth2/v2.0/token";
            BaseUrl = "https://api.powerbi.com/v1.0/myorg/groups";
            _logger = logger;
        }
    
        


        [Function("GetAccessToken")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "auth/token")] HttpRequestData req)
        {
            _logger.LogInformation("Retrieving Power BI access token...");

            if (string.IsNullOrEmpty(ClientId) || string.IsNullOrEmpty(TenantId) || string.IsNullOrEmpty(ClientSecret))
            {
                _logger.LogError("Missing environment variables.");
                var errorResponse = req.CreateResponse(System.Net.HttpStatusCode.InternalServerError);
                await errorResponse.WriteStringAsync("Environment variables for authentication are missing.");
                return errorResponse;
            }

            try
            {
                string accessToken = await GetAccessTokenAsync();
                var response = req.CreateResponse(System.Net.HttpStatusCode.OK);
                response.Headers.Add("Content-Type", "application/json; charset=utf-8");
                await response.WriteStringAsync(JsonSerializer.Serialize(new { AccessToken = accessToken }));

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to retrieve access token: {ex.Message}");
                var errorResponse = req.CreateResponse(System.Net.HttpStatusCode.InternalServerError);
                await errorResponse.WriteStringAsync($"Error: {ex.Message}");
                return errorResponse;
            }
        }

       
        public  async Task<string> GetAccessTokenAsync()
        {
            string formattedTokenUrl = TokenUrl.Replace("{TENANT_ID}", TenantId);

            var data = new Dictionary<string, string>
            {
                { "grant_type", "client_credentials" },
                { "scope", "https://analysis.windows.net/powerbi/api/.default" },
                { "client_id", ClientId },
                { "client_secret", ClientSecret }
            };

            using var content = new FormUrlEncodedContent(data);
            HttpResponseMessage response = await _httpClient.PostAsync(formattedTokenUrl, content);
            string responseJson = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to retrieve token: {responseJson}");
            }

            var tokenObj = JsonSerializer.Deserialize<Response.TokenResponse>(responseJson);
            return tokenObj?.AccessToken ?? throw new Exception("Access token not found in response.");
        }
    }

   
}
