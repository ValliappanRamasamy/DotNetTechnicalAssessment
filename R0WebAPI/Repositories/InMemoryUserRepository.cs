using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using R0WebAPI.Repositories;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WebAPI.Controllers;
using WebAPI.Models;
using static System.Net.Mime.MediaTypeNames;


namespace WebAPI.Repositories
{    
    public class InMemoryUserRepository : IUserRepository
    {
        // This could be replaced with a real database context
        private readonly List<User> _users = new();

        private readonly HttpClient _httpClient;
        private readonly ILogger<InMemoryUserRepository> _logger;
        private readonly ServiceUtilityRepository _serviceUtilityRepository;

        private string RegisterUrl;
        private string SignInUrl;
        private string SignOutUrl;

        public InMemoryUserRepository(HttpClient httpClient, ILogger<InMemoryUserRepository> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _serviceUtilityRepository = new ServiceUtilityRepository(_httpClient);

            var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory) // Set the base path to the current directory
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

            RegisterUrl = configuration["Urls:User:RegisterUrl"];
            SignInUrl = configuration["Urls:User:SignInUrl"];
            SignOutUrl = configuration["Urls:User:SignOutUrl"];
        }

        public Task<bool> UserExistsAsync(string email)
        {
            return Task.FromResult(_users.Any(u => u.email.Equals(email, StringComparison.OrdinalIgnoreCase)));
        }

        public async Task<User> RegisterUserAsync(UserRegisterRequest userInfo)
        //public async Task<bool> RegisterUserAsync(UserRegisterRequest userInfo)
        {
            // Create a new User entity
            // Optional
            var user = new User
            {
                name = userInfo.name,
                email = userInfo.email               
            };
            _users.Add(user);

            var jsonContent = JsonSerializer.Serialize(userInfo);
                        
            // call the function to prepare http content and Make a Post Async call https://api.opensensemap.org/users/register with UserRegisterRequest object as httpContent
            var response = await _serviceUtilityRepository.prepareHttpContentMakePostCallWOHeader(RegisterUrl, jsonContent);

            User userReqisterResponse = await response.Content.ReadFromJsonAsync<User>();
            // Read the response content as a string
            //string responseContent = await response.Content.ReadAsStringAsync();

            if (!userReqisterResponse.code.Equals("Created"))
            {
                //Debug.WriteLine(response.IsSuccessStatusCode + "=" + userReqisterResponse.code + ", " + userReqisterResponse.message + ", " + userReqisterResponse.token); 
                _logger.LogDebug(userReqisterResponse.code + ", " + userReqisterResponse.message + ", " + userReqisterResponse.token);
            }
            
            return userReqisterResponse;
            //return response.IsSuccessStatusCode;
        }

        public async Task<User> SigninUserAsync(UserSigninRequest signinInfo)
        {
            var jsonContent = JsonSerializer.Serialize(signinInfo);                       

            // call the function to prepare http content and Make a Post Async call https://api.opensensemap.org/users/sign-in with UserSigninRequest object as httpContent
            var response = await _serviceUtilityRepository.prepareHttpContentMakePostCallWOHeader(SignInUrl, jsonContent);
            
            // Read the response content as a JSON
            User userSigninResponse = await response.Content.ReadFromJsonAsync<User>();

            // Read the response content as a string
            //string responseContent = await response.Content.ReadAsStringAsync();

            if (!userSigninResponse.code.Equals("Authorized")) {
                //Debug.WriteLine(response.IsSuccessStatusCode + "=" + userSigninResponse.code + ", " + userSigninResponse.message + ", " + userSigninResponse.token);
                _logger.LogDebug(userSigninResponse.code + ", " + userSigninResponse.message + ", " + userSigninResponse.token);
            }
            return userSigninResponse;
        }

        //POST
        //Logout API
        //https://api.opensensemap.org/users/sign-out

        //Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJyb2xlIjoidXNlciIsImlhdCI6MTc1MjEzOTY2MCwiZXhwIjoxNzUyMTQzMjYwLCJpc3MiOiJodHRwczovL2FwaS5vcGVuc2Vuc2VtYXAub3JnIiwic3ViIjoiYXNzZXNzbWVudDA3QHRlc3QuY29tIiwianRpIjoiMWRlY2YwODUtNGI2Zi00MTljLTgyNzgtYjVlNTQ3NzNkNDNmIn0.RLhbY2r4llT1cyyZ9FRrTu-VIByt68OUwb-lcz3GRVQ

        // Set the headers
        //_httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJyb2xlIjoidXNlciIsImlhdCI6MTc1MjEzOTY2MCwiZXhwIjoxNzUyMTQzMjYwLCJpc3MiOiJodHRwczovL2FwaS5vcGVuc2Vuc2VtYXAub3JnIiwic3ViIjoiYXNzZXNzbWVudDA3QHRlc3QuY29tIiwianRpIjoiMWRlY2YwODUtNGI2Zi00MTljLTgyNzgtYjVlNTQ3NzNkNDNmIn0.RLhbY2r4llT1cyyZ9FRrTu-VIByt68OUwb-lcz3GRVQ");

        //ResponseBody
        //{
        //"code": "Ok",
        //"message": "Successfully signed out"
        //}

        public async Task<User> SignoutUserAsync(string token)
        {
           
            var jsonContent = JsonSerializer.Serialize("");
                       
            // call the function to prepare http content, header bearer token, and Make a Post Async call https://api.opensensemap.org/users/sign-out with token as Authorization header "Bearer " + token value
            var  response = await _serviceUtilityRepository.prepareHttpContentMakePostCallWithHeader(SignOutUrl, jsonContent, token);

            // Read the response content as a JSON
            User userReqisterResponse = await response.Content.ReadFromJsonAsync<User>(); 
            
            // Read the response content as a string
            //string responseContent = await response.Content.ReadAsStringAsync();

            if (!userReqisterResponse.code.Equals("Ok"))
            {
                //Debug.WriteLine(response.IsSuccessStatusCode + "=" + userReqisterResponse.code + ", " + userReqisterResponse.message + ", " + userReqisterResponse.token); 
                _logger.LogDebug(userReqisterResponse.code + ", " + userReqisterResponse.message + ", " + userReqisterResponse.token);
            }
            
            return userReqisterResponse;
        }

        

    }
    
}
