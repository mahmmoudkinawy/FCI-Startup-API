namespace API.Helpers;
public sealed class Constants
{
    public const string CorsPolicyName = "default";
    public const string CorsOriginSectionKey = "CrossOriginRequests:AllowedOrigins";

    public const string DefaultConnection = "DefaultConnection";

    public const string TokenKey = "Token:Key";

    public sealed class MailgunSettings
    {
        public const string ApiKey = "MailgunSettings:ApiKey";
        public const string ApiUrl = "MailgunSettings:ApiUrl";
        public const string SandBoxDomain = "MailgunSettings:SandBoxDomain";
        public const string From = "MailgunSettings:From";
    }

    public sealed class CloudinarySettings
    {
        public const string CloudName = "CloudinarySettings:CloudName";
        public const string ApiKey = "CloudinarySettings:ApiKey";
        public const string ApiSecret = "CloudinarySettings:ApiSecret";
    }
}
