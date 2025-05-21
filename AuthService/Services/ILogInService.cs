using AuthClassLibrary;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Services
{
    public interface ILogInService
    {
        Task<ActionResult<string>> GetAuthentication();
        Task<AuthResult> Login(string email, string password);
        Task Logout();
    }
}
