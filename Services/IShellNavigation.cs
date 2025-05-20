namespace FlickrApp.Services;

public interface IShellNavigation
{
    public Task GoToAsync(ShellNavigationState state);
    public Task GoToAsync(ShellNavigationState state, IDictionary<string, object> parameters);
}