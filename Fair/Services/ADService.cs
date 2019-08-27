using System.DirectoryServices.AccountManagement;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Fair.Services
{
    public interface IADService
    {
        bool Authenticate(string username, string password);
    }

    public class ADService : IADService
    {
        private readonly string path;

        private readonly ILogger logger;

        public ADService(IConfiguration config, ILogger<ADService> logger)
        {
            path = config.GetValue<string>("ActiveDirectory:Path");
            this.logger = logger;
        }

        public bool Authenticate(string username, string password)
        {
            bool authenticated = false;
            using (PrincipalContext pc = new PrincipalContext(ContextType.Domain, path))
            {
                authenticated = pc.ValidateCredentials(username, password);
                logger.LogInformation("Authenticate AD user {username}: {result}", username, authenticated);
            }

            return authenticated;
        }
    }

    public class MockADService : IADService
    {
        public bool Authenticate(string username, string password)
        {
            return true;
        }
    }
}
