using AuthorizationAPI.Infrastructure.Models;
using MongoDB.Bson;

namespace AuthorizationAPI.Infrastructure.Repositories;

public interface IUserRepository
{
    public Task InsertNewUser(User newUser);
    public Task<User?> RetrieveUser(ObjectId id);
    public Task<User?> RetrieveUser(string email);
    public Task<bool> DoesUserExist(string email);
    public Task<bool> UpdateUser(User updatedUser);
    public Task<bool> UpdateUserSettings(ObjectId userId, AppSettings updatedUserSettings);
    public Task<bool> UpdateUserLastLogin(ObjectId userId, string? networkAddress, DateTime loginTime, string refreshToken, DateTime refreshTokenExpiration);
    public Task<bool> DeleteUser(ObjectId id);
    public Task<List<User>> GetUsers();
    Task<bool> DoesUserExist(ObjectId objectId);
}