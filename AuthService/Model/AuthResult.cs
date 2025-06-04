namespace AuthService.Model
{
    public class AuthResult
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public DateTime ExpiryDate { get; set; }

        //TODO: UserRole is not used yet Enum
        //public string UserRole { get; set; }
    }
}


public record JWTConfig
{
    public string secret { get; set; }
    public string issuer { get; set; }
    public string audience { get; set; }
    public int expirationMinutes { get; set; }

}

public record LogIn
{
    public string Email { get; set; }
    public string Password { get; set; }

}


public record User
{
    public Guid Guid { get; set; }
    public string UserRole { get; set; }

}