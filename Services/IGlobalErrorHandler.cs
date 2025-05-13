namespace FlickrApp.Services;

public interface IGlobalErrorHandler
{
    void HandlerException(Exception ex, string platformContext, bool isTerminating);
}