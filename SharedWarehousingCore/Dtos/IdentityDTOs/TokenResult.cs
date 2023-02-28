namespace SharedWarehousingCore.Dtos.IdentityDTOs
{
    public class TokenResult
    {
        public string Username { get; set; }
        public string Token { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
    }
}