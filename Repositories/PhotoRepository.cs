using FlickrApp.Entities;
using SQLite;
using SQLiteNetExtensionsAsync.Extensions;

// Per List<T>

// Per Task

namespace FlickrApp.Repositories
{
    public class PhotoRepository(SQLiteAsyncConnection database) : IPhotoRepository
    {
        public string StatusMessage { get; private set; } = string.Empty;

        public async Task<List<PhotoEntity>> GetAllPhotosAsync()
        {
            try
            {
                var list = await database.Table<PhotoEntity>().ToListAsync();
                StatusMessage = list.Count > 0
                    ? $"{list.Count} photo(s) retrieved successfully."
                    : "No photos found in the database.";
                return list;
            }
            catch (SQLiteException ex)
            {
                StatusMessage = $"Error retrieving photos: {ex.Message}";
                return [];
            }
        }

        public async Task<List<PhotoEntity>> GetAllPhotosAsync(int pageNumber, int pageSize)
        {
            try
            {
                var itemsToSkip = (pageNumber - 1) * pageSize;
                var list = await database.Table<PhotoEntity>().Skip(itemsToSkip).Take(pageSize).ToListAsync();
                StatusMessage = list.Count > 0
                    ? $"{list.Count} photo(s) retrieved successfully."
                    : "No photos found in the database.";
                return list;
            }
            catch (SQLiteException ex)
            {
                StatusMessage = $"Error retrieving photos: {ex.Message}";
                return [];
            }
        }

        public async Task<PhotoEntity?> GetPhotoWithDetailByIdAsync(string id)
        {
            try
            {
                var photo = await database.GetWithChildrenAsync<PhotoEntity>(id);
                StatusMessage = photo != null
                    ? $"Photo with ID '{id}' retrieved successfully."
                    : $"Photo with ID '{id}' not found.";
                return photo;
            }
            catch (SQLiteException ex)
            {
                StatusMessage = $"Error finding photo with ID '{id}': {ex.Message}";
                return null;
            }
        }

        public async Task<PhotoEntity?> GetPhotoByIdAsync(string id)
        {
            try
            {
                var photo = await database.FindAsync<PhotoEntity>(id);
                StatusMessage = photo != null
                    ? $"Photo with ID '{id}' retrieved successfully."
                    : $"Photo with ID '{id}' not found.";
                return photo;
            }
            catch (SQLiteException ex)
            {
                StatusMessage = $"Error finding photo with ID '{id}': {ex.Message}";
                return null;
            }
        }

        public async Task<int> AddPhotoAsync(PhotoEntity photoEntity)
        {
            try
            {
                var existingPhoto = await database.Table<PhotoEntity>().Where(p => p.Id == photoEntity.Id)
                    .FirstOrDefaultAsync();
                if (existingPhoto == null)
                {
                    await database.InsertOrReplaceWithChildrenAsync(photoEntity);
                    StatusMessage = $"Photo with ID '{photoEntity.Id}' added successfully.";
                    return 1;
                }

                StatusMessage = $"Photo with ID '{photoEntity.Id}' already exists. Add operation skipped.";
                return 0;   
            }
            catch (SQLiteException ex)
            {
                StatusMessage = $"Error adding photo with ID '{photoEntity.Id}': {ex.Message}";
                return 0;
            }
        }

        public async Task<int> UpdatePhotoAsync(PhotoEntity photoEntity)
        {
            try
            {
                var existingPhoto = await database.Table<PhotoEntity>().Where(p => p.Id == photoEntity.Id)
                    .FirstOrDefaultAsync();
                if (existingPhoto != null)
                {
                    var result = await database.UpdateAsync(photoEntity);
                    StatusMessage = result > 0
                        ? $"Photo with ID '{photoEntity.Id}' updated successfully."
                        : $"Photo with ID '{photoEntity.Id}' was not updated. (No changes or photo not found by update operation).";
                    return result;
                }

                StatusMessage = $"Photo with ID '{photoEntity.Id}' not found. Update operation skipped.";
                return 0;
            }
            catch (SQLiteException ex)
            {
                StatusMessage = $"Error updating photo with ID '{photoEntity.Id}': {ex.Message}";
                return 0;
            }
        }

        public async Task<int> DeletePhotoAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                StatusMessage = "Photo ID cannot be null or empty for deletion.";
                return 0;
            }

            try
            {
                var result = await database.DeleteAsync<PhotoEntity>(id);
                if (result > 0)
                    StatusMessage = $"Photo with ID '{id}' deleted successfully.";
                else
                    StatusMessage = $"Photo with ID '{id}' not found or could not be deleted.";
                return result;
            }
            catch (SQLiteException ex)
            {
                StatusMessage = $"Error deleting photo with ID '{id}': {ex.Message}";
                return 0;
            }
        }

        public async Task<bool> IsPhotoSavedLocallyAsync(string photoId)
        {
            if (string.IsNullOrEmpty(photoId))
            {
                StatusMessage = "Photo ID cannot be null or empty for local check.";
                return false;
            }

            try
            {
                var photo = await database.FindAsync<PhotoEntity>(photoId);
                var isSaved = photo != null && !string.IsNullOrEmpty(photo.LocalFilePath) &&
                              File.Exists(photo.LocalFilePath); 

                if (photo == null)
                    StatusMessage = $"Photo with ID '{photoId}' not found in database for local check.";
                else if (string.IsNullOrEmpty(photo.LocalFilePath))
                    StatusMessage = $"Photo with ID '{photoId}' found, but has no local file path specified.";
                else if (!File.Exists(photo.LocalFilePath))
                    StatusMessage =
                        $"Photo with ID '{photoId}' has a local path ('{photo.LocalFilePath}'), but the file does not exist.";
                else
                    StatusMessage = $"Photo with ID '{photoId}' is saved locally at '{photo.LocalFilePath}'.";
                return isSaved;
            }
            catch (SQLiteException ex)
            {
                StatusMessage = $"Database error checking local status for photo ID '{photoId}': {ex.Message}";
                return false;
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error checking local status for photo ID '{photoId}': {ex.Message}";
                return false;
            }
        }
    }
}