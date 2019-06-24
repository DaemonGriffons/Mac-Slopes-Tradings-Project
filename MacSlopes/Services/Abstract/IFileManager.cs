using System.IO;
using Microsoft.AspNetCore.Http;

namespace MacSlopes.Services.Abstract
{
    public interface IFileManager
    {
        string SaveImage(IFormFile image);
        FileStream ImageStream(string image);
        bool RemoveImage(string image);
    }
}
