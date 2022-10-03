<p align="center">
  <img src="https://raw.githubusercontent.com/meilisearch/integration-guides/main/assets/logos/meilisearch_dotnet.svg" alt="Meilisearch-Dotnet" width="200" height="200" />
</p>

<h1 align="center">Meilisearch .NET</h1>

<h4 align="center">
  <a href="https://github.com/meilisearch/meilisearch">Meilisearch</a> |
  <a href="https://docs.meilisearch.com">Documentation</a> |
  <a href="https://slack.meilisearch.com">Slack</a> |
  <a href="https://roadmap.meilisearch.com/tabs/1-under-consideration">Roadmap</a> |
  <a href="https://www.meilisearch.com">Website</a> |
  <a href="https://docs.meilisearch.com/faq">FAQ</a>
</h4>

<p align="center">
  <a href="https://www.nuget.org/packages/meilisearch"><img src="https://img.shields.io/nuget/v/meilisearch" alt="NuGet"></a>
  <a href="https://github.com/meilisearch/meilisearch-dotnet/actions"><img src="https://github.com/meilisearch/meilisearch-dotnet/workflows/Tests/badge.svg?branch=main" alt="GitHub Workflow Status"></a>
  <a href="https://github.com/meilisearch/meilisearch-dotnet/blob/main/LICENSE"><img src="https://img.shields.io/badge/license-MIT-informational" alt="License"></a>
  <a href="https://app.bors.tech/repositories/28784"><img src="https://bors.tech/images/badge_small.svg" alt="Bors enabled"></a>
</p>

<p align="center">⚡ The Meilisearch API client written for .NET</p>

**Meilisearch .NET** is the Meilisearch API client for C# developers.

**Meilisearch** is an open-source search engine. [Discover what Meilisearch is!](https://github.com/meilisearch/meilisearch)

## Table of Contents <!-- omit in toc -->

- [🎃 Hacktoberfest](#-hacktoberfest)
- [📖 Documentation](#-documentation)
- [🔧 Installation](#-installation)
- [🚀 Getting Started](#-getting-started)
- [🤖 Compatibility with Meilisearch](#-compatibility-with-meilisearch)
- [🎬 Examples](#-examples)
  - [Indexes](#indexes)
  - [Documents](#documents)
  - [Get Task information](#get-task-information)
  - [Search](#search)
- [🧰 Use a Custom HTTP Client](#-use-a-custom-http-client)
- [⚙️ Development Workflow and Contributing](#️-development-workflow-and-contributing)

## 🎃 Hacktoberfest

It’s Hacktoberfest 2022 @Meilisearch

[Hacktoberfest](https://hacktoberfest.com/) is a celebration of the open-source community. This year, and for the third time in a row, Meilisearch is participating in this fantastic event.

You’d like to contribute? Don’t hesitate to check out our [contributing guidelines](./CONTRIBUTING.md).

## 📖 Documentation

See our [Documentation](https://docs.meilisearch.com/learn/tutorials/getting_started.html) or our [API References](https://docs.meilisearch.com/reference/api/).

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

There are many easy ways to [download and run a Meilisearch instance](https://docs.meilisearch.com/reference/features/installation.html#download-and-launch).

For example, using the `curl` command in [your Terminal](https://itconnect.uw.edu/learn/workshops/online-tutorials/web-publishing/what-is-a-terminal/):

```bash
# Install Meilisearch
curl -L https://install.meilisearch.com | sh
# Launch Meilisearch
./meilisearch --master-key=masterKey
```

NB: you can also download Meilisearch from **Homebrew** or **APT** or even run it using **Docker**.

## 🚀 Getting Started

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

With the `uid`, you can check the status (`enqueued`, `processing`, `succeeded` or `failed`) of your documents addition using the [task](https://docs.meilisearch.com/reference/api/tasks.html#get-task).

#### Basic Search <!-- omit in toc -->

```c#
# Meilisearch is typo-tolerant:
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

All the supported options are described in the [search parameters](https://docs.meilisearch.com/reference/features/search_parameters.html) section of the documentation.

```c#
SearchResult<Movie> movies = await index.SearchAsync<Movie>(
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

## 🤖 Compatibility with Meilisearch

This package only guarantees the compatibility with the [version v0.29.0 of Meilisearch](https://github.com/meilisearch/meilisearch/releases/tag/v0.29.0).

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

## ⚙️ Development Workflow and Contributing

Any new contribution is more than welcome in this project!

If you want to know more about the development workflow or want to contribute, please visit our [contributing guidelines](/CONTRIBUTING.md) for detailed instructions!

<hr>

**Meilisearch** provides and maintains many **SDKs and Integration tools** like this one. We want to provide everyone with an **amazing search experience for any kind of project**. If you want to contribute, make suggestions, or just know what's going on right now, visit us in the [integration-guides](https://github.com/meilisearch/integration-guides) repository.
