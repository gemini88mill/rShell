using Spectre.Console;
using Spectre.Console.Cli;
using rShell.Commands;
using rShell.Helpers;

var app = new CommandApp();
app.Configure(config =>
{
  config.AddCommand<EchoCommand>("echo")
    .WithDescription("Echo the given text");
  config.AddCommand<LsCommand>("ls")
    .WithDescription("List the contents of the current directory");
});

// Check if we have command line arguments (direct mode)
if (args.Length > 0)
{
  // Direct mode: execute command and exit
  return await app.RunAsync(args);
}
else
{
  // Interactive mode: start REPL loop
  AnsiConsole.MarkupLine("[bold blue]rShell[/] - Interactive REPL");
  AnsiConsole.MarkupLine("Type [yellow]exit[/] or [yellow]quit[/] to exit, [yellow]help[/] for available commands.");
  Logger.Write("");

  while (true)
  {
    try
    {
      var input = AnsiConsole.Ask<string>($"[bold green]rShell>[/] [dim]{StringHelpers.GetCurrentDirectory()}[/]> ");

      if (string.IsNullOrWhiteSpace(input))
        continue;

      var trimmedInput = input.Trim();

      // Handle exit commands
      if (trimmedInput.Equals("exit", StringComparison.OrdinalIgnoreCase) ||
          trimmedInput.Equals("quit", StringComparison.OrdinalIgnoreCase) ||
          trimmedInput.Equals("q", StringComparison.OrdinalIgnoreCase))
      {
        AnsiConsole.MarkupLine("[yellow]Goodbye![/]");
        break;
      }

      // Handle help command
      if (trimmedInput.Equals("help", StringComparison.OrdinalIgnoreCase))
      {
        await app.RunAsync(new[] { "--help" });
        continue;
      }


      // Parse and execute the command
      var commandArgs = trimmedInput.Split(' ', StringSplitOptions.RemoveEmptyEntries);
      var result = await app.RunAsync(commandArgs);

      if (result != 0)
      {
        Logger.Error($"Command failed with exit code {result}");
      }
    }
    catch (Exception ex)
    {
      Logger.Exception(ex);
    }
  }
}

return 0;
