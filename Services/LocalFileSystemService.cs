using Debug = System.Diagnostics.Debug;

namespace FlickrApp.Services
{
    public class LocalFileSystemService : ILocalFileSystemService
    {
        private readonly HttpClient _httpClient;
        private readonly IFileSystemOperations _fileSystem;
        private readonly string _appDataDirectory;

        public LocalFileSystemService(HttpClient httpClient, IFileSystemOperations fileSystem)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
            _appDataDirectory = _fileSystem.GetAppDataDirectory();
        }

        public string GetAppSpecificPhotosDirectory()
        {
            var photosDirectory = _fileSystem.Combine(_appDataDirectory, "DownloadedPhotos");
            _fileSystem.CreateDirectory(photosDirectory);
            return photosDirectory;
        }

        public string GetAppSpecificDownloadsDirectory()
        {
            var downloadsDirectory = _fileSystem.Combine(_appDataDirectory, "GeneralDownloads");
            _fileSystem.CreateDirectory(downloadsDirectory);
            return downloadsDirectory;
        }

        public async Task<string?> SaveImageAsync(string imageUrl, string photoId, string targetDirectory)
        {
            if (string.IsNullOrEmpty(imageUrl) || string.IsNullOrEmpty(targetDirectory) ||
                string.IsNullOrEmpty(photoId))
            {
                Debug.WriteLine("SaveImageAsync: imageUrl, photoId, or targetDirectory is null/empty.");
                return null;
            }

            try
            {
                var fileExtension = ".jpg";
                try
                {
                    var uri = new Uri(imageUrl);
                    var ext = Path.GetExtension(uri.LocalPath);
                    if (!string.IsNullOrEmpty(ext) && (ext.Equals(".jpg", StringComparison.OrdinalIgnoreCase) ||
                                                       ext.Equals(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                                                       ext.Equals(".png", StringComparison.OrdinalIgnoreCase)))
                        fileExtension = ext;
                }
                catch
                {
                }

                var fileName = $"{photoId}{fileExtension}";
                var filePath = _fileSystem.Combine(targetDirectory, fileName);

                if (_fileSystem.FileExists(filePath))
                {
                     Debug.WriteLine($"File already exists at: {filePath}. Skipping download.");
                     return filePath;
                }

                var imageBytes = await _httpClient.GetByteArrayAsync(imageUrl);
                await _fileSystem.WriteAllBytesAsync(filePath, imageBytes);

                Debug.WriteLine($"Image saved locally to: {filePath}");
                return filePath;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"### ERROR saving image from URL {imageUrl} to {targetDirectory}: {ex.Message}");
                return null;
            }
        }

        public async Task DeleteFileAsync(string localFilePath)
        {
            if (string.IsNullOrEmpty(localFilePath) || !_fileSystem.FileExists(localFilePath))
            {
                Debug.WriteLine($"DeleteFileAsync: File path is null/empty or file does not exist: '{localFilePath}'");
                return;
            }

            try
            {
                await _fileSystem.DeleteFileAsync(localFilePath);
                Debug.WriteLine($"File deleted: {localFilePath}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"### ERROR deleting file {localFilePath}: {ex.Message}");
            }
        }
    }
}