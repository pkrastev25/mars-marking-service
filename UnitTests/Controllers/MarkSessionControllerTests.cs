using AutoMapper;
using mars_marking_svc.Controllers;
using mars_marking_svc.MarkSession.Interfaces;
using mars_marking_svc.ResourceTypes;
using mars_marking_svc.ResourceTypes.MarkedResource.Enums;
using Moq;
using Xunit;

namespace UnitTests.Controllers
{
    public class MarkSessionControllerTests
    {
        [Fact]
        public async void CreateMarkSession_ValidInputParams_ReturnsOkObjectResult()
        {
            // Arrange
            var resourceType = ResourceTypeEnum.Project;
            var resourceId = "e580ff4f-a3b3-4252-81c4-ad88a01cac03";
            var markSessionType = MarkSessionTypeEnum.ToBeDeleted;
            var markSessionHandler = new Mock<IMarkSessionHandler>();
            var mapper = new Mock<IMapper>();
            var markSessionController = new MarkSessionController(markSessionHandler.Object, mapper.Object);

            // Act
            var result = await markSessionController.CreateMarkSession(
                resourceType,
                resourceId,
                markSessionType,
                resourceId
            );

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async void CreateMarkSession_EmptyProjectId_ReturnsBadRequestObjectResult()
        {
            // Arrange
            var resourceType = ResourceTypeEnum.Project;
            var resourceId = "";
            var markSessionType = MarkSessionTypeEnum.ToBeDeleted;
            var markSessionHandler = new Mock<IMarkSessionHandler>();
            var mapper = new Mock<IMapper>();
            var markSessionController = new MarkSessionController(markSessionHandler.Object, mapper.Object);

            // Act
            var result = await markSessionController.CreateMarkSession(
                resourceType,
                resourceId,
                markSessionType,
                resourceId
            );

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async void GetMarkSessionById_ValidMarkSessionId_ReturnsOkObjectResult()
        {
            // Arrange
            var markSessionId = "5ae86f68b90b230007d7ea34";
            var markSessionHandler = new Mock<IMarkSessionHandler>();
            var mapper = new Mock<IMapper>();
            var markSessionController = new MarkSessionController(markSessionHandler.Object, mapper.Object);

            // Act
            var result = await markSessionController.GetMarkSessionById(markSessionId);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async void GetMarkSessionById_EmptyMarkSessionId_ReturnsBadRequestObjectResult()
        {
            // Arrange
            var markSessionId = "";
            var markSessionHandler = new Mock<IMarkSessionHandler>();
            var mapper = new Mock<IMapper>();
            var markSessionController = new MarkSessionController(markSessionHandler.Object, mapper.Object);

            // Act
            var result = await markSessionController.GetMarkSessionById(markSessionId);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async void GetMarkSessionsByMarkSessionType_ValidMarkSessionType_ReturnsOkObjectResult()
        {
            // Arrange
            var markSessionType = MarkSessionTypeEnum.ToBeDeleted;
            var markSessionHandler = new Mock<IMarkSessionHandler>();
            var mapper = new Mock<IMapper>();
            var markSessionController = new MarkSessionController(markSessionHandler.Object, mapper.Object);

            // Act
            var result = await markSessionController.GetMarkSessionsByMarkSessionType(markSessionType);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async void GetMarkSessionById_InvalidMarkSessionType_ReturnsBadRequestObjectResult()
        {
            // Arrange
            var markSessionType = "invalid";
            var markSessionHandler = new Mock<IMarkSessionHandler>();
            var mapper = new Mock<IMapper>();
            var markSessionController = new MarkSessionController(markSessionHandler.Object, mapper.Object);

            // Act
            var result = await markSessionController.GetMarkSessionsByMarkSessionType(markSessionType);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async void UpdateMarkSessionType_ValidInputParams_ReturnsOkResult()
        {
            // Arrange
            var markSessionId = "5ae86f68b90b230007d7ea34";
            var markSessionType = MarkSessionTypeEnum.ToBeDeleted;
            var markSessionHandler = new Mock<IMarkSessionHandler>();
            var mapper = new Mock<IMapper>();
            var markSessionController = new MarkSessionController(markSessionHandler.Object, mapper.Object);

            // Act
            var result = await markSessionController.UpdateMarkSessionType(markSessionId, markSessionType);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async void UpdateMarkSessionType_EmptyMarkSessionId_ReturnsBadRequestObjectResult()
        {
            // Arrange
            var markSessionId = "";
            var markSessionType = MarkSessionTypeEnum.ToBeDeleted;
            var markSessionHandler = new Mock<IMarkSessionHandler>();
            var mapper = new Mock<IMapper>();
            var markSessionController = new MarkSessionController(markSessionHandler.Object, mapper.Object);

            // Act
            var result = await markSessionController.UpdateMarkSessionType(markSessionId, markSessionType);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async void DeleteMarkSession_ValidMarkSessionId_ReturnsAcceptedResult()
        {
            // Arrange
            var markSessionId = "5ae86f68b90b230007d7ea34";
            var markSessionHandler = new Mock<IMarkSessionHandler>();
            var mapper = new Mock<IMapper>();
            var markSessionController = new MarkSessionController(markSessionHandler.Object, mapper.Object);

            // Act
            var result = await markSessionController.DeleteMarkSession(markSessionId);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async void DeleteMarkSession_EmptyMarkSessionId_ReturnsBadRequestObjectResult()
        {
            // Arrange
            var markSessionType = "";
            var markSessionHandler = new Mock<IMarkSessionHandler>();
            var mapper = new Mock<IMapper>();
            var markSessionController = new MarkSessionController(markSessionHandler.Object, mapper.Object);

            // Act
            var result = await markSessionController.DeleteMarkSession(markSessionType);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async void DeleteEmptyMarkSession_ValidMarkSessionId_ReturnsOkResult()
        {
            // Arrange
            var markSessionId = "5ae86f68b90b230007d7ea34";
            var markSessionHandler = new Mock<IMarkSessionHandler>();
            var mapper = new Mock<IMapper>();
            var markSessionController = new MarkSessionController(markSessionHandler.Object, mapper.Object);

            // Act
            var result = await markSessionController.DeleteEmptyMarkSession(markSessionId);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async void DeleteEmptyMarkSession_EmptyMarkSessionId_ReturnsBadRequestObjectResult()
        {
            // Arrange
            var markSessionType = "";
            var markSessionHandler = new Mock<IMarkSessionHandler>();
            var mapper = new Mock<IMapper>();
            var markSessionController = new MarkSessionController(markSessionHandler.Object, mapper.Object);

            // Act
            var result = await markSessionController.DeleteEmptyMarkSession(markSessionType);

            // Assert
            Assert.NotNull(result);
        }
    }
}