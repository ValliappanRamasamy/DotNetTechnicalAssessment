using System;
using System.Threading.Tasks;
using WebAPI.Models;
using WebAPI.Repositories;

namespace WebAPI.Services
{
    
// Service to handle user registration
public class UserService : IUserService
    {
        // Example of a user repository. In a real application, this could connect to a database.
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserService> _logger;

        private User _responseContent;
        public static Dictionary<string, string> _tokenEmail;

        public UserService(IUserRepository userRepository, ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _logger = logger;

            _responseContent = new User();
            _tokenEmail = new Dictionary<string, string>();
        }

        public async Task<User> RegisterUserAsync(UserRegisterRequest userRegistrationInfo)
        {
            // Validate inputs
            if (string.IsNullOrWhiteSpace(userRegistrationInfo.name))
            {
                _logger.LogError("Name is required !");
                throw new ArgumentException("Name is required", nameof(userRegistrationInfo.name));               
            }
            if (string.IsNullOrWhiteSpace(userRegistrationInfo.email))
            {
                _logger.LogError("Email is required !");
                throw new ArgumentException("Email is required", nameof(userRegistrationInfo.email));                
            }
            if (string.IsNullOrWhiteSpace(userRegistrationInfo.password) || userRegistrationInfo.password.Length < 8)
            {
                _logger.LogError("Password is required and it must be at least 8 characters long !");
                throw new ArgumentException("Password is required and it must be at least 8 characters long", nameof(userRegistrationInfo.password));
            }

            // Check if the user already exists
            // Optional
            if (await _userRepository.UserExistsAsync(userRegistrationInfo.email))
            {
                _logger.LogError("User already exists");
                throw new InvalidOperationException("User already exists");                
            }

            // You should hash passwords!       
            //userRegistrationInfo.password = HashPassword(userRegistrationInfo.password);
            userRegistrationInfo.language = string.IsNullOrEmpty(userRegistrationInfo.language) ? "en_US" : userRegistrationInfo.language;

            // Save the user to the database / web API call
            _responseContent = await _userRepository.RegisterUserAsync(userRegistrationInfo);

            return _responseContent;   
        }       

        // Example hash function (This should be replaced with a secure hashing algorithm)
        private string HashPassword(string password)
        {
            // In real applications, use a better hashing algorithm like BCrypt
            return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(password));
        }

        public async Task<User> SigninUserAsync(UserSigninRequest userSigninInfo)
        {
            // Validate inputs
            if (string.IsNullOrWhiteSpace(userSigninInfo.email))
            {
                _logger.LogError("Email is required !");
                throw new ArgumentException("Email is required", nameof(userSigninInfo.email));
            }
            if (string.IsNullOrWhiteSpace(userSigninInfo.password) || userSigninInfo.password.Length < 8)
            {
                _logger.LogError("Password is required and it must be at least 8 characters long !");
                throw new ArgumentException("Password is required and it must be at least 8 characters long", nameof(userSigninInfo.password));
            }

            // You should hash passwords!       
            //userSigninInfo.password = HashPassword(userSigninInfo.password);

            //Call UserRepository SigninUserAsync API by passing the parameter UserSigninRequest object containing mandatory fields email and password.
            // Signin the user with the database / web API call
            _responseContent = await _userRepository.SigninUserAsync(userSigninInfo);

            //if the UserRegisterResponse Object code equals "Authorized", then it is a Successfully signed in with a token returned back.
            if (_responseContent.code.Equals("Authorized"))
                //Upon successful log in, store / persist the logged in user's token and corresponding email
                _tokenEmail.Add(_responseContent.token, userSigninInfo.email);            
            
            return _responseContent;
        }

        public async Task<User> SignoutUserAsync(string token)
        {
            _responseContent = await _userRepository.SignoutUserAsync(token);
            if (_responseContent.code.Equals("Ok"))
            {
                //Upon successful log out, clear the token in the cache or object(s) that we have persisted.
                _tokenEmail.Remove(token);
            }
            // Signout the user with the database / web API call
            return _responseContent;
        }
    }

}
