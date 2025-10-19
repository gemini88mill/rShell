using Spectre.Console.Cli;
using Spectre.Console;

namespace rShell.Commands;

public class LsCommand : Command<LsCommand.Settings>
{
  public class Settings : CommandSettings
  {
    [CommandOption("-l")]
    public bool LongFormat { get; set; }

    [CommandOption("-a")]
    public bool All { get; set; }

    [CommandOption("-r")]
    public bool Recursive { get; set; }


  }

  public override int Execute(CommandContext context, Settings settings)
  {


    return 0;
  }
}