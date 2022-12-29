using ImageFryer.Interfaces;

namespace ImageFryer.Services
{
    public class ImageValidationService : IImageValidationService
    {
        private readonly IConfiguration _configuration;
        private readonly Dictionary<string, List<byte[]>> _fileSignature = new Dictionary<string, List<byte[]>>
        {
            { ".png", new List<byte[]> { new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A } } },
            { ".jpeg", new List<byte[]>
                {
                    new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 },
                    new byte[] { 0xFF, 0xD8, 0xFF, 0xE2 },
                    new byte[] { 0xFF, 0xD8, 0xFF, 0xE3 },
                }
            },
            { ".jpg", new List<byte[]>
                {
                    new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 },
                    new byte[] { 0xFF, 0xD8, 0xFF, 0xE1 },
                    new byte[] { 0xFF, 0xD8, 0xFF, 0xE8 },
                }
            },
        };

        public ImageValidationService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        
        public bool IsValidFileExtension(IFormFile file)
        {
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            var permittedExtensions = _configuration.GetSection("FileUpload:PermitedExtensions").Get<string[]>();

            if (string.IsNullOrEmpty(ext) || !(permittedExtensions ??= new string[1]).Contains(ext))
            {
                return false;
            }
            return true;
        }

        public bool IsValidFileSignature(IFormFile file)
        {
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            var signatures = _fileSignature[ext];

            if (signatures == null)
            {
                return false;
            }

            Stream stream = file.OpenReadStream();

            stream.Position = 0;
            var reader = new BinaryReader(stream);
            var headerBytes = reader.ReadBytes(signatures.Max(m => m.Length));

            return signatures.Any(signature => headerBytes.Take(signature.Length).SequenceEqual(signature));
        }

        public bool IsValidFileSize(IFormFile file)
        {
            var fileSizeLimit = _configuration["FileUpload:SizeLimit"];
            
            if (fileSizeLimit == null)
            {
                return false;
            }

            return file.Length < int.Parse(fileSizeLimit);
        }
    }
}
