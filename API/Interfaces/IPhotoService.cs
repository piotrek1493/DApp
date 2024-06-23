using CloudinaryDotNet.Actions;

namespace API.Interfaces
{
    public interface IPhotoService
    {
        Task<ImageUploadResult> AddPhotoAsync(IFormFile file, CancellationToken cancellationToken);
        Task<DeletionResult> DeletePhotoAsync(string publicId, CancellationToken cancellationToken);
    }
}
