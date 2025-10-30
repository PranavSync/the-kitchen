namespace TheKitchen.Services.Interfaces
{
    public interface IImageService
    {
        Task<string?> SaveImageAsync(IFormFile imageFile, string subFolder = "recipes");
        bool DeleteImage(string imagePath);
    }
}