namespace Meilisearch.Tests
{
    using System;

    public class DocumentFixture : IDisposable
    {
        public DocumentFixture()
        {
            this.SetUp();
            this.SetUpForDelete();
        }

        public Meilisearch.Index DocumentsIndex { get; private set; }

        public Meilisearch.Index DocumentDeleteIndex { get; private set; }

        public void SetUp()
        {
            try
            {
                var client = new MeilisearchClient("http://localhost:7700", "masterKey");
                var index = client.GetIndex("Movies").Result;

                if (index == null)
                {
                    this.DocumentsIndex = client.CreateIndex("Movies").Result;
                }
                else
                {
                    this.DocumentsIndex = index;
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
                var updateStatus = this.DocumentsIndex.AddDocuments(movies).Result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void Dispose()
        {
        }

        private void SetUpForDelete()
        {
            try
            {
                var client = new MeilisearchClient("http://localhost:7700", "masterKey");
                var index = client.GetIndex("MoviesToDelete").Result;
                if (index == null)
                {
                    this.DocumentDeleteIndex = client.CreateIndex("MoviesToDelete").Result;
                }
                else
                {
                    this.DocumentDeleteIndex = index;
                }

                var movies = new[] { new Movie { Id = "10", Name = "SuperMan" } };
                var updateStatus = this.DocumentDeleteIndex.AddDocuments(movies).Result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
