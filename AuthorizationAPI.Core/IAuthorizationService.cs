using AuthorizationAPI.Core.Requests;
using AuthorizationAPI.Core.Responses;
using Microsoft.AspNetCore.Identity.Data;

namespace AuthorizationAPI.Core;

public interface IAuthorizationService
{
    /// <summary>
    /// Validates user sign-in details and returns authorization, refresh tokens on success
    /// </summary>
    /// <param name="request">Login details</param>
    /// <param name="userIp">Client machine network address</param>
    /// <returns>Authorization details if user sign-in is successful</returns>
    public Task<LoginResponse> ValidateUserCredentials(LoginRequest request, string? userIp);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <param name="userIp"></param>
    /// <returns></returns>
    public Task<LoginResponse> RefreshUserToken(TokenRefreshRequest request, string? userIp);
}