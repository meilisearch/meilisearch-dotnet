<h1 align="center">MeiliSearch Dotnet</h1>

<h4 align="center">
  <a href="https://github.com/meilisearch/MeiliSearch">MeiliSearch</a> |
  <a href="https://www.meilisearch.com">Website</a> |
  <a href="https://blog.meilisearch.com">Blog</a> |
  <a href="https://twitter.com/meilisearch">Twitter</a> |
  <a href="https://docs.meilisearch.com">Documentation</a> |
  <a href="https://docs.meilisearch.com/faq">FAQ</a>
</h4>

![.NET Core](https://github.com/satish860/meilisearch-dotnet/workflows/.NET%20Core/badge.svg?branch=master)


<p align="center">⚡ Lightning Fast, Ultra Relevant, and Typo-Tolerant Search Engine MeiliSearch client written in dotnet</p>

**MeiliSearch dotnet** is a client for **MeiliSearch** written in Dotnet. **MeiliSearch** is a powerful, fast, open-source, easy to use and deploy search engine. Both searching and indexing are highly customizable. Features such as typo-tolerance, filters, and synonyms are provided out-of-the-box

## Table of Contents <!-- omit in toc -->

- [🔧 Installation](#-installation)
- [🚀 Getting started](#-getting-started)
- [🎬 Examples](#-examples)
  - [Indexes](#indexes)
  - [Documents](#documents)
  - [Update status](#update-status)
  - [Search](#search)
- [⚙️ Development Workflow](#️-development-workflow)
  - [Install dependencies](#install-dependencies)
  - [Tests and Linter](#tests-and-linter)
  - [Release](#release)
- [🤖 Compatibility with MeiliSearch](#-compatibility-with-meilisearch)

## 🔧 Installation

TODO

### Run MeiliSearch <!-- omit in toc -->

There are many easy ways to [download and run a MeiliSearch instance](https://docs.meilisearch.com/guides/advanced_guides/installation.html#download-and-launch).

For example, if you use Docker:
```bash
$ docker run -it --rm -p 7700:7700 getmeili/meilisearch:latest ./meilisearch --master-key=masterKey
```

NB: you can also download MeiliSearch from **Homebrew** or **APT**.

## 🚀 Getting started

// TODO

## 🎬 Examples

### Indexes

#### Create an index <!-- omit in toc -->
 ```c#
 var index = client.CreateIndex("uid1");
```

#### Create an index and give the primary-key
```c#
client.CreateIndex("uid2", "movieId");
```

#### List all an index <!-- omit in toc -->

```c#
var client = new MeilisearchClient(_httpClient);
var indexes = await client.GetAllIndexes();
```

#### Get an Index object <!-- omit in toc -->
```c#
 var client = new MeilisearchClient(_httpClient);
 var indexes = await client.GetIndex("somerandomIndex");
```
### Documents

#### Add Documents

```c#
 var updateStatus = await index.AddorUpdateDocuments(new[]{new  Movie {Id = "1", Name = "Batman"}});
```
Update Status has a reference `UpdateId` to get status of the action.

#### Get Documents
```c#
 var documents = await index.GetDocuments<Movie>(new DocumentQuery {Limit = 1});
```

#### Get Document by Id

```c#
var documents = await index.GetDocument<Movie>("10");
```

#### Delete documents

```c#
 var updateStatus = await index.DeleteOneDocument("11");
```
#### Delete in Batch

```c#
var updateStatus = await index.DeleteDocuments(new []{"12","13","14"});
```

#### Delete all documents
```c#
var updateStatus = await indextoDelete.DeleteAllDocuments();
```
### Status

#### Get Update Status By Id
```c#
 UpdateStatus individualStatus = await index.GetUpdateStatus(1);
```

#### Get All Update Status
```c#
 var status = await index.GetAllUpdateStatus();
```
#### Search
```c#
var movies = await this.index.Search<Movie>("ironman");
```

#### Custom Search
```c#
var movies = await this.index.Search<Movie>("ironman", new SearchQuery {Limit = 100});
```

⚙️ Development Workflow

If you want to contribute, this sections describes the steps to follow.

### Tests

```bash
# Tests
docker run -d -p 7700:7700 getmeili/meilisearch:latest ./meilisearch  --no-analytics
dotnet restore
dotnet test
```

### Release

TODO 

```xml
<Version>x.x.x</Version>
```

## 🤖 Compatibility with MeiliSearch

This package works for MeiliSearch >=0.10.x
