using FlickrApp.Entities;

namespace FlickrApp.Repositories;

/// <summary>
///     Defines a contract for data operations related to photos.
///     Implementations of this interface will handle the persistence
///     and retrieval of <see cref="PhotoEntity" /> objects.
/// </summary>
public interface IPhotoRepository
{
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
    ///     Asynchronously retrieves a specific <see cref="PhotoEntity" /> using its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the photo to retrieve.</param>
    /// <returns>
    ///     A <see cref="Task{TResult}" /> that represents the asynchronous operation.
    ///     The task result contains the <see cref="PhotoEntity" /> object if found; otherwise, <c>null</c>.
    /// </returns>
    /// <remarks>
    ///     Implementations are expected to return <c>null</c> if the ID does not match any photo,
    ///     rather than throwing a "not found" exception.
    /// </remarks>
    /// <exception cref="System.ArgumentNullException">
    ///     May be thrown if <paramref name="id" /> is null or empty, depending on
    ///     the implementation.
    /// </exception>
    Task<PhotoEntity?> GetPhotoByIdAsync(string id); // Kept PhotoEntity? to indicate nullability

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