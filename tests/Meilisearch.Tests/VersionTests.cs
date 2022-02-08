namespace Meilisearch.Tests
{
    using System.IO;
    using System.Xml;
    using Xunit;

    public class VersionTests
    {
        private Version version;

        public VersionTests()
        {
            this.version = new Version();
        }

        [Fact]
        public void GetQualifiedVersion()
        {
            var qualifiedVersion = this.version.GetQualifiedVersion();
            var version = this.version.GetVersion();

            Assert.Equal(qualifiedVersion, $"Meilisearch .NET (v{version})");
        }

        [Fact]
        public void GetSimpleVersionFromCsprojFile()
        {
            // get the current version defined in the csproj file
            var xmldoc = new XmlDocument();
            var currentDir = Directory.GetParent(Directory.GetCurrentDirectory()).FullName;
            var path = Path.Combine(currentDir, @"../../../../src/Meilisearch/Meilisearch.csproj");
            xmldoc.Load(path);
            var mgr = new XmlNamespaceManager(xmldoc.NameTable);
            mgr.AddNamespace("x", "http://schemas.microsoft.com/developer/msbuild/2003");
            var versionFromCsproj = xmldoc.FirstChild.FirstChild.SelectSingleNode("Version").InnerText;

            var value = this.version.GetVersion();

            Assert.NotNull(value);
            Assert.Equal(versionFromCsproj, value);
        }
    }
}
