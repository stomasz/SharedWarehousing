using System.Security.Claims;
using SharedWarehousingCore.Dtos.IdentityDTOs;

namespace SharedWarehousingCore.Services.TokenServices;

public interface ITokenService
{
    Task<string> CreateTokenIdentity(List<Claim> claims);
    Task<RefreshTokenDto> RefreshToken(RefreshTokenDto tokenModel);
    Task RevokeRefreshToken(string username);
    Task RevokeAllRefreshToken();
    Task<string> GenerateRefreshToken();
}