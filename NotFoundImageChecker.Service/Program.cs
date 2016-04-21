namespace SexyFishHorse.NotFoundImageChecker
{
    using System;
    using System.ServiceProcess;
    using SexyFishHorse.NotFoundImageChecker.Domain.Configuration;
    using SexyFishHorse.NotFoundImageChecker.Domain.Services;

    public static class Program
    {
        public static void Main()
        {
            var service = new CheckerService(new Configuration(), new FileSystemService(new Configuration()));

#if DEBUG
            service.InitialFolderCheck();
            service.StartFolderWatcher();
#else
            var servicesToRun = new ServiceBase[]
            {
                new NotFoundImageCheckerService(service)
            };

            ServiceBase.Run(servicesToRun);
#endif

            Console.ReadLine();
        }
    }
}
