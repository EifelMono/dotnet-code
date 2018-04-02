var target = Argument ("target", "Default");
var uninstallBeforeInstall = Argument<bool> ("uninstallBeforeInstall", true);

// Name of the csproj 
string csprojName;

// Directory of the package output!!
string nupkgDirectory = "nupkg";

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

Task ("UnInstall")
    .Description ("Removes an installed command from the 'user home'/.dotnet/tools folder")
    .Does (() => {
        var userDirectory = Environment.GetFolderPath (Environment.SpecialFolder.UserProfile);
        var dotnetTools = System.IO.Path.Combine (userDirectory, ".dotnet", "tools");
        if (DirectoryExists (dotnetTools)) {
            foreach (var file in System.IO.Directory.GetFiles (dotnetTools, $"{csprojName}*"))
                DeleteFile (file);
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
        Information(nugetNupkgFile);
        var exitCode = StartProcess ("nuget", new ProcessSettings {
            Arguments = $"push {nugetNupkgFile} -Source https://api.nuget.org/v3/index.json"
        });
    });

Task ("Default")
    .IsDependentOn ("Restore")
    .IsDependentOn ("Pack")
    .IsDependentOn ("InstallFromNuGetLocal")
    .Does (() => { });

RunTarget (target);