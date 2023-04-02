using System.IdentityModel.Tokens.Jwt;
using System.Security.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SharedWarehousingCore.DAL;
using SharedWarehousingCore.Dtos.IdentityDTOs;
using SharedWarehousingCore.Helpers;
using SharedWarehousingCore.Model.IdentityModel;
using SharedWarehousingCore.Services.TokenServices;

namespace SharedWarehousingCore.Services.AccountServices;

public class AccountService : IAccountService
{
    private readonly ITokenService _tokenService;
    private readonly MainDbContext _mainDbContext;
    private readonly RoleManager<AppRole> _roleManager;
    private readonly IConfiguration _configuration;
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;

    public AccountService(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,
        ITokenService tokenService, MainDbContext mainDbContext,
        RoleManager<AppRole> roleManager, IConfiguration configuration)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _tokenService = tokenService;
        _mainDbContext = mainDbContext;
        _roleManager = roleManager;
        _configuration = configuration;
    }


    public async Task<List<string>> GetRoleNames()
    {
        return await _mainDbContext.Roles.Select(x => x.Name).ToListAsync();
    }

    public async Task<TokenResult?> Register(RegisterDto registerDto)
    {
        using (var dbContextTransaction = await _mainDbContext.Database.BeginTransactionAsync())
        {
            if (await UserNameExists(registerDto.Username))
            {
                await dbContextTransaction.RollbackAsync();
                throw new InvalidCredentialException("Username is taken");
            }

            if (await UserEmailExists(registerDto.Email))
            {
                await dbContextTransaction.RollbackAsync();
                throw new InvalidCredentialException("Email is taken");
            }

            var user = new AppUser
            {
                UserName = registerDto.Username.ToLower(),
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                Email = registerDto.Email.ToLower(),
                PhoneNumber = registerDto.PhoneNumber.ToLower()
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded)
            {
                await dbContextTransaction.RollbackAsync();
                throw new AuthenticationException(result.Errors.ToString());
            }

            var roleName = await _roleManager.Roles
                .Where(x => x.NormalizedName == "USER")
                .Select(x => x.Name)
                .FirstOrDefaultAsync();

            var roleResult = await _userManager.AddToRoleAsync(user, roleName);

            if (!roleResult.Succeeded)
            {
                await dbContextTransaction.RollbackAsync();
                throw new AuthenticationException(result.Errors.ToString());
            }


            await dbContextTransaction.CommitAsync();

            return await LoginUser(user);
        }
    }

    public async Task<TokenResult?> Login(LoginDto loginDto)
    {
        var user = await _userManager.Users
            .FirstOrDefaultAsync(x => x.NormalizedEmail == loginDto.Email.ToUpper());

        if (user is null)
        {
            throw new InvalidCredentialException("Uzytkownik nie istnieje");
        }

        var result = await _signInManager
            .CheckPasswordSignInAsync(user, loginDto.Password, false);

        if (!result.Succeeded)
        {
            throw new InvalidCredentialException("nieprawidłowe dane logowania");
        }

        return await LoginUser(user);
    }


    public async Task<bool> AssignRoles(UserRolesDto userRoles)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == userRoles.UserName)
                   ?? throw new BasicException("Użytkownik nie istnieje");
        var result = await _userManager.AddToRolesAsync(user, userRoles.Roles);
        if (!result.Succeeded)
        {
            throw new Exception("BasicException");
        }

        return true;
    }

    public async Task<RefreshTokenDto> RefreshToken(RefreshTokenDto tokenModel)
    {
        return await _tokenService.RefreshToken(tokenModel);
    }

    public async Task<bool> UnassignRoles(UserRolesDto userRoles)
    {
        var result = await _userManager.RemoveFromRolesAsync(
            await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == userRoles.UserName), userRoles.Roles);
        if (!result.Succeeded)
        {
            throw new Exception("BasicException");
        }

        return true;
    }

    public async Task<bool> CreateRole(string name)
    {
        var result = await _roleManager.CreateAsync(new AppRole { Name = name });
        if (!result.Succeeded)
        {
            return false;
        }

        return true;
    }

    public async Task<string> GeneratePasswordResetTokenByEmail(string userEmail)
    {
        var user = await GetExistedUser(userEmail);

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        return token;
    }

    public async Task ChangeUserPassword(string userEmail, string token, string newPassword)
    {
        var user = await GetExistedUser(userEmail);

        await _userManager.ResetPasswordAsync(user, token, newPassword);
    }

    public async Task<string> GenerateEmailConfirmationTokenAsync(string userEmail)
    {
        var user = await GetExistedUser(userEmail);

        return await _userManager.GenerateEmailConfirmationTokenAsync(user);
    }

    public async Task ConfirmEmail(string userEmail, string token)
    {
        var user = await GetExistedUser(userEmail);
        var identityResult =await _userManager.ConfirmEmailAsync(user, token);
        
        if (!identityResult.Succeeded)
        {
            throw new Exception("Wystapil błąd");
        }
    }

    private async Task<TokenResult> LoginUser(AppUser user)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.NameId, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
        };

        if (user.EmailConfirmed is false)
        {
            throw new Exception("koniecznosc potwierdzenia Email");
        }
        var roles = await _userManager.GetRolesAsync(user);

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var tokenResult = new TokenResult();

        var refreshTokenValidityInDays = DateTime.UtcNow.AddDays(15);

        tokenResult.Username = user.UserName;
        tokenResult.Token = await _tokenService.CreateTokenIdentity(claims);
        tokenResult.RefreshToken = await _tokenService.GenerateRefreshToken();
        tokenResult.RefreshTokenExpiryTime = refreshTokenValidityInDays;

        user.RefreshToken = tokenResult.RefreshToken;
        user.RefreshTokenExpiryTime = refreshTokenValidityInDays;

        await _userManager.UpdateAsync(user);

        return tokenResult;
    }


    private async Task<bool> UserNameExists(string username)
    {
        return await _userManager.Users.AnyAsync(x => x.NormalizedUserName == username.ToUpper());
    }

    private async Task<AppUser> GetExistedUser(string userEmail)
    {
        var zxc = userEmail.ToUpper();
        var zxcv = await _userManager.Users.Select(x => x.NormalizedEmail).ToListAsync();
        
        return await _userManager.Users.FirstOrDefaultAsync(x => x.NormalizedEmail == userEmail.ToUpper()) ??
               throw new Exception("Użytkownik nie istnieje");
    }

    private async Task<bool> UserEmailExists(string email)
    {
        return await _userManager.Users.AnyAsync(x => x.NormalizedEmail == email.ToUpper());
    }
}