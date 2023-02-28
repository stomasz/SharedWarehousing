using SharedWarehousingCore.Dtos.IdentityDTOs;

namespace SharedWarehousingCore.Services.AccountServices;

public interface IAccountService
{
    Task<List<string>> GetRoleNames();
    Task<TokenResult> Register(RegisterDto registerDto);
    Task<TokenResult> Login(LoginDto loginDto);
    Task<bool> AssignRoles(UserRolesDto userRoles);
    Task<RefreshTokenDto> RefreshToken(RefreshTokenDto tokenModel);
    Task<bool> UnassignRoles(UserRolesDto userRoles);
    Task<bool> CreateRole(string name);
}