namespace FlickrApp.Services;

public interface IFileSystemOperations
{
    string GetAppDataDirectory();
    string Combine(string path1, string path2);
    string Combine(string path1, string path2, string path3);
    void CreateDirectory(string path);
    bool FileExists(string path);
    Task WriteAllBytesAsync(string path, byte[] bytes);
    Task DeleteFileAsync(string path);
    string GetFileNameWithoutExtension(string path);
    string GetExtension(string path);
}