using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FlickrApp.Models;
using FlickrApp.Services;

namespace FlickrApp.ViewModels;

public partial class AppShellViewModel : ObservableObject
{
    private readonly IFlickrApiService? _flickr;

    private int _lastIndex = -1;
    private List<FlickrPhoto> _photos = [];

    [ObservableProperty] private bool _isFlyoutOpen;
    [ObservableProperty] private string _headerBackgroundSource = "flyout_header.jpg";

    public AppShellViewModel()
    {
    }

    public AppShellViewModel(IFlickrApiService flickr)
    {
        _flickr = flickr;
        Task.Run(GetBackgroundSourceAsync);
    }

    partial void OnIsFlyoutOpenChanged(bool value)
    {
        if (value) return;
        Task.Run(GetBackgroundSourceAsync);
    }

    private async Task GetBackgroundSourceAsync()
    {
        if (_flickr == null) return;

        if (_photos.Count == 0)
            _photos = await _flickr.SearchAsync("background", string.Empty, 1, 10);

        var index = Random.Shared.Next(0, _photos.Count);
        while (_lastIndex == index)
            index = Random.Shared.Next(0, _photos.Count);

        HeaderBackgroundSource = _photos[index].MediumUrl;
        _lastIndex = index;
    }
}