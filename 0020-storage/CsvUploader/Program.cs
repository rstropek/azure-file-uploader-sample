using CsvUploader;
using Serilog;
using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Diagnostics.CodeAnalysis;

Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();

var rootCommand = GetRootCommand();
rootCommand.Add(GetListCommand());
rootCommand.Add(GetUploadCommand());
rootCommand.Add(GetParseCommand());
rootCommand.Add(GetShareCommand());
await rootCommand.InvokeAsync(args);

Command GetRootCommand()
{
    var accountKeyOption = new Option<string>("--account-key", "Account key of storage account");
    accountKeyOption.AddAlias("-k");

    var accountNameOption = new Option<string>("--account-name", "Account name of storage account");
    accountNameOption.AddAlias("-n");

    var useAzuriteOption = new Option<bool>("--use-azurite", () => false, "Use local azurite");
    useAzuriteOption.AddAlias("-a");

    var containerNameOption = new Option<string>("--container-name", () => "csv-upload", "Name of the container");
    containerNameOption.AddAlias("-c");

    return new RootCommand
    {
        accountKeyOption,
        accountNameOption,
        containerNameOption,
        useAzuriteOption,
    };
}

Command GetListCommand()
{
    return new Command("list")
    {
        Handler = CommandHandler.Create<ConnectionParameters>(CsvUploaderCommands.List),
    };
}

Command GetUploadCommand()
{
    var fileNameOption = new Option<string>("--source-file", "Name of file to upload");
    fileNameOption.IsRequired = true;
    fileNameOption.AddAlias("-f");

    var destinationFileNameOption = new Option<string>("--destination-file", "Filename in destination");
    destinationFileNameOption.AddAlias("-d");

    var customerOption = new Option<string>("--customer", "Customer metadata for upload file");

    var child = new Command("upload")
    {
        fileNameOption,
        destinationFileNameOption,
        customerOption,
    };
    child.Handler = CommandHandler.Create<UploadParameters>(CsvUploaderCommands.Upload);

    return child;
}

Command GetParseCommand()
{
    var fileOption = new Option<string>("--file", "Name of uploaded file to parse");
    fileOption.IsRequired = true;
    fileOption.AddAlias("-f");

    var child = new Command("parse")
    {
        fileOption,
    };
    child.Handler = CommandHandler.Create<ParseParameters>(CsvUploaderCommands.Parse);

    return child;
}

Command GetShareCommand()
{
    var fileOption = new Option<string>("--file", "Name of uploaded file to share");
    fileOption.IsRequired = true;
    fileOption.AddAlias("-f");

    var hoursOption = new Option<int>("--hours", () => 1, "Number of hours that the link should be valid");
    hoursOption.AddAlias("-h");

    var child = new Command("share")
    {
        fileOption,
        hoursOption,
    };
    child.Handler = CommandHandler.Create<ShareParameters>(CsvUploaderCommands.Share);

    return child;
}

internal record ConnectionParameters(
    string? AccountKey,
    string? AccountName,
    string ContainerName,
    bool UseAzurite)
{
    [MemberNotNull(nameof(AccountName), nameof(AccountKey))]
    public void EnsureNameAndKeyAreSet()
    {
        if (string.IsNullOrEmpty(AccountName))
        {
            throw new ArgumentException("Expecting account name", nameof(AccountName));
        }

        if (string.IsNullOrEmpty(AccountKey))
        {
            throw new ArgumentException("Expecting account key", nameof(AccountKey));
        }
    }
}

internal record UploadParameters(string? AccountKey,
    string? AccountName,
    string ContainerName,
    bool UseAzurite,
    string SourceFile,
    string? DestinationFile,
    string? Customer)
    : ConnectionParameters(AccountKey, AccountName, ContainerName, UseAzurite);

internal record ParseParameters(string? AccountKey,
    string? AccountName,
    string ContainerName,
    bool UseAzurite,
    string File)
    : ConnectionParameters(AccountKey, AccountName, ContainerName, UseAzurite);

internal record ShareParameters(string? AccountKey,
    string? AccountName,
    string ContainerName,
    bool UseAzurite,
    string File,
    int Hours)
    : ConnectionParameters(AccountKey, AccountName, ContainerName, UseAzurite);
