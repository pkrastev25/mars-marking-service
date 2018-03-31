using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using mars_marking_svc.Exceptions;
using mars_marking_svc.MarkedResource.Models;
using mars_marking_svc.ResourceTypes.Metadata.Interfaces;
using mars_marking_svc.ResourceTypes.Metadata.Models;
using mars_marking_svc.Services.Models;
using Newtonsoft.Json;

namespace mars_marking_svc.ResourceTypes.Metadata
{
    public class MetadataClient : IMetadataClient
    {
        private readonly IHttpService _httpService;
        private readonly ILoggerService _loggerService;

        public MetadataClient(
            IHttpService httpService,
            ILoggerService loggerService
        )
        {
            _httpService = httpService;
            _loggerService = loggerService;
        }

        public async Task<MetadataModel> GetMetadata(string metadataId)
        {
            var response = await _httpService.GetAsync($"http://metadata-svc/metadata/{metadataId}");

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new FailedToGetResourceException(
                    $"Failed to get metadata with id: {metadataId} from metadata-svc!"
                );
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<MetadataModel>(jsonResponse);
        }

        public async Task<List<MetadataModel>> GetMetadataForProject(string projectId)
        {
            var response = await _httpService.GetAsync($"http://metadata-svc/metadata?project={projectId}");

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new FailedToGetResourceException(
                    $"Failed to get metadata for projectId: {projectId} from metadata-svc!"
                );
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<List<MetadataModel>>(jsonResponse);
        }

        public async Task<MarkedResourceModel> MarkMetadata(string metadataId)
        {
            var metadata = await GetMetadata(metadataId);

            return await MarkMetadata(metadata);
        }

        public async Task<MarkedResourceModel> MarkMetadata(MetadataModel metadataModel)
        {
            if (MetadataModel.ToBeDeletedState.Equals(metadataModel.State))
            {
                throw new ResourceAlreadyMarkedException(
                    $"Cannot mark metadata with id: {metadataModel.DataId}, it is already marked!"
                );
            }

            if (!MetadataModel.FinishedState.Equals(metadataModel.State) &&
                !MetadataModel.FailedState.Equals(metadataModel.State)
            )
            {
                throw new CannotMarkResourceException(
                    $"Cannot mark metadata with id: {metadataModel.DataId}, it must be in state: {MetadataModel.FinishedState} or state: {MetadataModel.FailedState} beforehand!"
                );
            }

            var metadataPreviousState = metadataModel.State;
            metadataModel.State = MetadataModel.ToBeDeletedState;

            var response = await _httpService.PutAsync(
                $"http://metadata-svc/metadata/{metadataModel.DataId}?state={MetadataModel.ToBeDeletedState}",
                metadataModel
            );

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new FailedToUpdateResourceException(
                    $"Failed to update metadata with id: {metadataModel.DataId} from metadata-svc!"
                );
            }

            var markedResource = new MarkedResourceModel("metadata", metadataModel.DataId);
            markedResource.PreviousState = metadataPreviousState;
            _loggerService.LogMarkEvent(markedResource.ToString());

            return markedResource;
        }

        public async Task UnmarkMetadata(MarkedResourceModel markedResourceModel)
        {
            if (!await DoesMetadataExist(markedResourceModel.ResourceId))
            {
                _loggerService.LogSkipEvent(markedResourceModel.ToString());
                return;
            }

            var metadataModel = new MetadataModel
            {
                DataId = markedResourceModel.ResourceId,
                State = markedResourceModel.PreviousState
            };

            var response = await _httpService.PutAsync(
                $"http://metadata-svc/metadata/{metadataModel.DataId}?state={MetadataModel.FinishedState}",
                metadataModel
            );

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new FailedToUpdateResourceException(
                    $"Failed to update {markedResourceModel} from metadata-svc!"
                );
            }

            _loggerService.LogUnmarkEvent(markedResourceModel.ToString());
        }

        private async Task<bool> DoesMetadataExist(string metadataId)
        {
            var response = await _httpService.GetAsync($"http://metadata-svc/metadata/{metadataId}");

            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    return true;
                case HttpStatusCode.NotFound:
                    return false;
                default:
                    throw new FailedToGetResourceException(
                        $"Failed to get metadata with id: {metadataId} from metadata-svc!"
                    );
            }
        }
    }
}