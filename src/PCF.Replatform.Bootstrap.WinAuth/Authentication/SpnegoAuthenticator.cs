﻿using Kerberos.NET;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using PivotalServices.CloudFoundry.Replatform.Bootstrap.Base;
using System;
using System.Web;

namespace PivotalServices.CloudFoundry.Replatform.Bootstrap.WinAuth.Authentication
{
    public class SpnegoAuthenticator : ISpnegoAuthenticator
    {
        private readonly IConfiguration configuration;
        private readonly ITicketIssuer issuer;
        private readonly ILogger<SpnegoAuthenticator> logger;

        public SpnegoAuthenticator(IConfiguration configuration, ITicketIssuer issuer, ILogger<SpnegoAuthenticator> logger)
        {
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            this.issuer = issuer ?? throw new ArgumentNullException(nameof(issuer));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public AuthenticateResult Authenticate(HttpContextBase contextBase)
        {
            AuthenticateResult authenticateResult = HandleAuthenticate(contextBase);

            if (authenticateResult?.Failure == null)
            {
                if ((authenticateResult?.Ticket)?.Principal != null)
                    logger.LogDebug($"AuthenticationSchemeAuthenticated, {AuthConstants.SPNEGO_DEFAULT_SCHEME}");
                else
                    logger.LogError($"AuthenticationSchemeNotAuthenticated, {AuthConstants.SPNEGO_DEFAULT_SCHEME}");
            }
            else
                logger.LogError($"AuthenticationSchemeNotAuthenticatedWithFailure, {AuthConstants.SPNEGO_DEFAULT_SCHEME}, {authenticateResult.Failure.Message}");

            return authenticateResult;
        }

        public void Challenge(AuthenticationProperties properties, HttpContextBase contextBase)
        {
            properties = properties ?? new AuthenticationProperties();
            HandleChallenge(properties, contextBase);
            logger.LogError($"AuthenticationSchemeChallenged, {AuthConstants.SPNEGO_DEFAULT_SCHEME}");
        }

        private AuthenticateResult HandleAuthenticate(HttpContextBase contextBase)
        {
            if (contextBase == null)
                throw new ArgumentNullException(nameof(contextBase));

            string authorizationHeader = contextBase.Request.Headers["Authorization"];

            if (string.IsNullOrEmpty(authorizationHeader))
                return AuthenticateResult.NoResult();

            if (!authorizationHeader.StartsWith($"{AuthConstants.SPNEGO_DEFAULT_SCHEME} ", StringComparison.OrdinalIgnoreCase))
                return AuthenticateResult.NoResult();

            var base64Token = authorizationHeader.Substring(AuthConstants.SPNEGO_DEFAULT_SCHEME.Length).Trim();

            if (string.IsNullOrEmpty(base64Token))
            {
                const string noCredentialsMessage = "No credentials";
                logger.LogInformation(noCredentialsMessage);
                return AuthenticateResult.Fail(noCredentialsMessage);
            }

            try
            {
                try
                {
                    logger.LogTrace($"===SPNEGO Token==={Environment.NewLine}{base64Token}");

                    return AuthenticateResult.Success(issuer.Authenticate(base64Token));
                }
                catch (KerberosValidationException e)
                {
                    return AuthenticateResult.Fail(e);
                }
            }
            catch
            {
                return AuthenticateResult.Fail("Access denied!");
            }
        }

        private void HandleChallenge(AuthenticationProperties properties, HttpContextBase contextBase)
        {
            contextBase.Response.StatusCode = 401;
            contextBase.Response.Headers.Set(HeaderNames.WWWAuthenticate, AuthConstants.SPNEGO_DEFAULT_SCHEME);
        }
    }
}