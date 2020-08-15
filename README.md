# RecoreFX sample app

[![CI](https://github.com/brcrista/RecoreFX-Sample-App/workflows/CI/badge.svg)](https://github.com/recorefx/RecoreFX-Sample-App/actions?query=workflow%3ACI)

## Running the app

Both the client and service simply look at your current working directory. You can run them with

```bash
dotnet path/to/FileSync.Service.dll
dotnet path/to/FileSync.Client.dll
```

See the [app-playground](./app-playground) directory for a working example.

## How it works

This sample app is to test and demo the features of [RecoreFX](https://github.com/recorefx/RecoreFX).

The app syncs files between two file system subtrees.
* The [service](src/FileSync.Service/) runs in one subtree and the [client](src/FileSync.Client/) runs on the other.
* The client and the service communicate over HTTP.
* Every time the client runs, it will pull any files that it is missing, then push any files that the service doesn't have, and then exit.
* If the client and service both have files with the same name but different contents (determined by the file hashes), the client will choose the file with the latest modified time.
* Deleting files through syncing is not supported. If a file is deleted on the client, it will get pulled again from the service on the next run. Likewise, if a file is deleted on the service, any clients that have it will push it the next time they run.

When the client runs, it will produce output like

```
Files on the client:
  ./client-only.txt
  ./shared-file.txt
  ./shared-dir/shared-file.txt

Files on the service:
  ./service-only-dir/subdir/service-only.txt
  ./service-only-dir/service-only.txt
  ./shared-dir/service-only.txt
  ./shared-dir/shared-file.txt
  ./service-only.txt
  ./shared-file.txt

'./shared-file.txt' exists on both the client and the service. Choosing the service's version.

Files to upload:
  ./client-only.txt

Files to download:
  ./service-only-dir/subdir/service-only.txt
  ./service-only-dir/service-only.txt
  ./shared-dir/service-only.txt
  ./service-only.txt
  ./shared-file.txt


===== Summary =====

Sent files: 1
  ./client-only.txt

New files: 4
  ./service-only-dir/subdir/service-only.txt
  ./service-only-dir/service-only.txt
  ./shared-dir/service-only.txt
  ./service-only.txt

Changed files: 1
  ./shared-file.txt
```

## Service API

The service has two endpoints, both under the path `api/v1/`.

### Directory listing

The first is `listing/` (think `ls`). It takes an optional `path` query parameter for a subdirectory. Note that all paths between the client and service should be sent with the `/` separator.

The `listing/` response looks like this:

```json
[
  {
    "relativePath": "./service-only-dir",
    "listingUrl": "api/v1/listing?path=./service-only-dir"
  },
  {
    "relativePath": "./shared-dir",
    "listingUrl": "api/v1/listing?path=./shared-dir"
  },
  {
    "relativePath": "./client-only.txt",
    "lastWriteTimeUtc": "2020-08-15T19:05:51.8308581Z",
    "sha1": "iSvxz7m3jN7ke7NojEUdPKULc8o=",
    "contentUrl": "api/v1/content?path=./client-only.txt"
  },
  {
    "relativePath": "./service-only.txt",
    "lastWriteTimeUtc": "2020-08-15T18:48:10.2941682Z",
    "sha1": "kkzwM/eBQHW36aCJee4EALCOieA=",
    "contentUrl": "api/v1/content?path=./service-only.txt"
  },
  {
    "relativePath": "./shared-file.txt",
    "lastWriteTimeUtc": "2020-08-15T18:48:10.2950817Z",
    "sha1": "9ttsPkCP9LjGEFh9AZVffUCsAvs=",
    "contentUrl": "api/v1/content?path=./shared-file.txt"
  }
]
```

The first two items are directories, and the last three are files. The directory records include the URL that the client should visit to get its listing so the client can walk the tree recursively. The file records include the information the client needs to determine whether it needs to download the file or upload its version, as well as the URL to download the file.

### File content

As you've probably noticed, the other endpoint is `content/`. This takes a `path` parameter in the same way as `listing/`. Here the client can download the binary content of any file on the service. It can also upload new files. The service will automatically create any directories in the filepath.
