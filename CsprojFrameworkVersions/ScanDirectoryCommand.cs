using Spectre.Console;
using Spectre.Console.Cli;

namespace CsprojFrameworkVersions
{
    public class ScanDirectoryCommand : Command<ScanSettings>
    {
        public override int Execute(CommandContext context, ScanSettings settings)
        {
            var directory = settings.Directory;
            if (directory == null)
            {
                AnsiConsole.MarkupLine("[red]Error:[/] No directory specified");
                return 1;
            }
            AnsiConsole.MarkupLine("Scanning directory [yellow]{0}[/]", directory);

            AnsiConsole.Status()
                .AutoRefresh(false)
                .Spinner(Spinner.Known.BouncingBar)
                .SpinnerStyle(Style.Parse("green bold"))
                .Start($"Scanning directory {directory}", ctx =>
                {
                    var scanner = new DirectoryScanner();
                    scanner.Scan(directory, settings.ExcludeUnreadable, settings.ExcludeParameterized);

                    ctx.Refresh();
                });

            return 0;
        }
    }
}
