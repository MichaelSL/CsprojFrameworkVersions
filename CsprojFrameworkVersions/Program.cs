using Spectre.Console.Cli;

namespace CsprojFrameworkVersions
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var app = new CommandApp();

            app.Configure(config =>
            {
                config.AddCommand<ScanDirectoryCommand>("scan");
            });

            app.Run(args);
        }
    }
}
