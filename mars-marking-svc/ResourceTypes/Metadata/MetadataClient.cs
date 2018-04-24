﻿using System.Collections.Generic;
using System.Threading.Tasks;
using mars_marking_svc.Exceptions;
using mars_marking_svc.MarkedResource.Models;
using mars_marking_svc.ResourceTypes.Metadata.Interfaces;
using mars_marking_svc.ResourceTypes.Metadata.Models;
using mars_marking_svc.Services.Models;
using mars_marking_svc.Utils;

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

        public async Task<MetadataModel> GetMetadata(
            string metadataId
        )
        {
            var response = await _httpService.GetAsync(
                $"http://metadata-svc/metadata/{metadataId}"
            );

            response.ThrowExceptionIfNotSuccessfulResponse(
                new FailedToGetResourceException(
                    $"Failed to get metadata with id: {metadataId} from metadata-svc! The response status code is {response.StatusCode}"
                )
            );

            return await response.Deserialize<MetadataModel>();
        }

        public async Task<List<MetadataModel>> GetMetadataForProject(
            string projectId
        )
        {
            var response = await _httpService.GetAsync(
                $"http://metadata-svc/metadata?project={projectId}"
            );

            response.ThrowExceptionIfNotSuccessfulResponse(
                new FailedToGetResourceException(
                    $"Failed to get metadata for projectId: {projectId} from metadata-svc! The response status code is {response.StatusCode}"
                )
            );

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

            var metadataPreviousState = metadataModel.State;
            metadataModel.State = MetadataModel.ToBeDeletedState;

            var response = await _httpService.PutAsync(
                $"http://metadata-svc/metadata/{metadataModel.DataId}/state?state={MetadataModel.ToBeDeletedState}",
                metadataModel
            );

            response.ThrowExceptionIfNotSuccessfulResponse(
                new FailedToUpdateResourceException(
                    $"Failed to update metadata with id: {metadataModel.DataId} from metadata-svc! The response status code is {response.StatusCode}"
                )
            );

            var markedResource = new DependantResourceModel("metadata", metadataModel.DataId)
            {
                PreviousState = metadataPreviousState
            };
            _loggerService.LogMarkEvent(markedResource.ToString());

            return markedResource;
        }

        public async Task UnmarkMetadata(
            DependantResourceModel dependantResourceModel
        )
        {
            var metadataModel = new MetadataModel
            {
                DataId = dependantResourceModel.ResourceId,
                State = dependantResourceModel.PreviousState
            };

            var response = await _httpService.PutAsync(
                $"http://metadata-svc/metadata/{metadataModel.DataId}/state?state={MetadataModel.FinishedState}",
                metadataModel
            );

            response.ThrowExceptionIfNotSuccessfulResponseOrNot404Response(
                new FailedToUpdateResourceException(
                    $"Failed to update {dependantResourceModel} from metadata-svc! The response status code is {response.StatusCode}"
                )
            );

            _loggerService.LogUnmarkEvent(dependantResourceModel.ToString());
        }
    }
}