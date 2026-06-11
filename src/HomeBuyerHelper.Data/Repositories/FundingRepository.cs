using HomeBuyerHelper.Core.Interfaces;
using HomeBuyerHelper.Core.Models;
using HomeBuyerHelper.Data.Entities;

namespace HomeBuyerHelper.Data.Repositories;

/// <summary>
/// SQLite implementation of IFundingRepository.
/// </summary>
public class FundingRepository : IFundingRepository
{
    private readonly DatabaseService _databaseService;

    public FundingRepository(DatabaseService databaseService)
    {
        _databaseService = databaseService;
    }

    public async Task<FundingSource?> GetByIdAsync(int id)
    {
        var db = await _databaseService.GetConnectionAsync();
        var entity = await db.FindAsync<FundingSourceEntity>(id);
        return entity == null ? null : MapToModel(entity);
    }

    public async Task<IReadOnlyList<FundingSource>> GetAllAsync()
    {
        var db = await _databaseService.GetConnectionAsync();
        var entities = await db.Table<FundingSourceEntity>()
            .OrderBy(s => s.Id)
            .ToListAsync();
        return entities.Select(MapToModel).ToList();
    }

    public async Task<int> CreateAsync(FundingSource source)
    {
        var db = await _databaseService.GetConnectionAsync();
        var entity = MapToEntity(source);
        entity.CreatedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;
        await db.InsertAsync(entity);
        return entity.Id;
    }

    public async Task UpdateAsync(FundingSource source)
    {
        var db = await _databaseService.GetConnectionAsync();
        var entity = MapToEntity(source);
        entity.UpdatedAt = DateTime.UtcNow;
        await db.UpdateAsync(entity);
    }

    public async Task DeleteAsync(int id)
    {
        var db = await _databaseService.GetConnectionAsync();
        await db.DeleteAsync<FundingSourceEntity>(id);
    }

    private static FundingSource MapToModel(FundingSourceEntity entity) => new()
    {
        Id = entity.Id,
        Name = entity.Name,
        CurrentAmount = entity.CurrentAmount,
        ExpectedAmount = entity.ExpectedAmount,
        FundingType = (FundingType)entity.FundingType,
        IsLiquid = entity.IsLiquid,
        IsDocumented = entity.IsDocumented,
        MonthlyContribution = entity.MonthlyContribution,
        CostBasis = entity.CostBasis,
        IsLongTermHolding = entity.IsLongTermHolding,
        OwnerAge = entity.OwnerAge,
        IsFirstTimeBuyer = entity.IsFirstTimeBuyer,
        RothContributionPortion = entity.RothContributionPortion,
        IsRothAccount5Years = entity.IsRothAccount5Years,
        Is401kLoan = entity.Is401kLoan,
        DonorName = entity.DonorName,
        Notes = entity.Notes,
        CreatedAt = entity.CreatedAt,
        UpdatedAt = entity.UpdatedAt
    };

    private static FundingSourceEntity MapToEntity(FundingSource model) => new()
    {
        Id = model.Id,
        Name = model.Name,
        CurrentAmount = model.CurrentAmount,
        ExpectedAmount = model.ExpectedAmount,
        FundingType = (int)model.FundingType,
        IsLiquid = model.IsLiquid,
        IsDocumented = model.IsDocumented,
        MonthlyContribution = model.MonthlyContribution,
        CostBasis = model.CostBasis,
        IsLongTermHolding = model.IsLongTermHolding,
        OwnerAge = model.OwnerAge,
        IsFirstTimeBuyer = model.IsFirstTimeBuyer,
        RothContributionPortion = model.RothContributionPortion,
        IsRothAccount5Years = model.IsRothAccount5Years,
        Is401kLoan = model.Is401kLoan,
        DonorName = model.DonorName,
        Notes = model.Notes,
        CreatedAt = model.CreatedAt,
        UpdatedAt = model.UpdatedAt
    };
}
