using System.Diagnostics;

namespace FlickrApp.Services;

public class GlobalErrorHandler : IGlobalErrorHandler
{
    public void HandlerException(Exception ex, string platformContext, bool isTerminating)
    {
        var errorMessage =
            $"GLOBAL ERROR CAUGHT\n" +
            $"Platform: {platformContext}\n" +
            $"App is terminating: {isTerminating}\n" +
            $"Exception Type: {ex.GetType().FullName}\n" +
            $"Message: {ex.Message}\n" +
            $"Stack Trace:\n{ex.StackTrace}";

        Debug.WriteLine("**************************************************");
        Debug.WriteLine(errorMessage);
        Debug.WriteLine("**************************************************");
    }
}