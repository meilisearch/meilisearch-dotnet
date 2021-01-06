<p align="center">
  <img src="https://res.cloudinary.com/meilisearch/image/upload/v1587402338/SDKs/meilisearch_dotnet.svg" alt="MeiliSearch-Dotnet" width="200" height="200" />
</p>

<h1 align="center">MeiliSearch .NET</h1>

<h4 align="center">
  <a href="https://github.com/meilisearch/MeiliSearch">MeiliSearch</a> |
  <a href="https://docs.meilisearch.com">Documentation</a> |
  <a href="https://roadmap.meilisearch.com/tabs/1-under-consideration">Roadmap</a> |
  <a href="https://www.meilisearch.com">Website</a> |
  <a href="https://blog.meilisearch.com">Blog</a> |
  <a href="https://twitter.com/meilisearch">Twitter</a> |
  <a href="https://docs.meilisearch.com/faq">FAQ</a>
</h4>

<p align="center">
  <a href="https://www.nuget.org/packages/MeiliSearch"><img src="https://img.shields.io/nuget/v/MeiliSearch" alt="NuGet"></a>
  <a href="https://github.com/meilisearch/meilisearch-dotnet/actions"><img src="https://github.com/meilisearch/meilisearch-dotnet/workflows/Tests/badge.svg?branch=master" alt="GitHub Workflow Status"></a>
  <a href="https://github.com/meilisearch/meilisearch-dotnet/blob/master/LICENSE"><img src="https://img.shields.io/badge/license-MIT-informational" alt="License"></a>
  <a href="https://slack.meilisearch.com"><img src="https://img.shields.io/badge/slack-MeiliSearch-blue.svg?logo=slack" alt="Slack"></a>
  <a href="https://app.bors.tech/repositories/28784"><img src="https://bors.tech/images/badge_small.svg" alt="Bors enabled"></a>
</p>

<p align="center">⚡ The MeiliSearch API client written for .NET</p>

**MeiliSearch .NET** is the MeiliSearch API client for C# developers. **MeiliSearch** is a powerful, fast, open-source, easy to use and deploy search engine. Both searching and indexing are highly customizable. Features such as typo-tolerance, filters, facets, and synonyms are provided out-of-the-box.

## Table of Contents <!-- omit in toc -->

- [📖 Documentation](#-documentation)
- [🔧 Installation](#-installation)
- [🚀 Getting Started](#-getting-started)
- [🤖 Compatibility with MeiliSearch](#-compatibility-with-meilisearch)
- [🎬 Examples](#-examples)
  - [Indexes](#indexes)
  - [Documents](#documents)
  - [Update Status](#update-status)
  - [Search](#search)
- [🧰 Use a Custom HTTP Client](#-use-a-custom-http-client)
- [⚙️ Development Workflow and Contributing](#️-development-workflow-and-contributing)

## 📖 Documentation

See our [Documentation](https://docs.meilisearch.com/guides/introduction/quick_start_guide.html) or our [API References](https://docs.meilisearch.com/references/).

## 🔧 Installation

Using the [.NET Core command-line interface (CLI) tools](https://docs.microsoft.com/en-us/dotnet/core/tools/):

```bash
$ dotnet add package MeiliSearch
```

or with the [Package Manager Console](https://docs.microsoft.com/en-us/nuget/tools/package-manager-console):

```bash
$ Install-Package MeiliSearch
```

### Run MeiliSearch <!-- omit in toc -->

There are many easy ways to [download and run a MeiliSearch instance](https://docs.meilisearch.com/guides/advanced_guides/installation.html#download-and-launch).

For example, if you use Docker:
```bash
$ docker pull getmeili/meilisearch:latest # Fetch the latest version of MeiliSearch image from Docker Hub
$ docker run -it --rm -p 7700:7700 getmeili/meilisearch:latest ./meilisearch --master-key=masterKey
```

NB: you can also download MeiliSearch from **Homebrew** or **APT**.

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
        public class Book
        {
            public string BookId { get; set; }
            public string Title { get; set; }
        }
        static async Task Main(string[] args)
        {
            MeilisearchClient client = new MeilisearchClient("http://localhost:7700", "masterKey");
            var index = await client.CreateIndex("books"); // If your index does not exist
            // OR
            var index = await client.GetIndex("books"); // If your index exists
            var documents = new Book[] {
                new Book { BookId = "123",  Title = "Pride and Prejudice" },
                new Book { BookId = "456",  Title = "Le Petit Prince" },
                new Book { BookId = "1",    Title = "Alice In Wonderland" },
                new Book { BookId = "1344", Title = "The Hobbit" },
                new Book { BookId = "4",    Title = "Harry Potter and the Half-Blood Prince" },
                new Book { BookId = "42",   Title = "The Hitchhiker's Guide to the Galaxy" }
            };
            var update = await index.AddDocuments<Book>(documents); # => { "updateId": 0 }
        }
    }
}

```

With the `updateId` (via `update.UpdateId`), you can check the status (`enqueued`, `processed` or `failed`) of your documents addition using the [update endpoint](https://docs.meilisearch.com/references/updates.html#get-an-update-status).

#### Basic Search <!-- omit in toc -->

```c#
# MeiliSearch is typo-tolerant:
SearchResult<Book> books = await index.Search<Book>("harry pottre");
foreach(var prop in books.Hits) {
    Console.WriteLine (prop.Title);
}
```

JSON Output:

```json
{
    "hits": [
        {
            "book_id": 4,
            "title": "Harry Potter and the Half-Blood Prince",
        }
    ],
    "offset": 0,
    "limit": 20,
    "processingTimeMs": 10,
    "query": "harry pottre"
}
```

#### Custom Search <!-- omit in toc -->

All the supported options are described in the [search parameters](https://docs.meilisearch.com/guides/advanced_guides/search_parameters.html) section of the documentation.

```c#
SearchResult<Book> books = await index.Search<Book>(
    "prince",
    new SearchQuery
    {
        AttributesToHighlight = new string[] { "title" },
        Filters = "book_id > 10"
    }
);
foreach(var prop in books.Hits) {
    Console.WriteLine (prop.Title);
}
```

JSON Output:

```json
{
    "hits": [
        {
            "book_id": 456,
            "title": "Le Petit Prince",
            "_formatted": {
                "book_id": 456,
                "title": "Le Petit <em>Prince</em>"
            }
        }
    ],
    "offset": 0,
    "limit": 20,
    "processingTimeMs": 10,
    "query": "prince"
}
```

## 🤖 Compatibility with MeiliSearch

This package only guarantees the compatibility with the [version v0.19.0 of MeiliSearch](https://github.com/meilisearch/MeiliSearch/releases/tag/v0.19.0).

## 🎬 Examples

### Indexes

#### Create an index <!-- omit in toc -->

 ```c#
var index = client.CreateIndex("books");
```

#### Create an index and give the primary-key <!-- omit in toc -->

```c#
var index = client.CreateIndex("books", "book_id");
```

#### List all an index <!-- omit in toc -->

```c#
var indexes = await client.GetAllIndexes();
```

#### Get an Index object <!-- omit in toc -->

```c#
var index = await client.GetIndex("books");
```

### Documents

#### Add or Update Documents <!-- omit in toc -->

```c#
 var updateStatus = await index.AddDocuments(new Book[] { new Book { BookId = "1", Title = "Alice In Wonderland" } } );
 var updateStatus = await index.UpdateDocuments(new Book[] { new Book { BookId = "1", Title = "Alice aux Pays des Merveilles" } } );
```

Update Status has a reference `UpdateId` to get the status of the action.

#### Get Documents <!-- omit in toc -->

```c#
 var documents = await index.GetDocuments<Book>(new DocumentQuery {Limit = 1});
```

#### Get Document by Id <!-- omit in toc -->

```c#
var document = await index.GetDocument<Book>("10");
```

#### Delete documents <!-- omit in toc -->

```c#
 var updateStatus = await index.DeleteOneDocument("11");
```

#### Delete in Batch <!-- omit in toc -->

```c#
var updateStatus = await index.DeleteDocuments(new []{"12","13","14"});
```

#### Delete all documents <!-- omit in toc -->

```c#
var updateStatus = await indextoDelete.DeleteAllDocuments();
```

### Update Status

#### Get Update Status By Id <!-- omit in toc -->

```c#
UpdateStatus individualStatus = await index.GetUpdateStatus(1);
```

#### Get All Update Status <!-- omit in toc -->

```c#
var status = await index.GetAllUpdateStatus();
```

### Search

#### Basic Search <!-- omit in toc -->

```c#
var books = await this.index.Search<Book>("prince");
```

#### Custom Search <!-- omit in toc -->

```c#
var books = await this.index.Search<Book>("prince", new SearchQuery {Limit = 100});
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

**MeiliSearch** provides and maintains many **SDKs and Integration tools** like this one. We want to provide everyone with an **amazing search experience for any kind of project**. If you want to contribute, make suggestions, or just know what's going on right now, visit us in the [integration-guides](https://github.com/meilisearch/integration-guides) repository.
