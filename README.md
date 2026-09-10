# Cat Facts API

A small ASP.NET Core Minimal API that fetches a random cat fact from `catfact.ninja` and appends the response as a new JSON line to a local file or Azure Blob.

## Requirements

- .NET 10 SDK

## Run locally

From the repository root:

```powershell
dotnet restore
dotnet run --project ".\Netwise Task\Netwise Task.csproj"
```

The HTTP profile listens on `http://localhost:5246`.

Open the following address in a browser to fetch and save a cat fact:

<http://localhost:5246/cat-facts>

Refreshing the page sends another request and appends another JSON line to:

```text
Netwise Task/Data/cat-facts.txt
```

## Tests

```powershell
dotnet test
```

The tests cover the HTTP client, service and local file storage.

## Configuration

The Cat Facts API URL is configured in `Netwise Task/appsettings.json`:

```json
{
  "CatFacts": {
    "BaseUrl": "https://catfact.ninja/"
  }
}
```

The default storage provider is the local file system:

```json
{
  "Storage": {
    "Provider": "File",
    "File": {
      "Path": "Data/cat-facts.txt"
    }
  }
}
```

## Azure Blob Storage

The application also supports Azure Blob Storage through an `ICatFactStore` implementation based on Azure Append Blob.

Set the following environment variables to use it:

```text
Storage__Provider=AzureBlob
Storage__AzureBlob__ServiceUri=https://<storage-account>.blob.core.windows.net/
Storage__AzureBlob__ContainerName=cat-facts
Storage__AzureBlob__BlobName=cat-facts.txt
```
For local Azure Blob development, sign in with Azure CLI:

```powershell
az login
```

The signed-in identity must have the `Storage Blob Data Contributor` role. In Azure, enable a Managed Identity with the same role.

## Architecture

```text
GET /cat-facts
      |
      v
CatFactService
      |
      +-- ICatFactClient -> Cat Facts API
      |
      +-- ICatFactStore
            +-- FileCatFactStore
            +-- AzureBlobCatFactStore
```

The storage implementation is selected during application startup from `Storage:Provider`.
