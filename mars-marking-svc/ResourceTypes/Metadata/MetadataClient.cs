using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using mars_marking_svc.Exceptions;
using mars_marking_svc.MarkedResource.Models;
using mars_marking_svc.ResourceTypes.Metadata.Interfaces;
using mars_marking_svc.ResourceTypes.Metadata.Models;
using mars_marking_svc.Services;
using mars_marking_svc.Services.Models;
using mars_marking_svc.Utils;

namespace mars_marking_svc.ResourceTypes.Metadata
{
    public class MetadataClient : IMetadataClient
    {
        private readonly string _baseUrl;
        private readonly IHttpService _httpService;

        public MetadataClient(
            IHttpService httpService
        )
        {
            var baseUrl = Environment.GetEnvironmentVariable(Constants.Constants.MetadataSvcUrlKey);
            _baseUrl = string.IsNullOrEmpty(baseUrl) ? "metadata-svc" : baseUrl;
            _httpService = httpService;
        }

        public async Task<MetadataModel> GetMetadata(
            string metadataId
        )
        {
            var response = await _httpService.GetAsync(
                $"http://{_baseUrl}/metadata/{metadataId}"
            );

            response.ThrowExceptionIfNotSuccessfulResponse(
                new FailedToGetResourceException(
                    await response.FormatRequestAndResponse(
                        $"Failed to get metadata with id: {metadataId} from metadata-svc!"
                    )
                )
            );

            return await response.Deserialize<MetadataModel>();
        }

        public async Task<List<MetadataModel>> GetMetadataForProject(
            string projectId
        )
        {
            var response = await _httpService.GetAsync(
                $"http://{_baseUrl}/metadata?projectId={projectId}"
            );

            response.ThrowExceptionIfNotSuccessfulResponseOrNot404Response(
                new FailedToGetResourceException(
                    await response.FormatRequestAndResponse(
                        $"Failed to get metadata for projectId: {projectId} from metadata-svc!"
                    )
                )
            );

            if (response.IsEmptyResponse())
            {
                return new List<MetadataModel>();
            }

            return await response.Deserialize<List<MetadataModel>>();
        }

        public async Task<DependantResourceModel> MarkMetadata(
            string metadataId
        )
        {
            var metadata = await GetMetadata(metadataId);

            return await MarkMetadata(metadata);
        }

        public async Task<DependantResourceModel> MarkMetadata(
            MetadataModel metadataModel
        )
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

            var response = await _httpService.PutAsync(
                $"http://{_baseUrl}/metadata/{metadataModel.DataId}/state?state={MetadataModel.ToBeDeletedState}",
                ""
            );

            /**
             * Sometimes the HttpClient gets corrupted and it sends a malformed request to the metadata-svc only.
             * Because of the time limitation, a workaround has been made which creates a new client for this request only.
             */
            if (response.StatusCode == HttpStatusCode.MethodNotAllowed)
            {
                response = await new HttpService(new HttpClient()).PutAsync(
                    $"http://{_baseUrl}/metadata/{metadataModel.DataId}/state?state={MetadataModel.ToBeDeletedState}",
                    ""
                );
            }

            response.ThrowExceptionIfNotSuccessfulResponse(
                new FailedToUpdateResourceException(
                    await response.FormatRequestAndResponse(
                        $"Failed to update metadata with id: {metadataModel.DataId} from metadata-svc!"
                    )
                )
            );

            var markedResource = new DependantResourceModel(ResourceTypeEnum.Metadata, metadataModel.DataId)
            {
                PreviousState = metadataModel.State
            };

            return markedResource;
        }

        public async Task UnmarkMetadata(
            DependantResourceModel dependantResourceModel
        )
        {
            var response = await _httpService.PutAsync(
                $"http://{_baseUrl}/metadata/{dependantResourceModel.ResourceId}/state?state={dependantResourceModel.PreviousState}",
                ""
            );
            
            /**
             * Sometimes the HttpClient gets corrupted and it sends a malformed request to the metadata-svc only.
             * Because of the time limitation, a workaround has been made which creates a new client for this request only.
             */
            if (response.StatusCode == HttpStatusCode.MethodNotAllowed)
            {
                response = await new HttpService(new HttpClient()).PutAsync(
                    $"http://{_baseUrl}/metadata/{dependantResourceModel.ResourceId}/state?state={dependantResourceModel.PreviousState}",
                    ""
                );
            }

            response.ThrowExceptionIfNotSuccessfulResponseOrNot404Response(
                new FailedToUpdateResourceException(
                    await response.FormatRequestAndResponse(
                        $"Failed to update {dependantResourceModel} from metadata-svc!"
                    )
                )
            );
        }
    }
}