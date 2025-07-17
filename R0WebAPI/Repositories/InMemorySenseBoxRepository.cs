using System.Net.Http;
using System.Net.Http.Headers;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Models;
using WebAPI.Controllers;
using System.Diagnostics;
using System.Net.Http.Json;
using System;
using System.Security.Cryptography.Xml;
using Microsoft.Extensions.Configuration;

namespace WebAPI.Repositories
{    
    public class InMemorySenseBoxRepository : ISenseBoxRepository
    {
        // This could be replaced with a real database context
        private readonly List<SenseBox> _senseboxes = new();

        private readonly HttpClient _httpClient;
        private readonly ILogger<InMemorySenseBoxRepository> _logger;

        private string SenseBoxUrl;

        public InMemorySenseBoxRepository(HttpClient httpClient, ILogger<InMemorySenseBoxRepository> logger)
        {
            _httpClient = httpClient;
            _logger = logger;            

            var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory) // Set the base path to the current directory
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

            SenseBoxUrl = configuration["Urls:SenseBox:SenseBoxUrl"];
        }

        //POST
        //NewSenseBox API

        // https://api.opensensemap.org/boxes
        //Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJyb2xlIjoidXNlciIsImlhdCI6MTc1MjEzOTY2MCwiZXhwIjoxNzUyMTQzMjYwLCJpc3MiOiJodHRwczovL2FwaS5vcGVuc2Vuc2VtYXAub3JnIiwic3ViIjoiYXNzZXNzbWVudDA3QHRlc3QuY29tIiwianRpIjoiMWRlY2YwODUtNGI2Zi00MTljLTgyNzgtYjVlNTQ3NzNkNDNmIn0.RLhbY2r4llT1cyyZ9FRrTu-VIByt68OUwb-lcz3GRVQ

        //{
        //"email": "Assessment01@test.com",
        //"name": "NewBoxForAssessment",
        //"exposure": "indoor",
        //"model": "homeV2Wifi",
        //"location": {
        //"lat": 90,
        //"lng": 180,
        //"height": 100
        //}
        //}

        public async Task<SenseBoxResponse> SaveNewSenseBoxAsync(SenseBoxRequest senseboxRequest,string token)
        {
            // Create a new SenseBox entity
            // Optional
            var sensebox = new SenseBox
            {
                name = senseboxRequest.name,
                email = senseboxRequest.email,
                model = senseboxRequest.model,
                exposure = senseboxRequest.exposure                
            };
            _senseboxes.Add(sensebox);

            var jsonContent = JsonSerializer.Serialize(senseboxRequest);

            // call the function to prepare http content, header bearer token, and Make a Post Async call https://api.opensensemap.org/boxes with SenseBoxRequest object as httpContent
            var response = await prepareHttpContentMakePostCallWithHeader(SenseBoxUrl, jsonContent, token);            

            // Read the response content as a JSON
            SenseBoxResponse senseboxResponse = await response.Content.ReadFromJsonAsync<SenseBoxResponse>();

            // Read the response content as a string
            //string responseContent = await response.Content.ReadAsStringAsync();

            //if the SenseBoxResponse Object data is not null
            if (senseboxResponse.data != null)
            {
                //if the SenseBoxResponse Object data._id is null or empty or message is not equals to "Box successfully created", then the new sense box in OpenSenseMap is not created or not saved.
                if (string.IsNullOrEmpty(senseboxResponse.data._id) || !senseboxResponse.message.Equals("Box successfully created"))
                {
                    //Debug.WriteLine(response.IsSuccessStatusCode + "=" + senseboxResponse.code + ", " + senseboxResponse.message);
                    _logger.LogDebug(response.IsSuccessStatusCode + "=" + senseboxResponse.code + ", " + senseboxResponse.message);
                }
            }
            else
            //if the SenseBoxResponse Object data is null, then the new sense box in OpenSenseMap is not created or not saved.
            {
                //Debug.WriteLine(response.IsSuccessStatusCode + "=" + senseboxResponse.code + ", " + senseboxResponse.message);
                _logger.LogDebug(response.IsSuccessStatusCode + "=" + senseboxResponse.code + ", " + senseboxResponse.message);
            }
            return senseboxResponse;
            
        }


        //GET
        //GetSenseBoxById API

        // https://api.opensensemap.org/boxes/686f88b45e06c100080ad7f5?format=json
        public async Task<SenseBox> GetSenseBoxByIdAsync(string senseBoxId)
        {
        // call the function to prepare http content, and Make a Get Async call https://api.opensensemap.org/boxes/686f88b45e06c100080ad7f5?format=json with senseBoxId
        var response = await _httpClient.GetAsync(SenseBoxUrl + "/" + senseBoxId + "?format=json");

        // Read the response content as a JSON
        SenseBox senseboxResponse = await response.Content.ReadFromJsonAsync<SenseBox>();

        // Read the response content as a string
        //string responseContent = await response.Content.ReadAsStringAsync();

        //if the SenseBoxResponse Object _id is null or empty, then the sense box by id OpenSenseMap is not found or not available.
        if (string.IsNullOrEmpty(senseboxResponse._id))
        {
            //Debug.WriteLine("The sense box by id {" + senseBoxId + "} OpenSenseMap is not found or not available.");
            _logger.LogDebug("The sense box by id {" + senseBoxId + "} OpenSenseMap is not found or not available.");
        }
        return senseboxResponse;
        }

        private async Task<HttpResponseMessage> prepareHttpContentMakePostCallWithHeader(string url, string? jsonContent, string token)
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
