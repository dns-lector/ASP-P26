namespace ASP_P26.Services.Storage
{
    public interface IStorageService
    {
        byte[] GetItemBytes(String itemName);
        String TryGetMimeType(String itemName);
        String SaveItem(IFormFile formFile);
        Task<String> SaveItemAsync(IFormFile formFile);
    }
}
