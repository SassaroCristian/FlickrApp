using FlickrApp.Services;
using Foundation;
using UIKit;

namespace FlickrApp;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
    protected override MauiApp CreateMauiApp()
    {
        return MauiProgram.CreateMauiApp();
    }

    public override bool FinishedLaunching(UIApplication app, NSDictionary options)
    {
        AppDomain.CurrentDomain.UnhandledException += HandleiOSUnhandledException;
        return base.FinishedLaunching(app, options);
    }

    // ReSharper disable once InconsistentNaming
    private static void HandleiOSUnhandledException(object? sender, UnhandledExceptionEventArgs e)
    {
        var exception = e.ExceptionObject as Exception;
        if (exception == null) return;

        var errorHandler = IPlatformApplication.Current?.Services.GetService<IGlobalErrorHandler>();

        errorHandler?.HandlerException(exception, "Android Environment.UnhandledExceptionRaiser", true);
    }
}