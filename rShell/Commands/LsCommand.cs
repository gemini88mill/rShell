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
    try
    {
      var currentDirectory = Environment.CurrentDirectory;
      var items = GetDirectoryItems(currentDirectory, settings);

      if (settings.LongFormat)
      {
        DisplayLongFormat(items);
      }
      else
      {
        DisplaySimpleFormat(items);
      }

      return 0;
    }
    catch (Exception ex)
    {
      AnsiConsole.MarkupLine($"[red]Error:[/] {ex.Message}");
      return 1;
    }
  }

  private List<FileSystemInfo> GetDirectoryItems(string directory, Settings settings)
  {
    var items = new List<FileSystemInfo>();
    var searchOption = settings.Recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

    // Get files
    var files = Directory.GetFiles(directory, "*", searchOption)
      .Select(f => new FileInfo(f))
      .Where(f => settings.All || !IsHidden(f))
      .Cast<FileSystemInfo>();

    // Get directories
    var directories = Directory.GetDirectories(directory, "*", searchOption)
      .Select(d => new DirectoryInfo(d))
      .Where(d => settings.All || !IsHidden(d))
      .Cast<FileSystemInfo>();

    items.AddRange(files);
    items.AddRange(directories);

    return items.OrderBy(i => i.Name).ToList();
  }

  private bool IsHidden(FileSystemInfo item)
  {
    return item.Attributes.HasFlag(FileAttributes.Hidden) ||
           item.Name.StartsWith(".");
  }

  private void DisplaySimpleFormat(List<FileSystemInfo> items)
  {
    var names = items.Select(item =>
    {
      var name = item.Name;
      if (item is DirectoryInfo)
      {
        name = $"[blue]{name}[/]";
      }
      return name;
    }).ToArray();

    AnsiConsole.Write(new Columns(names));
  }

  private void DisplayLongFormat(List<FileSystemInfo> items)
  {
    var table = new Table();
    table.Border = TableBorder.None;
    table.AddColumn("Name");
    table.AddColumn("Owner");
    table.AddColumn("Size");
    table.AddColumn("Modified");

    foreach (var item in items)
    {
      var name = item.Name;
      if (item is DirectoryInfo)
      {
        name = $"[blue]{name}[/]";
      }

      var owner = GetOwner(item);
      var size = GetFormattedSize(item);
      var modified = item.LastWriteTime.ToString("yyyy-MM-dd HH:mm");

      table.AddRow(name, owner, size, modified);
    }

    AnsiConsole.Write(table);
  }

  private string GetOwner(FileSystemInfo item)
  {
    try
    {
#if WINDOWS
      if (item is FileInfo fileInfo)
      {
        var accessControl = fileInfo.GetAccessControl();
        var owner = accessControl.GetOwner(typeof(System.Security.Principal.SecurityIdentifier));
        return owner?.Value ?? "Unknown";
      }
      else if (item is DirectoryInfo dirInfo)
      {
        var accessControl = dirInfo.GetAccessControl();
        var owner = accessControl.GetOwner(typeof(System.Security.Principal.SecurityIdentifier));
        return owner?.Value ?? "Unknown";
      }
#endif
      return "Unknown";
    }
    catch
    {
      return "Unknown";
    }
  }

  private string GetFormattedSize(FileSystemInfo item)
  {
    if (item is DirectoryInfo)
    {
      return "-";
    }

    var fileInfo = (FileInfo)item;
    var bytes = fileInfo.Length;

    if (bytes < 1024)
      return $"{bytes} B";
    else if (bytes < 1024 * 1024)
      return $"{bytes / 1024.0:F1} KB";
    else if (bytes < 1024 * 1024 * 1024)
      return $"{bytes / (1024.0 * 1024.0):F1} MB";
    else
      return $"{bytes / (1024.0 * 1024.0 * 1024.0):F1} GB";
  }
}