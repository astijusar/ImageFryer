using SixLabors.ImageSharp;

namespace ImageFryer.Interfaces
{
    public interface IImageFryerService
    {
        Task<Image> Fry(IFormFile file, int saturation, int fryLevel);
    }
}
