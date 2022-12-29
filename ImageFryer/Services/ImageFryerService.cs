using ImageFryer.Interfaces;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace ImageFryer.Services
{
    public class ImageFryerService : IImageFryerService
    {
        public Task<Image> Fry(IFormFile file, int saturation, int fryLevel)
        {
            var image = Image.Load(file.OpenReadStream());

            image.Mutate(x => x.Saturate(saturation).GaussianSharpen(fryLevel));

            return Task.FromResult(image);
        }
    }
}
