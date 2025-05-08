using Debug = System.Diagnostics.Debug;

namespace FlickrApp.Services
{
    public class LocalFileSystemService(HttpClient httpClient) : ILocalFileSystemService
    {
        private readonly string _appDataDirectory = FileSystem.AppDataDirectory;

        public string GetAppSpecificPhotosDirectory()
        {
            var photosDirectory = Path.Combine(_appDataDirectory, "DownloadedPhotos");
            Directory.CreateDirectory(photosDirectory);
            return photosDirectory;
        }

        public string GetAppSpecificDownloadsDirectory()
        {
            var downloadsDirectory = Path.Combine(_appDataDirectory, "GeneralDownloads");
            Directory.CreateDirectory(downloadsDirectory);
            return downloadsDirectory;
        }

        public async Task<string?> SaveImageAsync(string imageUrl, string photoId, string targetDirectory)
        {
            if (string.IsNullOrEmpty(imageUrl) || string.IsNullOrEmpty(targetDirectory))
            {
                Debug.WriteLine("SaveImageAsync: imageUrl or targetDirectory is null/empty.");
                return null;
            }

            try
            {
                // TODO: Determinare l'estensione corretta dall'URL se possibile
                var fileName = $"{photoId}.jpg";
                var filePath = Path.Combine(targetDirectory, fileName);

                if (File.Exists(filePath))
                {
                     Debug.WriteLine($"File already exists at: {filePath}. Skipping download.");
                     return filePath; 
                }

                var imageBytes = await httpClient.GetByteArrayAsync(imageUrl);
                await File.WriteAllBytesAsync(filePath, imageBytes);

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
            if (string.IsNullOrEmpty(localFilePath) || !File.Exists(localFilePath))
            {
                Debug.WriteLine("DeleteFileAsync: File path is null/empty or file does not exist.");
                return;
            }

            try
            {
                await Task.Run(() => File.Delete(localFilePath));
                Debug.WriteLine($"File deleted: {localFilePath}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"### ERROR deleting file {localFilePath}: {ex.Message}");
                // TODO: Gestire l'errore di cancellazione
            }
        }

        // TODO: Aggiungere un metodo per salvare in directory pubbliche se necessario per il download button
    }
}