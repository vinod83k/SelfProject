using Project.CommandProcessor.Utilities;
using System;
using System.Text.RegularExpressions;

namespace Project.CommandProcessor
{
    public static class ExceptionHelper
    {
        public static bool CanHandle(Exception ex)
        {
            var exceptionParser = new ExceptionParser(ex);
            return CanHandle(exceptionParser.GetMessages());
        }

        public static string GetMessage(Exception ex)
        {
            var exceptionParser = new ExceptionParser(ex);
            return exceptionParser.GetMessages();
        }

        private static bool CanHandle(string message)
        {
            const string pattern = "UNIQUE KEY constraint|FOREIGN KEY constraint|Row was updated or deleted by another transaction|Record already deleted|No row with the given identifier exists";
            Match m = Regex.Match(message, pattern, RegexOptions.IgnoreCase);
            return m.Success;
        }
    }
}
