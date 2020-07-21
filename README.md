# RecoreFX sample app

This sample app is to test and demo the features of [RecoreFX](https://github.com/brcrista/RecoreFX).

The app syncs files between two file system subtrees.
The [service](SyncService/) runs in one subtree and the [client](SyncClient/) runs on the other.
The client and the service communicate over HTTP.
Every time the client runs, it will pull any files that it is missing, then post any files that the service doesn't have, and then exit.
If the client pulls a file and it already has a file with the same name, it will stop and prompt the user to rename the local file before trying again.