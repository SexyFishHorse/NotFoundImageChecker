namespace SexyFishHorse.NotFoundImageChecker
{
    using System.ServiceProcess;
    using SexyFishHorse.NotFoundImageChecker.Domain.Services;

    public class NotFoundImageCheckerService : ServiceBase
    {
        private readonly CheckerService service;

        public NotFoundImageCheckerService(CheckerService service)
        {
            this.service = service;
        }

        protected override void OnStart(string[] args)
        {
            base.OnStart(args);

            service.InitialFolderCheck();
        }
    }
}
