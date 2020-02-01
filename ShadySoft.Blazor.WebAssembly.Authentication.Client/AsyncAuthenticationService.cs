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
    public class AsyncAuthenticationService<TCredential> : AuthenticationStateProvider, IAuthenticationService<TCredential>
    {
        private Task<AuthenticationState> authenticationStateTask;
        private readonly ClientAuthenticationOptions _options;
        private readonly HttpClient _http;

        private static AuthenticationState newUnauthenticatedState = new AuthenticationState(new ClaimsPrincipal());
        private static Task<AuthenticationState> newUnauthenticatedStateTask => Task.FromResult(newUnauthenticatedState);

        public SignInResult LastSignInResult { get; private set; } = SignInResult.Success;

        public AsyncAuthenticationService(IOptions<ClientAuthenticationOptions> optionsAccessor, HttpClient http)
        {
            _options = optionsAccessor.Value;
            _http = http;
        }

        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            if (authenticationStateTask is null)
            {
                var resultSource = new TaskCompletionSource<bool>();
                var stateSource = new TaskCompletionSource<AuthenticationState>();

                authenticationStateTask = stateSource.Task;

                ServerFetchUserInfoAsync(resultSource, stateSource);
            }

            return authenticationStateTask;
        }

        public Task<SignInResult> SignInAsync(TCredential credentials)
        {
            var resultSource = new TaskCompletionSource<SignInResult>();
            var stateSource = new TaskCompletionSource<AuthenticationState>();

            authenticationStateTask = stateSource.Task;
            NotifyAuthenticationStateChanged(authenticationStateTask);

            ServerSignInAsync(credentials, resultSource, stateSource);

            return resultSource.Task;
        }

        public async Task SignOutAsync()
        {
            await _http.PostAsync(_options.LogoutUrl, null);
            authenticationStateTask = newUnauthenticatedStateTask;
            NotifyAuthenticationStateChanged(authenticationStateTask);
        }

        public Task<bool> RefreshUserInfoAsync()
        {
            var resultSource = new TaskCompletionSource<bool>();
            var stateSource = new TaskCompletionSource<AuthenticationState>();

            authenticationStateTask = stateSource.Task;
            NotifyAuthenticationStateChanged(authenticationStateTask);

            ServerFetchUserInfoAsync(resultSource, stateSource);

            return resultSource.Task;
        }

        private async Task ServerSignInAsync(TCredential credentials, TaskCompletionSource<SignInResult> resultSource, TaskCompletionSource<AuthenticationState> stateSource)
        {
            try
            {
                var claims = await _http.PostJsonWithPdAsync<Dictionary<string, string>>(_options.LoginUrl, credentials);
                LastSignInResult = SignInResult.Success;
                stateSource.SetResult(BuildAuthenticationState(claims));
                resultSource.SetResult(SignInResult.Success);
            }
            catch (HttpRequestException e)
            {
                var result = e.GetSignInResult();
                LastSignInResult = result;
                stateSource.SetResult(newUnauthenticatedState);
                resultSource.SetResult(result);
            }
        }

        private async Task ServerFetchUserInfoAsync(TaskCompletionSource<bool> resultSource, TaskCompletionSource<AuthenticationState> stateSource)
        {
            try
            {
                var claims = await _http.GetJsonWithPdAsync<Dictionary<string, string>>(_options.UserUrl);
                stateSource.SetResult(BuildAuthenticationState(claims));
                resultSource.SetResult(true);
            }
            catch (HttpRequestException e)
            {
                if (e.GetProblemDetails().Status == 404)
                {
                    stateSource.SetResult(newUnauthenticatedState);
                    resultSource.SetResult(false);
                }
                else
                {
                    throw e;
                }
            }
        }

        private AuthenticationState BuildAuthenticationState(Dictionary<string, string> claims)
        {
            var identity = new ClaimsIdentity("WebApiAuth");

            foreach(var claim in claims)
                identity.AddClaim(new Claim(claim.Key, claim.Value));

            return new AuthenticationState(new ClaimsPrincipal(identity));
        }
    }
}
