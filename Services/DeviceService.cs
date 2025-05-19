namespace FlickrApp.Services;

public class DeviceService : IDeviceService
{
    public DeviceIdiom Idiom => DeviceInfo.Idiom;
}