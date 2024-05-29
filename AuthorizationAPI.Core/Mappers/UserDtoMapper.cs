using AuthorizationAPI.Core.Responses;
using AuthorizationAPI.Infrastructure.Models;

namespace AuthorizationAPI.Core.Mappers
{
    public static class UserDtoMapper
    {
        public static UserListResponse.UserListEntry MapToUserListEntry(this User user)
        {
            return new UserListResponse.UserListEntry
            {
                Email = user.Email,
                IsLoginDisabled = user.AccessTerminated,
                Permissions = user.Permissions,
                Id = user.Id.ToString(),
                Ip = user.LastAccessDeviceNetworkAddress,
                LastLogin = user.LastLoginTime,
                CompanyId = user.Company              
            };
        }
    }
}
