using System.Collections.Generic;

namespace Meilisearch
{
    /// <summary>
    /// Wrapper class used to map all the supported types to be used in
    /// the `searchRules` claim in the Tenant Tokens.
    /// </summary>
    public class TenantTokenRules
    {
        private object _rules;

        public TenantTokenRules(Dictionary<string, object> rules)
        {
            _rules = rules;
        }

        public TenantTokenRules(string[] rules)
        {
            _rules = rules;
        }

        /// <summary>
        /// Accessor method used to retrieve the searchRules claim.
        /// </summary>
        /// <returns>A object with the supported type representing the `searchRules`.</returns>
        public object ToClaim()
        {
            return _rules;
        }
    }
}
