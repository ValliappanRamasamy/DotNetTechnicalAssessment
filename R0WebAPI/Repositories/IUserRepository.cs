using System.Threading.Tasks;
using WebAPI.Models;

namespace WebAPI.Repositories
{
    public interface IUserRepository
    {
        Task<bool> UserExistsAsync(string email);
        Task<User> RegisterUserAsync(UserRegisterRequest userInfo);
        Task<User> SigninUserAsync(UserSigninRequest signinInfo);
        Task<User> SignoutUserAsync(string token);
    }
}
