# 08 - Accessibility Guidelines

This document defines accessibility requirements, implementation patterns, and testing procedures to ensure HomeBuyerHelper is usable by everyone.

---

## Accessibility Commitment

HomeBuyerHelper is committed to WCAG 2.1 Level AA compliance across all platforms. Home buying is a significant life decision, and everyone deserves equal access to tools that help them make informed choices.

---

## WCAG 2.1 AA Requirements

### 1. Perceivable

**1.1 Text Alternatives**
- [ ] All images have meaningful alt text
- [ ] Icons have accessible names
- [ ] Charts have text summaries

**1.2 Time-based Media**
- N/A (no video/audio content)

**1.3 Adaptable**
- [ ] Content structure is semantic
- [ ] Reading order is logical
- [ ] Orientation is not restricted

**1.4 Distinguishable**
- [ ] Color is not the only indicator
- [ ] Contrast ratio ≥ 4.5:1 for text
- [ ] Contrast ratio ≥ 3:1 for UI components
- [ ] Text can be resized to 200%
- [ ] Content reflows at 320px width

---

### 2. Operable

**2.1 Keyboard Accessible**
- [ ] All functions available via keyboard (desktop)
- [ ] No keyboard traps
- [ ] Focus order is logical

**2.2 Enough Time**
- [ ] No time limits (none used in app)
- [ ] Auto-updating content can be paused

**2.3 Seizures**
- [ ] No content flashes more than 3 times/second

**2.4 Navigable**
- [ ] Skip links provided (if applicable)
- [ ] Page titles are descriptive
- [ ] Focus is visible
- [ ] Link purpose is clear

**2.5 Input Modalities**
- [ ] Touch targets ≥ 44x44 points
- [ ] Gestures have alternatives
- [ ] Motion input not required

---

### 3. Understandable

**3.1 Readable**
- [ ] Language is set programmatically
- [ ] Jargon has definitions (tooltips/glossary)

**3.2 Predictable**
- [ ] Navigation is consistent
- [ ] Components behave consistently

**3.3 Input Assistance**
- [ ] Errors are identified
- [ ] Labels are provided
- [ ] Error suggestions given
- [ ] Error prevention for important actions

---

### 4. Robust

**4.1 Compatible**
- [ ] Valid markup
- [ ] Name, role, value set for all components
- [ ] Status messages accessible

---

## Platform-Specific Implementation

### iOS (VoiceOver)

**XAML Accessibility Attributes**:
```xml
<!-- Button with accessibility -->
<Button
    Text="Add Property"
    AutomationProperties.Name="Add new property"
    AutomationProperties.HelpText="Opens form to add a new property for evaluation"
    SemanticProperties.Hint="Double tap to activate" />

<!-- Image with description -->
<Image
    Source="house_icon.png"
    AutomationProperties.Name="Property photo"
    AutomationProperties.IsInAccessibleTree="True" />

<!-- Decorative image (excluded) -->
<Image
    Source="decorative_line.png"
    AutomationProperties.IsInAccessibleTree="False" />

<!-- Custom control with role -->
<ContentView
    AutomationProperties.Name="Property score: 8.5 out of 10"
    SemanticProperties.Description="Overall weighted score for this property">
    <!-- Score visualization -->
</ContentView>
```

**iOS-Specific Requirements**:
- [ ] VoiceOver navigation tested
- [ ] Rotor actions configured where helpful
- [ ] Dynamic Type supported (text scaling)
- [ ] Reduce Motion respected

---

### Android (TalkBack)

**Android Accessibility**:
```xml
<!-- In XAML, same attributes work -->
<Entry
    Placeholder="Property nickname"
    AutomationProperties.Name="Property nickname"
    AutomationProperties.HelpText="Enter a memorable name for this property" />

<!-- Content description for images -->
<Image
    Source="score_chart.png"
    AutomationProperties.Name="Score comparison chart showing 3 properties" />
```

**Android-Specific Requirements**:
- [ ] TalkBack navigation tested
- [ ] Touch exploration works correctly
- [ ] Font scaling respected
- [ ] High contrast mode tested

---

### Windows (Narrator)

**Windows Accessibility**:
```xml
<!-- MAUI handles most automatically -->
<Button
    Text="Generate Report"
    AutomationProperties.Name="Generate PDF report"
    AutomationProperties.AcceleratorKey="Ctrl+P" />

<!-- Landmark regions -->
<Grid AutomationProperties.Name="Main navigation">
    <!-- Navigation content -->
</Grid>
```

**Windows-Specific Requirements**:
- [ ] Narrator navigation tested
- [ ] Keyboard navigation complete
- [ ] High contrast themes work
- [ ] UI Automation tree is logical

---

### macOS (VoiceOver)

**macOS-Specific Requirements**:
- [ ] VoiceOver navigation tested
- [ ] Keyboard shortcuts documented
- [ ] Menu bar accessible
- [ ] Full keyboard access mode works

---

## Component Patterns

### Accessible Score Input

```xml
<!-- Score selection with accessibility -->
<StackLayout Orientation="Horizontal" Spacing="8">
    <Label Text="Score:" />
    <RadioButton
        Content="1"
        Value="1"
        AutomationProperties.Name="Score 1, Poor"
        GroupName="ScoreGroup" />
    <RadioButton
        Content="5"
        Value="5"
        AutomationProperties.Name="Score 5, Average"
        GroupName="ScoreGroup" />
    <RadioButton
        Content="10"
        Value="10"
        AutomationProperties.Name="Score 10, Excellent"
        GroupName="ScoreGroup" />
</StackLayout>

<!-- Alternative: Slider with value announcement -->
<Slider
    Minimum="1"
    Maximum="10"
    Value="{Binding Score}"
    AutomationProperties.Name="Property score"
    AutomationProperties.HelpText="Slide to select score from 1 to 10">
    <Slider.Behaviors>
        <behaviors:SliderValueAnnounceBehavior />
    </Slider.Behaviors>
</Slider>
```

---

### Accessible Data Table (Comparison Matrix)

```xml
<!-- CollectionView with table semantics -->
<CollectionView
    ItemsSource="{Binding ComparisonRows}"
    AutomationProperties.Name="Property comparison table"
    SemanticProperties.Description="Table comparing {PropertyCount} properties across {CriteriaCount} criteria">

    <CollectionView.ItemTemplate>
        <DataTemplate x:DataType="models:ComparisonRow">
            <Grid ColumnDefinitions="*,*,*,*">
                <!-- Row header (criterion name) -->
                <Label
                    Grid.Column="0"
                    Text="{Binding CriterionName}"
                    AutomationProperties.IsInAccessibleTree="True"
                    FontAttributes="Bold" />

                <!-- Property scores -->
                <Label
                    Grid.Column="1"
                    Text="{Binding Property1Score}"
                    AutomationProperties.Name="{Binding Property1AccessibleName}" />
                    <!-- e.g., "Blue House scored 8 on Kitchen Quality" -->
            </Grid>
        </DataTemplate>
    </CollectionView.ItemTemplate>
</CollectionView>
```

---

### Accessible Form

```xml
<VerticalStackLayout Spacing="16">
    <!-- Form field with label and error -->
    <VerticalStackLayout>
        <Label
            Text="Property Nickname"
            AutomationProperties.IsInAccessibleTree="False" />
        <Entry
            Text="{Binding Nickname}"
            AutomationProperties.Name="Property nickname, required"
            AutomationProperties.HelpText="Enter a memorable name like Blue House on Oak Street"
            ReturnType="Next" />
        <Label
            Text="{Binding NicknameError}"
            TextColor="Red"
            IsVisible="{Binding HasNicknameError}"
            AutomationProperties.Name="{Binding NicknameErrorAccessible}"
            SemanticProperties.Description="Error message" />
    </VerticalStackLayout>

    <!-- Currency input with format hint -->
    <VerticalStackLayout>
        <Label Text="Asking Price" />
        <Entry
            Text="{Binding PriceText}"
            Keyboard="Numeric"
            AutomationProperties.Name="Asking price in dollars"
            AutomationProperties.HelpText="Enter price without dollar sign or commas" />
    </VerticalStackLayout>
</VerticalStackLayout>
```

---

### Accessible Navigation

```xml
<!-- Tab bar with accessibility -->
<TabBar>
    <Tab
        Title="Properties"
        Icon="properties_icon.png"
        AutomationProperties.Name="Properties tab, shows your saved properties">
        <ShellContent ContentTemplate="{DataTemplate pages:PropertyListPage}" />
    </Tab>
    <Tab
        Title="Compare"
        Icon="compare_icon.png"
        AutomationProperties.Name="Compare tab, side by side property comparison">
        <ShellContent ContentTemplate="{DataTemplate pages:ComparisonPage}" />
    </Tab>
</TabBar>
```

---

## Color and Contrast

### Color Palette with Contrast

| Use Case | Light Mode | Dark Mode | Contrast (Light) | Contrast (Dark) |
|----------|------------|-----------|------------------|-----------------|
| Primary Text | #000000 | #FFFFFF | 21:1 | 21:1 |
| Secondary Text | #6B6B6B | #EBEBF5 | 5.9:1 | 4.6:1 |
| Link/Accent | #007AFF | #0A84FF | 4.5:1 | 4.5:1 |
| Success (text) | #248A3D | #32D74B | 4.5:1 | 4.7:1 |
| Warning (text) | #9A6700 | #FFD60A | 4.5:1 | 4.5:1 |
| Error (text) | #D70015 | #FF453A | 5.2:1 | 4.5:1 |

### Color-Independent Indicators

Always pair color with another indicator:

```xml
<!-- Score indicators use color + icon -->
<Grid ColumnDefinitions="Auto,*">
    <!-- Icon indicates status -->
    <Image
        Grid.Column="0"
        Source="{Binding ScoreIcon}"
        WidthRequest="24"
        HeightRequest="24" />

    <!-- Color reinforces but isn't sole indicator -->
    <Label
        Grid.Column="1"
        Text="{Binding ScoreText}"
        TextColor="{Binding ScoreColor}"
        AutomationProperties.Name="{Binding ScoreAccessibleText}" />
        <!-- e.g., "Kitchen Quality: 8 out of 10, Good" -->
</Grid>
```

**Icon Mapping**:
| Score Range | Icon | Color |
|-------------|------|-------|
| 8-10 | checkmark_circle | Green |
| 5-7 | minus_circle | Yellow |
| 1-4 | x_circle | Red |

---

## Touch Targets

### Minimum Size Requirements

```xml
<!-- Ensure minimum touch target -->
<Button
    Text="+"
    WidthRequest="44"
    HeightRequest="44"
    MinimumWidthRequest="44"
    MinimumHeightRequest="44"
    Padding="10"
    AutomationProperties.Name="Add criterion" />

<!-- For smaller visual elements, use padding -->
<Button
    Text="Edit"
    HeightRequest="44"
    Padding="16,0"
    AutomationProperties.Name="Edit property" />
```

### Spacing Between Targets

- Minimum 8pt spacing between interactive elements
- 16pt preferred for frequently used actions

---

## Screen Reader Testing Checklist

### Per-Page Testing

For each page, verify with screen reader:

1. **Page Title Announced**
   - [ ] Meaningful title read on page load
   - [ ] Title describes page purpose

2. **Logical Reading Order**
   - [ ] Content read in expected order
   - [ ] No hidden content read
   - [ ] Skip patterns available

3. **Interactive Elements**
   - [ ] All buttons/links reachable
   - [ ] Actions clearly described
   - [ ] State changes announced

4. **Forms**
   - [ ] Labels associated with inputs
   - [ ] Required fields indicated
   - [ ] Errors announced when occur
   - [ ] Success confirmed

5. **Dynamic Content**
   - [ ] Changes announced via LiveRegion
   - [ ] Loading states communicated
   - [ ] Results announced

### Testing Commands

**iOS VoiceOver**:
- Two-finger swipe down: Read all
- One-finger swipe: Navigate elements
- Double-tap: Activate

**Android TalkBack**:
- Explore by touch
- Swipe right/left: Navigate
- Double-tap: Activate

**Windows Narrator**:
- Caps Lock + Space: Scan mode
- Tab: Navigate interactive elements
- Enter/Space: Activate

---

## Automated Accessibility Testing

### Unit Test Accessibility

```csharp
public class AccessibilityTests
{
    [Fact]
    public void PropertyCard_HasRequiredAccessibility()
    {
        // Arrange
        var card = new PropertyCard();
        card.BindingContext = new Property { Nickname = "Test" };

        // Assert
        card.GetValue(AutomationProperties.NameProperty)
            .Should().NotBeNullOrEmpty();
    }

    [Theory]
    [InlineData(typeof(PropertyListPage))]
    [InlineData(typeof(ComparisonPage))]
    [InlineData(typeof(SettingsPage))]
    public void Page_AllButtonsHaveAccessibilityNames(Type pageType)
    {
        // Arrange
        var page = (ContentPage)Activator.CreateInstance(pageType)!;

        // Act
        var buttons = FindAllDescendants<Button>(page);

        // Assert
        foreach (var button in buttons)
        {
            var name = button.GetValue(AutomationProperties.NameProperty);
            name.Should().NotBeNullOrEmpty(
                $"Button '{button.Text}' missing AutomationProperties.Name");
        }
    }
}
```

### Accessibility Insights

Use [Accessibility Insights](https://accessibilityinsights.io/) for Windows testing:

1. Run FastPass for quick automated checks
2. Use Tab Stops tool for keyboard navigation
3. Color Contrast Analyzer for color checks
4. Complete Assessment for full WCAG audit

---

## Accessibility Documentation

### In-App Help

- [ ] Screen reader quick start guide
- [ ] Keyboard shortcuts reference
- [ ] Color customization options
- [ ] Text size adjustment instructions

### Developer Documentation

Each component should document:

```csharp
/// <summary>
/// Displays a property score with accessible announcements.
/// </summary>
/// <remarks>
/// Accessibility:
/// - AutomationProperties.Name: "[Criterion]: [Score] out of 10, [Level]"
/// - Color paired with icon indicator
/// - Touch target: 44x44pt minimum
/// </remarks>
public class ScoreIndicator : ContentView
{
    // Implementation
}
```

---

## Accessibility Review Sign-Off

### Phase Completion Accessibility Approval

```
## Accessibility Review Sign-Off

Phase: [1/2/3/4]
Date: [YYYY-MM-DD]
Reviewer: [Name]

### Manual Testing Completed
- [ ] VoiceOver (iOS)
- [ ] TalkBack (Android)
- [ ] Narrator (Windows)
- [ ] VoiceOver (macOS)

### Automated Testing
- [ ] Accessibility Insights FastPass: Pass
- [ ] Unit tests for AutomationProperties: Pass
- [ ] Contrast checker: All ratios ≥ 4.5:1

### Issues Found
| Issue | Severity | Status |
|-------|----------|--------|
| [Description] | [High/Medium/Low] | [Fixed/Deferred] |

### Approval
[x] Phase meets WCAG 2.1 AA requirements
[ ] Phase blocked - issues must be resolved
```

---

## Resources

- [WCAG 2.1 Guidelines](https://www.w3.org/WAI/WCAG21/quickref/)
- [.NET MAUI Accessibility](https://learn.microsoft.com/en-us/dotnet/maui/fundamentals/accessibility)
- [iOS Accessibility Guide](https://developer.apple.com/accessibility/)
- [Android Accessibility Guide](https://developer.android.com/guide/topics/ui/accessibility)
- [Windows Accessibility](https://docs.microsoft.com/en-us/windows/apps/accessibility)
- [Accessibility Insights](https://accessibilityinsights.io/)
