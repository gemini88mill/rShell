using Spectre.Console;
using System.Text;

namespace rShell.Helpers;

public static class Input
{
  /// <summary>
  /// Reads a line of input with command history support and arrow navigation
  /// </summary>
  /// <param name="prompt">The prompt to display</param>
  /// <param name="history">The command history instance</param>
  /// <returns>The entered command</returns>
  public static string ReadLine(string prompt, CommandHistory history)
  {
    // Reset history position to end
    history.Reset();

    // Display the prompt with markup
    AnsiConsole.Markup(prompt);

    var input = new StringBuilder();
    int cursorPosition = 0;

    while (true)
    {
      var keyInfo = Console.ReadKey(intercept: true);

      switch (keyInfo.Key)
      {
        case ConsoleKey.Enter:
          // Submit the command
          Console.WriteLine(); // Move to next line
          var result = input.ToString();

          // Add to history if not empty
          if (!string.IsNullOrWhiteSpace(result))
          {
            history.AddCommand(result);
          }

          return result;

        case ConsoleKey.UpArrow:
          // Get previous command from history
          var previousCommand = history.GetPrevious();
          input.Clear();
          input.Append(previousCommand);
          cursorPosition = input.Length;
          RedrawLine(prompt, input.ToString(), cursorPosition);
          break;

        case ConsoleKey.DownArrow:
          // Get next command from history
          var nextCommand = history.GetNext();
          input.Clear();
          input.Append(nextCommand);
          cursorPosition = input.Length;
          RedrawLine(prompt, input.ToString(), cursorPosition);
          break;

        case ConsoleKey.LeftArrow:
          // Move cursor left
          if (cursorPosition > 0)
          {
            cursorPosition--;
            UpdateCursorPosition(prompt, cursorPosition);
          }
          break;

        case ConsoleKey.RightArrow:
          // Move cursor right
          if (cursorPosition < input.Length)
          {
            cursorPosition++;
            UpdateCursorPosition(prompt, cursorPosition);
          }
          break;

        case ConsoleKey.Home:
          // Move cursor to start
          cursorPosition = 0;
          UpdateCursorPosition(prompt, cursorPosition);
          break;

        case ConsoleKey.End:
          // Move cursor to end
          cursorPosition = input.Length;
          UpdateCursorPosition(prompt, cursorPosition);
          break;

        case ConsoleKey.Backspace:
          // Delete character before cursor
          if (cursorPosition > 0)
          {
            input.Remove(cursorPosition - 1, 1);
            cursorPosition--;
            RedrawLine(prompt, input.ToString(), cursorPosition);
          }
          break;

        case ConsoleKey.Delete:
          // Delete character at cursor
          if (cursorPosition < input.Length)
          {
            input.Remove(cursorPosition, 1);
            RedrawLine(prompt, input.ToString(), cursorPosition);
          }
          break;

        case ConsoleKey.C when keyInfo.Modifiers == ConsoleModifiers.Control:
          // Ctrl+C - clear line and return empty
          Console.WriteLine();
          return string.Empty;

        default:
          // Regular character input
          if (!char.IsControl(keyInfo.KeyChar))
          {
            input.Insert(cursorPosition, keyInfo.KeyChar);
            cursorPosition++;
            RedrawLine(prompt, input.ToString(), cursorPosition);
          }
          break;
      }
    }
  }

  /// <summary>
  /// Redraws the entire input line with the current content
  /// </summary>
  private static void RedrawLine(string prompt, string input, int cursorPosition)
  {
    // Move cursor to beginning of line
    Console.SetCursorPosition(0, Console.CursorTop);

    // Clear the line
    Console.Write(new string(' ', Console.WindowWidth - 1));

    // Move cursor back to beginning
    Console.SetCursorPosition(0, Console.CursorTop);

    // Redraw prompt and input
    AnsiConsole.Markup(prompt);
    Console.Write(input);

    // Position cursor correctly
    UpdateCursorPosition(prompt, cursorPosition);
  }

  /// <summary>
  /// Updates cursor position after the prompt
  /// </summary>
  private static void UpdateCursorPosition(string prompt, int cursorPosition)
  {
    // Calculate the actual cursor position accounting for ANSI escape sequences
    var promptLength = GetDisplayLength(prompt);
    Console.SetCursorPosition(promptLength + cursorPosition, Console.CursorTop);
  }

  /// <summary>
  /// Gets the display length of a string with ANSI markup
  /// </summary>
  private static int GetDisplayLength(string text)
  {
    // Strip ANSI escape sequences and Spectre markup to get actual display length
    var cleanText = System.Text.RegularExpressions.Regex.Replace(text, @"\x1b\[[0-9;]*[mK]", "");
    // Also strip Spectre markup like [bold green] and [/]
    cleanText = System.Text.RegularExpressions.Regex.Replace(cleanText, @"\[[^\]]*\]", "");
    return cleanText.Length;
  }
}
