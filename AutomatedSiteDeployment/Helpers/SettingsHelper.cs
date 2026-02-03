using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace AutomatedSiteDeployment.Helpers
{
    internal class SettingsHelper
    {
        private readonly IConfiguration _config;
        public readonly string _domainsAPIBase;
        public readonly string _apiKey;
        public readonly string _stgUsername;
        public readonly string _stgPassword;
        public readonly string _AZ102Username;
        public readonly string _AZ102Password;
        public readonly string _AZ201Username;
        public readonly string _AZ201Password;
        public readonly string _stgPath;
        public readonly string _AZ102Path;
        public readonly string _AZ201Path;

        public SettingsHelper(IConfiguration configuration)
        {
            _config = configuration
                ?? throw new ArgumentNullException(nameof(configuration));
            _domainsAPIBase = _config["DomainsAPI:BaseURL"] ?? string.Empty;
            _apiKey = _config["APIKey"] ?? string.Empty;
            _stgUsername = _config["ImpersonationCredentials:Staging:Username"] ?? string.Empty;
            _stgPassword = _config["ImpersonationCredentials:Staging:Password"] ?? string.Empty;
            _AZ102Username = _config["ImpersonationCredentials:AZ102:Username"] ?? string.Empty;
            _AZ102Password = _config["ImpersonationCredentials:AZ102:Password"] ?? string.Empty;
            _AZ201Username = _config["ImpersonationCredentials:AZ201:Username"] ?? string.Empty;
            _AZ201Password = _config["ImpersonationCredentials:AZ201:Password"] ?? string.Empty;
            _stgPath = _config["ServerPaths:Staging"] ?? string.Empty;
            _AZ102Path = _config["ServerPaths:AZ102"] ?? string.Empty;
            _AZ201Path = _config["ServerPaths:AZ201"] ?? string.Empty;
        }
    }
}
