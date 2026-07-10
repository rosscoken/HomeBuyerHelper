using HomeBuyerHelper.Core.Interfaces;
using HomeBuyerHelper.Core.Models;
using HomeBuyerHelper.Data.Entities;

namespace HomeBuyerHelper.Data.Repositories;

/// <summary>
/// SQLite implementation of IOfferScenarioRepository.
/// </summary>
public class OfferScenarioRepository : IOfferScenarioRepository
{
    private readonly DatabaseService _databaseService;

    public OfferScenarioRepository(DatabaseService databaseService)
    {
        _databaseService = databaseService;
    }

    public async Task<OfferScenario?> GetByIdAsync(int id)
    {
        var db = await _databaseService.GetConnectionAsync();
        var entity = await db.FindAsync<OfferScenarioEntity>(id);
        return entity == null ? null : MapToModel(entity);
    }

    public async Task<IReadOnlyList<OfferScenario>> GetByPropertyIdAsync(int propertyId)
    {
        var db = await _databaseService.GetConnectionAsync();
        var entities = await db.Table<OfferScenarioEntity>()
            .Where(o => o.PropertyId == propertyId)
            .OrderBy(o => o.Id)
            .ToListAsync();
        return entities.Select(MapToModel).ToList();
    }

    public async Task<IReadOnlyList<OfferScenario>> GetAllAsync()
    {
        var db = await _databaseService.GetConnectionAsync();
        var entities = await db.Table<OfferScenarioEntity>()
            .OrderBy(o => o.Id)
            .ToListAsync();
        return entities.Select(MapToModel).ToList();
    }

    public async Task<int> CreateAsync(OfferScenario offer)
    {
        var db = await _databaseService.GetConnectionAsync();
        var entity = MapToEntity(offer);
        entity.CreatedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;
        await db.InsertAsync(entity);
        return entity.Id;
    }

    public async Task UpdateAsync(OfferScenario offer)
    {
        var db = await _databaseService.GetConnectionAsync();
        var existing = await db.FindAsync<OfferScenarioEntity>(offer.Id);
        if (existing == null)
        {
            return;
        }

        var entity = MapToEntity(offer);
        entity.CreatedAt = existing.CreatedAt;
        entity.UpdatedAt = DateTime.UtcNow;
        await db.UpdateAsync(entity);
    }

    public async Task DeleteAsync(int id)
    {
        var db = await _databaseService.GetConnectionAsync();
        await db.DeleteAsync<OfferScenarioEntity>(id);
    }

    public async Task DeleteByPropertyIdAsync(int propertyId)
    {
        var db = await _databaseService.GetConnectionAsync();
        await db.Table<OfferScenarioEntity>()
            .Where(o => o.PropertyId == propertyId)
            .DeleteAsync();
    }

    private static OfferScenario MapToModel(OfferScenarioEntity entity) => new()
    {
        Id = entity.Id,
        PropertyId = entity.PropertyId,
        Name = entity.Name,
        OfferPrice = entity.OfferPrice,
        EscalationMaxPrice = entity.EscalationMaxPrice,
        DownPaymentPercent = entity.DownPaymentPercent,
        InterestRate = entity.InterestRate,
        TermYears = entity.TermYears,
        DiscountPoints = entity.DiscountPoints,
        SellerCredit = entity.SellerCredit,
        LenderCredit = entity.LenderCredit,
        EarnestMoney = entity.EarnestMoney,
        WaiveInspection = entity.WaiveInspection,
        WaiveFinancing = entity.WaiveFinancing,
        WaiveAppraisal = entity.WaiveAppraisal,
        ClosingDays = entity.ClosingDays,
        Notes = entity.Notes,
        CreatedAt = entity.CreatedAt,
        UpdatedAt = entity.UpdatedAt
    };

    private static OfferScenarioEntity MapToEntity(OfferScenario model) => new()
    {
        Id = model.Id,
        PropertyId = model.PropertyId,
        Name = model.Name,
        OfferPrice = model.OfferPrice,
        EscalationMaxPrice = model.EscalationMaxPrice,
        DownPaymentPercent = model.DownPaymentPercent,
        InterestRate = model.InterestRate,
        TermYears = model.TermYears,
        DiscountPoints = model.DiscountPoints,
        SellerCredit = model.SellerCredit,
        LenderCredit = model.LenderCredit,
        EarnestMoney = model.EarnestMoney,
        WaiveInspection = model.WaiveInspection,
        WaiveFinancing = model.WaiveFinancing,
        WaiveAppraisal = model.WaiveAppraisal,
        ClosingDays = model.ClosingDays,
        Notes = model.Notes,
        CreatedAt = model.CreatedAt,
        UpdatedAt = model.UpdatedAt
    };
}
