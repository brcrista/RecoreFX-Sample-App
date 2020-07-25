# RecoreFX sample app

[![CI](https://github.com/brcrista/RecoreFX-Sample-App/workflows/CI/badge.svg)](https://github.com/recorefx/RecoreFX-Sample-App/actions?query=workflow%3ACI)

This sample app is to test and demo the features of [RecoreFX](https://github.com/recorefx/RecoreFX).

The app syncs files between two file system subtrees.
* The [service](src/FileSync.Service/) runs in one subtree and the [client](src/FileSync.Client/) runs on the other.
* The client and the service communicate over HTTP.
* Every time the client runs, it will pull any files that it is missing, then push any files that the service doesn't have, and then exit.
* If the client and service both have files with the same name, the client will choose the file with the latest modified time.
* Deleting files through syncing is not supported. If a file is deleted on the client, it will get pulled again from the service on the next run. Likewise, if a file is deleted on the service, any clients that have it will push it the next time they run.