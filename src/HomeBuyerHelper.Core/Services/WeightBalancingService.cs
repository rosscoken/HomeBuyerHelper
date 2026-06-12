
using HomeBuyerHelper.Core.Models;

namespace HomeBuyerHelper.Core.Services;
/// <summary>
/// Service for balancing criterion weights to ensure they sum to 100%.
/// </summary>
public class WeightBalancingService : IWeightBalancingService
{
    /// <summary>
    /// The warning threshold for a single criterion weight (40%).
    /// </summary>
    public const int WarningThreshold = 40;

    /// <summary>
    /// Rebalances weights so they sum to 100, respecting locked weights.
    /// </summary>
    public WeightBalanceResult Rebalance(IList<EvaluationCriterion> criteria, int? changedCriterionId = null)
    {
        if (criteria.Count == 0)
        {
            return new WeightBalanceResult { Success = true };
        }

        var result = new WeightBalanceResult { Success = true };
        var totalWeight = criteria.Sum(c => c.Weight);

        // Check for high weight warnings
        var highWeightCriteria = criteria.Where(c => c.Weight > WarningThreshold);
        foreach (var criterion in highWeightCriteria)
        {
            result.Warnings.Add($"'{criterion.Name}' has a weight of {criterion.Weight}%, which exceeds the recommended maximum of {WarningThreshold}%.");
        }

        // If already balanced, return early
        if (totalWeight == 100)
        {
            return result;
        }

        var lockedCriteria = criteria.Where(c => c.IsWeightLocked).ToList();
        var unlockedCriteria = criteria.Where(c => !c.IsWeightLocked).ToList();

        var lockedTotal = lockedCriteria.Sum(c => c.Weight);

        // If locked weights exceed 100%, we cannot balance
        if (lockedTotal >= 100)
        {
            result.Success = false;
            result.Warnings.Add("Locked weights exceed 100%. Unlock some criteria to rebalance.");
            return result;
        }

        // No unlocked criteria to adjust
        if (unlockedCriteria.Count == 0)
        {
            result.Success = false;
            result.Warnings.Add("All criteria are locked. Unlock some to rebalance weights.");
            return result;
        }

        var remainingWeight = 100 - lockedTotal;
        var unlockedTotal = unlockedCriteria.Sum(c => c.Weight);

        if (unlockedTotal == 0)
        {
            // Distribute remaining weight equally among unlocked criteria
            var equalWeight = remainingWeight / unlockedCriteria.Count;
            var remainder = remainingWeight % unlockedCriteria.Count;

            for (int i = 0; i < unlockedCriteria.Count; i++)
            {
                unlockedCriteria[i].Weight = equalWeight + (i < remainder ? 1 : 0);
            }
        }
        else
        {
            // Proportionally adjust unlocked weights
            var scaleFactor = (decimal)remainingWeight / unlockedTotal;

            int distributedWeight = 0;
            for (int i = 0; i < unlockedCriteria.Count - 1; i++)
            {
                var newWeight = (int)Math.Round(unlockedCriteria[i].Weight * scaleFactor);
                newWeight = Math.Max(1, newWeight); // Ensure minimum weight of 1
                unlockedCriteria[i].Weight = newWeight;
                distributedWeight += newWeight;
            }

            // Assign remainder to last criterion to ensure exact sum of 100
            var lastWeight = remainingWeight - distributedWeight;
            unlockedCriteria[^1].Weight = Math.Max(1, lastWeight);
        }

        return result;
    }

    /// <summary>
    /// Normalizes weights to sum to 100 using proportional scaling.
    /// Does not respect locks - used for initial setup.
    /// </summary>
    public void NormalizeToPercent(IList<EvaluationCriterion> criteria)
    {
        if (criteria.Count == 0)
        {
            return;
        }

        var totalWeight = criteria.Sum(c => c.Weight);
        if (totalWeight == 0)
        {
            // Distribute equally
            var equalWeight = 100 / criteria.Count;
            var remainder = 100 % criteria.Count;
            for (int i = 0; i < criteria.Count; i++)
            {
                criteria[i].Weight = equalWeight + (i < remainder ? 1 : 0);
            }
            return;
        }

        var scaleFactor = 100.0m / totalWeight;
        int distributedWeight = 0;

        for (int i = 0; i < criteria.Count - 1; i++)
        {
            var newWeight = (int)Math.Round(criteria[i].Weight * scaleFactor);
            newWeight = Math.Max(1, newWeight);
            criteria[i].Weight = newWeight;
            distributedWeight += newWeight;
        }

        criteria[^1].Weight = Math.Max(1, 100 - distributedWeight);
    }

    /// <summary>
    /// Calculates suggested weights based on ranking position.
    /// </summary>
    public void ApplyRankingWeights(IList<EvaluationCriterion> criteria)
    {
        if (criteria.Count == 0)
        {
            return;
        }

        // Ranking formula: #1 = 25%, #2 = 20%, descending from there
        var weights = new[] { 25, 20, 15, 12, 10, 8, 5, 3, 2 };
        int totalAssigned = 0;

        for (int i = 0; i < criteria.Count; i++)
        {
            if (i < weights.Length)
            {
                criteria[i].Weight = weights[i];
                totalAssigned += weights[i];
            }
            else
            {
                criteria[i].Weight = 1;
                totalAssigned += 1;
            }
        }

        // Adjust to sum to 100
        if (totalAssigned != 100)
        {
            NormalizeToPercent(criteria);
        }
    }
}

/// <summary>
/// Result of a weight balancing operation.
/// </summary>
public class WeightBalanceResult
{
    public bool Success { get; set; }
    public List<string> Warnings { get; } = new();
}
