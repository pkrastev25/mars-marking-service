using System;
using System.Threading;
using System.Threading.Tasks;
using mars_marking_svc.MarkSession.Interfaces;
using mars_marking_svc.Services.Models;
using mars_marking_svc.Utils;
using static mars_marking_svc.Constants.Constants;

namespace mars_marking_svc.Services
{
    /// <summary>
    /// Based on https://blog.maartenballiauw.be/post/2017/08/01/building-a-scheduled-cache-updater-in-aspnet-core-2.html
    /// </summary>
    public class HostedMarkSessionCronService : AHostedService
    {
        private readonly IDbMarkSessionClient _dbMarkSessionClient;
        private readonly IDbMarkSessionHandler _dbMarkSessionHandler;
        private readonly ILoggerService _loggerService;

        public HostedMarkSessionCronService(
            IDbMarkSessionClient dbMarkSessionClient,
            IDbMarkSessionHandler dbMarkSessionHandler,
            ILoggerService loggerService
        )
        {
            _dbMarkSessionClient = dbMarkSessionClient;
            _dbMarkSessionHandler = dbMarkSessionHandler;
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

            await taskFactory.StartNew(
                async () =>
                {
                    try
                    {
                        var markSessions = await _dbMarkSessionClient.GetAll();

                        foreach (var dbMarkSessionModel in markSessions)
                        {
                            if (dbMarkSessionModel.IsMarkSessionExpired())
                            {
                                await _dbMarkSessionHandler.UnmarkResourcesForMarkSession(dbMarkSessionModel);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        _loggerService.LogErrorEvent(e);
                    }
                }, cancellationToken);
        }
    }
}