using Models.Shared.Models.Domains;
namespace Domains.API.Helpers
{
    public class ValidationHelper
    {
        public ValidationHelper()
        {
        }

        public void ValidateDomain(ref Domain domain)
        {
            string message = string.Empty;

            if (string.IsNullOrWhiteSpace(domain.DomainName))
            {
                message += "DomainName is required. ";
            }
            if (string.IsNullOrWhiteSpace(domain.Status))
            {
                message += "Status is required. ";
            }

            string validPattern = "^[a-zA-Z0-9-]+\\.(com|org|net)$";
            if (!System.Text.RegularExpressions.Regex.IsMatch(domain.DomainName, validPattern))
            {
                message += "DomainName format is invalid. ";
            }

            List<string> validStatuses = new List<string> { "active", "inactive", "pending" };
            if (!validStatuses.Contains(domain.Status.ToLower()))
            {
                message += "Status value is invalid. ";
            }
            
            if (!string.IsNullOrWhiteSpace(message))
            {
                domain.Success = false;
                domain.Message = message.Trim();
            }
        }

        public bool ValidateHostedSiteDetails(HostedSiteDetails hostedSiteDetails, ref string message)
        {
            if (string.IsNullOrWhiteSpace(hostedSiteDetails.HostingProvider))
            {
                message += "HostingProvider is required. ";
            }
            if (hostedSiteDetails.RenewalDate == DateTime.MinValue)
            {
                message += "RenewalDate is required. ";
            }
            else if (hostedSiteDetails.RenewalDate < DateTime.Now)
            {
                message += "RenewalDate cannot be in the past. ";
            }
            List<string> validServerNames = new List<string> { "AZ102", "AZ201" };
            if (string.IsNullOrWhiteSpace(hostedSiteDetails.ServerName))
            {
                message += "ServerName is required. ";
            }

            return string.IsNullOrWhiteSpace(message);
        }
    }
}
