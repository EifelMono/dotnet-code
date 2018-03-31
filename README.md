# dotnet-code

dotnet-code starts Visual Studio Code in the current folder.
<br>If there is a code-workspace file in the folder, dotnet-code starts code with this,
<br>otherwise with the current folder as parameter."

On Windows you can insert dotnet-code also in the address line.

## Requirements

[2.1.300-preview1](https://www.microsoft.com/net/download/dotnet-core/sdk-2.1.300-preview1) .NET Core SDK or newer

How to check the installed dotnet version
```
dotnet --version
```

## Install

### from NuGet.org

not done for now

```
dotnet install tool --global dotnet-code
```
This downloads dotnet-code from NuGet.org

### local after the build

You can also clone the repo and run the cakebuiild build.ps1 or build.sh batch file.<br>
This will install the dotnet-code from the new build without NuGet.org.

## Uninstall

Goto your personal folder

* Windows
```
cd %userprofile%
```
* Mac and Linux
```
cd ~
``` 
Then goto the folder .dotnet/tools

```
cd .dotnet/tools
```

Here you find the program and you can delete it.

Or use the cakebuild batch files build.sh or build.ps1 with the target uninstall
```
./build.ps1 -target=uninstall
or
./build.sh -target=uninstall
```
