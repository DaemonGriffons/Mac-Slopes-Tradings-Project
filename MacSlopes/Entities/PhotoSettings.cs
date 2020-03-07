using System.IO;
using System.Linq;

namespace MacSlopes.Entities
{
    public class PhotoSettings
    {
        public int MaxBytes { get; set; }
        public string[] AcceptedFileTypes { get; set; }
        public bool IsSupported(string fileName)
        {
            return AcceptedFileTypes.Any(x => x.Equals(Path.GetExtension(fileName).ToLower()));
        }
    }
}
