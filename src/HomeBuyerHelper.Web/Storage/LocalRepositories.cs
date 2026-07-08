using HomeBuyerHelper.Core.Interfaces;
using HomeBuyerHelper.Core.Models;
using Microsoft.JSInterop;

namespace HomeBuyerHelper.Web.Storage;

/// <summary>
/// localStorage-backed implementations of the Core repository interfaces
/// for the web preview. Mirrors the behavior of the SQLite repositories.
/// </summary>
public class LocalPropertyRepository : IPropertyRepository
{
    private readonly LocalStore<Property> _store;

    public LocalPropertyRepository(IJSRuntime js)
    {
        _store = new LocalStore<Property>(js, "properties", p => p.Id, (p, id) => p.Id = id);
    }

    public Task<Property?> GetByIdAsync(int id) => Task.FromResult(_store.Find(id));

    public Task<IReadOnlyList<Property>> GetAllAsync() =>
        Task.FromResult<IReadOnlyList<Property>>(_store.Items.ToList());

    public Task<IReadOnlyList<Property>> GetActiveAsync() =>
        Task.FromResult<IReadOnlyList<Property>>(_store.Items.Where(p => !p.IsArchived).ToList());

    public Task<IReadOnlyList<Property>> GetFavoritesAsync() =>
        Task.FromResult<IReadOnlyList<Property>>(
            _store.Items.Where(p => p.IsFavorite && !p.IsArchived).ToList());

    public Task<int> CreateAsync(Property property)
    {
        property.CreatedAt = DateTime.UtcNow;
        property.UpdatedAt = DateTime.UtcNow;
        return Task.FromResult(_store.Add(property));
    }

    public Task UpdateAsync(Property property)
    {
        property.UpdatedAt = DateTime.UtcNow;
        _store.Update(property);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(int id)
    {
        _store.Delete(id);
        return Task.CompletedTask;
    }

    public Task<bool> ExistsAsync(int id) => Task.FromResult(_store.Find(id) != null);

    public Task<int> GetCountAsync() => Task.FromResult(_store.Items.Count);
}

/// <summary>
/// localStorage-backed criteria repository.
/// </summary>
public class LocalCriteriaRepository : ICriteriaRepository
{
    private readonly LocalStore<EvaluationCriterion> _store;

    public LocalCriteriaRepository(IJSRuntime js)
    {
        _store = new LocalStore<EvaluationCriterion>(js, "criteria", c => c.Id, (c, id) => c.Id = id);
    }

    public Task<EvaluationCriterion?> GetByIdAsync(int id) => Task.FromResult(_store.Find(id));

    public Task<IReadOnlyList<EvaluationCriterion>> GetAllAsync() =>
        Task.FromResult<IReadOnlyList<EvaluationCriterion>>(
            _store.Items.OrderBy(c => c.DisplayOrder).ToList());

    public Task<IReadOnlyList<EvaluationCriterion>> GetByCategoryAsync(CriterionCategory category) =>
        Task.FromResult<IReadOnlyList<EvaluationCriterion>>(
            _store.Items.Where(c => c.Category == category).OrderBy(c => c.DisplayOrder).ToList());

    public Task<int> CreateAsync(EvaluationCriterion criterion)
    {
        criterion.CreatedAt = DateTime.UtcNow;
        criterion.UpdatedAt = DateTime.UtcNow;
        return Task.FromResult(_store.Add(criterion));
    }

    public async Task CreateManyAsync(IEnumerable<EvaluationCriterion> criteria)
    {
        foreach (var criterion in criteria)
        {
            await CreateAsync(criterion);
        }
    }

    public Task UpdateAsync(EvaluationCriterion criterion)
    {
        criterion.UpdatedAt = DateTime.UtcNow;
        _store.Update(criterion);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(int id)
    {
        _store.Delete(id);
        return Task.CompletedTask;
    }

    public Task<bool> ExistsAsync(int id) => Task.FromResult(_store.Find(id) != null);

    public Task<int> GetCountAsync() => Task.FromResult(_store.Items.Count);

    public Task ReorderAsync(IEnumerable<(int Id, int NewOrder)> newOrders)
    {
        foreach (var (id, newOrder) in newOrders)
        {
            var criterion = _store.Find(id);
            if (criterion != null)
            {
                criterion.DisplayOrder = newOrder;
            }
        }
        _store.Save();
        return Task.CompletedTask;
    }
}

/// <summary>
/// localStorage-backed score repository.
/// </summary>
public class LocalScoreRepository : IScoreRepository
{
    private readonly LocalStore<PropertyScore> _store;
    private readonly ICriteriaRepository _criteriaRepository;

    public LocalScoreRepository(IJSRuntime js, ICriteriaRepository criteriaRepository)
    {
        _store = new LocalStore<PropertyScore>(js, "scores", s => s.Id, (s, id) => s.Id = id);
        _criteriaRepository = criteriaRepository;
    }

    public async Task<PropertyScore?> GetByIdAsync(int id)
    {
        var score = _store.Find(id);
        return score == null ? null : await WithCriterionAsync(score);
    }

    public async Task<IReadOnlyList<PropertyScore>> GetByPropertyIdAsync(int propertyId)
    {
        var scores = _store.Items.Where(s => s.PropertyId == propertyId).ToList();
        foreach (var score in scores)
        {
            await WithCriterionAsync(score);
        }
        return scores;
    }

    public async Task<IReadOnlyList<PropertyScore>> GetByCriterionIdAsync(int criterionId)
    {
        var scores = _store.Items.Where(s => s.CriterionId == criterionId).ToList();
        foreach (var score in scores)
        {
            await WithCriterionAsync(score);
        }
        return scores;
    }

    public async Task<PropertyScore?> GetByPropertyAndCriterionAsync(int propertyId, int criterionId)
    {
        var score = _store.Items.FirstOrDefault(
            s => s.PropertyId == propertyId && s.CriterionId == criterionId);
        return score == null ? null : await WithCriterionAsync(score);
    }

    public Task<int> UpsertAsync(PropertyScore score)
    {
        var existing = _store.Items.FirstOrDefault(
            s => s.PropertyId == score.PropertyId && s.CriterionId == score.CriterionId);

        if (existing != null)
        {
            existing.Score = score.Score;
            existing.Notes = score.Notes;
            existing.UpdatedAt = DateTime.UtcNow;
            _store.Save();
            return Task.FromResult(existing.Id);
        }

        score.CreatedAt = DateTime.UtcNow;
        score.UpdatedAt = DateTime.UtcNow;
        return Task.FromResult(_store.Add(score));
    }

    public Task DeleteAsync(int id)
    {
        _store.Delete(id);
        return Task.CompletedTask;
    }

    public Task DeleteByPropertyIdAsync(int propertyId)
    {
        _store.Items.RemoveAll(s => s.PropertyId == propertyId);
        _store.Save();
        return Task.CompletedTask;
    }

    public Task DeleteByCriterionIdAsync(int criterionId)
    {
        _store.Items.RemoveAll(s => s.CriterionId == criterionId);
        _store.Save();
        return Task.CompletedTask;
    }

    public async Task<int> GetTotalWeightedScoreAsync(int propertyId)
    {
        var scores = await GetByPropertyIdAsync(propertyId);
        return scores.Sum(s => s.WeightedScore);
    }

    public async Task<int> GetMaxPossibleScoreAsync()
    {
        var criteria = await _criteriaRepository.GetAllAsync();
        // Scores use a 1-10 scale (matches the SQLite repository).
        return criteria.Sum(c => 10 * c.Weight);
    }

    private async Task<PropertyScore> WithCriterionAsync(PropertyScore score)
    {
        score.Criterion ??= await _criteriaRepository.GetByIdAsync(score.CriterionId);
        return score;
    }
}

/// <summary>
/// localStorage-backed user preferences repository.
/// </summary>
public class LocalUserPreferencesRepository : IUserPreferencesRepository
{
    private readonly LocalStore<UserPreferences> _store;

    public LocalUserPreferencesRepository(IJSRuntime js)
    {
        _store = new LocalStore<UserPreferences>(js, "preferences", p => p.Id, (p, id) => p.Id = id);
    }

    public Task<UserPreferences> GetAsync()
    {
        var preferences = _store.Items.FirstOrDefault();
        if (preferences == null)
        {
            preferences = new UserPreferences();
            _store.Add(preferences);
        }
        return Task.FromResult(preferences);
    }

    public Task SaveAsync(UserPreferences preferences)
    {
        preferences.UpdatedAt = DateTime.UtcNow;
        if (_store.Find(preferences.Id) == null)
        {
            _store.Add(preferences);
        }
        else
        {
            _store.Update(preferences);
        }
        return Task.CompletedTask;
    }

    public Task ResetAsync()
    {
        _store.Items.Clear();
        _store.Save();
        return Task.CompletedTask;
    }

    public async Task<bool> HasCompletedOnboardingAsync() => (await GetAsync()).HasCompletedOnboarding;

    public async Task SetOnboardingCompleteAsync()
    {
        var preferences = await GetAsync();
        preferences.HasCompletedOnboarding = true;
        await SaveAsync(preferences);
    }
}

/// <summary>
/// localStorage-backed budget repositories.
/// </summary>
public class LocalIncomeRepository : IIncomeRepository
{
    private readonly LocalStore<IncomeSource> _store;

    public LocalIncomeRepository(IJSRuntime js)
    {
        _store = new LocalStore<IncomeSource>(js, "income", s => s.Id, (s, id) => s.Id = id);
    }

    public Task<IncomeSource?> GetByIdAsync(int id) => Task.FromResult(_store.Find(id));

    public Task<IReadOnlyList<IncomeSource>> GetAllAsync() =>
        Task.FromResult<IReadOnlyList<IncomeSource>>(_store.Items.ToList());

    public Task<int> CreateAsync(IncomeSource source) => Task.FromResult(_store.Add(source));

    public Task UpdateAsync(IncomeSource source)
    {
        _store.Update(source);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(int id)
    {
        _store.Delete(id);
        return Task.CompletedTask;
    }
}

/// <summary>
/// localStorage-backed expense repository.
/// </summary>
public class LocalExpenseRepository : IExpenseRepository
{
    private readonly LocalStore<Expense> _store;

    public LocalExpenseRepository(IJSRuntime js)
    {
        _store = new LocalStore<Expense>(js, "expenses", e => e.Id, (e, id) => e.Id = id);
    }

    public Task<Expense?> GetByIdAsync(int id) => Task.FromResult(_store.Find(id));

    public Task<IReadOnlyList<Expense>> GetAllAsync() =>
        Task.FromResult<IReadOnlyList<Expense>>(_store.Items.ToList());

    public Task<int> CreateAsync(Expense expense) => Task.FromResult(_store.Add(expense));

    public Task UpdateAsync(Expense expense)
    {
        _store.Update(expense);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(int id)
    {
        _store.Delete(id);
        return Task.CompletedTask;
    }
}

/// <summary>
/// localStorage-backed one-time event repository.
/// </summary>
public class LocalOneTimeEventRepository : IOneTimeEventRepository
{
    private readonly LocalStore<OneTimeEvent> _store;

    public LocalOneTimeEventRepository(IJSRuntime js)
    {
        _store = new LocalStore<OneTimeEvent>(js, "events", e => e.Id, (e, id) => e.Id = id);
    }

    public Task<OneTimeEvent?> GetByIdAsync(int id) => Task.FromResult(_store.Find(id));

    public Task<IReadOnlyList<OneTimeEvent>> GetAllAsync() =>
        Task.FromResult<IReadOnlyList<OneTimeEvent>>(_store.Items.OrderBy(e => e.Date).ToList());

    public Task<int> CreateAsync(OneTimeEvent oneTimeEvent) => Task.FromResult(_store.Add(oneTimeEvent));

    public Task UpdateAsync(OneTimeEvent oneTimeEvent)
    {
        _store.Update(oneTimeEvent);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(int id)
    {
        _store.Delete(id);
        return Task.CompletedTask;
    }
}

/// <summary>
/// localStorage-backed funding source repository.
/// </summary>
public class LocalFundingRepository : IFundingRepository
{
    private readonly LocalStore<FundingSource> _store;

    public LocalFundingRepository(IJSRuntime js)
    {
        _store = new LocalStore<FundingSource>(js, "funding", f => f.Id, (f, id) => f.Id = id);
    }

    public Task<FundingSource?> GetByIdAsync(int id) => Task.FromResult(_store.Find(id));

    public Task<IReadOnlyList<FundingSource>> GetAllAsync() =>
        Task.FromResult<IReadOnlyList<FundingSource>>(_store.Items.ToList());

    public Task<int> CreateAsync(FundingSource source) => Task.FromResult(_store.Add(source));

    public Task UpdateAsync(FundingSource source)
    {
        _store.Update(source);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(int id)
    {
        _store.Delete(id);
        return Task.CompletedTask;
    }
}

/// <summary>
/// localStorage-backed offer scenario repository.
/// </summary>
public class LocalOfferScenarioRepository : IOfferScenarioRepository
{
    private readonly LocalStore<OfferScenario> _store;

    public LocalOfferScenarioRepository(IJSRuntime js)
    {
        _store = new LocalStore<OfferScenario>(js, "offers", o => o.Id, (o, id) => o.Id = id);
    }

    public Task<OfferScenario?> GetByIdAsync(int id) => Task.FromResult(_store.Find(id));

    public Task<IReadOnlyList<OfferScenario>> GetByPropertyIdAsync(int propertyId) =>
        Task.FromResult<IReadOnlyList<OfferScenario>>(
            _store.Items.Where(o => o.PropertyId == propertyId).OrderBy(o => o.Id).ToList());

    public Task<IReadOnlyList<OfferScenario>> GetAllAsync() =>
        Task.FromResult<IReadOnlyList<OfferScenario>>(_store.Items.OrderBy(o => o.Id).ToList());

    public Task<int> CreateAsync(OfferScenario offer)
    {
        offer.CreatedAt = DateTime.UtcNow;
        offer.UpdatedAt = DateTime.UtcNow;
        return Task.FromResult(_store.Add(offer));
    }

    public Task UpdateAsync(OfferScenario offer)
    {
        offer.UpdatedAt = DateTime.UtcNow;
        _store.Update(offer);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(int id)
    {
        _store.Delete(id);
        return Task.CompletedTask;
    }

    public Task DeleteByPropertyIdAsync(int propertyId)
    {
        _store.Items.RemoveAll(o => o.PropertyId == propertyId);
        _store.Save();
        return Task.CompletedTask;
    }
}

/// <summary>
/// localStorage-backed photo records (no file storage in the browser preview;
/// kept so shared services like PropertyService can cascade cleanly).
/// </summary>
public class LocalPhotoRepository : IPhotoRepository
{
    private readonly LocalStore<PropertyPhoto> _store;

    public LocalPhotoRepository(IJSRuntime js)
    {
        _store = new LocalStore<PropertyPhoto>(js, "photos", p => p.Id, (p, id) => p.Id = id);
    }

    public Task<IReadOnlyList<PropertyPhoto>> GetByPropertyIdAsync(int propertyId) =>
        Task.FromResult<IReadOnlyList<PropertyPhoto>>(
            _store.Items.Where(p => p.PropertyId == propertyId).OrderBy(p => p.SortOrder).ToList());

    public Task<int> CreateAsync(PropertyPhoto photo) => Task.FromResult(_store.Add(photo));

    public Task DeleteAsync(int id)
    {
        _store.Delete(id);
        return Task.CompletedTask;
    }

    public Task DeleteByPropertyIdAsync(int propertyId)
    {
        _store.Items.RemoveAll(p => p.PropertyId == propertyId);
        _store.Save();
        return Task.CompletedTask;
    }
}

/// <summary>
/// localStorage-backed pros/cons repository.
/// </summary>
public class LocalProConRepository : IProConRepository
{
    private readonly LocalStore<PropertyProCon> _store;

    public LocalProConRepository(IJSRuntime js)
    {
        _store = new LocalStore<PropertyProCon>(js, "proscons", p => p.Id, (p, id) => p.Id = id);
    }

    public Task<IReadOnlyList<PropertyProCon>> GetByPropertyIdAsync(int propertyId) =>
        Task.FromResult<IReadOnlyList<PropertyProCon>>(
            _store.Items.Where(p => p.PropertyId == propertyId).OrderBy(p => p.SortOrder).ToList());

    public Task<int> CreateAsync(PropertyProCon item) => Task.FromResult(_store.Add(item));

    public Task UpdateAsync(PropertyProCon item)
    {
        _store.Update(item);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(int id)
    {
        _store.Delete(id);
        return Task.CompletedTask;
    }

    public Task DeleteByPropertyIdAsync(int propertyId)
    {
        _store.Items.RemoveAll(p => p.PropertyId == propertyId);
        _store.Save();
        return Task.CompletedTask;
    }
}
