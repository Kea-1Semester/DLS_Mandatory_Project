using AuthService.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthService.Services
{
    public class LogInService : ILogInService
    {
        private readonly IConfiguration _config;
        private readonly HttpClient _httpClient;
        private IHttpClientFactory _httpClientFactory;

        public LogInService(IConfiguration config, IHttpClientFactory httpClientFactory)
        {
            _config = config;
            _httpClient = httpClientFactory.CreateClient("UserServiceClient");
        }

        public Task<ActionResult<string>> GetAuthentication()
        {
            throw new NotImplementedException();
        }

        public async Task<AuthResult> Login(string email, string password)
        {
            var user = new LogIn
            {
                Email = email,
                Password = password
            };

            // Call the UserService to validate the user
            var response = await _httpClient.PostAsJsonAsync("login-data", user);
            if (response.IsSuccessStatusCode)
            {
                // Deserialize the response to get the user data
                var userData = await response.Content.ReadFromJsonAsync<User>();
                if (userData == null)
                {
                    return null;
                }
                // Create a token for the user
                var token = CreateToken(userData);
                return new AuthResult { Token = token.Token, ExpiryDate = token.ExpiryDate, RefreshToken = token.RefreshToken };
            }
            else
            {
                return null;
            }
        }

        public Task Logout()
        {
            throw new NotImplementedException();
        }



        private AuthResult CreateToken(User user)
        {
            var expirationDate = DateTime.UtcNow.AddMinutes(2);

            List<Claim> claims = new List<Claim>()
            {
                 new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            foreach (var userRole in user.UserRole.ToString().Split(","))
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole));
                claims.Add(new Claim("Role", userRole));

            }

            // symbole ! (The null-forgiving operator ) is used to tell the compiler that the value will not be null
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _config.GetSection("JWT:Token").Value!));

            //Create a new instance of the signing credentials
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            //Create a new instance of the JwtSecurityToken
            var token = new JwtSecurityToken
            (
                issuer: _config.GetSection("JWT:Issuer").Value,
                audience: _config.GetSection("JWT:Audience").Value,
                claims: claims,
                expires: expirationDate,
                signingCredentials: creds
            );

            //Create a new instance of the JwtSecurityTokenHandler
            // retrun the jwt token with the claims
            var tokenHandler = new JwtSecurityTokenHandler().WriteToken(token);
            //return the token
            return
            new AuthResult
            {
                Token = tokenHandler,
                ExpiryDate = expirationDate,
                RefreshToken = Guid.NewGuid().ToString()
            };
        }

        //TODO: Create a logic for refresh token
        public Task<AuthResult> RefreshToken(string token)
        {
            throw new NotImplementedException();
        }
    }
}
