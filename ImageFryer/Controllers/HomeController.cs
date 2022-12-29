using ImageFryer.Interfaces;
using ImageFryer.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using SixLabors.ImageSharp;

namespace ImageFryer.Controllers
{
    public class HomeController : Controller
    {
        private const int MAX_SATURATION = 30;
        private const int MIN_SATURATION = 2;
        private const int MAX_FRY = 30;
        private const int MIN_FRY = 2;
        private readonly ILogger<HomeController> _logger;
        private readonly IImageValidationService _imageValidationService;
        private readonly IImageFryerService _imageFryerService;

        public HomeController(ILogger<HomeController> logger, IImageValidationService imageValidationService, IImageFryerService imageFryerService)
        {
            _logger = logger;
            _imageValidationService = imageValidationService;
            _imageFryerService = imageFryerService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(IFormFile file, int saturation, int fryLevel)
        {
            if (saturation > MAX_SATURATION || saturation < MIN_SATURATION)
            {
                ViewBag.Error = "Saturation must be between 2 and 30";
                return View();
            }
            else if (fryLevel > MAX_FRY || fryLevel < MIN_FRY)
            {
                ViewBag.Error = "Fry level must be between 2 and 30";
                return View();
            }
            else if (file == null || file.Length == 0)
            {
                ViewBag.Error = "Please select a file";
                return View();
            }
            else if (!_imageValidationService.IsValidFileSize(file))
            {
                ViewBag.Error = "File size must be less than 2MB";
                return View();
            }
            else if (!_imageValidationService.IsValidFileExtension(file))
            {
                ViewBag.Error = "Invalid file extension";
                return View();
            }
            else if (!_imageValidationService.IsValidFileSignature(file))
            {
                ViewBag.Error = "Invalid file signature";
                return View();
            }

            Image friedImage = await _imageFryerService.Fry(file, saturation, fryLevel);

            using (var output = new MemoryStream())
            {
                await friedImage.SaveAsJpegAsync(output);
                return File(output.ToArray(), "image/jpeg");
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}