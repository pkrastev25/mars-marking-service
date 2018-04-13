using System.Threading;
using System.Threading.Tasks;
using mars_marking_svc.MarkSession.Interfaces;
using mars_marking_svc.Services.Models;
using static mars_marking_svc.Constants.Constants;

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
            while (!cancellationToken.IsCancellationRequested)
            {
                await CleanMarkSessions(cancellationToken);
                await Task.Delay(MarkSessionCronServiceTimeInterval, cancellationToken);
            }
        }

        private async Task CleanMarkSessions(CancellationToken cancellationToken)
        {
            var taskFactory = new TaskFactory(cancellationToken);

            // TODO: Start a long running unmarking job when the application is started and the markSession is in the Marking state!!!
            /*
            await taskFactory.StartNew(
                async () =>
                {
                    try
                    {
                        var markSessions = await _markSessionRepository.GetAll();

                        foreach (var dbMarkSessionModel in markSessions)
                        {
                            if (dbMarkSessionModel.IsMarkSessionExpired())
                            {
                                await _markSessionHandler.UnmarkResourcesForMarkSession(dbMarkSessionModel);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        _loggerService.LogErrorEvent(e);
                    }
                }, cancellationToken);
                */
        }
    }
}