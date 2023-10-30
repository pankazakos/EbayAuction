namespace webapi.Utilities.ServiceUtils;

public interface IAppLogHelper
{
    void LogSuccess(string className, string methodName, string message);
    void LogFailure(string className, string methodName, string message, Exception ex);
}