namespace AuthClassLibrary
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