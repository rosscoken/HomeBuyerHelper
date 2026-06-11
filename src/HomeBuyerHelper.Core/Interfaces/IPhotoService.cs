using HomeBuyerHelper.Core.Models;

namespace HomeBuyerHelper.Core.Interfaces;

/// <summary>
/// Property photo capture and storage (P3-PHO-001).
/// </summary>
public interface IPhotoService
{
    /// <summary>
    /// Captures a photo with the camera and attaches it to a property.
    /// Returns null if the user cancels.
    /// </summary>
    Task<PropertyPhoto?> CapturePhotoAsync(int propertyId);

    /// <summary>
    /// Picks a photo from the gallery and attaches it to a property.
    /// Returns null if the user cancels.
    /// </summary>
    Task<PropertyPhoto?> PickPhotoAsync(int propertyId);

    /// <summary>
    /// Gets all photos for a property.
    /// </summary>
    Task<IReadOnlyList<PropertyPhoto>> GetPhotosAsync(int propertyId);

    /// <summary>
    /// Deletes a photo record and its file.
    /// </summary>
    Task DeletePhotoAsync(PropertyPhoto photo);
}
