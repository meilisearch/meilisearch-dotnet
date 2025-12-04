<p align="center">
  <img src="https://raw.githubusercontent.com/meilisearch/integration-guides/main/assets/logos/meilisearch_dotnet.svg" alt="Meilisearch-Dotnet" width="200" height="200" />
</p>

<h1 align="center">Meilisearch .NET</h1>

<h4 align="center">
  <a href="https://github.com/meilisearch/meilisearch">Meilisearch</a> |
<a href="https://www.meilisearch.com/cloud?utm_campaign=oss&utm_source=github&utm_medium=meilisearch-dotnet">Meilisearch Cloud</a> |
  <a href="https://www.meilisearch.com/docs">Documentation</a> |
  <a href="https://discord.meilisearch.com">Discord</a> |
  <a href="https://roadmap.meilisearch.com/tabs/1-under-consideration">Roadmap</a> |
  <a href="https://www.meilisearch.com">Website</a> |
  <a href="https://www.meilisearch.com/docs/faq">FAQ</a>
</h4>

<p align="center">
  <a href="https://www.nuget.org/packages/meilisearch"><img src="https://img.shields.io/nuget/v/meilisearch" alt="NuGet"></a>
  <a href="https://github.com/meilisearch/meilisearch-dotnet/actions"><img src="https://github.com/meilisearch/meilisearch-dotnet/workflows/Tests/badge.svg?branch=main" alt="GitHub Workflow Status"></a>
  <a href="https://github.com/meilisearch/meilisearch-dotnet/blob/main/LICENSE"><img src="https://img.shields.io/badge/license-MIT-informational" alt="License"></a>
</p>

<p align="center">⚡ The Meilisearch API client written for .NET</p>

**Meilisearch .NET** is the Meilisearch API client for C# developers.

**Meilisearch** is an open-source search engine. [Learn more about Meilisearch.](https://github.com/meilisearch/meilisearch)

## Table of Contents <!-- omit in TOC -->

- [📖 Documentation](#-documentation)
- [🔧 Installation](#-installation)
- [🚀 Getting started](#-getting-started)
- [🤖 Compatibility with Meilisearch](#-compatibility-with-meilisearch)
- [🎬 Examples](#-examples)
  - [Indexes](#indexes)
  - [Documents](#documents)
  - [Get Task information](#get-task-information)
  - [Search](#search)
- [🧰 Use a Custom HTTP Client](#-use-a-custom-http-client)
- [⚙️ Contributing](#️-contributing)

## 📖 Documentation

This readme contains all the documentation you need to start using this Meilisearch SDK.

For general information on how to use Meilisearch—such as our API reference, tutorials, guides, and in-depth articles—refer to our [main documentation website](https://www.meilisearch.com/docs/).


## 🔧 Installation

This package targets .NET Standard 2.1.

Using the [.NET Core command-line interface (CLI) tools](https://docs.microsoft.com/en-us/dotnet/core/tools/):

```bash
dotnet add package MeiliSearch
```

or with the [Package Manager Console](https://docs.microsoft.com/en-us/nuget/tools/package-manager-console):

```bash
Install-Package MeiliSearch
```

### Run Meilisearch <!-- omit in toc -->

⚡️ **Launch, scale, and streamline in minutes with Meilisearch Cloud**—no maintenance, no commitment, cancel anytime. [Try it free now](https://cloud.meilisearch.com/login?utm_campaign=oss&utm_source=github&utm_medium=meilisearch-dotnet).

🪨  Prefer to self-host? [Download and deploy](https://www.meilisearch.com/docs/learn/self_hosted/getting_started_with_self_hosted_meilisearch?utm_campaign=oss&utm_source=github&utm_medium=meilisearch-dotnet) our fast, open-source search engine on your own infrastructure.

## 🚀 Getting started

#### Add Documents <!-- omit in toc -->

```c#
using System;
using System.Threading.Tasks;
using Meilisearch;

namespace GettingStarted
{
    class Program
    {
        public class Movie
        {
            public string Id { get; set; }
            public string Title { get; set; }
            public IEnumerable<string> Genres { get; set; }
        }

        static async Task Main(string[] args)
        {
            MeilisearchClient client = new MeilisearchClient("http://localhost:7700", "masterKey");

            // An index is where the documents are stored.
            var index = client.Index("movies");
            var documents = new Movie[] {
                new Movie { Id = "1", Title = "Carol", Genres = new string[] { "Romance", "Drama" }  },
                new Movie { Id = "2", Title = "Wonder Woman", Genres = new string[] { "Action", "Adventure" } },
                new Movie { Id = "3", Title = "Life of Pi", Genres = new string[] { "Adventure", "Drama" } },
                new Movie { Id = "4", Title = "Mad Max: Fury Road", Genres = new string[] { "Adventure", "Science Fiction"} },
                new Movie { Id = "5", Title = "Moana", Genres = new string[] { "Fantasy", "Action" } },
                new Movie { Id = "6", Title = "Philadelphia", Genres = new string[] { "Drama" } }
            };

            // If the index 'movies' does not exist, Meilisearch creates it when you first add the documents.
            var task = await index.AddDocumentsAsync<Movie>(documents); // # => { "uid": 0 }
        }
    }
}
```

With the `uid`, you can check the status (`enqueued`, `canceled`, `processing`, `succeeded` or `failed`) of your documents addition using the [task](https://www.meilisearch.com/docs/reference/api/tasks).

#### Basic Search <!-- omit in toc -->

```c#
//Meilisearch is typo-tolerant:
SearchResult<Movie> movies = await index.SearchAsync<Movie>("philadalphia");
foreach(var prop in movies.Hits) {
    Console.WriteLine (prop.Title);
}
```

JSON Output:

```json
{
    "hits": [
        {
            "id": 6,
            "title": "Philadelphia",
        }
    ],
    "offset": 0,
    "limit": 20,
    "processingTimeMs": 10,
    "query": "philadalphia"
}
```

#### Custom Search <!-- omit in toc -->

All the supported options are described in the [search parameters](https://www.meilisearch.com/docs/reference/api/search#search-parameters) section of the documentation.

```c#
var movies = await index.SearchAsync<Movie>(
    "car",
    new SearchQuery
    {
        AttributesToHighlight = new string[] { "title" },
    }
);

foreach(var prop in movies.Hits) {
    Console.WriteLine (prop.Title);
}
```

JSON Output:

```json
{
    "hits": [
        {
            "id": 1,
            "title": "Carol",
            "_formatted": {
                "id": 1,
                "title": "<em>Car</em>ol"
            }
        }
    ],
    "offset": 0,
    "limit": 20,
    "processingTimeMs": 10,
    "query": "car"
}
```

#### Custom Search With Filters <!-- omit in toc -->

If you want to enable filtering, you must add your attributes to the `FilterableAttributes` index setting.

```c#
TaskInfo task = await index.UpdateFilterableAttributesAsync(
    new string[] { "id", "genres" }
);
```

You only need to perform this operation once.

Note that MeiliSearch will rebuild your index whenever you update `FilterableAttributes`. Depending on the size of your dataset, this might take time. You can track the process using the [update status](https://www.meilisearch.com/docs/reference/api/tasks#status).

Then, you can perform the search:

```c#
var movies = await index.SearchAsync<Movie>(
    "wonder",
    new SearchQuery
    {
        Filter = "id > 1 AND genres = Action",
    }
);
```

JSON Output:

```json
{
  "hits": [
    {
      "id": 2,
      "title": "Wonder Woman",
      "genres": ["Action","Adventure"]
    }
  ],
  "offset": 0,
  "limit": 20,
  "estimatedTotalHits": 1,
  "processingTimeMs": 0,
  "query": "wonder"
}
```

#### Using SearchQueryFilterBuilder <!-- omit in toc -->

Instead of writing filter strings manually, you can use the fluent `SearchQueryFilterBuilder` to construct type-safe filter expressions:

```c#
using Meilisearch;

// Simple equality filter
var filter = SearchQueryFilterBuilder.Where("genre", "Action");
// Result: "genre = Action"

// Comparison operators
var filter = SearchQueryFilterBuilder.WhereGreaterThan("rating", 85);
// Result: "rating > 85"

// IN operator for multiple values
var filter = SearchQueryFilterBuilder.WhereIn("genre", "horror", "comedy", "action");
// Result: "genre IN [horror, comedy, action]"

// Combine filters with AND
var filter = SearchQueryFilterBuilder.Where("genre", "Action")
    .And(SearchQueryFilterBuilder.WhereGreaterThan("rating", 80));
// Result: "genre = Action AND rating > 80"

// Combine filters with OR and group them
var filter = SearchQueryFilterBuilder.GroupOr(
    SearchQueryFilterBuilder.Where("genre", "horror"),
    SearchQueryFilterBuilder.Where("genre", "comedy")
).And(SearchQueryFilterBuilder.WhereGreaterThan("release_date", 795484800));
// Result: "(genre = horror OR genre = comedy) AND release_date > 795484800"

// Geo radius search
var filter = SearchQueryFilterBuilder.WhereGeoRadius(45.4777599, 9.1967508, 2000);
// Result: "_geoRadius(45.4777599, 9.1967508, 2000)"

// Use in search query
var movies = await index.SearchAsync<Movie>(
    "wonder",
    new SearchQuery
    {
        Filter = SearchQueryFilterBuilder.Where("genre", "Action")
            .And(SearchQueryFilterBuilder.WhereGreaterThan("id", 1))
            .Build()
    }
);
```

The `SearchQueryFilterBuilder` supports the following operations:
- **Equality**: `Where`, `WhereNot`
- **Comparison**: `WhereGreaterThan`, `WhereGreaterThanOrEqual`, `WhereLessThan`, `WhereLessThanOrEqual`
- **Range**: `WhereBetween`
- **List**: `WhereIn`
- **Existence**: `WhereExists`, `WhereNotExists`
- **Empty/Null checks**: `WhereEmpty`, `WhereNotEmpty`, `WhereNull`, `WhereNotNull`
- **Geo filters**: `WhereGeoRadius`, `WhereGeoBoundingBox`
- **Logical operators**: `And`, `Or`, `Not`, `Group`, `GroupAnd`, `GroupOr`

#### Search with Limit and Offset

You can paginate search results by making queries combining both [offset](https://www.meilisearch.com/docs/reference/api/search#offset) and [limit](https://www.meilisearch.com/docs/reference/api/search#limit).

```c#
var results = await index.SearchAsync<T>(query, new SearchQuery()
{
    Limit = 5,
    Offset = 0
});

if (results is SearchResult<T> limitedResults)
{
    var estimatedTotalHits = limitedResults.EstimatedTotalHits;
}
```

#### Search with defined number of results per page

To get paginated results with page numbers, the [HitsPerPage](https://www.meilisearch.com/docs/reference/api/search#number-of-results-per-page) and [Page](https://www.meilisearch.com/docs/reference/api/search#page) properties must be defined.

```c#
var results = await index.SearchAsync<T>(query, new SearchQuery()
{
    HitsPerPage = pageSize,
    Page = pageNumber,
});

if (results is PaginatedSearchResult<T> paginatedResults)
{
    var totalHits = paginatedResults.TotalHits;
    var totalPages = paginatedResults.TotalPages;
}
```

#### Search with sort

To get results sorted by attributes and preferred sorting order, the [Sort](https://www.meilisearch.com/docs/reference/api/search#sort) property must be defined.

```c#
var results = await index.SearchAsync<T>(query, new SearchQuery()
{
    Sort = new List<string> { "genre:asc", "name:desc" }
});
```

## 🤖 Compatibility with Meilisearch

This package guarantees compatibility with [version v1.x of Meilisearch](https://github.com/meilisearch/meilisearch/releases/latest), but some features may not be present. Please check the [issues](https://github.com/meilisearch/meilisearch-dotnet/issues?q=is%3Aissue+is%3Aopen+label%3A%22good+first+issue%22+label%3Aenhancement) for more info.

## 🎬 Examples

### Indexes

#### Create an index <!-- omit in toc -->

```c#
var index = await client.CreateIndexAsync("movies");
```

#### Create an index and give the primary-key <!-- omit in toc -->

```c#
var index = await client.CreateIndexAsync("movies", "id");
```

#### List all an index <!-- omit in toc -->

```c#
var indexes = await client.GetAllIndexesAsync();
```

#### Get an Index object <!-- omit in toc -->

```c#
var index = await client.GetIndexAsync("movies");
```

### Documents

#### Add or Update Documents <!-- omit in toc -->

```c#
var task = await index.AddDocumentsAsync(new Movie[] { new Movie { Id = "1", Title = "Carol" } } );
var task = await index.UpdateDocumentsAsync(new Movie[] { new Movie { Id = "1", Title = "Carol" } } );
```

The returned `task` is a `TaskInfo` that can access to `Uid` to get the status of the task.

#### Get Documents <!-- omit in toc -->

```c#
var documents = await index.GetDocumentsAsync<Movie>(new DocumentsQuery { Limit = 1 });
```

#### Get Document by Id <!-- omit in toc -->

```c#
var document = await index.GetDocumentAsync<Movie>("10");
```

#### Delete documents <!-- omit in toc -->

```c#
var task = await index.DeleteOneDocumentAsync("11");
```

#### Delete in Batch <!-- omit in toc -->

```c#
var task = await index.DeleteDocumentsAsync(new [] {"12","13","14"});
```

#### Delete all documents <!-- omit in toc -->

```c#
var task = await indextoDelete.DeleteAllDocumentsAsync();
```

### Get Task information

#### Get one Task By Uid <!-- omit in toc -->

```c#
TaskInfo task = await index.GetTaskAsync(1);
// Or
TaskInfo task = await client.GetTaskAsync(1);
```

#### Get All Tasks <!-- omit in toc -->

```c#
var task = await index.GetTasksAsync();
// Or
var task = await client.GetTasksAsync();
```

### Search

#### Basic Search <!-- omit in toc -->

```c#
var movies = await this.index.SearchAsync<Movie>("prince");
```

#### Custom Search <!-- omit in toc -->

```c#
var movies = await this.index.SearchAsync<Movie>("prince", new SearchQuery { Limit = 100 });
```

## 🧰 Use a Custom HTTP Client

You can replace the default client used in this package by the one you want.

For example:

```c#
var _httpClient = ClientFactory.Instance.CreateClient<MeilisearchClient>();
var client = new MeilisearchClient(_httpClient);
```

Where `ClientFactory` is declared [like this](/tests/Meilisearch.Tests/ClientFactory.cs).

## ⚙️ Contributing

Any new contribution is more than welcome in this project!

If you want to know more about the development workflow or want to contribute, please visit our [contributing guidelines](/CONTRIBUTING.md) for detailed instructions!

<hr>

**Meilisearch** provides and maintains many **SDKs and Integration tools** like this one. We want to provide everyone with an **amazing search experience for any kind of project**. If you want to contribute, make suggestions, or just know what's going on right now, visit us in the [integration-guides](https://github.com/meilisearch/integration-guides) repository.
