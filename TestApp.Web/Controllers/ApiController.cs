using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using TestApp.Core.Configuration;

namespace TestApp.Web.Controllers
{
    [Authorize, ApiController]
    [Produces("application/json"), Consumes("application/json")]
    public abstract class ApiController : Controller
    {
        protected readonly string AuthenticatedUser;

        public ApiController(IOptions<BasicAuthenticationConfiguration> authConfig) =>
            // typically authenticated user should be taken from claims etc, but hardcoded auth user is ok in case of test app.
            AuthenticatedUser = authConfig.Value.Username;
    }
}