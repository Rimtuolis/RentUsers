namespace AuthorizationAPI.Infrastructure.Models;

public class AppSettings
{
    public bool AppContentsMaxWidth = false;
    public TransportAppTableSettings TransportAppTableSettings { get; set; } = null!;
}