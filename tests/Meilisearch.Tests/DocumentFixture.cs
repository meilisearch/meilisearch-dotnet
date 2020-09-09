namespace Meilisearch.Tests
{
    using System;

    public class DocumentFixture : IDisposable
    {
        public DocumentFixture()
        {
            this.SetUp();
            this.SetUpForDocumentsDeletion();
        }

        public Meilisearch.Index BasicIndexWithDocuments { get; private set; }

        public Meilisearch.Index IndexForDocumentsDeletion { get; private set; }

        public void SetUp()
        {
            try
            {
                var indexUid = "Movies";
                var client = new MeilisearchClient("http://localhost:7700", "masterKey");
                var index = client.GetIndex(indexUid).Result;

                if (index == null)
                {
                    this.BasicIndexWithDocuments = client.CreateIndex(indexUid).Result;
                }
                else
                {
                    this.BasicIndexWithDocuments = index;
                }

                var movies = new[]
                {
                    new Movie { Id = "10", Name = "Gladiator" },
                    new Movie { Id = "11", Name = "Interstellar" },
                    new Movie { Id = "12", Name = "Start Wars", Genre = "SF" },
                    new Movie { Id = "13", Name = "Harry Potter", Genre = "SF" },
                    new Movie { Id = "14", Name = "Iron Man", Genre = "Action" },
                    new Movie { Id = "15", Name = "Spider-Man", Genre = "Action" },
                    new Movie { Id = "16", Name = "Amélie Poulain", Genre = "French movie" },
                };
                var updateStatus = this.BasicIndexWithDocuments.AddDocuments(movies).Result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void Dispose()
        {
        }

        private void SetUpForDocumentsDeletion()
        {
            try
            {
                var indexUid = "MoviesToDelete";
                var client = new MeilisearchClient("http://localhost:7700", "masterKey");
                var index = client.GetIndex(indexUid).Result;
                if (index == null)
                {
                    this.IndexForDocumentsDeletion = client.CreateIndex(indexUid).Result;
                }
                else
                {
                    this.IndexForDocumentsDeletion = index;
                }

                var movies = new[] { new Movie { Id = "10", Name = "SuperMan" } };
                var updateStatus = this.IndexForDocumentsDeletion.AddDocuments(movies).Result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
