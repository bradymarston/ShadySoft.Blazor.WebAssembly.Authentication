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
        public static IServiceCollection AddWebAssemblyAuthenticationService(this IServiceCollection services)
        {
            services.AddSingleton<AuthenticationService>();
            services.AddSingleton<AuthenticationStateProvider>(sp => sp.GetRequiredService<AuthenticationService>());
            return services;
        }
    }
}
