namespace FlickrApp.Services;

public class NavigationService(IServiceProvider sp, IShellNavigation shellNavigation) : INavigationService
{
    private readonly IServiceProvider _sp = sp;

    private readonly IShellNavigation _shellNavigation =
        shellNavigation ?? throw new ArgumentNullException(nameof(shellNavigation));

    public async Task GoToAsync(string route, IDictionary<string, object>? parameters = null)
    {
        ShellNavigationState state = route;
        if (parameters?.Count > 0)
            await _shellNavigation.GoToAsync(state, parameters);
        else await _shellNavigation.GoToAsync(state);
    }

    public async Task GoBackAsync()
    {
        await _shellNavigation.GoToAsync("..");
    }
}