using FlickrApp.Entities;
using SQLite;

namespace FlickrApp.Repositories
{
    public class PhotoRepository(SQLiteAsyncConnection database) : IPhotoRepository
    {
        public async Task<List<Photo>> GetAllPhotosAsync()
        {
            return await database.Table<Photo>().ToListAsync();
        }

        public async Task<Photo> GetPhotoByIdAsync(string id)
        {
            return await database.FindAsync<Photo>(id);
        }

        public async Task<int> SavePhotoAsync(Photo photo)
        {
            var existingPhoto = await database.FindAsync<Photo>(photo.Id);

            if (existingPhoto != null) return await database.UpdateAsync(photo);
            else return await database.InsertAsync(photo);
            
        }

        public async Task<int> DeletePhotoAsync(string id)
        {
            return await database.DeleteAsync<Photo>(id);
        }

        public async Task<List<Photo>> SearchPhotosByTitleAsync(string title)
        {
            if (string.IsNullOrEmpty(title))
            {
                return await Task.FromResult(new List<Photo>());
            }

            return await database.Table<Photo>()
                .Where(p => p.Title != null && p.Title.Contains(title))
                .ToListAsync();
        }

        public async Task<List<Photo>> GetPhotosWithLocalFilesAsync()
        {
            return await database.Table<Photo>()
                .Where(p => !string.IsNullOrEmpty(p.LocalFilePath))
                .ToListAsync();
        }

        public async Task<bool> IsPhotoSavedLocallyAsync(string photoId)
        {
            var photo = await database.FindAsync<Photo>(photoId);

            return photo != null && !string.IsNullOrEmpty(photo.LocalFilePath);
        }
    }
}