using System;
using System.Threading;
using System.Threading.Tasks;
using mars_marking_svc.MarkSession.Interfaces;
using mars_marking_svc.Services.Models;

namespace mars_marking_svc.Services
{
    /// <summary>
    /// Based on https://blog.maartenballiauw.be/post/2017/08/01/building-a-scheduled-cache-updater-in-aspnet-core-2.html
    /// </summary>
    public class HostedStartupService : AHostedService
    {
        private readonly IMarkSessionRepository _markSessionRepository;
        private readonly IMarkSessionHandler _markSessionHandler;
        private readonly ILoggerService _loggerService;

        public HostedStartupService(
            IMarkSessionRepository markSessionRepository,
            IMarkSessionHandler markSessionHandler,
            ILoggerService loggerService
        )
        {
            _markSessionRepository = markSessionRepository;
            _markSessionHandler = markSessionHandler;
            _loggerService = loggerService;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            await CleanMarkSessions();
        }

        private async Task CleanMarkSessions()
        {
            try
            {
                var markSessionModels = await _markSessionRepository.GetAll();

                foreach (var markSessionModel in markSessionModels)
                {
                    if (markSessionModel.State == "Marking")
                    {
                        await _markSessionHandler.DeleteMarkSession(markSessionModel.Id.ToString());
                    }
                }
            }
            catch (Exception e)
            {
                _loggerService.LogErrorEvent(e);
            }
        }
    }
}