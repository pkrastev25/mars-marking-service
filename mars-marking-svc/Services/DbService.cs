﻿using System;
using System.Threading.Tasks;
using mars_marking_svc.Exceptions;
using mars_marking_svc.MarkedResource.Models;
using mars_marking_svc.Services.Models;
using MongoDB.Driver;

public class DbService : IDbService
{
    private readonly ILoggerService _loggerService;
    private readonly IMongoCollection<DbMarkSessionModel> _markedSessionCollection;

    public DbService(
        ILoggerService loggerService
    )
    {
        _loggerService = loggerService;
        var client = new MongoClient("mongodb://mongodb:27017");
        var db = client.GetDatabase("marked-resources");
        _markedSessionCollection = db.GetCollection<DbMarkSessionModel>("marked-resources");
    }

    public async Task InsertNewMarkSession(DbMarkSessionModel markSessionModel)
    {
        try
        {
            var existingMarkSession = await FindMarkSessionByResourceId(markSessionModel.ResourceId);

            if (existingMarkSession != null)
            {
                throw new MarkSessionAlreadyExistsException(
                    $"Cannot create {markSessionModel}, it already exists!"
                );
            }

            await _markedSessionCollection.InsertOneAsync(markSessionModel);
            _loggerService.LodCreateEvent(markSessionModel.ToString());
        }
        catch (MarkSessionAlreadyExistsException)
        {
            throw;
        }
        catch (Exception e)
        {
            throw new FailedToCreateMarkSessionException(
                $"Failed to create {markSessionModel}",
                e
            );
        }
    }

    public async Task UpdateMarkSession(DbMarkSessionModel markSessionModel)
    {
        await _markedSessionCollection.ReplaceOneAsync(
            GetFilterDefinitionForResourceId(markSessionModel.ResourceId),
            markSessionModel
        );
    }

    public async Task DeleteMarkSession(DbMarkSessionModel markSessionModel)
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

        await _markedSessionCollection.DeleteOneAsync(
            GetFilterDefinitionForResourceId(markSessionModel.ResourceId)
        );
        _loggerService.LogDeleteEvent(markSessionModel.ToString());
    }

    private FilterDefinition<DbMarkSessionModel> GetFilterDefinitionForResourceId(string resourceId)
    {
        return Builders<DbMarkSessionModel>.Filter.Eq("ResourceId", resourceId);
    }

    private async Task<DbMarkSessionModel> FindMarkSessionByResourceId(string resourceId)
    {
        var markSessionCursor = await _markedSessionCollection.FindAsync(
            GetFilterDefinitionForResourceId(resourceId)
        );

        return await markSessionCursor.FirstOrDefaultAsync();
    }
}