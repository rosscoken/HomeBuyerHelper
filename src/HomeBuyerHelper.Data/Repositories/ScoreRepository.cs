using HomeBuyerHelper.Core.Interfaces;
using HomeBuyerHelper.Core.Models;
using HomeBuyerHelper.Data.Entities;

namespace HomeBuyerHelper.Data.Repositories;

/// <summary>
/// SQLite implementation of IScoreRepository.
/// </summary>
public class ScoreRepository : IScoreRepository
{
    private readonly DatabaseService _databaseService;
    private readonly ICriteriaRepository _criteriaRepository;

    public ScoreRepository(DatabaseService databaseService, ICriteriaRepository criteriaRepository)
    {
        _databaseService = databaseService;
        _criteriaRepository = criteriaRepository;
    }

    public async Task<PropertyScore?> GetByIdAsync(int id)
    {
        var db = await _databaseService.GetConnectionAsync();
        var entity = await db.FindAsync<PropertyScoreEntity>(id);
        return entity == null ? null : await MapToModelAsync(entity);
    }

    public async Task<IReadOnlyList<PropertyScore>> GetByPropertyIdAsync(int propertyId)
    {
        var db = await _databaseService.GetConnectionAsync();
        var entities = await db.Table<PropertyScoreEntity>()
            .Where(s => s.PropertyId == propertyId)
            .ToListAsync();

        var scores = new List<PropertyScore>();
        foreach (var entity in entities)
        {
            scores.Add(await MapToModelAsync(entity));
        }
        return scores;
    }

    public async Task<IReadOnlyList<PropertyScore>> GetByCriterionIdAsync(int criterionId)
    {
        var db = await _databaseService.GetConnectionAsync();
        var entities = await db.Table<PropertyScoreEntity>()
            .Where(s => s.CriterionId == criterionId)
            .ToListAsync();

        var scores = new List<PropertyScore>();
        foreach (var entity in entities)
        {
            scores.Add(await MapToModelAsync(entity));
        }
        return scores;
    }

    public async Task<PropertyScore?> GetByPropertyAndCriterionAsync(int propertyId, int criterionId)
    {
        var db = await _databaseService.GetConnectionAsync();
        var entity = await db.Table<PropertyScoreEntity>()
            .Where(s => s.PropertyId == propertyId && s.CriterionId == criterionId)
            .FirstOrDefaultAsync();
        return entity == null ? null : await MapToModelAsync(entity);
    }

    public async Task<int> UpsertAsync(PropertyScore score)
    {
        var db = await _databaseService.GetConnectionAsync();

        var existingEntity = await db.Table<PropertyScoreEntity>()
            .Where(s => s.PropertyId == score.PropertyId && s.CriterionId == score.CriterionId)
            .FirstOrDefaultAsync();

        if (existingEntity != null)
        {
            existingEntity.Score = score.Score;
            existingEntity.Notes = score.Notes;
            existingEntity.UpdatedAt = DateTime.UtcNow;
            await db.UpdateAsync(existingEntity);
            return existingEntity.Id;
        }
        else
        {
            var entity = MapToEntity(score);
            entity.CreatedAt = DateTime.UtcNow;
            entity.UpdatedAt = DateTime.UtcNow;
            await db.InsertAsync(entity);
            return entity.Id;
        }
    }

    public async Task DeleteAsync(int id)
    {
        var db = await _databaseService.GetConnectionAsync();
        await db.DeleteAsync<PropertyScoreEntity>(id);
    }

    public async Task DeleteByPropertyIdAsync(int propertyId)
    {
        var db = await _databaseService.GetConnectionAsync();
        await db.ExecuteAsync("DELETE FROM PropertyScores WHERE PropertyId = ?", propertyId);
    }

    public async Task DeleteByCriterionIdAsync(int criterionId)
    {
        var db = await _databaseService.GetConnectionAsync();
        await db.ExecuteAsync("DELETE FROM PropertyScores WHERE CriterionId = ?", criterionId);
    }

    public async Task<int> GetTotalWeightedScoreAsync(int propertyId)
    {
        var scores = await GetByPropertyIdAsync(propertyId);
        return scores.Sum(s => s.WeightedScore);
    }

    public async Task<int> GetMaxPossibleScoreAsync()
    {
        var criteria = await _criteriaRepository.GetAllAsync();
        // Scores use a 1-10 scale, so max weighted score = sum of (10 * weight) for all criteria
        return criteria.Sum(c => 10 * c.Weight);
    }

    private async Task<PropertyScore> MapToModelAsync(PropertyScoreEntity entity)
    {
        var criterion = await _criteriaRepository.GetByIdAsync(entity.CriterionId);
        return new PropertyScore
        {
            Id = entity.Id,
            PropertyId = entity.PropertyId,
            CriterionId = entity.CriterionId,
            Score = entity.Score,
            Notes = entity.Notes,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            Criterion = criterion
        };
    }

    private static PropertyScoreEntity MapToEntity(PropertyScore model) => new()
    {
        Id = model.Id,
        PropertyId = model.PropertyId,
        CriterionId = model.CriterionId,
        Score = model.Score,
        Notes = model.Notes,
        CreatedAt = model.CreatedAt,
        UpdatedAt = model.UpdatedAt
    };
}
