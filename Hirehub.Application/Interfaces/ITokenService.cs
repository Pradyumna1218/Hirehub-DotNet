namespace Hirehub.Application.Interfaces;

public class GeneratedToken
{
    public string AccessToken { get; set; } = string.Empty;
    public DateTime AccessTokenExpiresAt { get; set; }
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime RefreshTokenExpiresAt { get; set; }
}

public interface ITokenService
{
    GeneratedToken GenerateTokens(string userId, string email, string fullName, IList<string> roles);
    string HashToken(string rawToken);
}