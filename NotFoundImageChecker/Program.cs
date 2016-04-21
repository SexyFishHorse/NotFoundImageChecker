namespace SexyFishHorse.NotFoundImageChecker
{
    using System.ServiceProcess;

    public static class Program
    {
        public static void Main()
        {
            var servicesToRun = new ServiceBase[] 
            { 
                new NotFoundImageCheckerService() 
            };
            ServiceBase.Run(servicesToRun);
        }
    }
}
