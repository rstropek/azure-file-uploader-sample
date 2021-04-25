using CsvUploader;
using Serilog;
using System;
using System.CommandLine;
using System.CommandLine.Invocation;

Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();

var accountKeyOption = new Option<string>("--account-key", "Account key of storage account");
accountKeyOption.AddAlias("-k");

var accountNameOption = new Option<string>("--account-name", "Account name of storage account");
accountNameOption.AddAlias("-n");

var useAzuriteOption = new Option<bool>("--use-azurite", () => false, "Use local azurite");
useAzuriteOption.AddAlias("-a");

var containerNameOption = new Option<string>("--container-name", () => "csv-upload", "Name of the container");
containerNameOption.AddAlias("-c");

var rootCommand = new RootCommand
{
    accountKeyOption,
    accountNameOption,
    containerNameOption,
    useAzuriteOption,
};

var child = new Command("list")
{
    Handler = CommandHandler.Create<ConnectionParameters>(CsvUploaderCommands.List),
};
rootCommand.Add(child);

await rootCommand.InvokeAsync(args);

internal record ConnectionParameters(
    string? AccountKey,
    string? AccountName,
    string ContainerName,
    bool UseAzurite);
