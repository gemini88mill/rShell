using LibGit2Sharp;
using rShell.Helpers;

public static class StringHelpers
{
  public static string GetCurrentDirectory(bool fullPath = false)
  {
    var currentDirectory = Environment.CurrentDirectory;
    if (fullPath)
    {
      return currentDirectory;
    }
    else
    {
      return Path.GetFileName(currentDirectory);
    }
  }

  public static string GetGitBranch()
  {
    try
    {
      using var repo = new Repository(Environment.CurrentDirectory);
      return repo.Head.FriendlyName;
    }
    catch (RepositoryNotFoundException)
    {
      // Not a Git repository
      Logger.Write("Not a Git repository", "yellow");
      return null;
    }
    catch
    {
      // Handle other exceptions (permissions, corrupted repo, etc.)
      Logger.Write("Error getting Git branch", "red");
      return null;
    }
  }

  public static string GetAskPrompt()
  {
    var user = Environment.UserName;
    var currentDirectory = GetCurrentDirectory();
    var gitBranch = GetGitBranch();

    var prompt = $"[bold red]{user}[/] [bold green]▶[/] [dim]{currentDirectory}[/] [bold yellow]({gitBranch})[/]";

    return prompt;
  }
}