using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using static System.Net.Mime.MediaTypeNames;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Text;

namespace R0WebAPI.Repositories
{
    public class ServiceUtilityRepository 
    {
        private readonly HttpClient _httpClient;
        public ServiceUtilityRepository(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<HttpResponseMessage> prepareHttpContentMakePostCallWOHeader(string url, string? jsonContent)
        {
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            // Make a Post Async call to the respective service API url with respective httpContent
            var response = await _httpClient.PostAsync(url, httpContent);

            return response;
        }

        public async Task<HttpResponseMessage> prepareHttpContentMakePostCallWithHeader(string url, string? jsonContent, string token)
        {
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            // Set the Content-type
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            // Set the Authorization header
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Make a Post Async call to the respective service API url with respective httpContent and httpHeader bearer
            var response = await _httpClient.PostAsync(url, httpContent);

            return response;
        }
    }
}
