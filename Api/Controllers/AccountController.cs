using Microsoft.AspNetCore.Mvc;
using SharedWarehousingCore.Dtos.IdentityDTOs;
using SharedWarehousingCore.Services.AccountServices;

namespace Api.Controllers;

public class AccountController : BaseApiController
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            var result = await _accountService.Register(registerDto);
            if (result is not null)
            {
                return Ok(result);
            }

            return Unauthorized();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            var result = await _accountService.Login(loginDto);
            if (result is not null)
            {
                return Ok(result);
            }

            return Unauthorized();
        }
        
        [HttpPost("createRole")]
        public async Task<IActionResult> CreateRole(string roleName)
        {
            var result = await _accountService.CreateRole(roleName);
            if (result)
            {
                return Ok();
            }

            return BadRequest();
        }

        [HttpPost("assignRole")]
        public async Task<IActionResult> AssignRole(UserRolesDto input)
        {
            var result = await _accountService.AssignRoles(input);
            
            if (result)
            {
                return Ok();
            }

            return BadRequest();
        }
        
        [HttpPost("unassignRole")]
        public async Task<IActionResult> UnAssignRole(UserRolesDto input)
        {
            var result = await _accountService.UnassignRoles(input);
            
            if (result)
            {
                return Ok();
            }

            return BadRequest();
        }

        [HttpPost("refreshtoken")]
        public async Task<IActionResult> RefreshToken(RefreshTokenDto tokenModel)
        {
            return Ok(await _accountService.RefreshToken(tokenModel));
        }
    }