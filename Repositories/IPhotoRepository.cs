using FlickrApp.Entities;
using System.Threading.Tasks;
namespace FlickrApp.Repositories;

public interface IPhotoRepository
{
    Task<List<Photo>> GetAllPhotosAsync(); 
    Task<Photo> GetPhotoByIdAsync(string id); 
    Task<int> SavePhotoAsync(Photo photo); 
    Task<int> DeletePhotoAsync(string id); 
    Task<List<Photo>> SearchPhotosByTitleAsync(string title);
    Task<List<Photo>> GetPhotosWithLocalFilesAsync(); 
    Task<bool> IsPhotoSavedLocallyAsync(string photoId); 
}