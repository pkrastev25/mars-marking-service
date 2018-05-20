using System;
using System.Threading.Tasks;
using Grpc.Core;
using mars_marking_svc.Exceptions;
using mars_marking_svc.MarkedResource.Models;
using mars_marking_svc.ResourceTypes.Project.Interfaces;
using mars_marking_svc.ResourceTypes.Project.Models;
using Project;

namespace mars_marking_svc.ResourceTypes.Project
{
    public class ProjectClient : IProjectClient, IDisposable
    {
        private readonly Channel _channel;
        private readonly ProjectService.ProjectServiceClient _projectServiceClient;

        public ProjectClient()
        {
            _channel = new Channel("project-svc:80", ChannelCredentials.Insecure);
            _projectServiceClient = new ProjectService.ProjectServiceClient(_channel);
        }

        public async Task<ProjectModel> GetProject(
            string projectId
        )
        {
            global::Project.Project project;

            try
            {
                project = await _projectServiceClient.GetProjectAsync(
                    new GetProjectByIdRequest
                    {
                        ProjectId = projectId
                    }
                );
            }
            catch (Exception e)
            {
                throw new FailedToGetResourceException(
                    $"Failed to get project for id: {projectId} from project-svc!",
                    e
                );
            }

            return new ProjectModel
            {
                Id = projectId,
                ToBeDeleted = project.ToBeDeleted
            };
        }

        public async Task<DependantResourceModel> MarkProject(
            string projectId
        )
        {
            var projectModel = await GetProject(projectId);

            return await MarkProject(projectModel);
        }

        public async Task<DependantResourceModel> MarkProject(
            ProjectModel projectModel
        )
        {
            if (projectModel.ToBeDeleted)
            {
                throw new ResourceAlreadyMarkedException(
                    $"Cannot mark project with id: {projectModel.Id}, it is already marked!"
                );
            }

            try
            {
                await _projectServiceClient.UpdateProjectAsync(
                    new UpdateProjectRequest
                    {
                        ProjectId = projectModel.Id,
                        ToBeDeleted = "true"
                    }
                );
            }
            catch (Exception e)
            {
                throw new FailedToUpdateResourceException(
                    $"Failed to update project with id: {projectModel.Id} from project-svc!",
                    e
                );
            }

            return new DependantResourceModel(ResourceTypeEnum.Project, projectModel.Id);
        }

        public async Task UnmarkProject(
            DependantResourceModel dependantResourceModel
        )
        {
            try
            {
                await _projectServiceClient.UpdateProjectAsync(
                    new UpdateProjectRequest
                    {
                        ProjectId = dependantResourceModel.ResourceId,
                        ToBeDeleted = "false"
                    }
                );
            }
            catch (Exception e)
            {
                throw new FailedToUpdateResourceException(
                    $"Failed to update project with id: {dependantResourceModel.ResourceId} from project-svc!",
                    e
                );
            }
        }

        public async void Dispose()
        {
            await _channel.ShutdownAsync();
        }
    }
}