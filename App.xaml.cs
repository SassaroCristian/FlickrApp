namespace FlickrApp;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        if (Current != null) Current.UserAppTheme = AppTheme.Light;
        return new Window(new AppShell());
    }
}