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
        public static IServiceCollection AddWebAssemblyAuthenticationService<TCredential>(this IServiceCollection services, bool useAsyncService = false) =>
            services.AddWebAssemblyAuthenticationService<TCredential>(null, useAsyncService);

        public static IServiceCollection AddWebAssemblyAuthenticationService<TCredential>(this IServiceCollection services, Action<ClientAuthenticationOptions> setupAction, bool useAsyncService = false)
        {
            services.AddAuthorizationCore();

            if (useAsyncService)
                services.AddSingleton<IAuthenticationService<TCredential>, AsyncAuthenticationService<TCredential>>();
            else
                services.AddSingleton<IAuthenticationService<TCredential>, AuthenticationService<TCredential>>();

            services.AddSingleton(sp => (AuthenticationStateProvider)sp.GetRequiredService<IAuthenticationService<TCredential>>());

            if (setupAction != null)
            {
                services.Configure(setupAction);
            }

            return services;
        }
    }
}
