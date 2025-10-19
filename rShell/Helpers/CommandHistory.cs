using System.Text;

namespace rShell.Helpers;

public class CommandHistory
{
  private readonly List<string> _history = new();
  private int _currentIndex = -1;
  private readonly string _historyFilePath;
  private const int MaxHistorySize = 1000;

  public CommandHistory()
  {
    _historyFilePath = Path.Combine(
      Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
      ".rshell_history"
    );
    LoadHistory();
  }

  /// <summary>
  /// Adds a command to the history
  /// </summary>
  /// <param name="command">The command to add</param>
  public void AddCommand(string command)
  {
    if (string.IsNullOrWhiteSpace(command))
      return;

    // Don't add duplicate consecutive commands
    if (_history.Count > 0 && _history[_history.Count - 1] == command)
      return;

    _history.Add(command);

    // Trim to max size if needed
    if (_history.Count > MaxHistorySize)
    {
      _history.RemoveAt(0);
    }

    _currentIndex = _history.Count;
    SaveHistory();
  }

  /// <summary>
  /// Gets the previous command in history (older)
  /// </summary>
  /// <returns>The previous command or empty string if at beginning</returns>
  public string GetPrevious()
  {
    if (_history.Count == 0)
      return string.Empty;

    if (_currentIndex > 0)
    {
      _currentIndex--;
    }

    return _currentIndex >= 0 ? _history[_currentIndex] : string.Empty;
  }

  /// <summary>
  /// Gets the next command in history (newer)
  /// </summary>
  /// <returns>The next command or empty string if at end</returns>
  public string GetNext()
  {
    if (_history.Count == 0)
      return string.Empty;

    if (_currentIndex < _history.Count - 1)
    {
      _currentIndex++;
      return _history[_currentIndex];
    }

    // At the end, return empty string
    _currentIndex = _history.Count;
    return string.Empty;
  }

  /// <summary>
  /// Resets the current position to the end of history
  /// </summary>
  public void Reset()
  {
    _currentIndex = _history.Count;
  }

  /// <summary>
  /// Loads history from the history file
  /// </summary>
  private void LoadHistory()
  {
    try
    {
      if (File.Exists(_historyFilePath))
      {
        var lines = File.ReadAllLines(_historyFilePath, Encoding.UTF8);
        _history.AddRange(lines.Where(line => !string.IsNullOrWhiteSpace(line)));

        // Trim to max size if file has more than max
        if (_history.Count > MaxHistorySize)
        {
          _history.RemoveRange(0, _history.Count - MaxHistorySize);
        }
      }
    }
    catch (Exception)
    {
      // If we can't load history, start with empty history
      _history.Clear();
    }

    _currentIndex = _history.Count;
  }

  /// <summary>
  /// Saves history to the history file
  /// </summary>
  private void SaveHistory()
  {
    try
    {
      var directory = Path.GetDirectoryName(_historyFilePath);
      if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
      {
        Directory.CreateDirectory(directory);
      }

      File.WriteAllLines(_historyFilePath, _history, Encoding.UTF8);
    }
    catch (Exception)
    {
      // If we can't save history, continue without saving
      // This prevents the application from crashing due to file system issues
    }
  }
}
