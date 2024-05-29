using AuthorizationAPI.Core.Requests;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using AuthorizationAPI.Core.Responses;
using IAuthorizationService = AuthorizationAPI.Core.IAuthorizationService;

namespace AuthorizationAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AuthorizationController(ILogger<AuthorizationController> logger, IHttpContextAccessor httpContextAccessor, IAuthorizationService authorizationService) : ControllerBase
    {
        /// <summary>
        /// Executes user existence and credentials validity
        /// </summary>
        /// <param name="loginRequest">Client login credentials</param>
        /// <returns>Authorization details if successful</returns>
        [AllowAnonymous]
        [HttpPost("/login")]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<LoginResponse>> Login(LoginRequest loginRequest)
        {
            var userIp = httpContextAccessor?.HttpContext?.Connection?.RemoteIpAddress?.ToString();
            logger.LogInformation("Login attempt by: {userIp}, {email}", userIp, loginRequest.Email);
            var result = await authorizationService.ValidateUserCredentials(loginRequest, userIp);
            if (string.IsNullOrEmpty(result.Token) || string.IsNullOrEmpty(result.RefreshToken))
            {
                return BadRequest("Neteisingi prisijungimo duomenys");
            }
            return Ok(result);
        }
        [AllowAnonymous]
        [HttpPost("/refresh")]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<LoginResponse>> Refresh(TokenRefreshRequest refreshRequest)
        {
            var userIp = httpContextAccessor?.HttpContext?.Connection?.RemoteIpAddress?.ToString();
            logger.LogInformation("Refresh token attempt by: {userIp}, {refreshTokenId}", userIp, refreshRequest.RefreshToken);

            if (string.IsNullOrEmpty(refreshRequest.AccessToken) || string.IsNullOrEmpty(refreshRequest.RefreshToken))
            {
                return BadRequest("Neteisingai pateikta pratęsimo užklausa");
            }

            var result = await authorizationService.RefreshUserToken(refreshRequest, userIp);
            return Ok(result);
        }
    }
}
