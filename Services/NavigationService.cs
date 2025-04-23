namespace FlickrApp.Services;

public class NavigationService(IServiceProvider sp) : INavigationService
{
    public async Task GoToAsync(string route, IDictionary<string, object>? parameters = null)
    {
        if (parameters != null && parameters.Any())
        {
            var queryParams = string.Join("&",
                parameters.Select(kvp =>
                    $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value.ToString())}"));
            if (!string.IsNullOrEmpty(queryParams))
            {
                route = $"{route}?{queryParams}";
            }
        }

        await Shell.Current.GoToAsync(route);
    }

    public async Task GoBackAsync()
    {
        await Shell.Current.GoToAsync("..");
    }
}