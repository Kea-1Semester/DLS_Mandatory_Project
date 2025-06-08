using AuthClassLibrary;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Data;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Xml.Linq;
using UserClassLibrary;

namespace DLS_Mandatory_Project.Client
{
    public class CustomClientAuthStateProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService _localStorage;

        public CustomClientAuthStateProvider(ILocalStorageService localStorageService)
        {
            _localStorage = localStorageService;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await _localStorage.GetItemAsStringAsync("token");

            if (string.IsNullOrWhiteSpace(token))
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

            var claims = GetClaimsPrincipal(token);
            var claimsPrincipal = SetClaimsPrincipal(claims);
            
            return new AuthenticationState(claimsPrincipal);
        }

        public void UpdateAuthState(string jwtToken)
        {
            UserSession claims = GetClaimsPrincipal(jwtToken);
            ClaimsPrincipal claimsPrincipal = SetClaimsPrincipal(claims);
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
        }

        private ClaimsPrincipal SetClaimsPrincipal(UserSession model)
        {
            return new ClaimsPrincipal(new ClaimsIdentity(
                new List<Claim>
                {
                    new(ClaimTypes.NameIdentifier, model.Id),
                    new(ClaimTypes.Name, model.Name),
                    new(ClaimTypes.Email, model.Email),
                    new(ClaimTypes.Role, model.Role),
                }, "JwtAuth"));
        }

        private UserSession GetClaimsPrincipal(string jwtToken)
        {
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            JwtSecurityToken token = handler.ReadJwtToken(jwtToken);
            IEnumerable<Claim> claims = token.Claims;

            string id = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)!.Value;
            Console.WriteLine(id);
            string name = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)!.Value;
            Console.WriteLine(name);
            string email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)!.Value;
            Console.WriteLine(email);
            string role = claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)!.Value;
            Console.WriteLine(role);

            return new UserSession(id, name, email, role);
        }
    }
}