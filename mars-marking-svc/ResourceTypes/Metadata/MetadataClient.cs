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

        public async Task<DependantResourceModel> MarkMetadata(string metadataId)
        {
            var metadata = await GetMetadata(metadataId);

            return await MarkMetadata(metadata);
        }

        public async Task<DependantResourceModel> MarkMetadata(MetadataModel metadataModel)
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

            var markedResource = new DependantResourceModel("metadata", metadataModel.DataId)
            {
                PreviousState = metadataPreviousState
            };
            _loggerService.LogMarkEvent(markedResource.ToString());

            return markedResource;
        }

        public async Task UnmarkMetadata(DependantResourceModel dependantResourceModel)
        {
            if (!await DoesMetadataExist(dependantResourceModel.ResourceId))
            {
                _loggerService.LogSkipEvent(dependantResourceModel.ToString());
                return;
            }

            var metadataModel = new MetadataModel
            {
                DataId = dependantResourceModel.ResourceId,
                State = dependantResourceModel.PreviousState
            };

            var response = await _httpService.PutAsync(
                $"http://metadata-svc/metadata/{metadataModel.DataId}?state={MetadataModel.FinishedState}",
                metadataModel
            );

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new FailedToUpdateResourceException(
                    $"Failed to update {dependantResourceModel} from metadata-svc!"
                );
            }

            _loggerService.LogUnmarkEvent(dependantResourceModel.ToString());
        }

        private async Task<bool> DoesMetadataExist(string metadataId)
        {
            var response = await _httpService.GetAsync($"http://metadata-svc/metadata/{metadataId}");

            if (response.StatusCode == HttpStatusCode.OK)
            {
                return true;
            }

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return false;
            }

            throw new FailedToGetResourceException(
                $"Failed to get metadata with id: {metadataId} from metadata-svc!"
            );
        }
    }
}