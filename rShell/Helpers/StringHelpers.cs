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
      var gitRepoPath = FindGitRepository(Environment.CurrentDirectory);
      if (string.IsNullOrEmpty(gitRepoPath))
      {
        return "";
      }

      using var repo = new Repository(gitRepoPath);
      return repo.Head.FriendlyName;
    }
    catch (RepositoryNotFoundException)
    {
      // Not a Git repository
      return "";
    }
    catch
    {
      // Handle other exceptions (permissions, corrupted repo, etc.)
      return "";
    }
  }

  private static string FindGitRepository(string startPath)
  {
    var currentPath = startPath;

    while (!string.IsNullOrEmpty(currentPath))
    {
      var gitPath = Path.Combine(currentPath, ".git");
      if (Directory.Exists(gitPath) || File.Exists(gitPath))
      {
        return currentPath;
      }

      var parentPath = Directory.GetParent(currentPath)?.FullName;
      if (parentPath == currentPath)
      {
        // Reached root directory
        break;
      }

      currentPath = parentPath;
    }

    return "";
  }

  public static string GetAskPrompt()
  {
    var user = Environment.UserName;
    var currentDirectory = GetCurrentDirectory();
    var gitBranch = GetGitBranch();

    var prompt = $"[bold red]{user}[/] [bold green]▶[/] [dim]{currentDirectory}[/] ";

    if (!string.IsNullOrEmpty(gitBranch))
    {
      prompt += $"[bold yellow]({gitBranch})[/]";
    }

    return prompt;
  }
}