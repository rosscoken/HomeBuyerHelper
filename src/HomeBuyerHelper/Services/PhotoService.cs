using HomeBuyerHelper.Core.Interfaces;
using HomeBuyerHelper.Core.Models;

namespace HomeBuyerHelper.Services;

/// <summary>
/// Property photo capture/storage using MAUI MediaPicker (P3-PHO-001).
/// Photos are copied into the app's local data folder and referenced by
/// property ID; nothing leaves the device.
/// </summary>
public class PhotoService : IPhotoService
{
    private readonly IPhotoRepository _photoRepository;

    public PhotoService(IPhotoRepository photoRepository)
    {
        _photoRepository = photoRepository;
    }

    private static string PhotosDirectory
    {
        get
        {
            var dir = Path.Combine(FileSystem.AppDataDirectory, "Photos");
            Directory.CreateDirectory(dir);
            return dir;
        }
    }

    public async Task<PropertyPhoto?> CapturePhotoAsync(int propertyId)
    {
        if (!MediaPicker.Default.IsCaptureSupported)
            return null;

        var result = await MediaPicker.Default.CapturePhotoAsync();
        return result == null ? null : await StorePhotoAsync(propertyId, result);
    }

    public async Task<PropertyPhoto?> PickPhotoAsync(int propertyId)
    {
        var result = await MediaPicker.Default.PickPhotoAsync();
        return result == null ? null : await StorePhotoAsync(propertyId, result);
    }

    public Task<IReadOnlyList<PropertyPhoto>> GetPhotosAsync(int propertyId)
        => _photoRepository.GetByPropertyIdAsync(propertyId);

    public async Task DeletePhotoAsync(PropertyPhoto photo)
    {
        await _photoRepository.DeleteAsync(photo.Id);

        if (File.Exists(photo.FilePath))
        {
            File.Delete(photo.FilePath);
        }
    }

    private async Task<PropertyPhoto> StorePhotoAsync(int propertyId, FileResult file)
    {
        var fileName = $"property_{propertyId}_{Guid.NewGuid():N}{Path.GetExtension(file.FileName)}";
        var destinationPath = Path.Combine(PhotosDirectory, fileName);

        await using (var source = await file.OpenReadAsync())
        await using (var destination = File.OpenWrite(destinationPath))
        {
            await source.CopyToAsync(destination);
        }

        var existing = await _photoRepository.GetByPropertyIdAsync(propertyId);
        var photo = new PropertyPhoto
        {
            PropertyId = propertyId,
            FilePath = destinationPath,
            SortOrder = existing.Count
        };
        photo.Id = await _photoRepository.CreateAsync(photo);

        return photo;
    }
}
