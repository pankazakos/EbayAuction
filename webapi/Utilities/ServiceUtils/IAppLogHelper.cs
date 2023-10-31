namespace webapi.Utilities.ServiceUtils;

public interface IAppLogHelper
{
    void LogSuccess(string className, string methodName, string message, params object[] args);
    void LogFailure(string className, string methodName, string message, params object[] args);
}