# Hadouken BitTorrent client
Hadouken is a BitTorrent client written in C# and designed to run as a Windows Service. It is *lightweight*, *extensible* and built on top of *libtorrent* to bring you the best in class of BitTorrent services.

## Extensions
Hadouken currently has two main extension points, *notifiers* and *plugins*. A *notifier* is responsible for sending notifications through some means, and a *plugin* is a background service extending functionality of Hadouken.

### Creating your own extension

  - Fork the repository.
  - Read through the [API documentation](https://github.com/hadouken/hadouken/wiki/Extension-API).
  - Create your extension. Don't forget the unit tests.
  - Send a pull request, *bonus points if it's from a topic branch*.

## Building from source
Hadouken uses PSake for building. To create a complete build, run,
```posh
PS> .\run-build.ps1
```

### Build parameters
The `run-build.ps1` can take a few different parameters to control the build.

 - `ChocolateyAPIKey`, used when publishing the Chocolatey package.
 - `Commit`, the commit revision number to embed in the CommonAssemblyInfo file. If Git is in your path, it will get it automatically.
 - `BranchName`, the current branch name to embed in the CommonAssemblyInfo file. If Git is in your path, it will get it automatically.
 - `GitHubToken`, a GitHub access token to use when publishing a new release to GitHub.
 - `NuGetAPIKey`, used when publishing the SDK to NuGet.
 - `Task`, the build task to run. The default is "Default", which does not require any parameters.
 - `Version`, the version number to build.

#### Example

```posh
PS> .\run-build.ps1 -Version 1.0
```

## More information
For more information, [see the wiki](https://github.com/hadouken/hadouken/wiki).