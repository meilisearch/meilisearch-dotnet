using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

using Microsoft.IdentityModel.Tokens;

namespace Meilisearch
{
    public class TenantToken
    {
        /// <summary>
        /// Generates a Tenant Token in a JWT string format.
        /// </summary>
        /// <returns>JWT string</returns>
        public static string GenerateToken(string apiKeyUid, TenantTokenRules searchRules, string apiKey, DateTime? expiresAt)
        {
            if (String.IsNullOrEmpty(apiKeyUid))
            {
                throw new MeilisearchTenantTokenApiKeyUidInvalid();
            }

            if (String.IsNullOrEmpty(apiKey) || apiKey.Length < 8)
            {
                throw new MeilisearchTenantTokenApiKeyInvalid();
            }

            if (expiresAt.HasValue && DateTime.Compare(DateTime.UtcNow, (DateTime)expiresAt) > 0)
            {
                throw new MeilisearchTenantTokenExpired();
            }

            var rules = searchRules.ToClaim();
            var isArray = rules is string[];
            var valueType = isArray ? JsonClaimValueTypes.JsonArray : JsonClaimValueTypes.Json;

            var identity = new ClaimsIdentity();
            identity.AddClaim(new Claim("apiKeyUid", apiKeyUid));
            identity.AddClaim(new Claim("searchRules", JsonSerializer.Serialize(searchRules.ToClaim()), valueType));

            var signingKey = Encoding.ASCII.GetBytes(apiKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = identity,
                Expires = expiresAt,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(signingKey), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler
            {
                SetDefaultTimesOnTokenCreation = false
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
