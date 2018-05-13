using mars_marking_svc.MarkedResource.Models;
using mars_marking_svc.ResourceTypes;
using mars_marking_svc.ResourceTypes.Metadata.Models;

namespace UnitTests._DataMocks
{
    public static class DependantResourceDataMocks
    {
        public static DependantResourceModel MockDependantResourceModel()
        {
            return new DependantResourceModel(
                ResourceTypeEnum.Metadata,
                "acd8b6d6-5490-4240-9cf3-045b214c7912"
            )
            {
                PreviousState = MetadataModel.FinishedState
            };
        }
    }
}