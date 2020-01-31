using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShadySoft.Blazor.WebAssembly.Authentication.Client
{
    public class ClientAuthenticationOptions
    {
        /// <summary>
        /// Gets or sets the relative url used for posting credentials at login. Defaults to "/authentication/login".
        /// </summary>
        public string LoginUrl { get; set; } = "/authentication/login";
        /// <summary>
        /// Gets or sets the relative url used for posting at logout. Defaults to "/authentication/login".
        /// </summary>
        public string LogoutUrl { get; set; } = "/authentication/logout";
        /// <summary>
        /// Gets or sets the relative url used for getting claims for the current user. Defaults to "/authentication/login".
        /// </summary>
        public string UserUrl { get; set; } = "/authentication/user";
    }
}
