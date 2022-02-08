namespace Meilisearch
{
    /// <summary>
    /// Information regarding an API key for the Meilisearch server.
    /// </summary>
    public class Version
    {
        /// <summary>
        /// Extracts version from Meilisearch.csproj.
        /// </summary>
        /// <returns>Returns a formatted version.</returns>
        public string GetQualifiedVersion()
        {
            return $"Meilisearch .NET (v{this.GetVersion()})";
        }

        /// <summary>
        /// Extracts the "major.minor.build" version from Meilisearch.csproj.
        /// </summary>
        /// <returns>Returns a version from the GetType as String.</returns>
        public string GetVersion()
        {
            return this.GetType().Assembly.GetName().Version.ToString(3);
        }
    }
}
