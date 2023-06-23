using Dotnet7API.Repos;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;

namespace Dotnet7API.Helper
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly LearndataContextb _context;
        public BasicAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, LearndataContextb context, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
        {
            _context = context;
        }

        protected async override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                return AuthenticateResult.Fail("No header found");
            }

            var headerValue = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);

            if (headerValue != null)
            {
                var bytes = Convert.FromBase64String(headerValue.Parameter);
                string credentials = Encoding.UTF8.GetString(bytes);
                string[] array = credentials.Split(':');
                string username = array[0];
                string password = array[1];
                var user = await _context.TblUsers.FirstOrDefaultAsync(item => item.Code == username && item.Password == password);
                if (user != null)
                {
                    var claim = new[] { new Claim(ClaimTypes.Name, user.Code ) };
                    var identity = new ClaimsIdentity(claim, Scheme.Name);
                    var principal = new ClaimsPrincipal(identity);
                    var ticket = new AuthenticationTicket(principal, Scheme.Name);

                    return AuthenticateResult.Success(ticket);
                }
                else
                {
                    return AuthenticateResult.Fail("UnAuthorized");
                }

            }
            else
            {
                return AuthenticateResult.Fail("Empty Header");
            }
        }
    }
}
