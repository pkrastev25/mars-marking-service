using System;
using System.Threading;
using System.Threading.Tasks;
using mars_marking_svc.MarkedResource.Models;
using mars_marking_svc.MarkSession.Interfaces;
using mars_marking_svc.Services.Models;
using mars_marking_svc.Utils;

namespace mars_marking_svc.Services
{
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

        protected override async Task ExecuteAsync(
            CancellationToken cancellationToken
        )
        {
            await CleanMarkSessions(cancellationToken);
        }

        private async Task CleanMarkSessions(
            CancellationToken cancellationToken
        )
        {
            _loggerService.LogStartupInfoEvent("Hosted service started! Attemping to delete legacy mark sessions.");

            try
            {
                var markSessionModels = await _markSessionRepository.GetAll();

                foreach (var markSessionModel in markSessionModels)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        break;
                    }

                    if (markSessionModel.State == MarkSessionModel.StateMarking &&
                        !markSessionModel.IsMarkSessionRecentlyUpdated()
                    )
                    {
                        try
                        {
                            await _markSessionHandler.DeleteMarkSession(markSessionModel.Id.ToString());
                        }
                        catch (Exception e)
                        {
                            _loggerService.LogStartupErrorEvent(e);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _loggerService.LogStartupErrorEvent(e);
            }

            _loggerService.LogStartupInfoEvent("Hosted service stopped!");
        }
    }
}