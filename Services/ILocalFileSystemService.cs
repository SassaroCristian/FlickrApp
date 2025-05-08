using System.Threading.Tasks;

namespace FlickrApp.Services
{
    public interface ILocalFileSystemService
    {
        Task<string?> SaveImageAsync(string imageUrl, string photoId, string targetDirectory);

        Task DeleteFileAsync(string localFilePath);

        string GetAppSpecificPhotosDirectory(); 
        string GetAppSpecificDownloadsDirectory(); 
    }
}