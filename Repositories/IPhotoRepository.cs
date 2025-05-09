using FlickrApp.Entities;

namespace FlickrApp.Repositories;

/// <summary>
///     Defines a contract for data operations related to photos.
///     Implementations of this interface will handle the persistence
///     and retrieval of <see cref="PhotoEntity" /> objects.
/// </summary>
public interface IPhotoRepository
{
    public string StatusMessage { get; }
    
    /// <summary>
    ///     Asynchronously retrieves all photos from the data source.
    /// </summary>
    /// <returns>
    ///     A <see cref="Task{TResult}" /> that represents the asynchronous operation.
    ///     The task result contains a <see cref="List{T}" /> of <see cref="PhotoEntity" /> objects.
    ///     Returns an empty list if no photos are found.
    /// </returns>
    Task<List<PhotoEntity>> GetAllPhotosAsync();

    /// <summary>
    ///     Asynchronously retrieves a paginated list of photos from the data source.
    /// </summary>
    /// <param name="pageNumber">The page number to retrieve (1-based index).</param>
    /// <param name="pageSize">The number of photos to retrieve per page.</param>
    /// <returns>
    ///     A <see cref="Task{TResult}" /> that represents the asynchronous operation.
    ///     The task result contains a <see cref="List{T}" /> of <see cref="PhotoEntity" /> objects for the requested page.
    ///     Returns an empty list if the page number is out of range or no photos are found for the given page.
    /// </returns>
    /// <remarks>
    ///     It's recommended that implementations handle invalid <paramref name="pageNumber" /> (e.g., less than 1)
    ///     or <paramref name="pageSize" /> (e.g., less than or equal to 0) gracefully,
    ///     for instance, by returning an empty list or adjusting to default valid values.
    /// </remarks>
    /// <exception cref="System.ArgumentOutOfRangeException">
    ///     May be thrown by implementations if <paramref name="pageNumber" /> or <paramref name="pageSize" />
    ///     are considered invalid and not handled by returning an empty list (e.g., page size <= 0).
    /// </exception>
    Task<List<PhotoEntity>> GetAllPhotosAsync(int pageNumber, int pageSize);

    /// <summary>
    ///     Asynchronously retrieves a specific <see cref="PhotoEntity" /> by its unique identifier,
    ///     including its associated <see cref="DetailEntity" />.
    /// </summary>
    /// <param name="id">The unique identifier of the photo to retrieve.</param>
    /// <returns>
    ///     A <see cref="Task{TResult}" /> that represents the asynchronous operation.
    ///     The task result contains the <see cref="PhotoEntity" /> object with its <see cref="PhotoEntity.Detail" />
    ///     property populated if the photo and its details are found; otherwise, <c>null</c> if the photo
    ///     with the specified ID is not found. The <see cref="PhotoEntity.Detail" /> property will also be
    ///     <c>null</c> if the photo is found but has no associated details.
    /// </returns>
    /// <remarks>
    ///     This method is expected to perform an "eager load" of the photo's details.
    ///     If the photo ID is invalid or not found, the method should return <c>null</c> rather than throwing an exception.
    /// </remarks>
    /// <exception cref="System.ArgumentNullException">
    ///     May be thrown by implementations if the provided <paramref name="id" /> is null or empty.
    /// </exception>
    /// <exception cref="System.Exception">
    ///     May be thrown by implementations if there's an issue accessing the data source,
    ///     though specific database exceptions (e.g., SQLiteException) are more likely.
    /// </exception>
    Task<PhotoEntity?> GetPhotoWithDetailByIdAsync(string id);

    Task<PhotoEntity?> GetPhotoByIdAsync(string id); 

    /// <summary>
    ///     Asynchronously adds a new <see cref="PhotoEntity" /> to the data source.
    /// </summary>
    /// <param name="photoEntity">The <see cref="PhotoEntity" /> object to add.</param>
    /// <returns>
    ///     A <see cref="Task{TResult}" /> that represents the asynchronous operation.
    ///     The task result contains the number of rows affected in the data source (typically 1 if successful).
    /// </returns>
    /// <exception cref="System.ArgumentNullException">
    ///     May be thrown if <paramref name="photoEntity" /> is null, depending on
    ///     the implementation.
    /// </exception>
    Task<int> AddPhotoAsync(PhotoEntity photoEntity);

    /// <summary>
    ///     Asynchronously updates an existing <see cref="PhotoEntity" /> in the data source.
    /// </summary>
    /// <param name="photoEntity">
    ///     The <see cref="PhotoEntity" /> object to update. It is assumed that the ID of this entity
    ///     matches an existing record.
    /// </param>
    /// <returns>
    ///     A <see cref="Task{TResult}" /> that represents the asynchronous operation.
    ///     The task result contains the number of rows affected in the data source (typically 1 if successful, 0 if the photo
    ///     was not found to update).
    /// </returns>
    /// <exception cref="System.ArgumentNullException">
    ///     May be thrown if <paramref name="photoEntity" /> is null, depending on
    ///     the implementation.
    /// </exception>
    Task<int> UpdatePhotoAsync(PhotoEntity photoEntity);

    /// <summary>
    ///     Asynchronously deletes a <see cref="PhotoEntity" /> from the data source using its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the photo to delete.</param>
    /// <returns>
    ///     A <see cref="Task{TResult}" /> that represents the asynchronous operation.
    ///     The task result contains the number of rows affected in the data source (typically 1 if successful, 0 if the photo
    ///     was not found).
    /// </returns>
    /// <exception cref="System.ArgumentNullException">
    ///     May be thrown if <paramref name="id" /> is null or empty, depending on
    ///     the implementation.
    /// </exception>
    Task<int> DeletePhotoAsync(string id);

    /// <summary>
    ///     Asynchronously checks if a photo with the specified ID is considered saved locally.
    /// </summary>
    /// <param name="photoId">The unique identifier of the photo to check.</param>
    /// <returns>
    ///     A <see cref="Task{TResult}" /> that represents the asynchronous operation.
    ///     The task result is <c>true</c> if the photo is considered saved locally; otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>
    ///     The definition of "saved locally" depends on the implementation (e.g., it might check
    ///     for the presence of a valid local file path and the existence of the file itself).
    /// </remarks>
    /// <exception cref="System.ArgumentNullException">
    ///     May be thrown if <paramref name="photoId" /> is null or empty, depending
    ///     on the implementation.
    /// </exception>
    Task<bool> IsPhotoSavedLocallyAsync(string photoId);
}