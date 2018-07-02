using System.Threading.Tasks;

namespace mars_marking_svc.ArchiveService.Interfaces
{
    public interface IArchiveServiceClient
    {
        Task EnsureArchiveRestoreIsNotRunning(
            string projectId
        );
    }
}