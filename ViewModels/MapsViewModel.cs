using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace FlickrApp.ViewModels;

public partial class MapsViewModel : ObservableObject
{
    public MapsViewModel()
    {
    }

    [RelayCommand]
    private async Task AddPinToMap()
    {
        //
    }
}