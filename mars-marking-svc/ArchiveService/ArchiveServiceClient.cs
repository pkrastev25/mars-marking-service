using System;
using System.Net;
using System.Threading.Tasks;
using mars_marking_svc.ArchiveService.Interfaces;
using mars_marking_svc.ArchiveService.Models;
using mars_marking_svc.Exceptions;
using mars_marking_svc.Services.Models;
using mars_marking_svc.Utils;

namespace mars_marking_svc.ArchiveService
{
    public class ArchiveServiceClient : IArchiveServiceClient
    {
        private readonly string _baseUrl;
        private readonly IHttpService _httpService;

        public ArchiveServiceClient(
            IHttpService httpService
        )
        {
            var baseUrl = Environment.GetEnvironmentVariable(Constants.Constants.ArchiveSvcUrlKey);
            _baseUrl = string.IsNullOrEmpty(baseUrl) ? "archive-svc" : baseUrl;
            _httpService = httpService;
        }

        public async Task EnsureArchiveRestoreIsNotRunning(
            string projectId
        )
        {
            var archiveRestoreModel = await GetArchiveRestore(projectId);

            if (archiveRestoreModel != null &&
                archiveRestoreModel.MarkSessionId == ArchiveRestoreModel.ArchiveRestoreMarkSessionType &&
                archiveRestoreModel.Status == ArchiveRestoreModel.ArchiveRestoreProcessingState)
            {
                throw new ArchiveRestoreForProjectIsRunningException(
                    $"Archive restore is running for project with id: {projectId}. A mark session cannot be created!"
                );
            }
        }

        private async Task<ArchiveRestoreModel> GetArchiveRestore(
            string projectId
        )
        {
            var response = await _httpService.GetAsync(
                $"http://{_baseUrl}/jobs/status/{projectId}"
            );

            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    return await response.Deserialize<ArchiveRestoreModel>();
                case HttpStatusCode.NotFound:
                    return null;
                default:
                    throw new FailedToGetResourceException(
                        await response.FormatRequestAndResponse(
                            $"Failed to get archive restore information for projectId: {projectId} from archive-svc!"
                        )
                    );
            }
        }
    }
}