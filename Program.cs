using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace NugetLicenseInfo
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0 || (args.Length == 1 && args[0] == "--help") || args.Length > 2)
                WriteInfo();
            else if (args.Length == 2)
            {
                var outputFile = "NugetLicenses.md";
                List<string> packages = new List<string>();

                foreach (var arg in args)
                {
                    if (arg.StartsWith("--packages="))
                    {
                        var packagesString = arg.Replace("--packages=", "");
                        packages = packagesString.Split(",").ToList();
                    }

                    if (arg.StartsWith("--output="))
                    {
                        outputFile = arg.Replace("--output=", "");
                    }
                }

                Console.WriteLine("NuGet Packages config files:");
                packages.ForEach(m => Console.WriteLine(m));

                Console.WriteLine(Environment.NewLine);
                Console.WriteLine("Processing started ...");

                var nugetManager = new NugetManager();
                packages.ForEach(m => nugetManager.AddPackageFile(m));
                var markDown = Task.Run(() => nugetManager.GetLicenseMD(true)).Result;

                var fileInfo = new FileInfo(outputFile);
                if (!Directory.Exists(fileInfo.Directory.FullName))
                    Directory.CreateDirectory(fileInfo.Directory.FullName);

                File.WriteAllText(outputFile, markDown);

                Console.WriteLine($"License MD file generated and saved to: {outputFile}");
            }



        }

        private static void WriteInfo()
        {
            Console.WriteLine("Parameters:");
            Console.WriteLine("--packages=[package.config PATH],[package.config PATH], ...");
            Console.WriteLine("--output=[PATH_TO_MD_FILE.md]");
        }


    }


}
