using System.Collections.ObjectModel;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FlickrApp.Models;
using FlickrApp.Services;
using Sensors = Microsoft.Maui.Devices.Sensors;

namespace FlickrApp.ViewModels;

public class WonderLocation
{
    public string Name { get; set; }
    public Sensors.Location Location { get; set; }

    public override string ToString()
    {
        return Name;
    }
}

public partial class MapsViewModel : ObservableObject
{
    private readonly IFlickrApiService _flickr;
    private readonly INavigationService _navigation;
    [ObservableProperty] private bool _isListFull;
    [ObservableProperty] private bool _isPinned;
    [ObservableProperty] private string? _location = string.Empty;
    [ObservableProperty] private ObservableCollection<FlickrPhoto> _photos = [];
    private double _pinLatitude;

    private double _pinLongitude;

    [ObservableProperty] private WonderLocation? _selectedWonder;


    public MapsViewModel()
    {
    }

    public MapsViewModel(INavigationService navigation, IFlickrApiService flickr)
    {
        _navigation = navigation;
        _flickr = flickr;

        Wonders = new ObservableCollection<WonderLocation>
        {
            new() { Name = "Great Wall of China", Location = new Sensors.Location(40.4319, 116.5704) },
            new() { Name = "Petra", Location = new Sensors.Location(30.3285, 35.4442) },
            new() { Name = "Colosseum", Location = new Sensors.Location(41.8902, 12.4922) },
            new() { Name = "Chichen Itza", Location = new Sensors.Location(20.6843, -88.5680) },
            new() { Name = "Machu Picchu", Location = new Sensors.Location(-13.1631, -72.5450) },
            new() { Name = "Taj Mahal", Location = new Sensors.Location(27.1751, 78.0421) },
            new() { Name = "Christ the Redeemer", Location = new Sensors.Location(-22.9519, -43.2105) },
            new() { Name = "OverIT - Fiume Veneto (HQ)", Location = new Sensors.Location(45.9453, 12.7754) },
            new() { Name = "OverIT - Milano", Location = new Sensors.Location(45.4655, 9.1885) },
            new() { Name = "OverIT - Roma", Location = new Sensors.Location(41.8327, 12.4088) },
            new() { Name = "OverIT - Udine", Location = new Sensors.Location(46.0725, 13.2366) },
            new() { Name = "OverIT - Torino", Location = new Sensors.Location(45.0987, 7.6608) },
            new() { Name = "OverIT - Miami", Location = new Sensors.Location(25.7623, -80.1914) },
            new() { Name = "OverIT - München", Location = new Sensors.Location(48.1376, 11.5343) },
            new() { Name = "OverIT - London", Location = new Sensors.Location(51.5113, -0.0915) },
            new() { Name = "OverIT - Boston", Location = new Sensors.Location(42.3023, -71.2165) }
        };
    }

    public ObservableCollection<WonderLocation> Wonders { get; }

    partial void OnSelectedWonderChanged(WonderLocation? value)
    {
        if (value != null)
        {
        }
    }

    public async Task AddPinToMap(double latitude, double longitude)
    {
        IsListFull = false;
        IsPinned = true;

        _pinLatitude = latitude;
        _pinLongitude = longitude;

        var locations = await Geocoding.GetPlacemarksAsync(latitude, longitude);
        Location = locations.FirstOrDefault()?.CountryName;

        await LoadItemsAsync(latitude, longitude);
        if (Photos.Count > 0) IsListFull = true;
    }

    private async Task LoadItemsAsync(double latitude, double longitude)
    {
        var photos =
            await _flickr.GetForLocationAsync(latitude.ToString(CultureInfo.InvariantCulture),
                longitude.ToString(CultureInfo.InvariantCulture), 1, 4);
        Photos = new ObservableCollection<FlickrPhoto>(photos);
    }

    [RelayCommand]
    private async Task GoToMapResultsAsync()
    {
        await _navigation.GoToAsync("MapResultsPage", new Dictionary<string, object>
        {
            { "Latitude", _pinLatitude.ToString(CultureInfo.InvariantCulture) },
            { "Longitude", _pinLongitude.ToString(CultureInfo.InvariantCulture) }
        });
    }
}