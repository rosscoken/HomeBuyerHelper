using FluentAssertions;
using HomeBuyerHelper.Core.Models;
using HomeBuyerHelper.Core.Services;
using Xunit;

namespace HomeBuyerHelper.Core.Tests;

/// <summary>
/// Tests for CriteriaTemplateService (P4-TMP-001/002).
/// </summary>
public class CriteriaTemplateServiceTests
{
    private readonly CriteriaTemplateService _service = new();

    private static EvaluationCriterion[] SampleCriteria => new[]
    {
        new EvaluationCriterion
        {
            Name = "Home Office Space",
            Description = "Dedicated room for work",
            Weight = 25,
            Category = CriterionCategory.Interior,
            ScoreAnchorLow = "No dedicated space",
            ScoreAnchorHigh = "Dedicated room with good light"
        },
        new EvaluationCriterion
        {
            Name = "Commute Time",
            Weight = 75,
            Category = CriterionCategory.Location
        }
    };

    [Fact]
    public void Export_ThenParse_RoundTrips()
    {
        var json = _service.ExportTemplate("Remote Worker Criteria", "For hybrid workers", SampleCriteria);

        var template = _service.ParseTemplate(json);

        template.Name.Should().Be("Remote Worker Criteria");
        template.Version.Should().Be("1.0");
        template.Criteria.Should().HaveCount(2);
        template.Criteria[0].Name.Should().Be("Home Office Space");
        template.Criteria[0].Weight.Should().Be(25);
        template.Criteria[0].ScoreAnchorHigh.Should().Be("Dedicated room with good light");
    }

    [Fact]
    public void Export_ContainsNoPersonalData()
    {
        var json = _service.ExportTemplate("Template", null, SampleCriteria);

        // Template schema has only criteria definition fields.
        json.Should().NotContain("propertyId");
        json.Should().NotContain("score\"");
        json.Should().NotContain("askingPrice");
    }

    [Fact]
    public void ToCriteria_MapsCategoriesAndOrder()
    {
        var json = _service.ExportTemplate("Template", null, SampleCriteria);
        var template = _service.ParseTemplate(json);

        var criteria = _service.ToCriteria(template);

        criteria.Should().HaveCount(2);
        criteria[0].Category.Should().Be(CriterionCategory.Interior);
        criteria[0].DisplayOrder.Should().Be(0);
        criteria[1].Category.Should().Be(CriterionCategory.Location);
        criteria[1].DisplayOrder.Should().Be(1);
        criteria.Should().AllSatisfy(c => c.IsSystemSuggested.Should().BeFalse());
    }

    [Fact]
    public void Parse_MalformedJson_ThrowsReadableError()
    {
        var act = () => _service.ParseTemplate("{ not json");

        act.Should().Throw<InvalidDataException>()
            .WithMessage("*not a valid criteria template*");
    }

    [Fact]
    public void Parse_WrongVersion_Throws()
    {
        var json = """{"version":"9.9","name":"X","criteria":[{"name":"A","weight":10}]}""";

        var act = () => _service.ParseTemplate(json);

        act.Should().Throw<InvalidDataException>()
            .WithMessage("*version*");
    }

    [Fact]
    public void Parse_EmptyCriteria_Throws()
    {
        var json = """{"version":"1.0","name":"X","criteria":[]}""";

        var act = () => _service.ParseTemplate(json);

        act.Should().Throw<InvalidDataException>()
            .WithMessage("*no criteria*");
    }

    [Fact]
    public void Parse_InvalidWeight_Throws()
    {
        var json = """{"version":"1.0","name":"X","criteria":[{"name":"A","weight":0}]}""";

        var act = () => _service.ParseTemplate(json);

        act.Should().Throw<InvalidDataException>()
            .WithMessage("*positive weight*");
    }

    [Fact]
    public void Parse_UnknownCategory_FallsBackToOther()
    {
        var json = """{"version":"1.0","name":"X","criteria":[{"name":"A","weight":10,"category":"Bogus"}]}""";

        var template = _service.ParseTemplate(json);
        var criteria = _service.ToCriteria(template);

        criteria[0].Category.Should().Be(CriterionCategory.Other);
    }

    [Fact]
    public void BuiltInTemplateLibrary_HasExpandedTemplates()
    {
        var templates = HomeBuyerHelper.Core.Data.CommonCriteria.Templates;

        templates.Keys.Should().Contain(new[]
        {
            "FirstTimeBuyer", "FamilyFocused", "InvestmentFocused", "RemoteWorker",
            "UrbanCondo", "SuburbanFamily", "RuralRetreat", "Downsizer", "MultiGenerational"
        });

        // Every template's suggested weights should total 100%.
        foreach (var (name, criteria) in templates)
        {
            criteria.Sum(c => c.SuggestedWeight).Should().Be(100, $"template '{name}' weights should total 100");
        }
    }
}
