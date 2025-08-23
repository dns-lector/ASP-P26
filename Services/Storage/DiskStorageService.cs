
using Microsoft.AspNetCore.Http;
using System.IO;

namespace ASP_P26.Services.Storage
{
    public class DiskStorageService : IStorageService
    {
        private const String basePath = "C:/storage/ASP26/";

        public byte[] GetItemBytes(string itemName)
        {
            String path = Path.Combine(basePath, itemName);
            if (File.Exists(path))
            {
                return File.ReadAllBytes(path);
            }
            else throw new FileNotFoundException();
        }

        public string TryGetMimeType(string itemName)
        {
            String ext = GetFileExtension(itemName);
            return ext switch
            {
                ".jpg" => "image/jpeg",
                ".png" => "image/png",
                ".bmp" => "image/bmp",
                _ => throw new ArgumentException($"Unsupported extension '{ext}'")
            };
        }

        public string SaveItem(IFormFile formFile)
        {
            // Виділяємо з імені файлу розширення (воно буде збережене у імені)
            String ext = GetFileExtension(formFile.FileName);

            // Генеруємо нове ім'я для збереження файлу
            String savedName = Guid.NewGuid() + ext;
            String path = Path.Combine(basePath, savedName);

            // Відкриваємо потік для збереження (не забуваємо автозакриття)
            using Stream stream = new StreamWriter(path).BaseStream;
            formFile.CopyTo(stream);

            return savedName;
        }

        public async Task<String> SaveItemAsync(IFormFile formFile)
        {
            String ext = GetFileExtension(formFile.FileName);
            String savedName = Guid.NewGuid() + ext;
            String path = Path.Combine(basePath, savedName);
            using Stream stream = new StreamWriter(path).BaseStream;
            await formFile.CopyToAsync(stream);
            return savedName;
        }

        private String GetFileExtension(String filename)
        {
            int dotIndex = filename.LastIndexOf('.');
            if (dotIndex < 0)
            {
                throw new ArgumentException("File name MUST have an extension");
            }
            return filename[dotIndex..];

        }
    }
}
