using AutoMapper;
using mars_marking_svc.Controllers;
using mars_marking_svc.MarkSession.Interfaces;
using mars_marking_svc.ResourceTypes;
using mars_marking_svc.ResourceTypes.MarkedResource.Enums;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace UnitTests.Controllers
{
    public class MarkSessionControllerTests
    {
        [Fact]
        public async void CreateMarkSession_ValidInputParams_ReturnsOkObjectResult()
        {
            // Assert
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
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async void CreateMarkSession_EmptyProjectId_ReturnsBadRequestObjectResult()
        {
            // Assert
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
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async void GetMarkSessionById_ValidMarkSessionId_ReturnsOkObjectResult()
        {
            // Assert
            var markSessionId = "5ae86f68b90b230007d7ea34";
            var markSessionHandler = new Mock<IMarkSessionHandler>();
            var mapper = new Mock<IMapper>();
            var markSessionController = new MarkSessionController(markSessionHandler.Object, mapper.Object);

            // Act
            var result = await markSessionController.GetMarkSessionById(markSessionId);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async void GetMarkSessionById_EmptyMarkSessionId_ReturnsBadRequestObjectResult()
        {
            // Assert
            var markSessionId = "";
            var markSessionHandler = new Mock<IMarkSessionHandler>();
            var mapper = new Mock<IMapper>();
            var markSessionController = new MarkSessionController(markSessionHandler.Object, mapper.Object);

            // Act
            var result = await markSessionController.GetMarkSessionById(markSessionId);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async void GetMarkSessionsByMarkSessionType_ValidMarkSessionType_ReturnsOkObjectResult()
        {
            // Assert
            var markSessionType = MarkSessionTypeEnum.ToBeDeleted;
            var markSessionHandler = new Mock<IMarkSessionHandler>();
            var mapper = new Mock<IMapper>();
            var markSessionController = new MarkSessionController(markSessionHandler.Object, mapper.Object);

            // Act
            var result = await markSessionController.GetMarkSessionsByMarkSessionType(markSessionType);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async void GetMarkSessionById_InvalidMarkSessionType_ReturnsBadRequestObjectResult()
        {
            // Assert
            var markSessionType = "invalid";
            var markSessionHandler = new Mock<IMarkSessionHandler>();
            var mapper = new Mock<IMapper>();
            var markSessionController = new MarkSessionController(markSessionHandler.Object, mapper.Object);

            // Act
            var result = await markSessionController.GetMarkSessionsByMarkSessionType(markSessionType);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async void UpdateMarkSessionType_ValidInputParams_ReturnsOkResult()
        {
            // Assert
            var markSessionId = "5ae86f68b90b230007d7ea34";
            var markSessionType = MarkSessionTypeEnum.ToBeDeleted;
            var markSessionHandler = new Mock<IMarkSessionHandler>();
            var mapper = new Mock<IMapper>();
            var markSessionController = new MarkSessionController(markSessionHandler.Object, mapper.Object);

            // Act
            var result = await markSessionController.UpdateMarkSessionType(markSessionId, markSessionType);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async void UpdateMarkSessionType_EmptyMarkSessionId_ReturnsBadRequestObjectResult()
        {
            // Assert
            var markSessionId = "";
            var markSessionType = MarkSessionTypeEnum.ToBeDeleted;
            var markSessionHandler = new Mock<IMarkSessionHandler>();
            var mapper = new Mock<IMapper>();
            var markSessionController = new MarkSessionController(markSessionHandler.Object, mapper.Object);

            // Act
            var result = await markSessionController.UpdateMarkSessionType(markSessionId, markSessionType);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async void DeleteMarkSession_ValidMarkSessionId_ReturnsAcceptedResult()
        {
            // Assert
            var markSessionId = "5ae86f68b90b230007d7ea34";
            var markSessionHandler = new Mock<IMarkSessionHandler>();
            var mapper = new Mock<IMapper>();
            var markSessionController = new MarkSessionController(markSessionHandler.Object, mapper.Object);

            // Act
            var result = await markSessionController.DeleteMarkSession(markSessionId);

            // Assert
            Assert.IsType<AcceptedResult>(result);
        }

        [Fact]
        public async void DeleteMarkSession_EmptyMarkSessionId_ReturnsBadRequestObjectResult()
        {
            // Assert
            var markSessionType = "";
            var markSessionHandler = new Mock<IMarkSessionHandler>();
            var mapper = new Mock<IMapper>();
            var markSessionController = new MarkSessionController(markSessionHandler.Object, mapper.Object);

            // Act
            var result = await markSessionController.DeleteMarkSession(markSessionType);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async void DeleteEmptyMarkSession_ValidMarkSessionId_ReturnsOkResult()
        {
            // Assert
            var markSessionId = "5ae86f68b90b230007d7ea34";
            var markSessionHandler = new Mock<IMarkSessionHandler>();
            var mapper = new Mock<IMapper>();
            var markSessionController = new MarkSessionController(markSessionHandler.Object, mapper.Object);

            // Act
            var result = await markSessionController.DeleteEmptyMarkSession(markSessionId);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async void DeleteEmptyMarkSession_EmptyMarkSessionId_ReturnsBadRequestObjectResult()
        {
            // Assert
            var markSessionType = "";
            var markSessionHandler = new Mock<IMarkSessionHandler>();
            var mapper = new Mock<IMapper>();
            var markSessionController = new MarkSessionController(markSessionHandler.Object, mapper.Object);

            // Act
            var result = await markSessionController.DeleteEmptyMarkSession(markSessionType);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}