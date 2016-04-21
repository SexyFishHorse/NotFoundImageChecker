namespace SexyFishHorse.NotFoundImageChecker.Domain.Services
{
    using System;
    using System.IO;
    using System.Linq;
    using SexyFishHorse.NotFoundImageChecker.Domain.Configuration;

    public class CheckerService
    {
        private readonly Configuration configuration;

        private readonly FileSystemService fileSystemService;

        private readonly FileSystemWatcher watcher;

        public CheckerService(Configuration configuration, FileSystemService fileSystemService)
        {
            this.configuration = configuration;
            this.fileSystemService = fileSystemService;

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

            var filePaths = fileSystemService.GetFilePaths();

            var filesToDelete = filePaths
                .Select(x => new { Path = x, Checksum = fileSystemService.GenerateChecksum(x) })
                .Where(x => configuration.KnownSha256Hashes.Contains(x.Checksum))
                .Select(x => x.Path);

            fileSystemService.DeleteFiles(filesToDelete);
        }

        public void StartFolderWatcher()
        {
            watcher.EnableRaisingEvents = true;
            Console.WriteLine("Folder watcher started");
        }

        private void WatcherOnChanged(object sender, FileSystemEventArgs fileSystemEventArgs)
        {
            Console.WriteLine("File changed: {0}", fileSystemEventArgs.Name);
            if (!File.Exists(fileSystemEventArgs.FullPath))
            {
                return;
            }

            var checksum = fileSystemService.GenerateChecksum(fileSystemEventArgs.FullPath);

            if (configuration.KnownSha256Hashes.Contains(checksum))
            {
                fileSystemService.DeleteFile(fileSystemEventArgs.FullPath);
            }
        }
    }
}
