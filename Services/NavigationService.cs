namespace FlickrApp.Services;

public class NavigationService(IServiceProvider sp) : INavigationService
{
    public async Task GoToAsync(string route, IDictionary<string, object>? parameters = null)
    {
        if (parameters?.Count > 0)
            await Shell.Current.GoToAsync(route, parameters);
        else await Shell.Current.GoToAsync(route);
    }

    public async Task GoBackAsync()
    {
        await Shell.Current.GoToAsync("..");
    }
}