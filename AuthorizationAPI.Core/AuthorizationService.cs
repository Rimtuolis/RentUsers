using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AuthorizationAPI.Core.Requests;
using AuthorizationAPI.Core.Responses;
using AuthorizationAPI.Core.Settings;
using AuthorizationAPI.Infrastructure.Models;
using AuthorizationAPI.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace AuthorizationAPI.Core;

public class AuthorizationService(IPasswordHasher<User> passwordHasher, IUserRepository userRepository, TokenValidationParameters tokenValidationParameters, IOptions<Jwt> jwtConfig)
    : IAuthorizationService
{
    public async Task<LoginResponse> ValidateUserCredentials(LoginRequest request, string? userIp)
    {
        var loginResponse = new LoginResponse();
        var user = await userRepository.RetrieveUser(request.Email);

        if (user is null)
        {
            return loginResponse;
        }

        var passwordVerificationResult = passwordHasher.VerifyHashedPassword(user, user.Password, request.Password);

        if (passwordVerificationResult is not (PasswordVerificationResult.Success
            or PasswordVerificationResult.SuccessRehashNeeded)) return loginResponse;

        var refreshToken = GenerateRefreshToken();
        var refreshTokenExpiration = DateTime.Now.AddDays(jwtConfig.Value.RefreshTokenLifeTime);
        loginResponse = new LoginResponse
        {
            Token = await CreateTokenAsync(user),
            RefreshToken = refreshToken
        };
        await userRepository.UpdateUserLastLogin(user.Id, userIp, DateTime.Now, refreshToken, refreshTokenExpiration);

        return loginResponse;
    }

    public async Task<LoginResponse> RefreshUserToken(TokenRefreshRequest request, string? userIp)
    {
        var loginResponse = new LoginResponse();

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(request.AccessToken, tokenValidationParameters, out _);
        var email = principal.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
        if (email == null) 
            return loginResponse;

        var user = await userRepository.RetrieveUser(email);
        if (user.RefreshTokenExpiryTime >= DateTime.Now && user.AccessTerminated && user.RefreshToken != request.RefreshToken) 
            return loginResponse;
        var refreshToken = GenerateRefreshToken();
        var refreshTokenExpiration = DateTime.Now.AddDays(jwtConfig.Value.RefreshTokenLifeTime);
        await userRepository.UpdateUserLastLogin(user.Id, userIp, DateTime.Now, refreshToken, refreshTokenExpiration);
        return new LoginResponse
        {
            Token = await CreateTokenAsync(user),
            RefreshToken = refreshToken
        };

    }

    private Task<string> CreateTokenAsync(User user)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.Email, user.Email)
        };
        claims.AddRange(user.Permissions.Select(x => new Claim(ClaimTypes.Role, x)));
        claims.Add(new Claim("CompanyReference",user.Company ?? ""));
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.Value.SigningKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.Now.Add(TimeSpan.FromMinutes(jwtConfig.Value.TokenLifeTime)),
            Issuer = jwtConfig.Value.Issuer,
            Audience = "Rent.Transport",
            SigningCredentials = credentials
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return Task.FromResult(tokenHandler.WriteToken(token));
    }

    private static string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}