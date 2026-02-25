using ToDoApp.Application.Models;
using ToDoApp.Domain;

namespace ToDoApp.Application.Interface
{
    public interface IAuthService
    {
        Task<AuthResponseModel?> RegisterAsync(RegisterRequestModel registerDto);
        Task<AuthResponseModel?> LoginAsync(LoginRequestModel loginDto);
        Task<User?> GetUserByEmailAsync(string email);
    }
}
