using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace ShadySoft.Blazor.WebAssembly.Authentication.Client
{
    public interface IAuthenticationService<TCredential>
    {
        SignInResult LastSignInResult { get; }

        Task<AuthenticationState> GetAuthenticationStateAsync();
        Task<bool> RefreshUserInfoAsync();
        Task<SignInResult> SignInAsync(TCredential credentials);
        Task SignOutAsync();
    }
}