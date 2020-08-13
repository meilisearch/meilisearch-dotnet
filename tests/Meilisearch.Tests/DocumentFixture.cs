using System;

namespace Meilisearch.Tests
{
    public class DocumentFixture : IDisposable
    {
        public Index documentIndex { get; private set; }

        public Index DocumentDeleteIndex { get; private set; }

        public DocumentFixture()
        {
           SetUp();
           SetUpForDelete();
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
                var movies = new[]
                {
                    new Movie {Id = "10", Name = "SuperMan"},
                };
                var updateStatus = this.DocumentDeleteIndex.AddDocuments(movies).Result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

            }

        }

        public void SetUp()
        {
            try
            {
                var client = new MeilisearchClient("http://localhost:7700", "masterKey");
                var index = client.GetIndex("Movies").Result;

                if (index == null)
                {
                    this.documentIndex = client.CreateIndex("Movies").Result;
                }
                else
                {
                    this.documentIndex = index;
                }

                var movies = new[]
                {
                    new Movie { Id = "10", Name = "Gladiator" },
                    new Movie { Id = "11", Name = "Interstellar" },
                    new Movie { Id = "12", Name = "Start Wars", Genre = "SF" },
                    new Movie { Id = "13", Name = "Harry Potter", Genre = "SF" },
                    new Movie { Id = "14", Name = "Iron Man", Genre = "Action" },
                    new Movie { Id = "15", Name = "Spider-Man", Genre = "Action" },
                    new Movie { Id = "16", Name = "Amélie Poulain", Genre = "French movie" }
                };
                var updateStatus = this.documentIndex.AddDocuments(movies).Result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

        }

        public void Dispose()
        {
        }
    }
}
