using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Newtonsoft.Json;

public class NugetManager
{
    private List<string> _packages;

    public NugetManager()
    {
        _packages = new List<string>();
    }

    public void AddPackageFile(string fileUri)
    {
        if (!File.Exists(fileUri))
            Console.WriteLine($"{fileUri} not found.");
        else
        {
            LoadPackages(fileUri);
        }
    }

    public async Task<string> GetLicenseMD(bool ignoreMicrosoftPackages)
    {
        List<License> licenses = new List<License>();


        Parallel.ForEach(_packages, (packageid) =>
        {
            var nugetInfos = Task.Run(() => GetNugetInfoJsonAsync(packageid)).Result;
            var license = Task.Run(() => GetLicense(nugetInfos, ignoreMicrosoftPackages)).Result;

            if (license != null)
                licenses.Add(license);
        });

        // foreach (var packageid in _packages)
        // {
        //     var nugetInfos = await GetNugetInfoJsonAsync(packageid);
        //     var license = await GetLicense(nugetInfos, ignoreMicrosoftPackages);

        //     if (license != null)
        //         licenses.Add(license);
        // }

        return GenerateMarkdownFile(licenses);
    }

    private void LoadPackages(string fileUri)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(Packages));
        using (TextReader reader = new StringReader(File.ReadAllText(fileUri)))
        {
            Packages result = (Packages)serializer.Deserialize(reader);

            result?.Package?.ForEach(m =>
            {
                if (!_packages.Any(n => n == m.Id))
                    _packages.Add(m.Id);
            });
        }
    }

    private async Task<NugetInfo> GetNugetInfoJsonAsync(string packageId)
    {
        using (var client = new HttpClient())
        {
            var url = $"https://api-v2v3search-0.nuget.org/query?q=packageid:{packageId}";
            HttpResponseMessage response = await client.GetAsync(url);
            string jsonString = await response.Content.ReadAsStringAsync();

            var model = JsonConvert.DeserializeObject<NugetInfo>(jsonString);

            return model;
        }
    }

    private async Task<License> GetLicense(NugetInfo nugetInfo, bool ignoreMicrosoftPackages)
    {
        if (nugetInfo == null)
            throw new ArgumentNullException("nugetInfo");

        var licenseUrl = nugetInfo?.Data?.FirstOrDefault().LicenseUrl;
        if (string.IsNullOrEmpty(licenseUrl))
            return null;

        if (ignoreMicrosoftPackages && nugetInfo?.Data?.FirstOrDefault().Authors?.Any(m => m == "Microsoft") == true)
            return null;

        if (licenseUrl.Contains("opensource.org"))
        {
            return new License()
            {
                PackageId = nugetInfo?.Data?.FirstOrDefault().Id,
                Content = $"[{licenseUrl}]({licenseUrl})"
            };
        }
        else
        {
            //if license.md on git, replace blob with raw.
            if (licenseUrl.Contains("github.com") && licenseUrl.Contains("/blob/"))
                licenseUrl = licenseUrl.Replace("/blob/", "/raw/");

            using (var client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(licenseUrl);
                string licenseContent = await response.Content.ReadAsStringAsync();

                if (string.IsNullOrEmpty(licenseContent))
                    return null;

                License license = new License()
                {
                    PackageId = nugetInfo?.Data?.FirstOrDefault().Id,
                    Title = licenseContent.Split(new[] { '\r', '\n' }).FirstOrDefault(),
                    Content = licenseContent
                };

                return license;
            }
        }
    }

    // private string RemoveHtmlTags(string licenseContent, string licenseUrl)
    // {
    //     if (licenseUrl.Contains("opensource.org"))
    //     {

    //     }

    //     if (Regex.IsMatch(licenseContent, "<.*?>"))
    //     {
    //         return Regex.Replace(licenseContent, "<.*?>", String.Empty);
    //     }

    //     return licenseContent;
    // }

    private string GenerateMarkdownFile(IEnumerable<License> licenses)
    {
        StringBuilder builder = new StringBuilder();
        builder.AppendLine("# Rechtliche Hinweise");
        builder.AppendLine("Die vorliegende Applikation ist geistiges Eigentum von Cubeware und unterliegt dem Urheberrecht. Die eingesetzten Komponenten von Drittanbietern werden unter Einhaltung der jeweils für die Komponenten uniform bzw. individuell geltenden Bedingungen genutzt.");
        builder.AppendLine(Environment.NewLine);
        builder.AppendLine("**Bei der Erstellung wurden folgende Technologien von Drittanbietern eingesetzt:**");
        builder.AppendLine(Environment.NewLine);

        foreach (var license in licenses)
        {
            builder.AppendLine($"## {license.PackageId}");
            builder.AppendLine(license.Content);
            builder.AppendLine(Environment.NewLine);
        }

        return builder.ToString();
    }
}
