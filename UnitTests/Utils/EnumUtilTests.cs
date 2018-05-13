using mars_marking_svc.ResourceTypes;
using mars_marking_svc.ResourceTypes.MarkedResource.Enums;
using mars_marking_svc.Utils;
using Xunit;

namespace UnitTests.Utils
{
    public class EnumUtilTests
    {
        [Fact]
        public void DoesResourceTypeExist_ProjectResourceType_ReturnsTrue()
        {
            // Arrange
            var resourceType = ResourceTypeEnum.Project;

            // Act
            var result = EnumUtil.DoesResourceTypeExist(resourceType);

            // Asset
            Assert.True(result);
        }

        [Fact]
        public void DoesResourceTypeExist_EmptryResourceType_ReturnsFalse()
        {
            // Arrange
            var resourceType = "";

            // Act
            var result = EnumUtil.DoesResourceTypeExist(resourceType);

            // Asset
            Assert.False(result);
        }

        [Fact]
        public void DoesMarkSessionTypeExist_ToBeDeletedMarkSessionType_ReturnsTrue()
        {
            // Arrange
            var markSessionType = MarkSessionTypeEnum.ToBeDeleted;

            // Act
            var result = EnumUtil.DoesMarkSessionTypeExist(markSessionType);

            // Asset
            Assert.True(result);
        }

        [Fact]
        public void DoesMarkSessionTypeExist_InvalidMarkSessionType_ReturnsFalse()
        {
            // Arrange
            var markSessionType = "invalid";

            // Act
            var result = EnumUtil.DoesMarkSessionTypeExist(markSessionType);

            // Asset
            Assert.False(result);
        }
    }
}