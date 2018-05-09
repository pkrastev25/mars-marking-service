using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace mars_marking_svc.BackgroundJobs.Interfaces
{
    public interface IBackgroundJobsHandler
    {
        Task<string> CreateBackgroundJob(
            Expression<Func<Task>> backgroundJob
        );

        Task<string> GetJobStatusForBackgroundJobId(
            string backgroundJobId
        );
    }
}