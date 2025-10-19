using Spectre.Console;

namespace rShell.Helpers;

public static class Logger
{
  /// <summary>
  /// Writes a message to the console safely, escaping any markup characters
  /// </summary>
  /// <param name="message">The message to write</param>
  public static void Write(string message)
  {
    var escapedMessage = Markup.Escape(message);
    AnsiConsole.WriteLine(escapedMessage);
  }

  /// <summary>
  /// Writes a message to the console with specified color markup
  /// </summary>
  /// <param name="message">The message to write</param>
  /// <param name="color">The color to apply (e.g., "yellow", "red", "blue", "green")</param>
  public static void Write(string message, string color)
  {
    AnsiConsole.MarkupLine($"[{color}]{message}[/]");
  }

  /// <summary>
  /// Displays a warning message with yellow formatting
  /// </summary>
  /// <param name="message">The warning message to display</param>
  public static void Warning(string message)
  {
    AnsiConsole.MarkupLine($"[yellow]Warning:[/] {message}");
  }

  /// <summary>
  /// Displays an error message with red formatting
  /// </summary>
  /// <param name="message">The error message to display</param>
  public static void Error(string message)
  {
    AnsiConsole.MarkupLine($"[red]Error:[/] {message}");
  }

  /// <summary>
  /// Displays an exception with rich formatting including stack trace
  /// </summary>
  /// <param name="ex">The exception to display</param>
  public static void Exception(Exception ex)
  {
    AnsiConsole.WriteException(ex);
  }

  /// <summary>
  /// Displays data in a grid/table format
  /// </summary>
  /// <param name="data">2D array where first row contains headers</param>
  public static void Grid(string[][] data)
  {
    if (data == null || data.Length == 0)
    {
      Write("No data to display");
      return;
    }

    var table = new Table();

    // Add columns from first row (headers)
    for (int i = 0; i < data[0].Length; i++)
    {
      table.AddColumn(data[0][i]);
    }

    // Add data rows (skip first row as it's headers)
    for (int i = 1; i < data.Length; i++)
    {
      table.AddRow(data[i]);
    }

    AnsiConsole.Write(table);
  }

  /// <summary>
  /// Displays data in a grid/table format with object data
  /// </summary>
  /// <param name="data">2D array where first row contains headers</param>
  public static void Grid(object[][] data)
  {
    if (data == null || data.Length == 0)
    {
      Write("No data to display");
      return;
    }

    var table = new Table();

    // Add columns from first row (headers)
    for (int i = 0; i < data[0].Length; i++)
    {
      table.AddColumn(data[0][i]?.ToString() ?? "");
    }

    // Add data rows (skip first row as it's headers)
    for (int i = 1; i < data.Length; i++)
    {
      var rowData = data[i].Select(item => item?.ToString() ?? "").ToArray();
      table.AddRow(rowData);
    }

    AnsiConsole.Write(table);
  }
}
