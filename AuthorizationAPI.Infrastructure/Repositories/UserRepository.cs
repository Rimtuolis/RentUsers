using AuthorizationAPI.Infrastructure.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace AuthorizationAPI.Infrastructure.Repositories;

public class UserRepository(IMongoDatabase database) : IUserRepository
{
    private readonly IMongoCollection<User> _usersCollection = database.GetCollection<User>(CollectionName);
    public const string CollectionName = "users";

    public async Task InsertNewUser(User newUser)
    { 
        await _usersCollection.InsertOneAsync(newUser);
    }

    public async Task<User?> RetrieveUser(ObjectId id)
    {
        return await _usersCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
    }

    public async Task<User?> RetrieveUser(string email)
    {
        return await _usersCollection.Find(x => x.Email == email).FirstOrDefaultAsync();
    }

    public async Task<bool> DoesUserExist(string email)
    {
        var existingUser = await _usersCollection.Find(x => x.Email == email).CountDocumentsAsync();
        return existingUser == 1;
    }
    public async Task<bool> DoesUserExist(ObjectId id)
    {
        var existingUser = await _usersCollection.Find(x => x.Id == id).CountDocumentsAsync();
        return existingUser == 1;
    }

    public async Task<bool> UpdateUser(User updatedUser)
    {
        var updateDefinition = Builders<User>.Update
            .Set(x => x.Email, updatedUser.Email)
            .Set(x => x.Password, updatedUser.Password)
            .Set(x => x.Permissions, updatedUser.Permissions)
            .Set(x => x.RefreshToken, updatedUser.RefreshToken)
            .Set(x => x.RefreshTokenExpiryTime, updatedUser.RefreshTokenExpiryTime)
            .Set(x => x.Company, updatedUser.Company)
            .Set(x => x.AccessTerminated, updatedUser.AccessTerminated);
        var updateResult = await _usersCollection.UpdateOneAsync(x => x.Id == updatedUser.Id, updateDefinition);
        return updateResult.IsAcknowledged;
    }

    public async Task<bool> UpdateUserSettings(ObjectId userId, AppSettings updatedUserSettings)
    {
        var updateDefinition = Builders<User>.Update
            .Set(x => x.UserAppSettings, updatedUserSettings);
        var updateResult = await _usersCollection.UpdateOneAsync(x => x.Id == userId, updateDefinition);
        return updateResult.IsAcknowledged;
    }

    public async Task<bool> UpdateUserLastLogin(ObjectId userId, string? networkAddress, DateTime loginTime, string refreshToken,
        DateTime refreshTokenExpiration)
    {
        var updateDefinition = Builders<User>.Update
            .Set(x => x.LastAccessDeviceNetworkAddress, networkAddress)
            .Set(x => x.LastLoginTime, loginTime)
            .Set(x => x.RefreshToken, refreshToken)
            .Set(x => x.RefreshTokenExpiryTime, refreshTokenExpiration);
        var updateResult = await _usersCollection.UpdateOneAsync(x => x.Id == userId, updateDefinition);
        return updateResult.IsAcknowledged;
    }

    public async Task<bool> DeleteUser(ObjectId id)
    {
        var deleteFilter = Builders<User>.Filter.Eq(x => x.Id, id);
        var result = await _usersCollection.DeleteOneAsync(deleteFilter);
        return result.IsAcknowledged && result.DeletedCount == 1;
    }

    public async Task<List<User>> GetUsers()
    {
        return await _usersCollection.Find(Builders<User>.Filter.Empty).ToListAsync();
    }
}