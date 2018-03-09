using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using mars_marking_svc.Models;
using mars_marking_svc.Services;
using Newtonsoft.Json;

namespace mars_marking_svc.Clients.Metadata
{
    public class MetadataServiceClient : IMetadataServiceClient
    {
        private readonly IHttpService _httpService;

        public MetadataServiceClient(IHttpService httpService)
        {
            _httpService = httpService;
        }

        public async Task<MetadataModel> GetMetadata(string metadataId)
        {
            var response = await _httpService.GetAsync($"http://localhost:8080/metadata/{metadataId}");

            if (response.StatusCode != HttpStatusCode.OK)
            {
                // TODO: Revert the process
                throw new Exception();
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<MetadataModel>(jsonResponse);
        }

        public async Task<MarkedResourceModel> MarkMetadata(string metadataId)
        {
            var metadata = GetMetadata(metadataId).Result;

            return await MarkMetadata(metadata);
        }

        public async Task<MarkedResourceModel> MarkMetadata(MetadataModel metadataModel)
        {
            if (MetadataModel.ToBeDeletedState.Equals(metadataModel.state))
            {
                // TODO: revert the process
            }

            if (!MetadataModel.FinishedState.Equals(metadataModel.state))
            {
                // TODO: revert the process
            }

            metadataModel.state = MetadataModel.ToBeDeletedState;

            var response = await _httpService.PutAsync(
                $"http://localhost:8080/metadata/{metadataModel.dataId}?state={MetadataModel.ToBeDeletedState}",
                metadataModel
            );

            if (response.StatusCode == HttpStatusCode.OK)
            {
                return new MarkedResourceModel
                {
                    resourceType = "metadata",
                    resourceId = metadataModel.dataId
                };
            }

            // TODO: Revert the process
            throw new Exception();
        }

        public async Task<List<MetadataModel>> GetMetadataForProject(string projectId)
        {
            var response = await _httpService.GetAsync($"http://localhost:8080/metadata?project={projectId}");

            if (response.StatusCode != HttpStatusCode.OK)
            {
                // TODO: Revert the process
                throw new Exception();
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<List<MetadataModel>>(jsonResponse);
        }
    }
}