using Serilog.Context;
using Serilog;

namespace webapi.Utilities.ServiceUtils
{
    public class AppLogHelper : IAppLogHelper
    {
        private readonly Serilog.ILogger _logger;

        public AppLogHelper(Serilog.ILogger logger)
        {
            _logger = logger;
        }

        public void LogSuccess(string className, string methodName, string message, params object[] args)
        {
            _logger.Information($"{ColoredText("success", "green")} {className}.{methodName}: {message}", args);
        }

        public void LogFailure(string className, string methodName, string message, params object[] args)
        {
            _logger.Error($"{ColoredText("fail", "red")} {className}.{methodName}: {message}", args);
        }

        private static string ColoredText(string text, string color)
        {
            // ANSI escaped colors

            const string escapeCode = "\u001b[";

            const string green = "32m";

            const string red = "31m";

            switch (color)
            {
                case "green":
                    color = green;
                    break;
                case "red":
                    color = red;
                    break;
                default:
                    return "";
            }

            const string resetEscapeCode = "\u001b[0m";

            return $"{escapeCode}{color}{text}{resetEscapeCode}";
        }
    }
}
