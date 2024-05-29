using AuthorizationAPI.Core.Mappers;
using AuthorizationAPI.Core.Requests;
using AuthorizationAPI.Core.Responses;
using AuthorizationAPI.Infrastructure.Models;
using AuthorizationAPI.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;
using System.Collections.Specialized;


namespace AuthorizationAPI.Core
{
    public class UserManagementService(IPasswordHasher<User> passwordHasher, IUserRepository userRepository) : IUserManagementService
    {
        public async Task<UserListResponse.UserListEntry?> CreateNewUser(CreateUserRequest userRequest)
        {
            if (await userRepository.DoesUserExist(userRequest.Email))
                return null;
            var newUser = new User { Email = userRequest.Email, Permissions = userRequest.Permissions, Company = userRequest.CompanyId };
            var passwordHash = passwordHasher.HashPassword(newUser, userRequest.Password);
            newUser.Password = passwordHash;
            await userRepository.InsertNewUser(newUser);

            return newUser.MapToUserListEntry();
        }

        public async Task<bool> DeleteUser(string id)
        {
            return await userRepository.DeleteUser(new ObjectId(id));
        }

        public async Task<UpdateUserDetailsResponse> UpdateUsersDetails(UpdateUserDetailsRequest updateUserRequest, string id)
        {
            var updatedUserResponse = new UpdateUserDetailsResponse();
            var user = await userRepository.RetrieveUser(new ObjectId(id));
            if (user == null)
                return updatedUserResponse;
            user.Permissions = updateUserRequest.Permissions!;
            user.Email = updateUserRequest.Email!;
            if (!string.IsNullOrEmpty(updateUserRequest.Password) &&
                !string.IsNullOrEmpty(updateUserRequest.PasswordRepeat) &&
                updateUserRequest.Password == updateUserRequest.PasswordRepeat)
            {
                user.Password = passwordHasher.HashPassword(user, updateUserRequest.Password);
            }
            user.AccessTerminated = updateUserRequest.IsAccessTerminated;
            if (user.AccessTerminated)
            {
                user.RefreshToken = null;
                user.RefreshTokenExpiryTime = null;
            }
            if(!string.IsNullOrEmpty(updateUserRequest.Company))
            {
                user.Company = updateUserRequest.Company;
            }
            updatedUserResponse = new UpdateUserDetailsResponse
            {
                Email = updateUserRequest.Email,
                Password = updateUserRequest.Password,
                Permissions = updateUserRequest.Permissions
            };
            await userRepository.UpdateUser(user);
            return updatedUserResponse;
        }

        public async Task<UserListResponse> RetrieveUsers()
        {
            var usersList = await userRepository.GetUsers();
            return new UserListResponse { Users = usersList.Select(user => user.MapToUserListEntry()).ToList() };
        }

        public async Task<UserListResponse.UserListEntry?> RetrieveUser(string id)
        {
            var user = await userRepository.RetrieveUser(new ObjectId(id));
            return user?.MapToUserListEntry();
        }

        public Task<bool> UserExists(string id)
        {
            return userRepository.DoesUserExist(new ObjectId(id));
        }
    }
}
