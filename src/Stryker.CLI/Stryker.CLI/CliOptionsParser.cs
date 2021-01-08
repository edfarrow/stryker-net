using System.Collections.Generic;
using System.Linq;
using McMaster.Extensions.CommandLineUtils;
using Stryker.Core.Options;
using Stryker.Core.Options.Inputs;

namespace Stryker.CLI
{
    public class CliOption
    {
        public StrykerInput InputType { get; set; }
        public string ArgumentName { get; set; }
        public string ArgumentShortName { get; set; }
        public string ArgumentHint { get; set; }
        public string Description { get; set; }
        public CommandOptionType OptionType { get; set; }
    }

    public static class CliOptionsParser
    {
        private static readonly IDictionary<string, CliOption> CliOptions = new Dictionary<string, CliOption>();
        private static readonly CliOption ConfigOption;
        private static readonly CliOption GenerateJsonConfigOption;

        static CliOptionsParser()
        {
            ConfigOption = AddCliOption(StrykerInput.None, "--config-file", "-cp",
                "Choose the file containing your stryker configuration relative to current working directory. | default: stryker-config.json", argumentHint: "file-path");
            GenerateJsonConfigOption = AddCliOption(StrykerInput.None, "--init", "-i",
                "Generate a stryker config file with selected and default options.", optionType: CommandOptionType.SingleOrNoValue, argumentHint: "file-path");

            PrepareCliOptions();
        }

        public static void RegisterCliOptions(CommandLineApplication app)
        {
            foreach (var (_, value) in CliOptions)
            {
                RegisterCliOption(app, value);
            }
        }

        public static string ConfigFilePath(string[] args, CommandLineApplication app)
        {
            RegisterCliOption(app, ConfigOption);
            return app.Parse(args).SelectedCommand.Options.SingleOrDefault(o => o.LongName == ConfigOption.ArgumentName)?.Value() ?? "stryker-config.json";
        }

        public static bool GenerateConfigFile(string[] args, CommandLineApplication app)
        {
            RegisterCliOption(app, GenerateJsonConfigOption);
            return app.Parse(args).SelectedCommand.Options.SingleOrDefault(o => o.LongName == GenerateJsonConfigOption.ArgumentName)?.HasValue() ?? false;
        }

        public static StrykerOptions EnrichFromCommandLineArguments(this StrykerOptions options, string[] args, CommandLineApplication app)
        {
            var enrichedOptions = options;
            foreach (var option in app.Parse(args).SelectedCommand.Options.Where(option => option.HasValue()))
            {
                var inputType = CliOptions[option.LongName].InputType;

                enrichedOptions = option.OptionType switch
                {
                    CommandOptionType.NoValue => enrichedOptions.With(inputType, option.HasValue()),
                    CommandOptionType.SingleOrNoValue => enrichedOptions.With(inputType, option.HasValue(), option.Value()),
                    CommandOptionType.SingleValue => enrichedOptions.With(inputType, option.Value()),
                    CommandOptionType.MultipleValue => enrichedOptions.With(inputType, option.Values),
                    _ => enrichedOptions
                };
            }

            return enrichedOptions;
        }

        private static void PrepareCliOptions()
        {
            AddCliOption(StrykerInput.Concurrency, "concurrency", "c", new ConcurrencyInput().HelpText, argumentHint: "number");

            AddCliOption(StrykerInput.ThresholdBreak, "break", "b", new ThresholdBreakInput().HelpText, argumentHint: "0-100");
            AddCliOption(StrykerInput.DevMode, "dev-mode", "dev", new DevModeInput().HelpText, optionType: CommandOptionType.NoValue);

            AddCliOption(StrykerInput.Mutate, "mutate", "m", new MutateInput().HelpText, optionType: CommandOptionType.MultipleValue, argumentHint: "glob-pattern");

            AddCliOption(StrykerInput.SolutionPath, "solution", "s", new SolutionPathInput().HelpText, argumentHint: "file-path");
            AddCliOption(StrykerInput.ProjectUnderTestName, "project", "p", new ProjectUnderTestNameInput().HelpText, argumentHint: "project-name.csproj");
            AddCliOption(StrykerInput.ProjectVersion, "version", "v", new ProjectVersionInput().HelpText);
            AddCliOption(StrykerInput.MutationLevel, "mutation-level", "l", new MutationLevelInput().HelpText);

            AddCliOption(StrykerInput.LogToFile, "log-to-file", "f", new LogToFileInput().HelpText, optionType: CommandOptionType.NoValue);
            AddCliOption(StrykerInput.LogLevel, "verbosity", "V", new LogLevelInput().HelpText);
            AddCliOption(StrykerInput.Reporters, "reporter", "r", new ReportersInput().HelpText, optionType: CommandOptionType.MultipleValue);

            AddCliOption(StrykerInput.DiffCompare, "since", "since", new DiffCompareInput().HelpText, optionType: CommandOptionType.SingleOrNoValue, argumentHint: "comittish");
            AddCliOption(StrykerInput.DashboardCompare, "with-baseline", "baseline", new DashboardCompareInput().HelpText, optionType: CommandOptionType.SingleOrNoValue, argumentHint: "comittish");

            AddCliOption(StrykerInput.DashboardApiKey, "dashboard-api-key", "dk", new DashboardApiKeyInput().HelpText);
            AddCliOption(StrykerInput.AzureFileStorageSas, "azure-fileshare-sas", "sas", new AzureFileStorageSasInput().HelpText);
        }

        private static void RegisterCliOption(CommandLineApplication app, CliOption option)
        {
            var argumentHint = option.OptionType switch
            {
                CommandOptionType.NoValue => "",
                CommandOptionType.SingleOrNoValue => $"[:<{option.ArgumentHint}>]",
                _ => $" <{option.ArgumentHint}>"
            };

            app.Option($"{option.ArgumentShortName}|{option.ArgumentName}{argumentHint}", option.Description, option.OptionType);
        }

        private static CliOption AddCliOption(StrykerInput inputType, string argumentName, string argumentShortName,
            string description, CommandOptionType optionType = CommandOptionType.SingleValue, string argumentHint = null)
        {
            var cliOption = new CliOption
            {
                InputType = inputType,
                ArgumentName = $"--{argumentName}",
                ArgumentShortName = $"-{argumentShortName}",
                Description = description,
                OptionType = optionType,
                ArgumentHint = argumentHint
            };

            CliOptions[argumentName] = cliOption;

            return cliOption;
        }
    }
}
