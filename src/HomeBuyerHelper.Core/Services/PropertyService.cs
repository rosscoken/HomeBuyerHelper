using HomeBuyerHelper.Core.Interfaces;
using HomeBuyerHelper.Core.Models;

namespace HomeBuyerHelper.Core.Services;

/// <summary>
/// Business logic service for property operations.
/// </summary>
public class PropertyService : IPropertyService
{
    private readonly IPropertyRepository _propertyRepository;
    private readonly IScoreRepository _scoreRepository;
    private readonly ICriteriaRepository _criteriaRepository;
    private readonly IPhotoRepository _photoRepository;
    private readonly IProConRepository _proConRepository;
    private readonly IOfferScenarioRepository _offerScenarioRepository;

    public PropertyService(
        IPropertyRepository propertyRepository,
        IScoreRepository scoreRepository,
        ICriteriaRepository criteriaRepository,
        IPhotoRepository photoRepository,
        IProConRepository proConRepository,
        IOfferScenarioRepository offerScenarioRepository)
    {
        _propertyRepository = propertyRepository;
        _scoreRepository = scoreRepository;
        _criteriaRepository = criteriaRepository;
        _photoRepository = photoRepository;
        _proConRepository = proConRepository;
        _offerScenarioRepository = offerScenarioRepository;
    }

    public async Task<IReadOnlyList<Property>> GetPropertiesWithScoresAsync()
    {
        var properties = await _propertyRepository.GetActiveAsync();
        var allCriteria = await _criteriaRepository.GetAllAsync();
        var totalCriteriaCount = allCriteria.Count;
        var totalPossibleWeight = allCriteria.Sum(c => c.Weight);

        // Calculate scores for each property
        var propertiesWithScores = new List<(Property Property, decimal OverallScore)>();
        foreach (var property in properties)
        {
            var scores = await _scoreRepository.GetByPropertyIdAsync(property.Id);
            property.Scores = scores.ToList();
            property.ScoredCriteriaCount = scores.Count;
            property.TotalCriteriaCount = totalCriteriaCount;

            // Calculate weighted score (0-10 scale)
            if (totalPossibleWeight > 0 && scores.Any())
            {
                var weightedSum = 0m;
                var scoredWeight = 0m;

                foreach (var score in scores)
                {
                    var criterion = allCriteria.FirstOrDefault(c => c.Id == score.CriterionId);
                    if (criterion != null)
                    {
                        weightedSum += (decimal)score.Score * criterion.Weight;
                        scoredWeight += criterion.Weight;
                    }
                }

                // Normalize to 10-point scale based on scored criteria weights
                property.OverallScore = scoredWeight > 0
                    ? Math.Round(weightedSum / scoredWeight, 1)
                    : 0;
            }

            propertiesWithScores.Add((property, property.OverallScore));
        }

        // Assign ranks based on overall score
        var ranked = propertiesWithScores
            .OrderByDescending(p => p.OverallScore)
            .ThenBy(p => p.Property.Nickname)
            .Select((p, index) => { p.Property.Rank = index + 1; return p.Property; })
            .ToList();

        return ranked;
    }

    public async Task<Property?> GetPropertyDetailAsync(int id)
    {
        var property = await _propertyRepository.GetByIdAsync(id);
        if (property == null)
        {
            return null;
        }

        var allCriteria = await _criteriaRepository.GetAllAsync();
        var scores = await _scoreRepository.GetByPropertyIdAsync(id);

        property.Scores = scores.ToList();
        property.ScoredCriteriaCount = scores.Count;
        property.TotalCriteriaCount = allCriteria.Count;

        // Calculate weighted score
        var totalPossibleWeight = allCriteria.Sum(c => c.Weight);
        if (totalPossibleWeight > 0 && scores.Any())
        {
            var weightedSum = 0m;
            var scoredWeight = 0m;

            foreach (var score in scores)
            {
                var criterion = allCriteria.FirstOrDefault(c => c.Id == score.CriterionId);
                if (criterion != null)
                {
                    weightedSum += (decimal)score.Score * criterion.Weight;
                    scoredWeight += criterion.Weight;
                }
            }

            property.OverallScore = scoredWeight > 0
                ? Math.Round(weightedSum / scoredWeight, 1)
                : 0;
        }

        // Get rank
        var rankings = await GetPropertyRankingsAsync();
        var propertyRank = rankings.FirstOrDefault(r => r.Property.Id == id);
        property.Rank = propertyRank?.Rank ?? 0;

        return property;
    }

    public async Task<Property> CreatePropertyAsync(Property property)
    {
        property.CreatedAt = DateTime.UtcNow;
        property.UpdatedAt = DateTime.UtcNow;
        var id = await _propertyRepository.CreateAsync(property);
        property.Id = id;
        return property;
    }

    public async Task UpdatePropertyAsync(Property property)
    {
        property.UpdatedAt = DateTime.UtcNow;
        await _propertyRepository.UpdateAsync(property);
    }

    public async Task ArchivePropertyAsync(int id)
    {
        var property = await _propertyRepository.GetByIdAsync(id);
        if (property != null)
        {
            property.IsArchived = true;
            property.UpdatedAt = DateTime.UtcNow;
            await _propertyRepository.UpdateAsync(property);
        }
    }

    public async Task DeletePropertyAsync(int id)
    {
        // Cascade: scores, photos (records + files), pros/cons, and offer
        // scenarios go with the property so nothing orphans or leaks.
        await _scoreRepository.DeleteByPropertyIdAsync(id);
        await _offerScenarioRepository.DeleteByPropertyIdAsync(id);

        foreach (var photo in await _photoRepository.GetByPropertyIdAsync(id))
        {
            if (File.Exists(photo.FilePath))
            {
                File.Delete(photo.FilePath);
            }
        }
        await _photoRepository.DeleteByPropertyIdAsync(id);
        await _proConRepository.DeleteByPropertyIdAsync(id);

        await _propertyRepository.DeleteAsync(id);
    }

    public async Task ToggleFavoriteAsync(int id)
    {
        var property = await _propertyRepository.GetByIdAsync(id);
        if (property != null)
        {
            property.IsFavorite = !property.IsFavorite;
            property.UpdatedAt = DateTime.UtcNow;
            await _propertyRepository.UpdateAsync(property);
        }
    }

    public async Task<IReadOnlyList<PropertyComparisonResult>> ComparePropertiesAsync(IEnumerable<int> propertyIds)
    {
        var results = new List<PropertyComparisonResult>();
        var maxPossibleScore = await _scoreRepository.GetMaxPossibleScoreAsync();

        foreach (var id in propertyIds)
        {
            var property = await _propertyRepository.GetByIdAsync(id);
            if (property == null)
            {
                continue;
            }

            var scores = await _scoreRepository.GetByPropertyIdAsync(id);
            var totalWeightedScore = scores.Sum(s => s.WeightedScore);
            var scorePercentage = maxPossibleScore > 0
                ? (decimal)totalWeightedScore / maxPossibleScore * 100
                : 0;

            results.Add(new PropertyComparisonResult
            {
                Property = property,
                Scores = scores,
                TotalWeightedScore = totalWeightedScore,
                ScorePercentage = scorePercentage
            });
        }

        return results.OrderByDescending(r => r.TotalWeightedScore).ToList();
    }

    public async Task<IReadOnlyList<PropertyRanking>> GetPropertyRankingsAsync()
    {
        var properties = await _propertyRepository.GetActiveAsync();
        var maxPossibleScore = await _scoreRepository.GetMaxPossibleScoreAsync();

        var rankings = new List<(Property Property, int Score, decimal Percentage)>();

        foreach (var property in properties)
        {
            var totalScore = await _scoreRepository.GetTotalWeightedScoreAsync(property.Id);
            var percentage = maxPossibleScore > 0
                ? (decimal)totalScore / maxPossibleScore * 100
                : 0;
            rankings.Add((property, totalScore, percentage));
        }

        return rankings
            .OrderByDescending(r => r.Score)
            .Select((r, index) => new PropertyRanking
            {
                Property = r.Property,
                Rank = index + 1,
                TotalWeightedScore = r.Score,
                ScorePercentage = r.Percentage
            })
            .ToList();
    }
}
