using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SharedWarehousingCore.Dtos.IdentityDTOs;
using SharedWarehousingCore.Extensions;
using SharedWarehousingCore.Model.IdentityModel;

namespace SharedWarehousingCore.Services.TokenServices;

public class TokenService : ITokenService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SymmetricSecurityKey _key;

    public TokenService(IConfiguration config, UserManager<AppUser> userManager)
    {
        _userManager = userManager;
        _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"]));
    }

    public Task<string> CreateTokenIdentity(List<Claim> claims)
    {
        var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(30),
            SigningCredentials = creds
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return Task.FromResult(tokenHandler.WriteToken(token));
    }

    public async Task<RefreshTokenDto> RefreshToken(RefreshTokenDto tokenModel)
    {
        if (tokenModel is null)
        {
            throw new Exception("Invalid client request");
        }

        string? accessToken = tokenModel.AccessToken;
        string? refreshToken = tokenModel.RefreshToken;

        var principal = await GetPrincipalFromExpiredToken(accessToken);
        if (principal == null || principal.Identity is null || principal.Identity.Name is null)
        {
            throw new Exception("Invalid access token or refresh token");
        }

        string username = principal.Identity.Name;
        //string username = principal.Identity.

        var user = await _userManager.FindByNameAsync(username);

        if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
        {
            throw new Exception("Invalid access token or refresh token");
        }

        var newAccessToken = await CreateTokenIdentity(principal.Claims.ToList());
        var newRefreshToken = await GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        await _userManager.UpdateAsync(user);

        return new RefreshTokenDto
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken
        };
    }

    public async Task RevokeRefreshToken(string username)
    {
        var user = await _userManager.FindByNameAsync(username);

        if (user == null)
        {
            throw new Exception("Invalid user name");
        }

        user.RefreshToken = null;
        await _userManager.UpdateAsync(user);
    }


    public async Task RevokeAllRefreshToken()
    {
        var users = _userManager.Users.ToList();
        foreach (var user in users)
        {
            user.RefreshToken = null;
            await _userManager.UpdateAsync(user);
        }
    }

    public Task<string> GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Task.FromResult(Convert.ToBase64String(randomNumber));
    }

    private Task<ClaimsPrincipal?> GetPrincipalFromExpiredToken(string? token)
    {
        var tokenValidationParameters = TokenValidationParametersHelper.GetTokenValidationParameters(_key);

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal =
            tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

        JwtSecurityToken jwtSecurityToken = new JwtSecurityToken(token);

        if (!jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha512,
                StringComparison.InvariantCultureIgnoreCase))
            throw new SecurityTokenException("Invalid token");

        return Task.FromResult(principal);
    }
}