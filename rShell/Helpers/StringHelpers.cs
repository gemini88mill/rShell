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
}