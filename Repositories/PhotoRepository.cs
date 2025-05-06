using FlickrApp.Entities;
using SQLite;

namespace FlickrApp.Repositories
{
    public class PhotoRepository(SQLiteAsyncConnection dbConnection) : IPhotoRepository
    {
        public Task<List<Photo>> GetAllPhotosAsync()
        {
            return dbConnection.Table<Photo>().ToListAsync();
        }

        public async Task<Photo> GetPhotoByIdAsync(string id)
        {
            return await dbConnection.FindAsync<Photo>(id);
        }

        public async Task<int> SavePhotoAsync(Photo photo)
        {
            var existingPhoto = await dbConnection.FindAsync<Photo>(photo.Id);

            if (existingPhoto != null)
            {
                return await dbConnection.UpdateAsync(photo);
            }
            else
            {
                return await dbConnection.InsertAsync(photo);
            }
        }

        public Task<int> DeletePhotoAsync(string id)
        {
            return dbConnection.DeleteAsync<Photo>(id);
        }

        public Task<List<Photo>> SearchPhotosByTitleAsync(string title)
        {
            if (string.IsNullOrEmpty(title))
            {
                return Task.FromResult(new List<Photo>());
            }

            return dbConnection.Table<Photo>()
                .Where(p => p.Title != null && p.Title.Contains(title))
                .ToListAsync();
        }

        public Task<List<Photo>> GetPhotosWithLocalFilesAsync()
        {
            return dbConnection.Table<Photo>()
                .Where(p => !string.IsNullOrEmpty(p.LocalFilePath))
                .ToListAsync();
        }

        public async Task<bool> IsPhotoSavedLocallyAsync(string photoId)
        {
            var photo = await dbConnection.FindAsync<Photo>(photoId);

            return photo != null && !string.IsNullOrEmpty(photo.LocalFilePath);
        }
    }
}