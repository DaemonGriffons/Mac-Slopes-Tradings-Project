using Microsoft.AspNetCore.Http;
using System.IO;

namespace MacSlopes.Services.Abstract
{
    public interface IFileManager
    {
        string SaveImage(IFormFile image);
        FileStream ImageStream(string image);
        bool RemoveImage(string image);
    }
}
