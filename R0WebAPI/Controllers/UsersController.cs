using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using WebAPI.Models;
using WebAPI.Services;

namespace WebAPI.Controllers
{

    // we create the API controller that will handle incoming HTTP requests.

    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {        
        private readonly IUserService _userService;
        private readonly ILogger<UsersController> _logger;

        private readonly HttpClient _httpClient;

        private User _responseContent;

        //Using MemeoryCache
        //public static readonly IMemoryCache _cache;
        //public UsersController(IUserService userService, ILogger<UsersController> logger, HttpClient httpClient, IMemoryCache cache)
        public UsersController(IUserService userService, ILogger<UsersController> logger, HttpClient httpClient)
        {
            _userService = userService;
            _logger = logger;           
            _httpClient = httpClient;

            //_cache = cache;

            _responseContent = new User();            
        }

        /// <summary>  
        /// Requirement 1
        /// Create RegisterUser API service method(POST) to register the user in OpenSenseMap.
        /// https://docs.opensensemap.org/#api-Users-register
        /// </summary>  
        /// <param name="UserRegisterRequest">userRegistrationInfo</param>  
        /// <returns>User code, message, and token</returns>
        [HttpPost]
        [Route("registeruser")]
        public async Task<IActionResult> RegisterUser([FromBody] UserRegisterRequest userRegistrationInfo)
        {
            try
            {
                //Example of Request Json:
                //* Name, email and password fields are mandatory
                //{
                //"name": "Assessment07",
                //"email": "Assessment07@test.com",
                //"password": "12345678"
                //}

                //Example of Response Json:
                //{"code":"Created","message":"Successfully registered new user","data":{"user":{"name":"Assessment10","email":"assessment10@test.com","role":"user","language":"en_US","boxes":[],"emailIsConfirmed":false}},
                //"token":"eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJyb2xlIjoidXNlciIsImlhdCI6MTc1MjEzNTAzMSwiZXhwIjoxNzUyMTM4NjMxLCJpc3MiOiJodHRwczovL2FwaS5vcGVuc2Vuc2VtYXAub3JnIiwic3ViIjoiYXNzZXNzbWVudDEwQHRlc3QuY29tIiwianRpIjoiZTI5ZTBhNDctOWY3OS00Yzk5LTk3YjItYWNlMGFmNzBjMzE2In0.VDxAZ1xzTsZDacyDQaINnJ2Nf1uKmcGF2GiwaK0qCFs",
                //"refreshToken":"CPRun5LI46gEHBio9mw3WwRRgtgyfsRXp+ma9Dz+x68="}

                //Call IUserService RegisterUserAsync API by passing the parameter UserRegisterRequest object containing mandatory fields Name, email and password.
                _responseContent = await _userService.RegisterUserAsync(userRegistrationInfo);

                //if the UserRegisterResponse Object code equals "Created", then it is a Successfully registered new user with a token returned back.
                if (_responseContent.code.Equals("Created"))                                 
                    return Ok(_responseContent.message + " & token = " + _responseContent.token);                
                //else i.e. the UserRegisterResponse Object code not equals "Created", then it is a Failure of new user registration.
                else
                    return BadRequest(_responseContent.message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        /// <summary>  
        /// Requirement 2
        /// Create Login API service method(POST) to login the user in OpenSenseMap.
        /// https://docs.opensensemap.org/#api-Users-sign_in
        /// </summary>  
        /// <param name="UserSigninRequest">userSigninInfo</param>  
        /// <returns>User code, message, and token</returns>
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] UserSigninRequest userSigninInfo)
        {
            try
            {
                //Example of Request Json:
                //* Email and password fields are mandatory
                //{
                //"email": "Assessment07@test.com",
                //"password": "12345678"
                //}

                //Example of Response Json:
                //{
                //    "code": "Authorized",
                //    "message": "Successfully signed in",
                //    "data": {
                //        "user": {
                //            "name": "Assessment07",
                //            "email": "assessment07@test.com",
                //            "role": "user",
                //            "language": "en_US",
                //            "boxes": [],
                //            "emailIsConfirmed": false
                //        }
                //},
                //    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJyb2xlIjoidXNlciIsImlhdCI6MTc1MjEzNjcxOCwiZXhwIjoxNzUyMTQwMzE4LCJpc3MiOiJodHRwczovL2FwaS5vcGVuc2Vuc2VtYXAub3JnIiwic3ViIjoiYXNzZXNzbWVudDA3QHRlc3QuY29tIiwianRpIjoiZDI5ZmY3YzYtMTBmZS00YzYyLWE4YWEtNmJjN2ZlZjAyMzAzIn0.o8CcvAiwgduCtlE3ku5uSew-tyMQ_nez-Gr__XCJ0dc",
                //    "refreshToken": "F3ZMfdLnQIdIcKxHy3DEmLJqFxSPwmHTfJ3n7be6yC8="
                //}
                                
                //Call IUserService SigninUserAsync API by passing the parameter UserSigninRequest object containing mandatory fields email and password.
                _responseContent = await _userService.SigninUserAsync(userSigninInfo);

                //if the UserRegisterResponse Object code equals "Authorized", then it is a Successfully signed in with a token returned back.
                if (_responseContent.code.Equals("Authorized"))
                {
                    // Store the token in memory cache with an expiration time
                    //_cache.Set("userToken", _responseContent.token, TimeSpan.FromMinutes(30));
                    return Ok(_responseContent.message + " & token = " + _responseContent.token);
                }
                else
                    //else i.e. the UserRegisterResponse Object code not equals "Authorized", then it is a Failure of user signing in i.e. Unauthorized.
                    return BadRequest(_responseContent.message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        /// <summary>  
        /// Requirement 5
        /// Create Logout API service method(POST) to logout the user in OpenSenseMap.
        /// https://docs.opensensemap.org/#api-Users-sign_out
        /// Upon successful log out, you need to clear the token in the cache or object(s) that you have persisted.
        /// </summary>   
        /// <param name="Authorization Header Example"> 
        /// Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJyb2xlIjoidXNlciIsImlhdCI6MTc1MjEzOTY2MCwiZXhwIjoxNzUyMTQzMjYwLCJpc3MiOiJodHRwczovL2FwaS5vcGVuc2Vuc2VtYXAub3JnIiwic3ViIjoiYXNzZXNzbWVudDA3QHRlc3QuY29tIiwianRpIjoiMWRlY2YwODUtNGI2Zi00MTljLTgyNzgtYjVlNTQ3NzNkNDNmIn0.RLhbY2r4llT1cyyZ9FRrTu-VIByt68OUwb-lcz3GRVQ
        /// </param> 
        /// <returns>User code, and message</returns>
        [HttpPost]
        [Route("logout")]
        public async Task<IActionResult> Logout()
        {

            try
            {
                //Authorization Header Example
                //Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJyb2xlIjoidXNlciIsImlhdCI6MTc1MjEzOTY2MCwiZXhwIjoxNzUyMTQzMjYwLCJpc3MiOiJodHRwczovL2FwaS5vcGVuc2Vuc2VtYXAub3JnIiwic3ViIjoiYXNzZXNzbWVudDA3QHRlc3QuY29tIiwianRpIjoiMWRlY2YwODUtNGI2Zi00MTljLTgyNzgtYjVlNTQ3NzNkNDNmIn0.RLhbY2r4llT1cyyZ9FRrTu-VIByt68OUwb-lcz3GRVQ

                //Example of Response Json:
                //{
                //"code": "Ok",
                //"message": "Successfully signed out"
                //}

                //Access the Authorization header
                var authorizationHeader = Request.Headers["Authorization"].ToString();

                //if authorizationHeader is null or empty or doesnot startws with "Bearer ", then return Unauthorized.
                if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer "))
                {
                    return Unauthorized(); // Return 401 Unauthorized if authorization header is missing or invalid.
                }

                //Assuming the header format is "Bearer <token>", you can validate and extract the token
                var token = authorizationHeader.StartsWith("Bearer ") ?
                            authorizationHeader.Substring("Bearer ".Length).Trim() : null;

                //if again token is null, then it is again an Invalid Authorization header.
                if (token == null)
                {
                    return BadRequest("Invalid Authorization header.");
                }
                //Call IUserService SignoutUserAsync API by passing the parameter token.
                _responseContent = await _userService.SignoutUserAsync(token);

                //if the UserRegisterResponse Object code equals "Ok", then it is a Successfully signed out and clear the tokens.
                if (_responseContent.code.Equals("Ok"))
                {
                    //Upon successful log out, clear the token in the cache or object(s) that we have persisted.
                    //_cache.Remove("userToken");

                    //Upon successful log out, clear the token in the currently loggedin User object.
                    _responseContent.token = "";
                    _responseContent.refreshToken = "";
                    return Ok(_responseContent.message);
                }
                else
                    //else i.e. the UserRegisterResponse Object code not equals "Ok", then it is a Failure of signing out.
                    return BadRequest(_responseContent.message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "An unexpected error occurred.");
            }
        }        


    }
}
