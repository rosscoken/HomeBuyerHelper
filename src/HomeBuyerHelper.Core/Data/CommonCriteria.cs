using HomeBuyerHelper.Core.Models;

namespace HomeBuyerHelper.Core.Data;

/// <summary>
/// Common criteria suggestions and templates.
/// </summary>
public static class CommonCriteria
{
    /// <summary>
    /// Location priority options for onboarding.
    /// </summary>
    public static readonly List<PriorityOption> LocationPriorities = new()
    {
        new("ShortCommute", "Short commute to work", CriterionCategory.Location),
        new("GoodSchools", "Good schools nearby", CriterionCategory.Location),
        new("Walkable", "Walkable neighborhood", CriterionCategory.Location),
        new("PeaceAndQuiet", "Peace and quiet", CriterionCategory.Neighborhood),
        new("NearFamily", "Near family/friends", CriterionCategory.Location),
        new("OutdoorAccess", "Access to outdoor activities", CriterionCategory.Lifestyle),
        new("Nightlife", "Nightlife and entertainment", CriterionCategory.Lifestyle),
        new("LowCost", "Low cost of living", CriterionCategory.Financial)
    };

    /// <summary>
    /// Home priority options for onboarding.
    /// </summary>
    public static readonly List<PriorityOption> HomePriorities = new()
    {
        new("MoveInReady", "Move-in ready condition", CriterionCategory.Interior),
        new("HomeOffice", "Space for home office(s)", CriterionCategory.Interior),
        new("OutdoorSpace", "Outdoor space (yard, patio, balcony)", CriterionCategory.Exterior),
        new("ModernKitchen", "Modern kitchen", CriterionCategory.Interior),
        new("NaturalLight", "Lots of natural light", CriterionCategory.Interior),
        new("Garage", "Garage or parking", CriterionCategory.Exterior),
        new("Storage", "Storage space", CriterionCategory.Interior),
        new("GuestSpace", "Guest accommodations", CriterionCategory.Interior),
        new("LowMaintenance", "Low maintenance", CriterionCategory.Exterior)
    };

    /// <summary>
    /// All common criteria organized by category.
    /// </summary>
    public static readonly Dictionary<CriterionCategory, List<CriterionTemplate>> AllCriteria = new()
    {
        [CriterionCategory.Location] = new List<CriterionTemplate>
        {
            new("Commute Time", "Time to get to work/school", "2+ hours each way", "Under 15 minutes"),
            new("Public Transit", "Access to buses, trains, metro", "No transit options", "Multiple lines within walking distance"),
            new("Neighborhood Safety", "Crime rates and general safety", "High crime area", "Very safe, low crime"),
            new("Walkability", "Ability to walk to shops, restaurants", "Car required for everything", "Walk to all daily needs"),
            new("School Quality", "Quality of nearby schools", "Low-rated schools", "Top-rated schools"),
            new("Proximity to Family", "Distance to family and friends", "Hours away", "Very close by")
        },
        [CriterionCategory.Neighborhood] = new List<CriterionTemplate>
        {
            new("Noise Level", "General noise from traffic, neighbors", "Very noisy area", "Quiet and peaceful"),
            new("Community Feel", "Sense of community, friendly neighbors", "Isolated, no community", "Strong, welcoming community"),
            new("Property Values", "Historical appreciation of area", "Declining values", "Strong appreciation"),
            new("Future Development", "Planned construction or changes", "Major disruptive projects", "Stable or positive development")
        },
        [CriterionCategory.Interior] = new List<CriterionTemplate>
        {
            new("Kitchen Quality", "Condition and features of kitchen", "Outdated, needs full renovation", "Modern, fully updated"),
            new("Bathroom Quality", "Condition of bathrooms", "Outdated, needs work", "Modern, well-maintained"),
            new("Living Space", "Size and layout of living areas", "Cramped, poor layout", "Spacious, open layout"),
            new("Bedroom Size", "Size of bedrooms", "Small, cramped bedrooms", "Large, comfortable bedrooms"),
            new("Storage Space", "Closets, cabinets, attic, basement", "Almost no storage", "Abundant storage"),
            new("Natural Light", "Windows and brightness", "Dark, few windows", "Bright, lots of natural light"),
            new("Home Office Potential", "Space for working from home", "No suitable space", "Dedicated office or perfect setup"),
            new("Overall Condition", "General maintenance and updates", "Needs major work", "Move-in ready, well-maintained")
        },
        [CriterionCategory.Exterior] = new List<CriterionTemplate>
        {
            new("Curb Appeal", "First impression and exterior look", "Needs work, unappealing", "Beautiful, well-maintained"),
            new("Yard Size", "Outdoor space for activities", "No yard", "Large, usable yard"),
            new("Outdoor Living", "Deck, patio, or outdoor entertaining space", "No outdoor space", "Great deck/patio setup"),
            new("Garage/Parking", "Covered parking and storage", "Street parking only", "Multiple car garage"),
            new("Landscaping", "Trees, gardens, lawn condition", "Bare or overgrown", "Beautifully landscaped"),
            new("Roof/Siding Condition", "Exterior structural condition", "Needs replacement", "New or excellent condition")
        },
        [CriterionCategory.Financial] = new List<CriterionTemplate>
        {
            new("Price vs. Budget", "How price fits your budget", "Significantly over budget", "Well under budget"),
            new("HOA Fees", "Monthly HOA costs", "Very high fees", "Low or no fees"),
            new("Property Taxes", "Annual tax burden", "Very high taxes", "Low property taxes"),
            new("Utility Costs", "Expected monthly utilities", "Expensive to heat/cool", "Efficient, low utility costs"),
            new("Insurance Costs", "Home insurance rates", "High-risk area, expensive", "Low insurance costs"),
            new("Renovation Needs", "Money needed for updates", "Major renovation required", "No updates needed")
        },
        [CriterionCategory.Lifestyle] = new List<CriterionTemplate>
        {
            new("Pet Friendliness", "Suitability for pets", "Not pet-friendly", "Perfect for pets"),
            new("Entertainment Access", "Restaurants, bars, activities", "Nothing nearby", "Great entertainment options"),
            new("Outdoor Recreation", "Parks, trails, recreation", "No outdoor recreation", "Excellent outdoor access"),
            new("Shopping Access", "Grocery, retail proximity", "Long drive to shops", "Shopping within walking distance"),
            new("Healthcare Access", "Hospitals, clinics nearby", "Far from healthcare", "Close to medical facilities"),
            new("Airport Access", "Distance to airport if needed", "Hours from airport", "Very close to airport")
        }
    };

    /// <summary>
    /// Starter template sets for quick onboarding.
    /// </summary>
    public static readonly Dictionary<string, List<CriterionTemplate>> Templates = new()
    {
        ["FirstTimeBuyer"] = new List<CriterionTemplate>
        {
            new("Price vs. Budget", "Staying within your first-home budget", "Significantly over budget", "Well under budget") { SuggestedWeight = 20 },
            new("Commute Time", "Distance to work", "2+ hours each way", "Under 15 minutes") { SuggestedWeight = 15 },
            new("Neighborhood Safety", "Safety of the area", "High crime area", "Very safe, low crime") { SuggestedWeight = 15 },
            new("Overall Condition", "Move-in readiness", "Needs major work", "Move-in ready") { SuggestedWeight = 15 },
            new("Kitchen Quality", "Kitchen condition and features", "Outdated, needs work", "Modern, updated") { SuggestedWeight = 10 },
            new("Living Space", "Size of living areas", "Cramped", "Spacious") { SuggestedWeight = 10 },
            new("Natural Light", "Brightness and windows", "Dark, few windows", "Bright, lots of light") { SuggestedWeight = 10 },
            new("Storage Space", "Closets and storage", "Almost no storage", "Abundant storage") { SuggestedWeight = 5 }
        },
        ["FamilyFocused"] = new List<CriterionTemplate>
        {
            new("School Quality", "Quality of nearby schools", "Low-rated schools", "Top-rated schools") { SuggestedWeight = 20 },
            new("Neighborhood Safety", "Safety for children", "High crime area", "Very safe") { SuggestedWeight = 15 },
            new("Yard Size", "Play space for kids", "No yard", "Large, usable yard") { SuggestedWeight = 15 },
            new("Bedroom Size", "Space for family", "Small bedrooms", "Large bedrooms") { SuggestedWeight = 10 },
            new("Living Space", "Family gathering space", "Cramped", "Open, spacious") { SuggestedWeight = 10 },
            new("Storage Space", "Room for family stuff", "No storage", "Abundant storage") { SuggestedWeight = 10 },
            new("Community Feel", "Family-friendly neighborhood", "Isolated", "Strong community") { SuggestedWeight = 10 },
            new("Overall Condition", "Move-in ready for family", "Needs work", "Ready to move in") { SuggestedWeight = 10 }
        },
        ["InvestmentFocused"] = new List<CriterionTemplate>
        {
            new("Property Values", "Area appreciation potential", "Declining values", "Strong appreciation") { SuggestedWeight = 20 },
            new("Price vs. Budget", "Value relative to comparables", "Overpriced", "Good deal") { SuggestedWeight = 20 },
            new("Renovation Needs", "Work needed for value-add", "Too much work", "Cosmetic updates only") { SuggestedWeight = 15 },
            new("Rental Potential", "Income if rented", "Low rent area", "High rent area") { SuggestedWeight = 15 },
            new("Location Desirability", "Tenant/buyer appeal", "Undesirable", "Highly desirable") { SuggestedWeight = 15 },
            new("Overall Condition", "Current state of property", "Major repairs", "Good condition") { SuggestedWeight = 10 },
            new("Future Development", "Area growth potential", "Stagnant", "Growing area") { SuggestedWeight = 5 }
        },
        ["RemoteWorker"] = new List<CriterionTemplate>
        {
            new("Home Office Potential", "Space for dedicated office", "No suitable space", "Perfect office setup") { SuggestedWeight = 20 },
            new("Internet Quality", "Reliable high-speed internet", "Slow/unreliable", "Fast fiber available") { SuggestedWeight = 15 },
            new("Natural Light", "Good lighting for video calls", "Dark rooms", "Bright, well-lit") { SuggestedWeight = 10 },
            new("Noise Level", "Quiet for meetings", "Very noisy", "Quiet and peaceful") { SuggestedWeight = 15 },
            new("Living Space", "Room to spread out", "Cramped", "Spacious layout") { SuggestedWeight = 10 },
            new("Outdoor Space", "Break time outdoor access", "No outdoor space", "Nice outdoor area") { SuggestedWeight = 10 },
            new("Walkability", "Access to coffee shops, etc.", "Nothing nearby", "Great walkability") { SuggestedWeight = 10 },
            new("Overall Condition", "Move-in readiness", "Needs work", "Ready to go") { SuggestedWeight = 10 }
        },
        ["UrbanCondo"] = new List<CriterionTemplate>
        {
            new("Walkability", "Daily errands without a car", "Car required for everything", "Walk score 90+") { SuggestedWeight = 20 },
            new("Transit Access", "Subway, bus, light rail nearby", "No transit options", "Steps from a major line") { SuggestedWeight = 15 },
            new("Building Amenities", "Gym, roof deck, doorman, package room", "No amenities", "Full-service building") { SuggestedWeight = 10 },
            new("HOA Fees", "Monthly condo fees vs. value", "High fees, little value", "Reasonable fees, great services") { SuggestedWeight = 15 },
            new("Noise Level", "Street and neighbor noise", "Constant noise", "Surprisingly quiet") { SuggestedWeight = 10 },
            new("Natural Light", "Windows and exposure", "Dark unit", "Corner unit, light all day") { SuggestedWeight = 10 },
            new("Entertainment Access", "Restaurants and nightlife", "Nothing nearby", "Best blocks in town") { SuggestedWeight = 10 },
            new("Price vs. Budget", "Value for the neighborhood", "Overpriced", "Under market") { SuggestedWeight = 10 }
        },
        ["SuburbanFamily"] = new List<CriterionTemplate>
        {
            new("School Quality", "District ratings and proximity", "Low-rated schools", "Top-rated schools") { SuggestedWeight = 25 },
            new("Yard Size", "Outdoor play and entertaining space", "No yard", "Large flat yard") { SuggestedWeight = 15 },
            new("Neighborhood Safety", "Traffic and crime", "Busy road, high crime", "Quiet cul-de-sac") { SuggestedWeight = 15 },
            new("Community Feel", "Neighbors, events, kid density", "No community", "Block parties and sidewalks") { SuggestedWeight = 10 },
            new("Commute Time", "Drive to work and school", "Over an hour", "Under 20 minutes") { SuggestedWeight = 10 },
            new("Living Space", "Room for the whole family", "Cramped", "Room to grow") { SuggestedWeight = 10 },
            new("Storage Space", "Garage, basement, closets", "No storage", "Abundant storage") { SuggestedWeight = 5 },
            new("Price vs. Budget", "Affordability", "Stretching too far", "Comfortable") { SuggestedWeight = 10 }
        },
        ["RuralRetreat"] = new List<CriterionTemplate>
        {
            new("Land & Acreage", "Usable land for your plans", "Tiny lot", "Acres of usable land") { SuggestedWeight = 20 },
            new("Privacy", "Distance from neighbors", "Neighbors on top of you", "Can't see another house") { SuggestedWeight = 15 },
            new("Internet Quality", "Connectivity options out here", "No broadband at all", "Fiber or strong fixed wireless") { SuggestedWeight = 15 },
            new("Well & Septic Condition", "Private utility systems", "Failing systems", "New, well-maintained") { SuggestedWeight = 15 },
            new("Self-Sufficiency Potential", "Garden, livestock, solar potential", "Not feasible", "Ready homestead") { SuggestedWeight = 10 },
            new("Access & Roads", "Year-round road access", "Impassable in winter", "Paved, maintained road") { SuggestedWeight = 10 },
            new("Distance to Services", "Groceries, hospital, schools", "Over an hour", "Under 20 minutes") { SuggestedWeight = 10 },
            new("Price vs. Budget", "Value for land and home", "Overpriced", "Great value") { SuggestedWeight = 5 }
        },
        ["Downsizer"] = new List<CriterionTemplate>
        {
            new("Single-Level Living", "Bedroom and bath on main floor", "Stairs everywhere", "True one-level living") { SuggestedWeight = 20 },
            new("Low Maintenance", "Yard and exterior upkeep", "High-maintenance property", "Lock-and-leave easy") { SuggestedWeight = 20 },
            new("Accessibility", "Aging-in-place readiness", "Many barriers", "Wide doors, walk-in shower") { SuggestedWeight = 15 },
            new("Healthcare Access", "Doctors and hospital proximity", "Far from healthcare", "Minutes from providers") { SuggestedWeight = 10 },
            new("Right-Sized Space", "Enough room without excess", "Still too big/small", "Just right") { SuggestedWeight = 10 },
            new("Community Feel", "Social opportunities nearby", "Isolated", "Active, welcoming community") { SuggestedWeight = 10 },
            new("Proximity to Family", "Distance to kids/grandkids", "A flight away", "Short drive") { SuggestedWeight = 10 },
            new("Price vs. Budget", "Frees up retirement equity", "Costs more than current home", "Significant equity freed") { SuggestedWeight = 5 }
        },
        ["MultiGenerational"] = new List<CriterionTemplate>
        {
            new("Separate Living Spaces", "In-law suite or separate level", "One shared space", "True separate suite") { SuggestedWeight = 20 },
            new("Separate Entrances", "Independent comings and goings", "Single shared entrance", "Private entrances") { SuggestedWeight = 15 },
            new("Bathroom Count", "Enough baths for everyone", "One bathroom", "Bath per household unit") { SuggestedWeight = 15 },
            new("Kitchen Flexibility", "Second kitchen or kitchenette potential", "No option", "Second kitchen exists") { SuggestedWeight = 10 },
            new("Privacy", "Sound separation between living areas", "Hear everything", "Well-isolated spaces") { SuggestedWeight = 15 },
            new("Accessibility", "Suitability for older family members", "Stairs only", "Accessible throughout") { SuggestedWeight = 10 },
            new("Living Space", "Total room for everyone", "Too tight", "Comfortable for all") { SuggestedWeight = 10 },
            new("Price vs. Budget", "Shared affordability", "Stretching everyone", "Comfortable when shared") { SuggestedWeight = 5 }
        }
    };

    /// <summary>
    /// Maps priority selections to suggested criteria.
    /// </summary>
    public static List<CriterionTemplate> GetSuggestedCriteria(
        BuyingSituation? situation,
        HouseholdType? household,
        WorkArrangement? work,
        PetType pets,
        List<string> locationPriorities,
        List<string> homePriorities)
    {
        var suggestions = new List<CriterionTemplate>();
        var addedNames = new HashSet<string>();

        void AddIfNew(CriterionTemplate template, string reason)
        {
            if (!addedNames.Contains(template.Name))
            {
                template.SuggestionReason = reason;
                suggestions.Add(template);
                addedNames.Add(template.Name);
            }
        }

        // Situation-based suggestions
        if (situation == BuyingSituation.FirstHome)
        {
            AddIfNew(new CriterionTemplate("Price vs. Budget", "Staying within budget", "Over budget", "Under budget") { SuggestedWeight = 20 },
                "First-time buyers often have strict budgets");
            AddIfNew(new CriterionTemplate("Overall Condition", "Move-in readiness", "Needs work", "Ready") { SuggestedWeight = 15 },
                "First homes often need to be move-in ready");
        }
        else if (situation == BuyingSituation.InvestmentProperty)
        {
            AddIfNew(new CriterionTemplate("Property Values", "Appreciation potential", "Declining", "Growing") { SuggestedWeight = 20 },
                "Investment properties need appreciation potential");
        }

        // Household-based suggestions
        if (household == HouseholdType.FamilyWithKids)
        {
            AddIfNew(new CriterionTemplate("School Quality", "School district quality", "Low-rated", "Top-rated") { SuggestedWeight = 18 },
                "You have children who will attend local schools");
            AddIfNew(new CriterionTemplate("Yard Size", "Outdoor play space", "No yard", "Large yard") { SuggestedWeight = 12 },
                "Kids benefit from outdoor play space");
            AddIfNew(new CriterionTemplate("Neighborhood Safety", "Safety for family", "Unsafe", "Very safe") { SuggestedWeight = 15 },
                "Safety is important for families");
        }

        // Work-based suggestions
        if (work == WorkArrangement.FullyRemote)
        {
            AddIfNew(new CriterionTemplate("Home Office Potential", "Space for working from home", "No space", "Perfect office") { SuggestedWeight = 18 },
                "You work from home full-time");
        }
        else if (work == WorkArrangement.FullyOnsite || work == WorkArrangement.Hybrid)
        {
            AddIfNew(new CriterionTemplate("Commute Time", "Time to workplace", "Long commute", "Short commute") { SuggestedWeight = 15 },
                "You commute to work regularly");
        }

        // Pet-based suggestions
        if ((pets & PetType.Dogs) != 0)
        {
            AddIfNew(new CriterionTemplate("Yard Size", "Space for dogs", "No yard", "Large yard") { SuggestedWeight = 12 },
                "Dogs need outdoor space");
            AddIfNew(new CriterionTemplate("Pet Friendliness", "Pet-friendly area", "Not friendly", "Very friendly") { SuggestedWeight = 8 },
                "You have pets");
        }

        // Location priority mappings
        foreach (var priority in locationPriorities)
        {
            var template = priority switch
            {
                "ShortCommute" => new CriterionTemplate("Commute Time", "Distance to work", "Long", "Short") { SuggestedWeight = 15 },
                "GoodSchools" => new CriterionTemplate("School Quality", "School ratings", "Low", "High") { SuggestedWeight = 15 },
                "Walkable" => new CriterionTemplate("Walkability", "Walk to amenities", "Nothing", "Everything") { SuggestedWeight = 12 },
                "PeaceAndQuiet" => new CriterionTemplate("Noise Level", "Area noise", "Loud", "Quiet") { SuggestedWeight = 12 },
                "NearFamily" => new CriterionTemplate("Proximity to Family", "Distance to family", "Far", "Close") { SuggestedWeight = 10 },
                "OutdoorAccess" => new CriterionTemplate("Outdoor Recreation", "Parks and trails", "None", "Abundant") { SuggestedWeight = 10 },
                "Nightlife" => new CriterionTemplate("Entertainment Access", "Restaurants and bars", "None", "Many") { SuggestedWeight = 10 },
                "LowCost" => new CriterionTemplate("Property Taxes", "Tax burden", "High", "Low") { SuggestedWeight = 10 },
                _ => null
            };
            if (template != null)
            {
                AddIfNew(template, $"You selected '{LocationPriorities.First(p => p.Key == priority).DisplayName}'");
            }
        }

        // Home priority mappings
        foreach (var priority in homePriorities)
        {
            var template = priority switch
            {
                "MoveInReady" => new CriterionTemplate("Overall Condition", "Move-in readiness", "Needs work", "Ready") { SuggestedWeight = 15 },
                "HomeOffice" => new CriterionTemplate("Home Office Potential", "Office space", "None", "Perfect") { SuggestedWeight = 12 },
                "OutdoorSpace" => new CriterionTemplate("Outdoor Living", "Deck/patio/yard", "None", "Great") { SuggestedWeight = 12 },
                "ModernKitchen" => new CriterionTemplate("Kitchen Quality", "Kitchen condition", "Outdated", "Modern") { SuggestedWeight = 12 },
                "NaturalLight" => new CriterionTemplate("Natural Light", "Windows and light", "Dark", "Bright") { SuggestedWeight = 10 },
                "Garage" => new CriterionTemplate("Garage/Parking", "Parking situation", "Street only", "Garage") { SuggestedWeight = 10 },
                "Storage" => new CriterionTemplate("Storage Space", "Closets and storage", "None", "Lots") { SuggestedWeight = 8 },
                "GuestSpace" => new CriterionTemplate("Living Space", "Extra room for guests", "None", "Guest suite") { SuggestedWeight = 8 },
                "LowMaintenance" => new CriterionTemplate("Landscaping", "Yard maintenance", "High", "Low") { SuggestedWeight = 8 },
                _ => null
            };
            if (template != null)
            {
                AddIfNew(template, $"You selected '{HomePriorities.First(p => p.Key == priority).DisplayName}'");
            }
        }

        // Normalize weights to sum to 100
        if (suggestions.Count > 0)
        {
            var totalWeight = suggestions.Sum(s => s.SuggestedWeight);
            if (totalWeight > 0)
            {
                foreach (var suggestion in suggestions)
                {
                    suggestion.SuggestedWeight = Math.Round(suggestion.SuggestedWeight * 100 / totalWeight, 0);
                }
                // Adjust rounding errors
                var diff = 100 - suggestions.Sum(s => s.SuggestedWeight);
                suggestions.First().SuggestedWeight += diff;
            }
        }

        return suggestions;
    }
}

/// <summary>
/// A priority option shown during onboarding.
/// </summary>
public class PriorityOption
{
    public string Key { get; }
    public string DisplayName { get; }
    public CriterionCategory Category { get; }

    public PriorityOption(string key, string displayName, CriterionCategory category)
    {
        Key = key;
        DisplayName = displayName;
        Category = category;
    }
}

/// <summary>
/// A template for creating evaluation criteria.
/// </summary>
public class CriterionTemplate
{
    public string Name { get; }
    public string Description { get; }
    public string ScoreAnchorLow { get; }
    public string ScoreAnchorHigh { get; }
    public decimal SuggestedWeight { get; set; }
    public string? SuggestionReason { get; set; }

    public CriterionTemplate(string name, string description, string lowAnchor, string highAnchor)
    {
        Name = name;
        Description = description;
        ScoreAnchorLow = lowAnchor;
        ScoreAnchorHigh = highAnchor;
        SuggestedWeight = 10; // Default weight
    }
}
