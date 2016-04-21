namespace SexyFishHorse.NotFoundImageChecker.Domain.Services
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography;
    using SexyFishHorse.NotFoundImageChecker.Domain.Configuration;

    public class CheckerService
    {
        private readonly Configuration configuration;

        private readonly FileSystemWatcher watcher;

        public CheckerService(Configuration configuration)
        {
            this.configuration = configuration;

            watcher = new FileSystemWatcher
            {
                Path = configuration.FolderPath,
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.LastAccess | NotifyFilters.FileName,
                Filter = "*.*"
            };

            watcher.Changed += WatcherOnChanged;
        }

        public void InitialFolderCheck()
        {
            Console.WriteLine("Initial folder check");
            var folderPath = configuration.FolderPath;

            if (!Directory.Exists(folderPath))
            {
                throw new DirectoryNotFoundException(string.Format("The directory {0} was not found", folderPath));
            }

            var filePaths = Directory.EnumerateFiles(folderPath);

            var filesToDelete = filePaths
                .Select(x => new { Path = x, Checksum = GenerateChecksum(x) })
                .Where(x => configuration.KnownSha256Hashes.Contains(x.Checksum))
                .Select(x => x.Path);

            foreach (var fileToDelete in filesToDelete)
            {
                File.Delete(fileToDelete);
                Console.WriteLine("Deleted " + fileToDelete);
            }
        }

        public void StartFolderWatcher()
        {
            watcher.EnableRaisingEvents = true;
            Console.WriteLine("Folder watcher started");
        }

        private string GenerateChecksum(string filePath)
        {
            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                var sha = new SHA256Managed();
                var checksumBytes = sha.ComputeHash(stream);

                return BitConverter.ToString(checksumBytes).Replace("-", string.Empty);
            }
        }

        private void WatcherOnChanged(object sender, FileSystemEventArgs fileSystemEventArgs)
        {
            Console.WriteLine("File changed: " + fileSystemEventArgs.FullPath);
            if (!File.Exists(fileSystemEventArgs.FullPath))
            {
                return;
            }

            try
            {
                var checksum = GenerateChecksum(fileSystemEventArgs.FullPath);

                if (checksum != null && configuration.KnownSha256Hashes.Contains(checksum))
                {
                    File.Delete(fileSystemEventArgs.FullPath);
                    Console.WriteLine("Deleted " + fileSystemEventArgs.FullPath);
                }
            }
            catch (IOException)
            {
            }
        }
    }
}
