namespace AuthorizationAPI.Core.Settings;

public class Jwt
{
    /// <summary>
    /// Token signature key
    /// </summary>
    public required string SigningKey { get; set; }
    /// <summary>
    /// Duration of token lifetime before expiring (in minutes)
    /// </summary>
    public int TokenLifeTime { get; set; }
    /// <summary>
    /// Duration of token lifetime before expiring (in days)
    /// </summary>
    public int RefreshTokenLifeTime { get; set; }
    /// <summary>
    /// Name of token issuer
    /// </summary>
    public required string Issuer { get; set; }
    /// <summary>
    /// Token should be valid only for these audiences
    /// </summary>
    public required List<string> Audiences { get; set; }
}