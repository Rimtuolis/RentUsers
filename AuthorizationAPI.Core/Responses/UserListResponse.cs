namespace AuthorizationAPI.Core.Responses
{
    public class UserListResponse
    {
        public List<UserListEntry> Users { get; set; } = [];
        public class UserListEntry
        {
            public required string Id { get; set; }
            public required string Email { get; set; }
            public string Password { get; set; } = "";
            public List<string> Permissions { get; set; } = [];
            public string? Ip { get; set; }
            public bool IsLoginDisabled { get; set; }
            public DateTime? LastLogin { get; set; }
            public string? CompanyId { get; set; }   
        }
    }
}
