using System.Security.Claims;
using AuthorizationAPI.Core;
using AuthorizationAPI.Core.Requests;
using AuthorizationAPI.Core.Requests.Validators;
using AuthorizationAPI.Core.Responses;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthorizationAPI.Controllers
{
    /// <summary>
    /// Manages users
    /// </summary>
    /// <param name="logger">Logging service</param>
    /// <param name="httpContextAccessor">Request context</param>
    /// <param name="userManagementService">User management service</param>
    [Route("[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "SysAdmin")]
    public class UsersController(ILogger<UsersController> logger, IHttpContextAccessor httpContextAccessor, IUserManagementService userManagementService) : ControllerBase
    {
        /// <summary>
        /// Creates user
        /// </summary>
        /// <param name="userRequest">User creation request</param>
        /// <returns>Created user on success</returns>
        [HttpPost]
        [ProducesResponseType(typeof(UserListResponse.UserListEntry), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(string), StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<UserListResponse.UserListEntry>> CreateUser(CreateUserRequest userRequest)
        {
            var userIp = httpContextAccessor?.HttpContext?.Connection?.RemoteIpAddress?.ToString();
            var requestMadeBy = httpContextAccessor?.HttpContext?.User.Claims.First(x => x.Type == ClaimTypes.Email);
            logger.LogInformation("User registration attempt by: {userIp}, {email}", userIp, requestMadeBy);
            var result = await userManagementService.CreateNewUser(userRequest);
            if (result is null)
                return BadRequest("Nepavyko sukurti vartotojo");
            return CreatedAtAction(nameof(GetUser), new { id = result.Id }, result);
        }

        /// <summary>
        /// Retrieves specific user by id
        /// </summary>
        /// <param name="id">User identification</param>
        /// <returns>User details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(UserListResponse.UserListEntry), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(string), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserListResponse.UserListEntry>> GetUser([IdentifierValidator] string id)
        {
            var user = await userManagementService.RetrieveUser(id);
            if (user is null) return NotFound("Toks vartotojas nerastas");
            return Ok(user);
        }

        /// <summary>
        /// Retrieves users
        /// </summary>
        /// <returns>User list</returns>
        [HttpGet]
        [ProducesResponseType(typeof(UserListResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(string), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserListResponse>> GetUsers()
        {
            return Ok(await userManagementService.RetrieveUsers());
        }

        /// <summary>
        /// Deletes specific user by id
        /// </summary>
        /// <param name="id">User identification</param>
        /// <returns>Delete users data or error reasoning</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(UserListResponse.UserListEntry), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(string), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserListResponse.UserListEntry>> DeleteUser([IdentifierValidator]string id)
        {
            var exists = await userManagementService.UserExists(id);
            if (!exists) return NotFound("Toks vartotojas nerastas");

            var user = await userManagementService.RetrieveUser(id);

            if (await userManagementService.DeleteUser(id))
            {
                return Ok(user);
            }
            return BadRequest("Ištrinti nepavyko");
        }

        /// <summary>
        /// Updates user details by specific id
        /// </summary>
        /// <param name="updateUserDetailsRequest">User update request</param>
        /// <param name="id">User identification</param>
        /// <returns>Updated user on success</returns>
        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(UserListResponse.UserListEntry), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(string), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserListResponse.UserListEntry>> UpdateUserDetails(UpdateUserDetailsRequest updateUserDetailsRequest, [IdentifierValidator]string id)
        {
            var exists = await userManagementService.UserExists(id!);
            if (!exists)
                return NotFound("Toks vartotojas nerastas");

            var result = await userManagementService.UpdateUsersDetails(updateUserDetailsRequest, id);
            if (result.Email is null || result.Permissions is null || result.Password is null)
                return BadRequest("Nepavyko atnaujinti vartotojo");

            return Ok(result);
        }

    }
}
