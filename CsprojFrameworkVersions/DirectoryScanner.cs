using Spectre.Console;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Xml.XPath;

namespace CsprojFrameworkVersions
{
    public class DirectoryScanner
    {
        private readonly StatusContext? statusContext;

        public DirectoryScanner()
        {
            
        }

        public DirectoryScanner(StatusContext statusContext)
        {
            this.statusContext = statusContext ?? throw new ArgumentNullException(nameof(statusContext));
        }

        public void Scan(string directory, bool excludeUnreadable, bool excludeParameterized)
        {
            var csprojFiles = Directory.GetFiles(directory, "*.csproj", SearchOption.AllDirectories);

            AnsiConsole.MarkupLine("Found [yellow]{0}[/] files", csprojFiles.Length);
            statusContext?.Refresh();

            var outputTable = new Table();
            outputTable.AddColumn("Project");
            outputTable.AddColumn("Version");

            var scanResults = csprojFiles
                                .AsParallel()
                                .Select(ScanCsprojFile);

            if (excludeUnreadable)
            {
                scanResults = scanResults.Where(x => x.success);
            }
            if (excludeParameterized)
            {
                var regex = new Regex(@"\$\(.*\)");
                scanResults = scanResults.Where(x => !regex.IsMatch(x.version));
            }

            statusContext?.Refresh();
            foreach (var (success, project, version) in scanResults)
            {
                outputTable.AddRow(project, version);
            }

            AnsiConsole.Write(outputTable);
        }

        private (bool success, string project, string version) ScanCsprojFile(string path)
        {
            if (!File.Exists(path))
            {
                AnsiConsole.MarkupLine("Project [red]{0}[/] doesn't exists", path);
            }

            XDocument doc = XDocument.Load(path);
            var frameworkVersion = doc.XPathSelectElement("/Project/PropertyGroup/TargetFrameworkVersion")?.Value;
            var coreVersion = doc.XPathSelectElement("/Project/PropertyGroup/TargetFramework")?.Value;

            string? versionOutput = frameworkVersion?.ToLowerInvariant() ?? coreVersion?.ToLowerInvariant();
            statusContext?.Refresh();

            if (string.IsNullOrEmpty(versionOutput))
            {
                return (false, path, "No Version element found");
            }
            return (true, path, versionOutput);
        }
    }
}
