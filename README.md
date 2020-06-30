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
- [🚀 Getting started](#-getting-started)
- [🤖 Compatibility with MeiliSearch](#-compatibility-with-meilisearch)
- [🎬 Examples](#-examples)
  - [Indexes](#indexes)
  - [Documents](#documents)
  - [Update Status](#update-status)
  - [Search](#search)
- [⚙️ Development Workflow](#️-development-workflow)

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
$ docker run -it --rm -p 7700:7700 getmeili/meilisearch:latest ./meilisearch --master-key=masterKey
```

NB: you can also download MeiliSearch from **Homebrew** or **APT**.

## 🚀 Getting started

// TODO

## 🤖 Compatibility with MeiliSearch

This package is compatible with the following MeiliSearch versions:
- `v0.12.X`
- `v0.11.X`

## 🎬 Examples

### Indexes

#### Create an index <!-- omit in toc -->
 ```c#
 var index = client.CreateIndex("uid1");
```

#### Create an index and give the primary-key <!-- omit in toc -->
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

#### Add Documents <!-- omit in toc -->

```c#
 var updateStatus = await index.AddorUpdateDocuments(new[]{new  Movie {Id = "1", Name = "Batman"}});
```
Update Status has a reference `UpdateId` to get status of the action.

#### Get Documents <!-- omit in toc -->
```c#
 var documents = await index.GetDocuments<Movie>(new DocumentQuery {Limit = 1});
```

#### Get Document by Id <!-- omit in toc -->

```c#
var documents = await index.GetDocument<Movie>("10");
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

## ⚙️ Development Workflow

If you want to contribute, this section describes the steps to follow.

### Tests <!-- omit in toc -->

```bash
$ docker run -d -p 7700:7700 getmeili/meilisearch:latest ./meilisearch --no-analytics=true
$ dotnet restore
$ dotnet test
```

### Release <!-- omit in toc -->

MeiliSearch tools follow the [Semantic Versioning Convention](https://semver.org/).

You must do a PR modifying the file [`src/Meilisearch/Meilisearch.csproj`](https://github.com/meilisearch/meilisearch-dotnet/blob/master/src/Meilisearch/Meilisearch.csproj) with the right version.

```xml
<Version>X.X.X</Version>
```

Once the changes are merged on `master`, you can publish the current draft release via the [GitHub interface](https://github.com/meilisearch/meilisearch-dotnet/releases).

A GitHub Action will be triggered and push the new package to [NuGet](https://www.nuget.org/packages/MeiliSearch/).

<hr>

**MeiliSearch** provides and maintains many **SDKs and Integration tools** like this one. We want to provide everyone with an **amazing search experience for any kind of project**. If you want to contribute, make suggestions, or just know what's going on right now, visit us in the [integration-guides](https://github.com/meilisearch/integration-guides) repository.
