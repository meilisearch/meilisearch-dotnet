using System.IO;
using System.Xml;

using Xunit;

namespace Meilisearch.Tests
{
    public class VersionTests
    {
        private readonly Version _version;

        public VersionTests()
        {
            _version = new Version();
        }

        [Fact]
        public void GetQualifiedVersion()
        {
            var qualifiedVersion = _version.GetQualifiedVersion();
            var version = _version.GetVersion();

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

            var value = _version.GetVersion();

            Assert.NotNull(value);
            Assert.Equal(versionFromCsproj, value);
        }
    }
}
