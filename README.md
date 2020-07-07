<p align="center">
  <img src="https://res.cloudinary.com/meilisearch/image/upload/v1587402338/SDKs/meilisearch_dotnet.svg" alt="MeiliSearch-Dotnet" width="200" height="200" />
</p>

<h1 align="center">MeiliSearch .NET</h1>

<h4 align="center">
  <a href="https://github.com/meilisearch/MeiliSearch">MeiliSearch</a> |
  <a href="https://www.meilisearch.com">Website</a> |
  <a href="https://blog.meilisearch.com">Blog</a> |
  <a href="https://twitter.com/meilisearch">Twitter</a> |
  <a href="https://docs.meilisearch.com">Documentation</a> |
  <a href="https://docs.meilisearch.com/faq">FAQ</a>
</h4>

<p align="center">
  <a href="https://www.nuget.org/packages/MeiliSearch"><img src="https://img.shields.io/nuget/v/MeiliSearch" alt="NuGet"></a>
  <a href="https://github.com/meilisearch/meilisearch-dotnet/actions"><img src="https://github.com/meilisearch/meilisearch-dotnet/workflows/.NET%20Core/badge.svg?branch=master" alt=".NET Core"></a>
  <a href="https://github.com/meilisearch/meilisearch-dotnet/blob/master/LICENSE"><img src="https://img.shields.io/badge/license-MIT-informational" alt="License"></a>
  <a href="https://slack.meilisearch.com"><img src="https://img.shields.io/badge/slack-MeiliSearch-blue.svg?logo=slack" alt="Slack"></a>
</p>

<p align="center">⚡ Lightning Fast, Ultra Relevant, and Typo-Tolerant Search Engine MeiliSearch client written in C#</p>

**MeiliSearch .NET** is a client for **MeiliSearch** written in C#. **MeiliSearch** is a powerful, fast, open-source, easy to use and deploy search engine. Both searching and indexing are highly customizable. Features such as typo-tolerance, filters, and synonyms are provided out-of-the-box.

## Table of Contents <!-- omit in toc -->

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
            public string Name { get; set; }
        }
        static async Task Main(string[] args)
        {
            MeilisearchClient client = new MeilisearchClient("http://localhost:7700", "masterKey");
            var index = await client.CreateIndex("movies"); // If your index does not exist
            // OR
            var index = await client.GetIndex("movies"); // If your index exists
            var documents = new Movie[] {
                new Movie {Id = "1", Name = "Batman"},
                new Movie {Id = "2", Name = "Interstellar"},
                new Movie {Id = "3", Name = "Batman Begins"}
            };
            var updateStatus = await index.AddDocuments<Movie>(documents);
            Movie movie = await index.GetDocument<Movie>("1");
            SearchResult<Movie> movies = await index.Search<Movie>("bat");
            foreach(var prop in movies.Hits) {
                Console.WriteLine (prop.Name);
            }
        }
    }
}

```

## 🤖 Compatibility with MeiliSearch

This package is compatible with the following MeiliSearch versions:
- `v0.12.X`
- `v0.11.X`

## 🎬 Examples

### Indexes

#### Create an index <!-- omit in toc -->

 ```c#
var index = client.CreateIndex("movies");
```

#### Create an index and give the primary-key <!-- omit in toc -->

```c#
var index = client.CreateIndex("movies", "movieId");
```

#### List all an index <!-- omit in toc -->

```c#
var indexes = await client.GetAllIndexes();
```

#### Get an Index object <!-- omit in toc -->

```c#
var index = await client.GetIndex("movies");
```

### Documents

#### Add or Update Documents <!-- omit in toc -->

```c#
 var updateStatus = await index.AddDocuments(new Movie[] {new Movie {Id = "1", Name = "Batman"}});
 var updateStatus = await index.UpdateDocuments(new Movie[] {new Movie {Id = "1", Name = "Batman"}});
```

Update Status has a reference `UpdateId` to get the status of the action.

#### Get Documents <!-- omit in toc -->

```c#
 var documents = await index.GetDocuments<Movie>(new DocumentQuery {Limit = 1});
```

#### Get Document by Id <!-- omit in toc -->

```c#
var document = await index.GetDocument<Movie>("10");
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
var movies = await this.index.Search<Movie>("ironman");
```

#### Custom Search <!-- omit in toc -->

```c#
var movies = await this.index.Search<Movie>("ironman", new SearchQuery {Limit = 100});
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
