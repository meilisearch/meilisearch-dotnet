using System;

using JWT.Algorithms;
using JWT.Builder;

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

            var builder = JwtBuilder
                .Create()
                .WithAlgorithm(new HMACSHA256Algorithm())
                .AddClaim("apiKeyUid", apiKeyUid)
                .AddClaim("searchRules", searchRules.ToClaim())
                .WithSecret(apiKey);

            if (expiresAt.HasValue)
            {
                if (DateTime.Compare(DateTime.UtcNow, (DateTime)expiresAt) > 0)
                {
                    throw new MeilisearchTenantTokenExpired();
                }

                builder.AddClaim("exp", ((DateTimeOffset)expiresAt).ToUnixTimeSeconds());
            }

            return builder.Encode();
        }
    }
}
