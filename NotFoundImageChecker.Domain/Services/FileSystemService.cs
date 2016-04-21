namespace SexyFishHorse.NotFoundImageChecker.Domain.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Security.Cryptography;
    using SexyFishHorse.NotFoundImageChecker.Domain.Configuration;

    public class FileSystemService
    {
        private readonly Configuration configuration;

        public FileSystemService(Configuration configuration)
        {
            this.configuration = configuration;
        }

        public IEnumerable<string> GetFilePaths()
        {
            return Directory.EnumerateFiles(configuration.FolderPath);
        }

        public string GenerateChecksum(string filePath)
        {
            try
            {
                using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    var sha = new SHA256Managed();
                    var checksumBytes = sha.ComputeHash(stream);

                    return BitConverter.ToString(checksumBytes).Replace("-", string.Empty);
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine("Error generating checksum: {0}", ex.Message);

                return string.Empty;
            }
        }

        public void DeleteFiles(IEnumerable<string> filesToDelete)
        {
            foreach (var fileToDelete in filesToDelete)
            {
                DeleteFile(fileToDelete);
            }
        }

        public void DeleteFile(string fileToDelete)
        {
            File.Delete(fileToDelete);
            Console.WriteLine("Deleted {0}", fileToDelete);
        }
    }
}
