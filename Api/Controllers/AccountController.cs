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
        public async Task<IActionResult> Register([FromBody]RegisterDto registerDto)
        {
            var result = await _accountService.Register(registerDto);
            
            if (result is not null)
            {
                return Ok(result);
            }

            return Unauthorized();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody]LoginDto loginDto)
        {
            var result = await _accountService.Login(loginDto);
            
            if (result is not null)
            {
                return Ok(result);
            }

            return Unauthorized();
        }
        
        [HttpPost("createRole")]
        public async Task<IActionResult> CreateRole([FromBody]string roleName)
        {
            var result = await _accountService.CreateRole(roleName);
            if (result)
            {
                return Ok();
            }

            return BadRequest();
        }

        [HttpPost("assignRole")]
        public async Task<IActionResult> AssignRole([FromBody]UserRolesDto input)
        {
            var result = await _accountService.AssignRoles(input);
            
            if (result)
            {
                return Ok();
            }

            return BadRequest();
        }
        
        [HttpPost("unassignRole")]
        public async Task<IActionResult> UnAssignRole([FromBody]UserRolesDto input)
        {
            var result = await _accountService.UnassignRoles(input);
            
            if (result)
            {
                return Ok();
            }

            return BadRequest();
        }

        [HttpPost("refreshtoken")]
        public async Task<IActionResult> RefreshToken([FromBody]RefreshTokenDto tokenModel)
        {
            return Ok(await _accountService.RefreshToken(tokenModel));
        }

        [HttpPost("GeneratePasswordResetTokenByEmail")]
        public async Task<IActionResult> GeneratePasswordResetTokenByEmail([FromBody]string userEmail)
        {
            return Ok(await _accountService.GeneratePasswordResetTokenByEmail(userEmail));
        }

        [HttpPost("ChangeUserPassword")]
        public async Task<IActionResult> ChangeUserPassword([FromBody] ChangeUserPasswordDto input)
        {
            await _accountService.ChangeUserPassword(input.UserEmail, input.Token, input.NewPassword);
            return Ok();
        }

        [HttpGet("GenerateEmailConfirmationTokenAsync")]
        public async Task<IActionResult> GenerateEmailConfirmationTokenAsync([FromQuery] string userEmail)
        {
            return Ok(await _accountService.GenerateEmailConfirmationTokenAsync(userEmail));
        }

        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail([FromQuery]ConfirmEmailDto input)
        {
            await _accountService.ConfirmEmail(input.UserEmail, input.Token);
            return Ok();
        }
    }