namespace Domains.API.Helpers
{
    public class SettingsHelper
    {
        private readonly IConfiguration _config;
        public readonly string _apiKey;
        public SettingsHelper(IConfiguration configuration)
        {
            _config = configuration
                ?? throw new ArgumentNullException(nameof(configuration));
            _apiKey = _config["APIKey"] ?? string.Empty;
        }
    }
}
