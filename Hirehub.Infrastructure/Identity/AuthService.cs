using Hirehub.Application.DTOs.Auth;
using Hirehub.Application.Interfaces;
using Hirehub.Domain.Entities;
using Hirehub.Domain.Enums;
using Hirehub.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Hirehub.Infrastructure.Identity;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITokenService _tokenService;
    private readonly ApplicationDbContext _db;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        ITokenService tokenService,
        ApplicationDbContext db)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _db = db;
    }

    public async Task<AuthResult> RegisterAsync(RegisterRequest request)
    {
        if (request.Role != AppRoles.Client && request.Role != AppRoles.Freelancer)
        {
            return AuthResult.Fail("Role must be either 'Client' or 'Freelancer'.");
        }

        var existing = await _userManager.FindByEmailAsync(request.Email);
        if (existing != null)
        {
            return AuthResult.Fail("An account with this email already exists.");
        }

        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber
        };

        var createResult = await _userManager.CreateAsync(user, request.Password);
        if (!createResult.Succeeded)
        {
            var errors = string.Join(" ", createResult.Errors.Select(e => e.Description));
            return AuthResult.Fail(errors);
        }

        await _userManager.AddToRoleAsync(user, request.Role);

        if (request.Role == AppRoles.Freelancer)
        {
            _db.FreelancerProfiles.Add(new FreelancerProfile
            {
                UserId = user.Id,
                FullName = request.FullName
            });
        }
        else
        {
            _db.ClientProfiles.Add(new ClientProfile
            {
                UserId = user.Id,
                FullName = request.FullName
            });
        }

        await _db.SaveChangesAsync();

        return await BuildAuthResultAsync(user, request.FullName);
    }

    public async Task<AuthResult> LoginAsync(LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null || !user.IsActive)
        {
            return AuthResult.Fail("Invalid email or password.");
        }

        var passwordValid = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!passwordValid)
        {
            return AuthResult.Fail("Invalid email or password.");
        }

        var fullName = await GetFullNameAsync(user.Id);
        return await BuildAuthResultAsync(user, fullName);
    }

    public async Task<AuthResult> RefreshAsync(string refreshToken)
    {
        var hash = _tokenService.HashToken(refreshToken);

        var stored = await _db.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.TokenHash == hash);

        if (stored == null || !stored.IsActive)
        {
            return AuthResult.Fail("Invalid or expired refresh token.");
        }

        // Rotate: revoke the used token, issue a new pair
        stored.RevokedAt = DateTime.UtcNow;

        var fullName = await GetFullNameAsync(stored.UserId);
        var result = await BuildAuthResultAsync(stored.User, fullName);

        stored.ReplacedByTokenHash = _tokenService.HashToken(result.Response!.RefreshToken);
        await _db.SaveChangesAsync();

        return result;
    }

    public async Task LogoutAsync(string refreshToken)
    {
        var hash = _tokenService.HashToken(refreshToken);
        var stored = await _db.RefreshTokens.FirstOrDefaultAsync(rt => rt.TokenHash == hash);

        if (stored != null && stored.RevokedAt == null)
        {
            stored.RevokedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
        }
    }

    private async Task<AuthResult> BuildAuthResultAsync(ApplicationUser user, string fullName)
    {
        var roles = await _userManager.GetRolesAsync(user);
        var tokens = _tokenService.GenerateTokens(user.Id, user.Email!, fullName, roles);

        _db.RefreshTokens.Add(new RefreshToken
        {
            UserId = user.Id,
            TokenHash = _tokenService.HashToken(tokens.RefreshToken),
            ExpiresAt = tokens.RefreshTokenExpiresAt
        });
        await _db.SaveChangesAsync();

        return AuthResult.Ok(new AuthResponse
        {
            AccessToken = tokens.AccessToken,
            RefreshToken = tokens.RefreshToken,
            AccessTokenExpiresAt = tokens.AccessTokenExpiresAt,
            Email = user.Email!,
            FullName = fullName,
            Role = roles.FirstOrDefault() ?? string.Empty
        });
    }

    private async Task<string> GetFullNameAsync(string userId)
    {
        var freelancer = await _db.FreelancerProfiles
            .Where(f => f.UserId == userId)
            .Select(f => f.FullName)
            .FirstOrDefaultAsync();

        if (freelancer != null) return freelancer;

        var client = await _db.ClientProfiles
            .Where(c => c.UserId == userId)
            .Select(c => c.FullName)
            .FirstOrDefaultAsync();

        return client ?? string.Empty;
    }
}