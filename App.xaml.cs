namespace FlickrApp;

public partial class App : Application
{
    private readonly AppShell _shell;

    public App(AppShell shell)
    {
        InitializeComponent();
        _shell = shell;
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        Current.UserAppTheme = AppTheme.Light;
        return new Window(_shell);
    }
}