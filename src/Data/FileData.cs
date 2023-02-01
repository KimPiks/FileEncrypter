using FileEncrypter.Enums;

namespace FileEncrypter.Data
{
    public class FileData
    {
        public FileType Type { get; set; }
        public string Name { get; set; } = null!;
        public string Path { get; set; } = null!;
    }
}
