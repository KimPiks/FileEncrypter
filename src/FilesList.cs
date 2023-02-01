using FileEncrypter.Data;

namespace FileEncrypter
{
    /// <summary>
    /// A class for getting a list of files and directories in a given location
    /// </summary>
    public static class FilesList
    {
        /// <summary>
        /// Download list of files and directories
        /// </summary>
        /// <param name="directory">The location from which to get the list of files and directories</param>
        /// <returns>List of files and directories in a given location</returns>
        public static List<FileData> Download(string directory)
        {
            var filesList = new List<FileData>();

            var parentDirectory = GetParentDirectory(directory);
            if (parentDirectory != null)
                filesList.Add(parentDirectory);

            filesList.AddRange(GetDirectories(directory));
            filesList.AddRange(GetFiles(directory));

            return filesList;
        }

        /// <summary>
        /// Gets directories in a given location
        /// </summary>
        /// <param name="directory">The location from which to get the list of directories</param>
        /// <returns>List of directories in a given location</returns>
        private static List<FileData> GetDirectories(string directory)
        {
            var dirs = Directory.GetDirectories(directory).ToList();
            var directoriesList = new List<FileData>();

            foreach (var dir in dirs)
            {
                directoriesList.Add(new FileData()
                {
                    Type = Enums.FileType.Directory,
                    Name = $"/{new DirectoryInfo(dir).Name}",
                    Path = dir
                });
            }

            return directoriesList;
        }

        /// <summary>
        /// Gets parent directory in a given location
        /// </summary>
        /// <param name="directory">The location from which to get the parent directory</param>
        /// <returns>Parent directory in a given location</returns>
        private static FileData? GetParentDirectory(string directory)
        {
            var parentDirectory = Directory.GetParent(directory);
            if (parentDirectory == null)
                return null;

            return new FileData()
            {
                Type = Enums.FileType.Directory,
                Name = "../",
                Path = parentDirectory.FullName
            };
        }

        /// <summary>
        /// Gets files in a given location
        /// </summary>
        /// <param name="directory">The location from which to get the list of files</param>
        /// <returns>List of files in a given location</returns>
        private static List<FileData> GetFiles(string directory)
        {
            var files = Directory.GetFiles(directory).ToList();
            var filesList = new List<FileData>();

            foreach (var file in files)
            {
                filesList.Add(new FileData()
                {
                    Type = Enums.FileType.File,
                    Name = Path.GetFileName(file),
                    Path = file
                });
            }

            return filesList;
        }
    }
}
