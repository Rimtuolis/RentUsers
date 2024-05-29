using AuthorizationAPI.Core.Requests;
using AuthorizationAPI.Core.Responses;

namespace AuthorizationAPI.Core
{
    public interface IUserManagementService
    {
        public Task<UserListResponse.UserListEntry?> CreateNewUser(CreateUserRequest userRequest);
        public Task<UpdateUserDetailsResponse> UpdateUsersDetails(UpdateUserDetailsRequest updateUserRequest, string id);
        public Task<bool> DeleteUser(string id);
        public Task<UserListResponse> RetrieveUsers();
        public Task<UserListResponse.UserListEntry?> RetrieveUser(string id);
        public Task<bool> UserExists(string id);
    }
}
