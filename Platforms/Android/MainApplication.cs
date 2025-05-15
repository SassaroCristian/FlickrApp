using Android.App;
using Android.Runtime;
using FlickrApp.Services;

namespace FlickrApp;

[Application]
public class MainApplication : MauiApplication
{
    public MainApplication(IntPtr handle, JniHandleOwnership ownership)
        : base(handle, ownership)
    {
    }

    public override void OnCreate()
    {
        base.OnCreate();
        AndroidEnvironment.UnhandledExceptionRaiser += HandleAndroidUnhandledException;
    }

    private static void HandleAndroidUnhandledException(object? sender, RaiseThrowableEventArgs e)
    {
        var errorHandler = IPlatformApplication.Current?.Services.GetService<IGlobalErrorHandler>();

        errorHandler?.HandlerException(e.Exception, "Android Environment.UnhandledExceptionRaiser", true);
    }

    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}