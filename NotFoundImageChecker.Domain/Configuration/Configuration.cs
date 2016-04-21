namespace SexyFishHorse.NotFoundImageChecker.Domain.Configuration
{
    using System.Collections.Generic;
    using System.Configuration;

    public class Configuration
    {
        private string folderPath;

        private IEnumerable<string> knownSha256Hashes;

        public string FolderPath
        {
            get
            {
                return folderPath ?? (folderPath = ConfigurationManager.AppSettings["FolderPath"]);
            }
        }

        public IEnumerable<string> KnownSha256Hashes
        {
            get
            {
                return knownSha256Hashes ?? (knownSha256Hashes = ConfigurationManager.AppSettings["KnownSha256Hashes"].Split(','));
            }
        }
    }
}
