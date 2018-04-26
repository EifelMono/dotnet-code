using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace DotNetCode
{
    class Program
    {
        static bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        static void Main(string[] args)
        {
            if (args?.Length > 0 && (args[0] == "--help" || args[0] == "-h"))
            {
                string Help = $"dotnet-code {System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString()} starts Visual Studio Code in the current folder.\r\n" +
                    "If there is a code-workspace file in the folder, dotnet-code starts code with this,\r\n" +
                    "otherwise with the current folder as parameter.";
                Console.WriteLine(Help);
                return;
            }

            string currentDirectory = Directory.GetCurrentDirectory();
            string workspaceFilename = Directory.GetFiles(currentDirectory, "*.code-workspace").FirstOrDefault();

            StartCode(string.IsNullOrEmpty(workspaceFilename) ? currentDirectory : workspaceFilename);
        }

        static void StartCode(string parameter)
        {
            try
            {
                if (isWindows)
                    Process.Start("cmd", $"/c code {parameter}");
                else
                    Process.Start("code", $"{parameter}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Can no start Visual Studio Code");
                Console.WriteLine(ex);
            }
        }
    }
}