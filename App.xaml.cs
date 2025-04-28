namespace FlickrApp;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        Application.Current.UserAppTheme = AppTheme.Light;
        return new Window(new AppShell());
    }
}