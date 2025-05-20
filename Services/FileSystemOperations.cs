namespace FlickrApp.Services;

public class FileSystemOperations : IFileSystemOperations
{
    public string GetAppDataDirectory()
    {
        return FileSystem.AppDataDirectory;
    }

    public string Combine(string path1, string path2)
    {
        return Path.Combine(path1, path2);
    }

    public string Combine(string path1, string path2, string path3)
    {
        return Path.Combine(path1, path2, path3);
    }

    public void CreateDirectory(string path)
    {
        Directory.CreateDirectory(path);
    }

    public bool FileExists(string path)
    {
        return File.Exists(path);
    }

    public Task WriteAllBytesAsync(string path, byte[] bytes)
    {
        return File.WriteAllBytesAsync(path, bytes);
    }

    public Task DeleteFileAsync(string path)
    {
        return Task.Run(() => File.Delete(path));
    }

    public string GetFileNameWithoutExtension(string path)
    {
        return Path.GetFileNameWithoutExtension(path);
    }

    public string GetExtension(string path)
    {
        return Path.GetExtension(path);
    }
}