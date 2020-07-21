# RecoreFX sample app

[![CI](https://github.com/brcrista/RecoreFX-Sample-App/workflows/CI/badge.svg)](https://github.com/recorefx/RecoreFX-Sample-App/actions?query=workflow%3ACI)

This sample app is to test and demo the features of [RecoreFX](https://github.com/recorefx/RecoreFX).

The app syncs files between two file system subtrees.
The [service](src/FileSync.Service/) runs in one subtree and the [client](src/FileSync.Client/) runs on the other.
The client and the service communicate over HTTP.
Every time the client runs, it will pull any files that it is missing, then post any files that the service doesn't have, and then exit.
If the client pulls a file and it already has a file with the same name, it will stop and prompt the user to rename the local file before trying again.
