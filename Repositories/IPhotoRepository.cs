using FlickrApp.Entities;

namespace FlickrApp.Repositories;

public interface IPhotoRepository
{
    public List<Photo> GetPhotos();
    Task<Photo> GetPhotoByIdAsync
}