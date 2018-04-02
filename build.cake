var target = Argument ("target", "Default");
var uninstallBeforeInstall = Argument<bool> ("uninstallBeforeInstall", true);

// dotnet tools defaults
var isWindows = System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform (System.Runtime.InteropServices.OSPlatform.Windows);

string userDirectory = Environment.GetFolderPath (Environment.SpecialFolder.UserProfile);
string dotnetDirectory = System.IO.Path.Combine (userDirectory, ".dotnet");
string dotnetToolsDirectory = System.IO.Path.Combine (dotnetDirectory, "tools");
string dotnetToolsPkgsDirectory = System.IO.Path.Combine (dotnetDirectory, "toolspkgs");

string nuGetOrg = "https://api.nuget.org/v3/index.json";

// Directory of the package output!!
string nupkgDirectory = "nupkg";

// Name of the csproj 
string csprojName;

void DotNetRun (string argument) {
    var exitCode = StartProcess ("dotnet", new ProcessSettings {
        Arguments = argument
    });
    if (exitCode != 0)
        throw new Exception ($"Error {exitCode} during cmd={argument}");
}

Setup (ctx => {
    var csproj = System.IO.Directory.GetFiles (".", "*.csproj").FirstOrDefault ();
    if (string.IsNullOrEmpty (csproj))
        throw new Exception ($"No csproj file found in this folder {System.IO.Directory.GetCurrentDirectory()}");
    csprojName = System.IO.Path.GetFileNameWithoutExtension (csproj);
});

Task ("Restore")
    .Does (() => {
        DotNetRun ("restore");
    });

Task ("Pack")
    .Does (() => {
        CleanDirectory (nupkgDirectory);
        DotNetRun ($"pack -c release -o {nupkgDirectory}");
    });

Task ("Build")
    .Does (() => {
        DotNetRun ($"build");
    });

Task ("UnInstall")
    .Description ("Removes an installed command from the 'user home' .dotnet/tools and .dotnet/toolspks folder")
    .Does (() => {
        var extension = isWindows? ".exe": "";
        if (DirectoryExists (dotnetToolsDirectory)) {
            foreach (var file in System.IO.Directory.GetFiles (dotnetToolsDirectory, $"{csprojName}{extension}"))
                DeleteFile (file);
        }
        if (DirectoryExists (dotnetToolsPkgsDirectory)) {
            var dotnetToolPkgsDirectory = System.IO.Path.Combine (dotnetToolsPkgsDirectory, csprojName);
            if (DirectoryExists (dotnetToolPkgsDirectory))
                DeleteDirectory (dotnetToolPkgsDirectory, new DeleteDirectorySettings {
                    Force = true,
                        Recursive = true
                });
        }
    });

Task ("UninstallBeforeInstall")
    // If you install again you need to remove the old one by hand
    // at this time needed for 2.1.300 Preview....
    .WithCriteria (uninstallBeforeInstall)
    .IsDependentOn ("UnInstall")
    .Does (() => { });

Task ("InstallFromNuGetLocal")
    .IsDependentOn ("UninstallBeforeInstall")
    .Does (() => {
        DotNetRun ($"install tool -g {csprojName} --source {nupkgDirectory} ");
    });

Task ("InstallFromNuGetOrg")
    .IsDependentOn ("UninstallBeforeInstall")
    .Does (() => {
        DotNetRun ($"install tool -g {csprojName}");
    });

Task ("PushToNuGetOrg")
    .Does (() => {
        var nugetNupkgFile = System.IO.Directory.GetFiles (nupkgDirectory, "*.nupkg").FirstOrDefault ();
        if (string.IsNullOrEmpty (nugetNupkgFile))
            throw new Exception ($"No nuget file found in this folder {nupkgDirectory}");
        Information (nugetNupkgFile);
        var exitCode = StartProcess ("nuget", new ProcessSettings {
            Arguments = $"push {nugetNupkgFile} -Source {nuGetOrg}"
        });
    });

Task ("Default")
    .IsDependentOn ("Restore")
    .IsDependentOn ("Pack")
    .IsDependentOn ("InstallFromNuGetLocal")
    .Does (() => { });

RunTarget (target);