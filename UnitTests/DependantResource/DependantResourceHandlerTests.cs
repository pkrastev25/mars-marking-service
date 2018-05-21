using System.Collections.Generic;
using mars_marking_svc.DependantResource;
using mars_marking_svc.MarkedResource.Models;
using mars_marking_svc.ResourceTypes.Metadata.Interfaces;
using mars_marking_svc.ResourceTypes.Metadata.Models;
using mars_marking_svc.ResourceTypes.Project.Interfaces;
using mars_marking_svc.ResourceTypes.ResultConfig.Interfaces;
using mars_marking_svc.ResourceTypes.ResultConfig.Models;
using mars_marking_svc.ResourceTypes.ResultData.Interfaces;
using mars_marking_svc.ResourceTypes.Scenario.Interfaces;
using mars_marking_svc.ResourceTypes.SimPlan.Interfaces;
using mars_marking_svc.ResourceTypes.SimRun.Interfaces;
using mars_marking_svc.Services.Models;
using Moq;
using UnitTests._DataMocks;
using Xunit;

namespace UnitTests.DependantResource
{
    public class DependantResourceHandlerTests
    {
        [Fact]
        public async void MarkResourcesForMarkSession_ProjectMarkSessionModel_ReturnsMarkSessionWithDependantResources()
        {
            // Arrange
            var emptyMarkSessionModel = MarkSessionModelDataMocks.MockMarkSessionModel();
            emptyMarkSessionModel.DependantResources = new List<DependantResourceModel>();
            var markSessionRepository = new Mock<IMarkSessionRepository>();
            var projectClient = new Mock<IProjectClient>();
            var metadataClient = new Mock<IMetadataClient>();
            metadataClient
                .Setup(m => m.GetMetadataForProject(It.IsAny<string>()))
                .ReturnsAsync(MetadataModelDataMocks.MockFinishedMetadataModelList);
            metadataClient
                .Setup(m => m.MarkMetadata(It.IsAny<MetadataModel>()))
                .ReturnsAsync(DependantResourceDataMocks.MockDependantResourceModel);
            var scenarioClient = new Mock<IScenarioClient>();
            var resultConfigClient = new Mock<IResultConfigClient>();
            resultConfigClient
                .Setup(m => m.GetResultConfigsForMetadata(It.IsAny<string>()))
                .ReturnsAsync(new List<ResultConfigModel>());
            var simPlanClient = new Mock<ISimPlanClient>();
            var simRunClient = new Mock<ISimRunClient>();
            var resultDataClient = new Mock<IResultDataClient>();
            var dependantResourceHandler = new DependantResourceHandler(
                projectClient.Object,
                markSessionRepository.Object,
                metadataClient.Object,
                scenarioClient.Object,
                resultConfigClient.Object,
                simPlanClient.Object,
                simRunClient.Object,
                resultDataClient.Object
            );

            // Act
            await dependantResourceHandler.MarkResourcesForMarkSession(emptyMarkSessionModel);

            // Assert
            Assert.NotEmpty(emptyMarkSessionModel.DependantResources);
        }

        [Fact]
        public async void
            UnmarkResourcesForMarkSession_ProjectMarkSessionModel_ReturnsMarkSessionWithoutDependantResources()
        {
            // Arrange
            var markSessionModel = MarkSessionModelDataMocks.MockMarkSessionModel();
            var markSessionRepository = new Mock<IMarkSessionRepository>();
            var projectClient = new Mock<IProjectClient>();
            var metadataClient = new Mock<IMetadataClient>();
            var scenarioClient = new Mock<IScenarioClient>();
            var resultConfigClient = new Mock<IResultConfigClient>();
            var simPlanClient = new Mock<ISimPlanClient>();
            var simRunClient = new Mock<ISimRunClient>();
            var resultDataClient = new Mock<IResultDataClient>();
            var dependantResourceHandler = new DependantResourceHandler(
                projectClient.Object,
                markSessionRepository.Object,
                metadataClient.Object,
                scenarioClient.Object,
                resultConfigClient.Object,
                simPlanClient.Object,
                simRunClient.Object,
                resultDataClient.Object
            );

            // Act
            await dependantResourceHandler.UnmarkResourcesForMarkSession(markSessionModel);

            // Assert
            Assert.Empty(markSessionModel.DependantResources);
        }
    }
}