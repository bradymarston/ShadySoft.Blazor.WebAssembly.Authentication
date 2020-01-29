using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ShadySoft.Blazor.WebAssembly.Authentication.Client
{
    public class AuthenticationService : AuthenticationStateProvider
    {
        private Task<AuthenticationState> authenticationStateTask = newUnauthenticatedStateTask;

        private static Task<AuthenticationState> newUnauthenticatedStateTask => Task.FromResult(new AuthenticationState(new ClaimsPrincipal()));

        public override Task<AuthenticationState> GetAuthenticationStateAsync() => authenticationStateTask;

        public Task<SignInResult> SignInAsync(Credentials credentials)
        {
            authenticationStateTask = BuildAuthenticationStateTask(credentials);

            NotifyAuthenticationStateChanged(authenticationStateTask);

            return Task.FromResult(SignInResult.Success);
        }

        public Task SignOutAsync()
        {
            authenticationStateTask = newUnauthenticatedStateTask;

            NotifyAuthenticationStateChanged(authenticationStateTask);

            return Task.CompletedTask;
        }

        private Task<AuthenticationState> BuildAuthenticationStateTask(Credentials credentials)
        {
            var identity = new ClaimsIdentity("WebApiAuth");

            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, credentials.UserName));
            identity.AddClaim(new Claim(ClaimsIdentity.DefaultNameClaimType, credentials.UserName));

            return Task.FromResult(new AuthenticationState(new ClaimsPrincipal(identity)));
        }
    }
}
