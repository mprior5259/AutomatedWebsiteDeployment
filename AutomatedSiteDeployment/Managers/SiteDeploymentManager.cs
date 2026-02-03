using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutomatedSiteDeployment.Helpers;
using AutomatedSiteDeployment.Models;
using Models.Shared.Models.Domains;
using AutomatedSiteDeployment.Agents;

namespace AutomatedSiteDeployment.Managers
{
    internal class SiteDeploymentManager
    {
        private readonly string? domainName = string.Empty;
        private readonly string? stgPath = string.Empty;
        private readonly string? livePath = string.Empty;
        private readonly Users stgUser;
        private readonly Users liveUser;

        private readonly FileSystemAgent stgWorker; // Reader
        private readonly FileSystemAgent liveWorker; // Writer

        public SiteDeploymentManager(Domain domain, SettingsHelper settings)
        {
            domainName = domain.DomainName;
            stgPath = settings._stgPath;
            stgUser = new Users()
            {
                Username = settings._stgUsername,
                Password = settings._stgPassword,
                Server = "AZstg"
            };
            var liveServer = domain.HostedSiteDetails?.ServerName;
            if (liveServer == "AZ102")
            {
                livePath = settings._AZ102Path;
                liveUser = new Users()
                {
                    Username = settings._AZ102Username,
                    Password = settings._AZ201Password,
                    Server = "AZ102"
                };
            }
            else if (liveServer == "AZ201")
            {
                livePath = settings._AZ201Path;
                liveUser = new Users()
                {
                    Username = settings._AZ201Username,
                    Password = settings._AZ201Password,
                    Server = "AZ201"
                };
            }
            else
            {
                throw new Exception($"Unsupported live server: {liveServer}");
            }

            // Initialize our producer and consumer workers
            stgWorker = new FileSystemAgent(stgUser.Username, stgUser.Password, stgUser.Server);
            liveWorker = new FileSystemAgent(liveUser.Username, liveUser.Password, liveUser.Server);
        }
    }
}
