namespace Hirehub.Domain.Enums;

public static class AppRoles
{
    public const string Admin = "Admin";
    public const string Client = "Client";
    public const string Freelancer = "Freelancer";

    public static readonly string[] All = { Admin, Client, Freelancer };
}