using FlickrApp.Entities;
using SQLite;
// Per List<T>

// Per Task

namespace FlickrApp.Repositories
{
    public class PhotoRepository(SQLiteAsyncConnection database) : IPhotoRepository
    {
        public string MessageStatus = string.Empty;

        public async Task<List<PhotoEntity>> GetAllPhotosAsync()
        {
            try
            {
                var list = await database.Table<PhotoEntity>().ToListAsync();
                if (list.Count > 0)
                    MessageStatus = $"{list.Count} photo(s) retrieved successfully.";
                else
                    MessageStatus = "No photos found in the database.";
                return list;
            }
            catch (SQLiteException ex)
            {
                MessageStatus = $"Error retrieving photos: {ex.Message}";
                return new List<PhotoEntity>();
            }
        }

        public async Task<PhotoEntity?> GetPhotoByIdAsync(string id)
        {
            try
            {
                var photo = await database.FindAsync<PhotoEntity>(id);
                if (photo != null)
                    MessageStatus = $"Photo with ID '{id}' retrieved successfully.";
                else
                    MessageStatus = $"Photo with ID '{id}' not found.";
                return photo;
            }
            catch (SQLiteException ex)
            {
                MessageStatus = $"Error finding photo with ID '{id}': {ex.Message}";
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
                    var result = await database.InsertAsync(photoEntity);
                    if (result > 0)
                        MessageStatus = $"Photo with ID '{photoEntity.Id}' added successfully.";
                    else
                        MessageStatus = $"Failed to add photo with ID '{photoEntity.Id}'. No rows affected.";
                    return result;
                }
                else
                {
                    MessageStatus = $"Photo with ID '{photoEntity.Id}' already exists. Add operation skipped.";
                    return 0;
                }
            }
            catch (SQLiteException ex)
            {
                MessageStatus = $"Error adding photo with ID '{photoEntity.Id}': {ex.Message}";
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
                    if (result > 0)
                        MessageStatus = $"Photo with ID '{photoEntity.Id}' updated successfully.";
                    else
                        MessageStatus =
                            $"Photo with ID '{photoEntity.Id}' was not updated. (No changes or photo not found by update operation).";
                    return result;
                }
                else
                {
                    MessageStatus = $"Photo with ID '{photoEntity.Id}' not found. Update operation skipped.";
                    return 0;
                }
            }
            catch (SQLiteException ex)
            {
                MessageStatus = $"Error updating photo with ID '{photoEntity.Id}': {ex.Message}";
                return 0;
            }
        }

        public async Task<int> DeletePhotoAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                MessageStatus = "Photo ID cannot be null or empty for deletion.";
                return 0;
            }

            try
            {
                var result = await database.DeleteAsync<PhotoEntity>(id);
                if (result > 0)
                    MessageStatus = $"Photo with ID '{id}' deleted successfully.";
                else
                    MessageStatus = $"Photo with ID '{id}' not found or could not be deleted.";
                return result;
            }
            catch (SQLiteException ex)
            {
                MessageStatus = $"Error deleting photo with ID '{id}': {ex.Message}";
                return 0;
            }
        }

        public async Task<bool> IsPhotoSavedLocallyAsync(string photoId)
        {
            if (string.IsNullOrEmpty(photoId))
            {
                MessageStatus = "Photo ID cannot be null or empty for local check.";
                return false;
            }

            try
            {
                var photo = await database.FindAsync<PhotoEntity>(photoId);
                var isSaved = photo != null && !string.IsNullOrEmpty(photo.LocalFilePath) &&
                              File.Exists(photo.LocalFilePath); // Aggiunto System.IO.File.Exists

                if (photo == null)
                    MessageStatus = $"Photo with ID '{photoId}' not found in database for local check.";
                else if (string.IsNullOrEmpty(photo.LocalFilePath))
                    MessageStatus = $"Photo with ID '{photoId}' found, but has no local file path specified.";
                else if (!File.Exists(photo.LocalFilePath))
                    MessageStatus =
                        $"Photo with ID '{photoId}' has a local path ('{photo.LocalFilePath}'), but the file does not exist.";
                else
                    MessageStatus = $"Photo with ID '{photoId}' is saved locally at '{photo.LocalFilePath}'.";
                return isSaved;
            }
            catch (SQLiteException ex)
            {
                MessageStatus = $"Database error checking local status for photo ID '{photoId}': {ex.Message}";
                return false;
            }
            catch (Exception ex)
            {
                MessageStatus = $"Error checking local status for photo ID '{photoId}': {ex.Message}";
                return false;
            }
        }
    }
}