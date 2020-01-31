using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using ShadySoft.Blazor.WebAssembly.Authentication.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ShadySoftBlazorWebAssemblyAuthServiceServiceCollectionExtensions
    {
        public static IServiceCollection AddWebAssemblyAuthenticationService<TCredential>(this IServiceCollection services) =>
            services.AddWebAssemblyAuthenticationService<TCredential>(null);

        public static IServiceCollection AddWebAssemblyAuthenticationService<TCredential>(this IServiceCollection services, Action<ClientAuthenticationOptions> setupAction)
        {
            services.AddAuthorizationCore();
            services.AddSingleton<AuthenticationService<TCredential>>();
            services.AddSingleton<AuthenticationStateProvider>(sp => sp.GetRequiredService<AuthenticationService<TCredential>>());

            if (setupAction != null)
            {
                services.Configure(setupAction);
            }

            return services;
        }
    }
}
