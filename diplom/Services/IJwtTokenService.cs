using System.Security.Claims;
using Api.Entities;

namespace Api.Services;

public interface IJwtTokenService
{
    (string Token, DateTime Expiry) GenerateAccessToken(ApplicationUser user);
    string GenerateRefreshToken();
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
}