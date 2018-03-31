using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace DotNetCode {
    class Program {
        static void Main (string[] args) {
            if (args?.Length > 0 && (args[0] == "--help" || args[0] == "-h")) {
                string Help = $"dotnet-code {System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString()} starts Visual Studio Code in the current folder.\r\n" +
                    "If there is a code-workspace file in the folder, dotnet-code starts code with this,\r\n" +
                    "otherwise with the current folder as parameter.";
                Console.WriteLine (Help);
                return;
            }
            var workspace = Directory.GetFiles (Directory.GetCurrentDirectory (), "*.code-workspace").FirstOrDefault ();
            if (string.IsNullOrEmpty (workspace))
                Process.Start ("code", $"{Directory.GetCurrentDirectory()}");
            else
                Process.Start ("code", $"{workspace}");
        }
    }
}