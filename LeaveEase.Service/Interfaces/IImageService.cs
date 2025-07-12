using Microsoft.AspNetCore.Http;

namespace LeaveEase.Service.Interfaces
{
    public interface IImageService
    {
        string GetUniqueFileName(string fileName);
        string? Upload(IFormFile? Image, string folder_name);
    }
}
