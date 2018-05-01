using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using mars_marking_svc.Exceptions;
using mars_marking_svc.MarkedResource.Models;
using mars_marking_svc.Services.Models;
using MongoDB.Driver;

public class MarkSessionRepository : IMarkSessionRepository
{
    private readonly IDbMongoService _dbMongoService;
    private readonly ILoggerService _loggerService;

    public MarkSessionRepository(
        IDbMongoService dbMongoService,
        ILoggerService loggerService
    )
    {
        _dbMongoService = dbMongoService;
        _loggerService = loggerService;
    }

    public async Task Create(
        MarkSessionModel markSessionModel
    )
    {
        try
        {
            await EnsureUniqueFieldForMarkSession(MarkSessionModel.BsonElementDefinitionResourceId);
            await _dbMongoService.GetMarkSessionCollection().InsertOneAsync(markSessionModel);
            _loggerService.LodCreateEvent(markSessionModel.ToString());
        }
        catch (MongoWriteException e)
        {
            if (e.WriteError.Category == ServerErrorCategory.DuplicateKey)
            {
                throw new MarkSessionAlreadyExistsException(
                    $"Cannot create {markSessionModel}, it already exists!"
                );
            }

            throw new FailedToCreateMarkSessionException(
                $"Failed to create {markSessionModel}",
                e
            );
        }
        catch (Exception e)
        {
            throw new FailedToCreateMarkSessionException(
                $"Failed to create {markSessionModel}",
                e
            );
        }
    }

    public async Task<MarkSessionModel> GetForFilter(
        FilterDefinition<MarkSessionModel> filterDefinition
    )
    {
        var markSessionCursor = await _dbMongoService.GetMarkSessionCollection().FindAsync(
            filterDefinition
        );

        return await markSessionCursor.FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<MarkSessionModel>> GetAllForFilter(
        FilterDefinition<MarkSessionModel> filterDefinition
    )
    {
        var markSessionCursors = await _dbMongoService.GetMarkSessionCollection().FindAsync(
            filterDefinition
        );

        return await markSessionCursors.ToListAsync();
    }

    public async Task<IEnumerable<MarkSessionModel>> GetAll()
    {
        return await _dbMongoService.GetMarkSessionCollection().Find(x => true).ToListAsync();
    }

    public async Task Update(
        MarkSessionModel markSessionModel
    )
    {
        markSessionModel.LatestUpdateTimestampInTicks = DateTime.Now.Ticks;

        await _dbMongoService.GetMarkSessionCollection().ReplaceOneAsync(
            GetFilterDefinitionForResourceId(markSessionModel.ResourceId),
            markSessionModel
        );
    }

    public async Task Delete(
        MarkSessionModel markSessionModel
    )
    {
        if (markSessionModel.DependantResources.Count != 0)
        {
            throw new CannotDeleteMarkSessionException(
                $"{markSessionModel} cannot be deleted because it has unmarked dependant resources!"
            );
        }

        if (markSessionModel.SourceDependency != null)
        {
            throw new CannotDeleteMarkSessionException(
                $"{markSessionModel} cannot be deleted because it has unmarked source dependant resources!"
            );
        }

        await _dbMongoService.GetMarkSessionCollection().DeleteOneAsync(
            GetFilterDefinitionForResourceId(markSessionModel.ResourceId)
        );
        _loggerService.LogDeleteEvent(markSessionModel.ToString());
    }

    private FilterDefinition<MarkSessionModel> GetFilterDefinitionForResourceId(
        string resourceId
    )
    {
        return Builders<MarkSessionModel>.Filter.Eq(MarkSessionModel.BsonElementDefinitionResourceId, resourceId);
    }

    private async Task EnsureUniqueFieldForMarkSession(
        string uniqueFieldDefinition
    )
    {
        var options = new CreateIndexOptions {Unique = true};
        var field = new StringFieldDefinition<MarkSessionModel>(uniqueFieldDefinition);
        var indexDefinition = new IndexKeysDefinitionBuilder<MarkSessionModel>().Ascending(field);

        await _dbMongoService.GetMarkSessionCollection().Indexes.CreateOneAsync(indexDefinition, options);
    }
}