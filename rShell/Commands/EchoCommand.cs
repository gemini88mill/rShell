using Spectre.Console;
using Spectre.Console.Cli;
using rShell.Helpers;

namespace rShell.Commands;

public class EchoCommand : Command<EchoCommand.Settings>
{
  public class Settings : CommandSettings
  {
    [CommandArgument(0, "[text]")]
    public string Text { get; set; } = string.Empty;

    [CommandOption("--uppercase|-u")]
    public bool Uppercase { get; set; }

    [CommandOption("--repeat|-r")]
    public int Repeat { get; set; } = 1;
  }

  public override int Execute(CommandContext context, Settings settings)
  {
    if (string.IsNullOrEmpty(settings.Text))
    {
      Logger.Error("No text provided to echo.");
      return 1;
    }

    var text = settings.Text;

    if (settings.Uppercase)
    {
      text = text.ToUpper();
    }

    for (int i = 0; i < settings.Repeat; i++)
    {
      Logger.Write(text);
    }

    return 0;
  }
}
