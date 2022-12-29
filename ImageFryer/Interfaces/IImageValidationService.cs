namespace ImageFryer.Interfaces
{
    public interface IImageValidationService
    {
        bool IsValidFileExtension(IFormFile file);
        bool IsValidFileSignature(IFormFile file);
        bool IsValidFileSize(IFormFile file);
    }
}
