﻿using System.Threading.Tasks;
using mars_marking_svc.MarkedResource.Models;
using mars_marking_svc.ResourceTypes.SimRun.Models;

namespace mars_marking_svc.ResourceTypes.ResultData.Interfaces
{
    public interface IResultDataClient
    {
        Task<DependantResourceModel> MarkResultData(
            string resultDataId
        );

        Task<DependantResourceModel> MarkResultData(
            SimRunModel simRunModel
        );

        Task UnmarkResultData(
            DependantResourceModel dependantResourceModel
        );
    }
}