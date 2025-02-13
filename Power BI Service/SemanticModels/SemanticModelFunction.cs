
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Power_BI_Service.Response;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker;
using System.Net.Http.Headers;
using Power_BI_Service.Request;

namespace Power_BI_Service.SemanticModels
{

    public class SemanticModelFunction
    {

        private readonly ILogger<SemanticModelFunction> _logger;

        private readonly Token _token;

        private readonly HttpClient _httpClient;
      
        public SemanticModelFunction(ILogger<SemanticModelFunction> logger, Token token, HttpClient httpClient)
        {
            _logger = logger;
            _token = token;
            _httpClient = httpClient;
        }



        [Function("DeleteSemanticModel")]
        public async Task<HttpResponseData> DeleteSemanticModel(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "workspace/{workspaceId}/semanticmodel/{modelId}")] HttpRequestData req,
            string workspaceId, string modelId)
        {
            _logger.LogInformation($"Deleting semantic model {modelId} in workspace {workspaceId}...");

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
                string deleteUrl = $"{_token.BaseUrl}/{workspaceId}/datasets/{modelId}";
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    HttpResponseMessage response = await client.DeleteAsync(deleteUrl);

                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception($"Error deleting semantic model: {await response.Content.ReadAsStringAsync()}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting semantic model: {ex.Message}");
                var errorResponse = req.CreateResponse(System.Net.HttpStatusCode.InternalServerError);
                await errorResponse.WriteStringAsync($"Error deleting semantic model: {ex.Message}");
                return errorResponse;
            }

            var successResponse = req.CreateResponse(System.Net.HttpStatusCode.OK);
            await successResponse.WriteStringAsync("Semantic model deleted successfully.");
            return successResponse;
        }

        [Function("DeleteAllSemanticModels")]
        public async Task<HttpResponseData> DeleteAllSemanticModels(
     [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "workspace/{workspaceId}/semanticmodels/delete")] HttpRequestData req,
     string workspaceId)
        {
            _logger.LogInformation($"Deleting all semantic models in workspace: {workspaceId}");

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
                string modelsUrl = $"{_token.BaseUrl}/{workspaceId}/datasets";
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    HttpResponseMessage response = await client.GetAsync(modelsUrl);
                    string responseJson = await response.Content.ReadAsStringAsync();
                    var responseObj = JsonSerializer.Deserialize<SemanticModelListResponse>(responseJson);
                    var models = responseObj?.Value ?? new List<SemanticModel>();

                    foreach (var model in models)
                    {
                        string deleteUrl = $"{_token.BaseUrl}/{workspaceId}/datasets/{model.Id}";
                        await client.DeleteAsync(deleteUrl);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting semantic models: {ex.Message}");
                var errorResponse = req.CreateResponse(System.Net.HttpStatusCode.InternalServerError);
                await errorResponse.WriteStringAsync($"Error deleting semantic models: {ex.Message}");
                return errorResponse;
            }

            var successResponse = req.CreateResponse(System.Net.HttpStatusCode.OK);
            await successResponse.WriteStringAsync("All semantic models deleted successfully.");
            return successResponse;
        }




        [Function("GetSemanticModelParameterValue")]
        public async Task<HttpResponseData> GetSemanticModelParameterValue(
          [HttpTrigger(AuthorizationLevel.Function, "get", Route = "workspace/{workspaceId}/semanticmodel/{modelId}/parameters")] HttpRequestData req,
          string workspaceId, string modelId)
        {
            _logger.LogInformation($"Retrieving parameters for semantic model {modelId} in workspace {workspaceId}...");

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
                string parametersUrl = $"{_token.BaseUrl}/{workspaceId}/datasets/{modelId}/parameters";
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    HttpResponseMessage response = await client.GetAsync(parametersUrl);
                    string responseJson = await response.Content.ReadAsStringAsync();

                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception($"Error retrieving parameters: {responseJson}");
                    }

                    var successResponse = req.CreateResponse(System.Net.HttpStatusCode.OK);
                    await successResponse.WriteStringAsync(responseJson);
                    return successResponse;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving semantic model parameters: {ex.Message}");
                var errorResponse = req.CreateResponse(System.Net.HttpStatusCode.InternalServerError);
                await errorResponse.WriteStringAsync($"Error retrieving semantic model parameters: {ex.Message}");
                return errorResponse;
            }
        }
        [Function("UpdateSemanticModelParameter")]
        public async Task<HttpResponseData> UpdateSemanticModelParameter(
                  [HttpTrigger(AuthorizationLevel.Function, "post", Route = "workspace/{workspaceId}/semanticmodel/{modelId}/updateparameter")] HttpRequestData req,
                  string workspaceId, string modelId)
        {
            _logger.LogInformation($"Updating parameter for semantic model {modelId} in workspace {workspaceId}...");

            
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

           
            var requestBody = await JsonSerializer.DeserializeAsync<UpdateParameterRequest>(req.Body);
            if (requestBody == null || string.IsNullOrEmpty(requestBody.ParameterName) || string.IsNullOrEmpty(requestBody.NewValue))
            {
                var errorResponse = req.CreateResponse(System.Net.HttpStatusCode.BadRequest);
                await errorResponse.WriteStringAsync("Invalid request: Parameter name and new value are required.");
                return errorResponse;
            }

            
            string updateUrl = $"https://api.powerbi.com/v1.0/myorg/groups/{workspaceId}/datasets/{modelId}/UpdateParameters";

            
            var jsonBody = JsonSerializer.Serialize(new
            {
                updateDetails = new[]
                {
                    new
                    {
                        name = requestBody.ParameterName,
                        newValue = requestBody.NewValue
                    }
                }
            });
            
            

            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                _logger.LogInformation($"Sending update request to: {updateUrl}");

                HttpResponseMessage response = await _httpClient.PostAsync(updateUrl, content);
                string responseJson = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"Error updating parameter: {responseJson}");
                    var errorResponse = req.CreateResponse(response.StatusCode);
                    await errorResponse.WriteStringAsync(responseJson);
                    return errorResponse;
                }

                var successResponse = req.CreateResponse(System.Net.HttpStatusCode.OK);
                await successResponse.WriteStringAsync("Parameter updated successfully.");
                return successResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating semantic model parameter: {ex.Message}");
                var errorResponse = req.CreateResponse(System.Net.HttpStatusCode.InternalServerError);
                await errorResponse.WriteStringAsync($"Error updating semantic model parameter: {ex.Message}");
                return errorResponse;
            }
        }



        [Function("GetAllSemanticModels")]
        public async Task<HttpResponseData> GetAllSemanticModels(
    [HttpTrigger(AuthorizationLevel.Function, "get", Route = "workspace/{workspaceId}/semanticmodels")] HttpRequestData req,
    string workspaceId)
        {
            _logger.LogInformation($"Retrieving all semantic models in workspace: {workspaceId}");

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
                string modelsUrl = $"{_token.BaseUrl}/{workspaceId}/datasets";
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    HttpResponseMessage response = await client.GetAsync(modelsUrl);
                    string responseJson = await response.Content.ReadAsStringAsync();

                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception($"Error retrieving semantic models: {responseJson}");
                    }

                    var successResponse = req.CreateResponse(System.Net.HttpStatusCode.OK);
                    await successResponse.WriteStringAsync(responseJson);
                    return successResponse;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving semantic models: {ex.Message}");
                var errorResponse = req.CreateResponse(System.Net.HttpStatusCode.InternalServerError);
                await errorResponse.WriteStringAsync($"Error retrieving semantic models: {ex.Message}");
                return errorResponse;
            }
        }
    }
}


