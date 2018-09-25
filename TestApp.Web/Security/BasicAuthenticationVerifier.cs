using System.Threading.Tasks;
using Bazinga.AspNetCore.Authentication.Basic;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TestApp.Core.Configuration;

namespace TestApp.Web.Security
{
    public class BasicAuthenticationVerifier : IBasicCredentialVerifier
    {
        public BasicAuthenticationVerifier(IOptions<BasicAuthenticationConfiguration> basicAuthenticationConfiguration, ILogger<BasicAuthenticationVerifier> logger)
        {
            BasicAuthenticationConfiguration = basicAuthenticationConfiguration;
            Logger = logger;
        }
        
        private IOptions<BasicAuthenticationConfiguration> BasicAuthenticationConfiguration { get; }
        private ILogger<BasicAuthenticationVerifier> Logger { get; }
        
        public Task<bool> Authenticate(string username, string password)
        {
            var result = username == BasicAuthenticationConfiguration.Value.Username && password == BasicAuthenticationConfiguration.Value.Password;
            if (!result) Logger.LogError($"Unsuccessful login attempt for basic auth: username: {username}; password: {password}.");
            return Task.FromResult(result);
        }
    }
}