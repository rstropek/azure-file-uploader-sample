using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace FileUploaders.Functions
{
    public interface IAuthorize
    {
        Task<ClaimsPrincipal?> GetUser(IHeaderDictionary headers);
        string? GetSubject(ClaimsPrincipal principal);
        Task<string?> TryGetSubject(IHeaderDictionary headers);
    }

    public class Authorize : IAuthorize
    {
        private readonly IConfiguration configuration;
        private readonly ILogger<Authorize> logger;
        private readonly string domain;
        private readonly string audience;
        private readonly JwtSecurityTokenHandler jwtHandler;
        private OpenIdConnectConfiguration? openIdConfiguration;

        public Authorize(IConfiguration configuration, ILogger<Authorize> logger)
        {
            this.configuration = configuration;
            this.logger = logger;
            domain = configuration["Authority"];
            audience = configuration["Audience"];
            jwtHandler = new JwtSecurityTokenHandler();
        }

        public async Task<ClaimsPrincipal?> GetUser(IHeaderDictionary headers)
        {
            const string bearerPrefix = "Bearer ";
            if (!headers.TryGetValue(HeaderNames.Authorization, out var authorizationHeaders)
                || !authorizationHeaders.Any())
            {
                return null;
            }

            var authorizationHeader = authorizationHeaders.First();
            if (!authorizationHeader.StartsWith(bearerPrefix, StringComparison.OrdinalIgnoreCase))
            {
                logger.LogWarning($"Bearer prefix not found in authorization token");
                return null;
            }

            var token = authorizationHeader[bearerPrefix.Length..];
            openIdConfiguration ??= await GetConfiguration();
            var validationParameters = new TokenValidationParameters
            {
                ValidIssuer = domain,
                ValidAudiences = new[] { audience },
                IssuerSigningKeys = openIdConfiguration.SigningKeys
            };
            for (var retryCount = 0; retryCount < 2; retryCount++)
            {
                try
                {
                    var user = jwtHandler.ValidateToken(token, validationParameters, out var validatedToken);
                    return user;
                }
                catch (Exception) when (retryCount == 0)
                {
                    // Refresh OpenID configuration and retry token validation
                    openIdConfiguration = await GetConfiguration();
                    validationParameters.IssuerSigningKeys = openIdConfiguration.SigningKeys;
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Exception when validating bearer token");
                }
            }

            return null;
        }

        public string? GetSubject(ClaimsPrincipal principal)
            => principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

        public async Task<string?> TryGetSubject(IHeaderDictionary headers)
        {
            var user = await GetUser(headers);
            if (user == null)
            {
                return null;
            }

            return GetSubject(user);
        }

        private async Task<OpenIdConnectConfiguration> GetConfiguration()
        {
            logger.LogInformation("Getting OpenID configuration");
            var configurationManager = new Microsoft.IdentityModel.Protocols.ConfigurationManager<OpenIdConnectConfiguration>(
                "https://login.microsoftonline.com/common/v2.0/.well-known/openid-configuration",
                new OpenIdConnectConfigurationRetriever());
            return await configurationManager.GetConfigurationAsync(CancellationToken.None);
        }
    }
}