using Hirehub.Application.DTOs.Auth;

namespace Hirehub.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResult> RegisterAsync(RegisterRequest request);
    Task<AuthResult> LoginAsync(LoginRequest request);
    Task<AuthResult> RefreshAsync(string refreshToken);
    Task LogoutAsync(string refreshToken);
}

public class AuthResult
{
    public bool Succeeded { get; set; }
    public string? Error { get; set; }
    public AuthResponse? Response { get; set; }

    public static AuthResult Fail(string error) => new() { Succeeded = false, Error = error };
    public static AuthResult Ok(AuthResponse response) => new() { Succeeded = true, Response = response };
}