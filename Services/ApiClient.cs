using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using RestSharp;
using OpenFoodFactService.Configuration;

namespace OpenFoodFactService.Services
{
    public class ApiClient : IApiClient
    {
        private readonly ServiceSettings _settings;
        private readonly ILogger<ApiClient> _logger;

        // Invalid http status error code list
        private static readonly List<HttpStatusCode> invalidStatusCode = new List<HttpStatusCode> {
            HttpStatusCode.BadRequest,
            HttpStatusCode.BadGateway,
            HttpStatusCode.InternalServerError,
            HttpStatusCode.RequestTimeout,
            HttpStatusCode.Forbidden,
            HttpStatusCode.GatewayTimeout
        };

        public ApiClient(
            ILogger<ApiClient> logger,
            IOptions<ServiceSettings> settings)
        {
            _settings = settings.Value;
            _logger = logger;
        }

        public OpenFoodFactInfo ConnectToApi(string id)
        {
            // Adding polly(policies) signatures
            var retryPolicy = Policy
                .HandleResult<IRestResponse>(resp => invalidStatusCode.Contains(resp.StatusCode))
                .WaitAndRetry(3, i => TimeSpan.FromSeconds(Math.Pow(3, i)), (result, TimeSpan, currentRetryCount, context) =>
                {
                    _logger.LogError($"Request failed due to {result.Result.StatusCode}. Waiting time {TimeSpan} before next retry access. This is the {currentRetryCount} retry.");
                });

            // Initiating a Rest Client
            var client = new RestClient($"{_settings.OpenFoodFactApiUrl}/cgi");

            // Initiating the Rest Request
            var request = new RestRequest(Method.GET);
            request.RequestFormat = DataFormat.Json;

            // Adding the request parameters
            request.AddParameter("id", id, ParameterType.GetOrPost);
            //request.AddParameter("limit", limit, ParameterType.GetOrPost);

            var policyResponse = retryPolicy.ExecuteAndCapture(() =>
            {
                return client.Get(request);
            });

            if (policyResponse.Result != null)
            {
                return JsonSerializer.Deserialize<OpenFoodFactInfo>(policyResponse.Result.Content);
            }
            else
            {
                return null;
            }
        }

        public record Ingredients(string data);
        public record OpenFoodFactInfo(Ingredients[] Ingredient);
    
    }
}
