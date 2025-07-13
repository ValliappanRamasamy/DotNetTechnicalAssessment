using WebAPI.Models;

namespace WebAPI.Services
{
   
    public interface IUserService
    {
        Task<User> RegisterUserAsync(UserRegisterRequest userRegistrationInfo);

        Task<User> SigninUserAsync(UserSigninRequest userSigninInfo);

        Task<User> SignoutUserAsync(string token);
    }

}
