using MongoDB.Bson;

namespace AuthorizationAPI.Infrastructure.Models
{
    public class User
    {
        public ObjectId Id { get; set; }
        public required string Email { get; set; }
        public string Password { get; set; } = null!;
        public required List<string> Permissions { get; set; }
        public AppSettings UserAppSettings { get; set; } = new();
        public string LastAccessDeviceNetworkAddress { get; set; } = "";
        public DateTime? LastLoginTime { get; set; }
        public bool AccessTerminated { get; set; } = false;
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }

        public string? Company { get; set; } = null;
    }
}
