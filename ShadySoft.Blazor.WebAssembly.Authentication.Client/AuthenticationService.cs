using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;
using ShadySoft.HttpClientExtensions;

namespace ShadySoft.Blazor.WebAssembly.Authentication.Client
{
    public class AuthenticationService<TCredential> : AuthenticationStateProvider, IAuthenticationService<TCredential>
    {
        private AuthenticationState authenticationState;
        private readonly ClientAuthenticationOptions _options;
        private readonly HttpClient _http;

        private static AuthenticationState newUnauthenticatedState = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

        public SignInResult LastSignInResult { get; private set; } = SignInResult.Success;

        public AuthenticationService(IOptions<ClientAuthenticationOptions> optionsAccessor, HttpClient http)
        {
            _options = optionsAccessor.Value;
            _http = http;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            if (authenticationState is null)
            {
                await ServerRefreshUserInfoAsync();
            }

            return authenticationState;
        }

        public async Task SignOutAsync()
        {
            await _http.PostAsync(_options.LogoutUrl, null);
            authenticationState = newUnauthenticatedState;
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        public async Task<SignInResult> SignInAsync(TCredential credentials)
        {
            try
            {
                var claims = await _http.PostJsonWithPdAsync<Dictionary<string, string>>(_options.LoginUrl, credentials);
                authenticationState = BuildAuthenticationState(claims);
                NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
                LastSignInResult = SignInResult.Success;
            }
            catch (HttpRequestException e)
            {
                try
                {
                    LastSignInResult = e.GetSignInResult();
                }
                catch
                {
                    throw e;
                }
            }

            return LastSignInResult;
        }

        public async Task<bool> RefreshUserInfoAsync()
        {
            var result = await ServerRefreshUserInfoAsync();
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
            return result;
        }

        private async Task<bool> ServerRefreshUserInfoAsync()
        {
            try
            {
                var claims = await _http.GetJsonWithPdAsync<Dictionary<string, string>>(_options.UserUrl);
                authenticationState = BuildAuthenticationState(claims);
                return true;
            }
            catch (HttpRequestException e)
            {
                if (e.GetProblemDetails().Status == 404)
                {
                    authenticationState = newUnauthenticatedState;
                    return false;
                }
                else
                    throw e;
            }
        }



        private AuthenticationState BuildAuthenticationState(Dictionary<string, string> claims)
        {
            var identity = new ClaimsIdentity("WebApiAuth");

            foreach (var claim in claims)
                identity.AddClaim(new Claim(claim.Key, claim.Value));

            return new AuthenticationState(new ClaimsPrincipal(identity));
        }

    }
}
