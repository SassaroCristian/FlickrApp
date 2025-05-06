using SQLite;
using FlickrApp.Entities;


namespace FlickrApp.Repositories
{
    public class PhotoRepository : IPhotoRepository
    {
        private readonly SQLiteAsyncConnection _database;


        public PhotoRepository(SQLiteAsyncConnection dbConnection)
        {
            _database = dbConnection;
        }


        public Task<List<Photo>> GetAllPhotosAsync()
        {
            return _database.Table<Photo>().ToListAsync();
        }

        public async Task<Photo> GetPhotoByIdAsync(string id)
        {
            return await _database.FindAsync<Photo>(id);
        }

        public async Task<int> SavePhotoAsync(Photo photo)
        {
            var existingPhoto = await _database.FindAsync<Photo>(photo.Id);

            if (existingPhoto != null)
            {
                return await _database.UpdateAsync(photo);
            }
            else
            {
                return await _database.InsertAsync(photo);
            }
        }

        public Task<int> DeletePhotoAsync(string id)
        {
            return _database.DeleteAsync<Photo>(id);
        }

        public Task<List<Photo>> SearchPhotosByTitleAsync(string title)
        {
            if (string.IsNullOrEmpty(title))
            {
                return Task.FromResult(new List<Photo>());
            }

            return _database.Table<Photo>()
                .Where(p => p.Title != null && p.Title.Contains(title))
                .ToListAsync();
        }

        public Task<List<Photo>> GetPhotosWithLocalFilesAsync()
        {
            return _database.Table<Photo>()
                .Where(p => !string.IsNullOrEmpty(p.LocalFilePath))
                .ToListAsync();
        }

        public async Task<bool> IsPhotoSavedLocallyAsync(string photoId)
        {
            var photo = await _database.FindAsync<Photo>(photoId);

            return photo != null && !string.IsNullOrEmpty(photo.LocalFilePath);
        }
    }
}