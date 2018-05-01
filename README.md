
# dotnet-code

[![NuGet][main-nuget-badge]][main-nuget]

[main-nuget]: https://www.nuget.org/packages/dotnet-code/
[main-nuget-badge]: https://img.shields.io/nuget/v/dotnet-code.svg?style=flat-square&label=nuget

dotnet-code starts Visual Studio Code in the current folder.
<br>If there is a code-workspace file in the folder, dotnet-code starts code with this,
<br>otherwise with the current folder as parameter.

On Windows you can insert dotnet-code also in the address line.

## In action

### dotnet-code in address line 
![](https://github.com/EifelMono/dotnet-code/blob/master/media/dotnet-code.gif)

### dotnet-code from terminal with workspace
![](https://github.com/EifelMono/dotnet-code/blob/master/media/dotnet-code-workspace.gif)
## Install
```
dotnet tool install dotnet-code -g
```

## Requirements

[2.1.300-preview2](https://www.microsoft.com/net/download/dotnet-core/sdk-2.1.300-preview2) .NET Core SDK or newer
Section "Global Tools"

How to check the installed dotnet version
```
dotnet --version
```

## Install .........

### from NuGet.org

```
dotnet tool install dotnet-code -g
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
Here you find the program starter which you can delete.

```
cd .dotnet/tools/.storage
```
Here you find the real program which you can delete.

Or use the dotnet uninstall programm
```
dotnet tool uninstall dotnet-code -g
```

Or use the cakebuild batch file build.sh or build.ps1 with the target uninstall
```
./build.ps1 -target=uninstall
or
./build.sh -target=uninstall
```
