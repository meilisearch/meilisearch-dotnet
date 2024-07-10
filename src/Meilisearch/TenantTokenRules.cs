using System.Collections.Generic;

namespace Meilisearch
{
    /// <summary>
    /// Wrapper class used to map all the supported types to be used in
    /// the `searchRules` claim in the Tenant Tokens.
    /// </summary>
    public class TenantTokenRules
    {
        private readonly object _rules;

        /// <summary>
        /// Initializes a new instance of the <see cref="TenantTokenRules"/> class based on a rules json object.        
        /// </summary>
        /// <param name="rules">
        /// 
        /// example:
        /// 
        /// {'*': {"filter": 'tag = Tale'}}
        /// 
        /// </param>
        public TenantTokenRules(IReadOnlyDictionary<string, object> rules)
        {
            _rules = rules;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TenantTokenRules"/> class based on a rules string array.        
        /// </summary>
        /// <param name="rules">
        /// 
        /// example:
        /// 
        /// ['books']
        /// 
        /// </param>
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
