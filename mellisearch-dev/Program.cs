using Meilisearch;

var client = new MeilisearchClient("http://localhost:7700", "masterKey");

// An index is where the documents are stored.
var index = await client.GetIndex("movies");
var documents = new Movie[] {
                new Movie { Id = "1", Title = "Carol", Genre = new[] { "Romance", "Drama" } },
                new Movie { Id = "2", Title = "Wonder Woman", Genre = new[] {"Action", "Adventure" }  },
                new Movie { Id = "3", Title = "Life of Pi", Genre = new[] { "Adventure", "Drama" } },
                new Movie { Id = "4", Title = "Mad Max: Fury Road", Genre = new[] { "Adventure", "Science Fiction" } },
                new Movie { Id = "5", Title = "Moana", Genre = new [] { "Fantasy", "Action" } },
                new Movie { Id = "6", Title = "Philadelphia", Genre = new [] { "Drama" } }
            };

// If the index "movies" does not exist, MeiliSearch creates it when you first add the documents.
var update = await index.AddDocuments<Movie>(documents); // Result: => { "updateId": 0 }


class Movie
{
    public string Id { get; set; }
    public string Title { get; set; }
    public string[] Genre { get; set; }
}
