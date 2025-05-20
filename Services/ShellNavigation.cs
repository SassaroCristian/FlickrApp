namespace FlickrApp.Services;

public class ShellNavigation : IShellNavigation
{
    public Task GoToAsync(ShellNavigationState state)
    {
        return Shell.Current.GoToAsync(state);
    }

    public Task GoToAsync(ShellNavigationState state, IDictionary<string, object> parameters)
    {
        return Shell.Current.GoToAsync(state, parameters);
    }
}