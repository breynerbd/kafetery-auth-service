// Ubicación sugerida: AuthService.Application/Interfaces/IJwtTokenGenerator.cs
using AuthService.Domain.Entitis;
using System.Collections.Generic;

namespace AuthService.Application.Services
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(User user, IList<string> roles);
        string GenerateRefreshToken(User user);
        string ValidateRefreshToken(string refreshToken);
    }
}