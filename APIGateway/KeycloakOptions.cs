namespace APIGateway;

public class KeycloakOptions
{
    public const string SectionName = "Keycloak";

    public string Realm { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public bool RequireHttpsMetadata { get; set; } = true;
    public bool ValidateIssuer { get; set; } = true;
    public bool ValidateAudience { get; set; } = true;
    public bool ValidateLifetime { get; set; } = true;
    public bool ValidateIssuerSigningKey { get; set; } = true;
    public int ClockSkewMinutes { get; set; } = 5;
    public string RoleClaimType { get; set; } = "realm_access.roles";
    public string NameClaimType { get; set; } = "preferred_username";
    public bool MapInboundClaims { get; set; } = false;
}