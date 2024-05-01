using Spectre.Console.Cli;

namespace CsprojFrameworkVersions
{
    public class ScanSettings : CommandSettings
    {
        [CommandArgument(0, "[DIRECTORY]")]
        public string? Directory { get; set; }

        [CommandOption("-u|--excludeUnreadable <EXCLUDE_UNREADABLE>")]
        public bool ExcludeUnreadable { get; set; }
        [CommandOption("-p|--excludeParameterized <EXCLUDE_PARAMETERIZED>")]
        public bool ExcludeParameterized { get; set; }
    }
}
