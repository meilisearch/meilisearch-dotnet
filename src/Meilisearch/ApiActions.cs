namespace Meilisearch
{
    /// <summary>
    /// List all available actions for a key.
    /// </summary>
    public static class ApiActions
    {
        /// <summary>
        /// Authorize all actions.
        /// </summary>
        public const string All = "*";

        /// <summary>
        /// Provides access to both POST and GET search endpoints on authorized indexes.
        /// </summary>
        public const string Search = "search";

        /// <summary>
        /// Provides access to the add documents and update documents endpoints on authorized indexes.
        /// </summary>
        public const string DocumentsAdd = "documents.add";
        /// <summary>
        /// Provides access to the get one document and get documents endpoints on authorized indexes.
        /// </summary>
        public const string DocumentsGet = "documents.get";
        /// <summary>
        /// Provides access to the delete one document, delete all documents, and batch delete endpoints on authorized indexes.
        /// </summary>
        public const string DocumentsDelete = "documents.delete";

        /// <summary>
        /// Provides access to the create index endpoint.
        /// </summary>
        public const string IndexesCreate = "indexes.create";
        /// <summary>
        /// Provides access to the get one index and list all indexes endpoints.
        /// Non-authorized indexes will be omitted from the response.
        /// </summary>
        public const string IndexesGet = "indexes.get";
        /// <summary>
        /// Provides access to the update index endpoint.
        /// </summary>
        public const string IndexesUpdate = "indexes.update";
        /// <summary>
        /// Provides access to the delete index endpoint.
        /// </summary>
        public const string IndexesDelete = "indexes.delete";

        /// <summary>
        /// Provides access to the get one task and get all tasks endpoints.
        /// Tasks from non-authorized indexes will be omitted from the response.
        /// Also provides access to the get one task by index and get all tasks by index endpoints on authorized indexes.
        /// </summary>
        public const string TasksGet = "tasks.get";

        /// <summary>
        /// Provides access to the get settings endpoint and equivalents for all subroutes on authorized indexes.
        /// </summary>
        public const string SettingsGet = "settings.get";
        /// <summary>
        /// Provides access to the update settings and reset settings endpoints and equivalents for all subroutes on authorized indexes.
        /// </summary>
        public const string SettingsUpdate = "settings.update";

        /// <summary>
        /// Provides access to the get stats of an index endpoint and the get stats of all indexes endpoint.
        /// For the latter, non-authorized indexes are omitted from the response.
        /// </summary>
        public const string StatsGet = "stats.get";

        /// <summary>
        /// Provides access to the create dump endpoint. Not restricted by indexes.
        /// </summary>
        public const string DumpsCreate = "dumps.create";
        /// <summary>
        /// Provides access to the get dump status endpoint. Not restricted by indexes.
        /// </summary>
        public const string DumpsGet = "dumps.get";

        /// <summary>
        /// Provides access to the get Meilisearch version endpoint.
        /// </summary>
        public const string Version = "version";
    }
}
