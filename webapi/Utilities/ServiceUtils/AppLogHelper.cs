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

        public void LogSuccess(string className, string methodName, string message)
        {
            var greenText = "\u001b[32m[success]\u001b[0m"; // \u001b[0m resets the color

            _logger.Information($"{greenText} {className} {methodName}: {message}");
        }

        public void LogFailure(string className, string methodName, string message, Exception ex)
        {
            using (LogContext.PushProperty("Status", "fail"))
            using (LogContext.PushProperty("SourceContext", $"{className}.{methodName}"))
            {
                _logger.Error("Operation failed - {ErrorMessage}", ex.Message);
            }
        }

    }
}
