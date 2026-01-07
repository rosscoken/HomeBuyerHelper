namespace HomeBuyerHelper.Core.Services;

using HomeBuyerHelper.Core.Models;

/// <summary>
/// Service for balancing criterion weights.
/// </summary>
public interface IWeightBalancingService
{
    /// <summary>
    /// Rebalances weights so they sum to 100, respecting locked weights.
    /// </summary>
    WeightBalanceResult Rebalance(IList<EvaluationCriterion> criteria, int? changedCriterionId = null);

    /// <summary>
    /// Normalizes weights to sum to 100 using proportional scaling.
    /// </summary>
    void NormalizeToPercent(IList<EvaluationCriterion> criteria);

    /// <summary>
    /// Applies ranking-based weights to criteria.
    /// </summary>
    void ApplyRankingWeights(IList<EvaluationCriterion> criteria);
}
