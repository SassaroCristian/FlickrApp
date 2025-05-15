using System.Diagnostics;
using FlickrApp.Repositories;

namespace FlickrApp;

public partial class App : Application
{
    private readonly IPhotoRepository _photoRepository;

    public App(IPhotoRepository photoRepository)
    {
        _photoRepository = photoRepository;
        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        if (Current != null) Current.UserAppTheme = AppTheme.Light;
        return new Window(new AppShell());
    }

    protected override void OnStart()
    {
        base.OnStart();
        _ = CleanDatabase();
    }

    private async Task CleanDatabase()
    {
        var photos = await _photoRepository.GetAllPhotosAsync();
        foreach (var photo in photos.Where(photo => !photo.IsSavedLocally))
        {
            await _photoRepository.DeletePhotoAsync(photo.Id);
            Debug.WriteLine(_photoRepository.StatusMessage);
        }
    }
}