using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Novell.Directory.Ldap;

namespace Fair.Services
{
    public interface IADService
    {
        bool Authenticate(string username, string password);
    }

    public class ADService : IADService
    {
        private readonly string domain;

        private readonly ILogger logger;

        public ADService(IConfiguration config, ILogger<ADService> logger)
        {
            domain = config.GetValue<string>("ActiveDirectory:Domain");
            this.logger = logger;
        }

        public bool Authenticate(string username, string password)
        {
            bool authenticated = false;
            try
            {
                using (var connection = new LdapConnection())
                {
                    connection.Connect(domain, LdapConnection.DEFAULT_PORT);
                    connection.Bind($"AD\\{username}", password);
                    if (connection.Bound)
                    {
                        authenticated = true;
                        logger.LogInformation("AD authentication successful for {username}", username);
                    }
                }
            }
            catch (LdapException e)
            {
                logger.LogInformation(e, "LDAP Exception for {username}", username);
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
