using AuthorizationAPI.Infrastructure.Models;
using AuthorizationAPI.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;

namespace AuthorizationAPI.Extensions;

public static class MongoDbSetupExtensions
{
    public static void AuthorizationDbInitialisation(this IMongoDatabase db, IPasswordHasher<User> passwordHasher)
    {
        const string rootUser = "root@ingmartus.lt";
        var usersCollection = db.GetCollection<User>(UserRepository.CollectionName);
        var user = usersCollection.Find(x => x.Email == rootUser).FirstOrDefault();
        if (user != null) return;
        const string secureRandomString = "admin";
        user = new User
        {
            Id = default,
            Email = rootUser,
            Password = null!,
            Permissions = ["SysAdmin"],
            UserAppSettings = new AppSettings(),
            AccessTerminated = false,
            RefreshToken = null,
            RefreshTokenExpiryTime = null
        };

        user.Password = passwordHasher.HashPassword(user, secureRandomString);
        usersCollection.InsertOne(user);
    }
}