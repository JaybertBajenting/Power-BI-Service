//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net.Http.Headers;
//using System.Text;
//using System.Text.Json;
//using System.Threading.Tasks;
//using Microsoft.Azure.Functions.Worker.Http;
//using Microsoft.Azure.Functions.Worker;
//using Microsoft.Extensions.Logging;
//using Power_BI_Service.Response;
//using Power_BI_Service.Workspaces;
//using Power_BI_Service.Request;



//namespace Power_BI_Service.Reports
//{



//    public class Reports
//    {

//        private readonly ILogger<Reports> _logger;
//        private static readonly HttpClient _httpClient = new HttpClient();
//        private static readonly string baseUrl = "https://api.powerbi.com/v1.0/myorg/groups";

//        private static readonly string clientId = Environment.GetEnvironmentVariable("FBDEV_AzureClientID");
//        private static readonly string tenantId = Environment.GetEnvironmentVariable("FBDEV_AzureTenantID");
//        private static readonly string clientSecret = Environment.GetEnvironmentVariable("FBDEV_AzureClientSecret");

//        public Reports(ILogger<Reports> logger)
//        {
//            _logger = logger;
//        }



//        private static async Task<string> GetAccessTokenAsync()
//        {
//            string tokenUrl = $"https://login.microsoftonline.com/{tenantId}/oauth2/v2.0/token";
//            var data = new Dictionary<string, string>
//            {
//                { "grant_type", "client_credentials" },
//                { "scope", "https://analysis.windows.net/powerbi/api/.default" },
//                { "client_id", clientId },
//                { "client_secret", clientSecret }
//            };

//            using var content = new FormUrlEncodedContent(data);
//            HttpResponseMessage response = await _httpClient.PostAsync(tokenUrl, content);
//            string responseJson = await response.Content.ReadAsStringAsync();

//            if (!response.IsSuccessStatusCode)
//            {
//                throw new Exception($"Failed to retrieve token: {responseJson}");
//            }

//            var tokenObj = JsonSerializer.Deserialize<TokenResponse>(responseJson);
//            return tokenObj?.AccessToken ?? throw new Exception("Access token not found in response.");
//        }







//        [Function("DeleteAllReportsInWorkspace")]
//        public async Task<HttpResponseData> DeleteAllReports(
//           [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "workspace/{workspaceId}/reports/delete")] HttpRequestData req,
//           string workspaceId)
//        {
//            _logger.LogInformation($"Deleting all reports in workspace: {workspaceId}");

//            string accessToken;
//            try
//            {
//                accessToken = await GetAccessTokenAsync();
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError($"Error getting access token: {ex.Message}");
//                var errorResponse = req.CreateResponse(System.Net.HttpStatusCode.InternalServerError);
//                await errorResponse.WriteStringAsync($"Error retrieving access token: {ex.Message}");
//                return errorResponse;
//            }

//            try
//            {
//                string reportsUrl = $"{baseUrl}/{workspaceId}/reports";
//                using (HttpClient client = new HttpClient())
//                {
//                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
//                    HttpResponseMessage response = await client.GetAsync(reportsUrl);
//                    string responseJson = await response.Content.ReadAsStringAsync();
//                    var responseObj = JsonSerializer.Deserialize<ReportListResponse>(responseJson);
//                    var reports = responseObj?.Value ?? new List<Report>();


//                    foreach (var report in reports)
//                    {
//                        string deleteUrl = $"{baseUrl}/{workspaceId}/reports/{report.Id}";
//                        await client.DeleteAsync(deleteUrl);
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError($"Error deleting reports: {ex.Message}");
//                var errorResponse = req.CreateResponse(System.Net.HttpStatusCode.InternalServerError);
//                await errorResponse.WriteStringAsync($"Error deleting reports: {ex.Message}");
//                return errorResponse;
//            }

//            var successResponse = req.CreateResponse(System.Net.HttpStatusCode.OK);
//            await successResponse.WriteStringAsync("All reports deleted successfully.");
//            return successResponse;
//        }






//    [Function("CreateReport")]
//        public async Task<HttpResponseData> CreateReport(
//    [HttpTrigger(AuthorizationLevel.Function, "post", Route = "workspace/{workspaceId}/reports/create")] HttpRequestData req,
//    string workspaceId)
//        {
//            _logger.LogInformation($"Creating a report in workspace: {workspaceId}");

//            string accessToken;
//            try
//            {
//                accessToken = await GetAccessTokenAsync();
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError($"Error getting access token: {ex.Message}");
//                var errorResponse = req.CreateResponse(System.Net.HttpStatusCode.InternalServerError);
//                await errorResponse.WriteStringAsync($"Error retrieving access token: {ex.Message}");
//                return errorResponse;
//            }

//            var requestBody = await JsonSerializer.DeserializeAsync<CreateReportRequest>(req.Body);
//            if (requestBody == null || string.IsNullOrEmpty(requestBody.ReportName) || string.IsNullOrEmpty(requestBody.DatasetId))
//            {
//                var errorResponse = req.CreateResponse(System.Net.HttpStatusCode.BadRequest);
//                await errorResponse.WriteStringAsync("Invalid request: Report name and dataset ID are required.");
//                return errorResponse;
//            }

//            try
//            {
//                string createReportUrl = $"{baseUrl}/{workspaceId}/reports";
//                using (HttpClient client = new HttpClient())
//                {
//                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
//                    var jsonBody = JsonSerializer.Serialize(new { name = requestBody.ReportName, datasetId = requestBody.DatasetId });
//                    var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
//                    HttpResponseMessage response = await client.PostAsync(createReportUrl, content);
//                    string responseJson = await response.Content.ReadAsStringAsync();

//                    if (!response.IsSuccessStatusCode)
//                    {
//                        throw new Exception($"Error creating report: {responseJson}");
//                    }

//                    var successResponse = req.CreateResponse(System.Net.HttpStatusCode.OK);
//                    await successResponse.WriteStringAsync(responseJson);
//                    return successResponse;
//                }
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError($"Error creating report: {ex.Message}");
//                var errorResponse = req.CreateResponse(System.Net.HttpStatusCode.InternalServerError);
//                await errorResponse.WriteStringAsync($"Error creating report: {ex.Message}");
//                return errorResponse;
//            }
//        }






//    } 
//}






using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Power_BI_Service.Response;
using Power_BI_Service.Workspaces;
using Power_BI_Service.Request;






namespace Power_BI_Service.Reports
{



    public class Reports
    {

        private readonly ILogger<Reports> _logger;
        private readonly HttpClient _httpClient;
        private readonly Token _token;

        public Reports(ILogger<Reports> logger, Token token, HttpClient httpClient)
        {
            _logger = logger;
            _token = token; 
            _httpClient = httpClient;
        }

       
        [Function("DeleteAllReportsInWorkspace")]
        public async Task<HttpResponseData> DeleteAllReports(
           [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "workspace/{workspaceId}/reports/delete")] HttpRequestData req,
           string workspaceId)
        {
            _logger.LogInformation($"Deleting all reports in workspace: {workspaceId}");

            string accessToken;
            try
            {
                accessToken = await _token.GetAccessTokenAsync();
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
                string reportsUrl = $"{_token.BaseUrl}/{workspaceId}/reports";
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    HttpResponseMessage response = await client.GetAsync(reportsUrl);
                    string responseJson = await response.Content.ReadAsStringAsync();
                    var responseObj = JsonSerializer.Deserialize<ReportListResponse>(responseJson);
                    var reports = responseObj?.Value ?? new List<Report>();


                    foreach (var report in reports)
                    {
                        string deleteUrl = $"{_token.BaseUrl}/{workspaceId}/reports/{report.Id}";
                        await client.DeleteAsync(deleteUrl);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting reports: {ex.Message}");
                var errorResponse = req.CreateResponse(System.Net.HttpStatusCode.InternalServerError);
                await errorResponse.WriteStringAsync($"Error deleting reports: {ex.Message}");
                return errorResponse;
            }

            var successResponse = req.CreateResponse(System.Net.HttpStatusCode.OK);
            await successResponse.WriteStringAsync("All reports deleted successfully.");
            return successResponse;
        }


        

        [Function("CreateReport")]
        public async Task<HttpResponseData> CreateReport(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "workspace/{workspaceId}/reports/create")] HttpRequestData req,
        string workspaceId)
        {
            _logger.LogInformation($"Creating a report in workspace: {workspaceId}");

            string accessToken;
            try
            {
                accessToken = await _token.GetAccessTokenAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting access token: {ex.Message}");
                var errorResponse = req.CreateResponse(System.Net.HttpStatusCode.InternalServerError);
                await errorResponse.WriteStringAsync($"Error retrieving access token: {ex.Message}");
                return errorResponse;
            }

            var requestBody = await JsonSerializer.DeserializeAsync<CreateReportRequest>(req.Body);
            if (requestBody == null || string.IsNullOrEmpty(requestBody.ReportName) || string.IsNullOrEmpty(requestBody.DatasetId))
            {
                var errorResponse = req.CreateResponse(System.Net.HttpStatusCode.BadRequest);
                await errorResponse.WriteStringAsync("Invalid request: Report name and dataset ID are required.");
                return errorResponse;
            }

            try
            {
                string createReportUrl = $"{_token.BaseUrl}/{workspaceId}/reports";
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    var jsonBody = JsonSerializer.Serialize(new { name = requestBody.ReportName, datasetId = requestBody.DatasetId });
                    var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync(createReportUrl, content);
                    string responseJson = await response.Content.ReadAsStringAsync();

                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception($"Error creating report: {responseJson}");
                    }

                    var successResponse = req.CreateResponse(System.Net.HttpStatusCode.OK);
                    await successResponse.WriteStringAsync(responseJson);
                    return successResponse;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating report: {ex.Message}");
                var errorResponse = req.CreateResponse(System.Net.HttpStatusCode.InternalServerError);
                await errorResponse.WriteStringAsync($"Error creating report: {ex.Message}");
                return errorResponse;
            }
        }






    }
}





