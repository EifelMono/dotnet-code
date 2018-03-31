var target = Argument ("target", "Default");

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
    .Does (() => {
        var userDirectory = Environment.GetFolderPath (Environment.SpecialFolder.UserProfile);
        var dotnetTools = System.IO.Path.Combine (userDirectory, ".dotnet", "tools");
        if (DirectoryExists (dotnetTools)) {
            foreach (var file in System.IO.Directory.GetFiles (dotnetTools, $"{csprojName}*"))
                DeleteFile (file);
        }
    });

Task ("InstallFromNuGetLocal")
    // If you install again you need to remove the old one by hand
    // 2.1.300 Preview....
    .IsDependentOn ("UnInstall")
    .Does (() => {
        DotNetRun ($"install tool -g {csprojName} --source {nupkgDirectory} ");
    });

Task ("InstallFromNuGetOrg")
    .Does (() => {
        DotNetRun ($"install tool -g {csprojName}");
    });

Task ("PushToNuGetOrg")
    .Does (() => { });

Task ("Default")
    .IsDependentOn ("Restore")
    .IsDependentOn ("Pack")
    .IsDependentOn ("InstallFromNuGetLocal")
    .Does (() => { });

RunTarget (target);