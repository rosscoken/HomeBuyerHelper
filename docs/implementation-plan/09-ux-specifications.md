# 09 - UX Specifications

This document provides detailed UX recommendations, wireframe guidance, and design system specifications for implementing HomeBuyerHelper.

---

## Design Philosophy

### Core UX Principles

1. **Guidance Over Overwhelm**: Complex features revealed progressively through guided flows
2. **Clarity Over Cleverness**: Clear labels, obvious actions, no hidden functionality
3. **Control Over Automation**: Users feel in control, no unexpected changes
4. **Speed Over Spectacle**: Fast interactions, minimal animations, focus on data
5. **Privacy Over Convenience**: Explicit consent, clear data handling

---

## Design System

### Typography

**Font Family**: System fonts for native feel
- iOS: SF Pro
- Android: Roboto
- Windows: Segoe UI Variable
- macOS: SF Pro

**Type Scale**:

| Style | Size (iOS) | Weight | Use Case |
|-------|------------|--------|----------|
| Display | 34pt | Bold | Page titles |
| Title 1 | 28pt | Bold | Section headers |
| Title 2 | 22pt | Semibold | Card titles |
| Title 3 | 20pt | Semibold | Subsections |
| Body | 17pt | Regular | Primary content |
| Callout | 16pt | Regular | Secondary content |
| Subhead | 15pt | Regular | Supporting text |
| Footnote | 13pt | Regular | Captions, hints |
| Caption | 12pt | Regular | Metadata |

---

### Color System

**Semantic Colors**:

| Token | Light Mode | Dark Mode | Usage |
|-------|------------|-----------|-------|
| `--primary` | #007AFF | #0A84FF | Primary actions, links |
| `--secondary` | #5856D6 | #5E5CE6 | Secondary actions |
| `--success` | #34C759 | #30D158 | Positive states, high scores |
| `--warning` | #FF9500 | #FF9F0A | Warnings, medium scores |
| `--danger` | #FF3B30 | #FF453A | Errors, low scores |
| `--background` | #FFFFFF | #000000 | Page background |
| `--surface` | #F2F2F7 | #1C1C1E | Card background |
| `--text-primary` | #000000 | #FFFFFF | Primary text |
| `--text-secondary` | #6B6B6B | #8E8E93 | Secondary text |
| `--separator` | #C6C6C8 | #38383A | Divider lines |

**Score Colors**:

| Score Range | Token | Light | Dark |
|-------------|-------|-------|------|
| 8-10 (Good) | `--score-good` | #34C759 | #30D158 |
| 5-7 (Okay) | `--score-okay` | #FF9500 | #FF9F0A |
| 1-4 (Poor) | `--score-poor` | #FF3B30 | #FF453A |

---

### Spacing

**Spacing Scale** (based on 4pt grid):

| Token | Value | Use Case |
|-------|-------|----------|
| `--space-xs` | 4pt | Tight spacing, icons to text |
| `--space-sm` | 8pt | Related elements |
| `--space-md` | 16pt | Standard padding |
| `--space-lg` | 24pt | Section separation |
| `--space-xl` | 32pt | Major section breaks |
| `--space-2xl` | 48pt | Page margins (tablet) |

---

### Components

#### Buttons

**Primary Button**:
```
┌─────────────────────────┐
│      Add Property       │  ← 17pt, Semibold, White
└─────────────────────────┘
     ↑ Background: --primary
     Height: 50pt
     Corner Radius: 12pt
     Horizontal Padding: 20pt
```

**Secondary Button**:
```
┌─────────────────────────┐
│        Cancel           │  ← 17pt, Regular, --primary
└─────────────────────────┘
     ↑ Background: Transparent
     Border: 1pt --primary
     Height: 50pt
```

**Text Button**:
```
    Skip for now →           ← 17pt, Regular, --primary
```

---

#### Cards

**Property Card**:
```
┌──────────────────────────────────────────┐
│  ┌─────┐                                 │
│  │ IMG │  Blue House on Oak St           │ ← Title 2
│  │     │  123 Oak Street, Austin, TX     │ ← Subhead, --text-secondary
│  └─────┘                                 │
│                                          │
│  $425,000          3 bd | 2 ba | 1,850 sf│ ← Body
│                                          │
│  ┌─────────────────────────────────────┐ │
│  │ ★ 8.2                        #2     │ │ ← Score Badge
│  └─────────────────────────────────────┘ │
└──────────────────────────────────────────┘
   Background: --surface
   Corner Radius: 16pt
   Padding: 16pt
   Shadow: 0 2pt 8pt rgba(0,0,0,0.1)
```

---

#### Score Badge

```
┌─────────────────┐
│  ★ 8.2          │
└─────────────────┘
  ↑ Background: --score-good (with 20% opacity)
  Text: --score-good
  Font: Title 3, Semibold
  Padding: 8pt 12pt
  Corner Radius: 8pt
```

---

#### Form Fields

**Text Input**:
```
Property Nickname *              ← Footnote, --text-secondary
┌──────────────────────────────┐
│ Blue House on Oak Street     │  ← Body
└──────────────────────────────┘
   ↑ Background: --surface
   Border: 1pt --separator
   Corner Radius: 8pt
   Height: 48pt
   Padding: 12pt

Enter a memorable name for this property   ← Caption, --text-secondary
```

**Input with Error**:
```
Property Nickname *
┌──────────────────────────────┐
│                              │  ← Border: 1pt --danger
└──────────────────────────────┘
Nickname is required              ← Caption, --danger
```

---

## Screen Specifications

### Welcome Screen

```
┌─────────────────────────────────────┐
│                                     │
│                                     │
│           [App Logo]                │  ← 120pt x 120pt
│                                     │
│        HomeBuyerHelper              │  ← Display, centered
│                                     │
│     Find your perfect home          │  ← Title 3, --text-secondary
│     with confidence                 │
│                                     │
│                                     │
│   ┌─────────────────────────────┐   │
│   │        Get Started          │   │  ← Primary Button
│   └─────────────────────────────┘   │
│                                     │
│        I have a backup →            │  ← Text Button
│                                     │
└─────────────────────────────────────┘
```

**Behavior**:
- Logo animates in on first launch (fade + slight scale)
- "Get Started" begins onboarding
- "I have a backup" opens file picker for JSON import

---

### Goal Selection Screen

```
┌─────────────────────────────────────┐
│  ←                         1 of 6   │  ← Navigation
├─────────────────────────────────────┤
│                                     │
│  What best describes                │  ← Title 1
│  your situation?                    │
│                                     │
│  ┌─────────────────────────────┐    │
│  │ ○  Buying my first home     │    │  ← Selection Row
│  └─────────────────────────────┘    │
│  ┌─────────────────────────────┐    │
│  │ ●  Upgrading to bigger home │    │  ← Selected state
│  └─────────────────────────────┘    │
│  ┌─────────────────────────────┐    │
│  │ ○  Downsizing or simplifying│    │
│  └─────────────────────────────┘    │
│  ┌─────────────────────────────┐    │
│  │ ○  Relocating to new area   │    │
│  └─────────────────────────────┘    │
│  ┌─────────────────────────────┐    │
│  │ ○  Buying investment        │    │
│  └─────────────────────────────┘    │
│                                     │
│   ┌─────────────────────────────┐   │
│   │         Continue            │   │  ← Disabled until selection
│   └─────────────────────────────┘   │
└─────────────────────────────────────┘
```

**Selection Row Styling**:
- Unselected: Background --surface, Border --separator
- Selected: Background --primary (10% opacity), Border --primary
- Radio indicator: 24pt diameter, --primary when filled

---

### Priority Ranking Screen

```
┌─────────────────────────────────────┐
│  ←                         5 of 6   │
├─────────────────────────────────────┤
│                                     │
│  Drag to rank these from            │  ← Title 2
│  most to least important            │
│                                     │
│  ┌─────────────────────────────┐    │
│  │ ≡  1. Commute Time          │    │  ← Drag handle
│  └─────────────────────────────┘    │
│  ┌─────────────────────────────┐    │
│  │ ≡  2. Home Office Space     │    │  ← Active drag state
│  └─────────────────────────────┘    │     (elevated, semi-transparent)
│  ┌─────────────────────────────┐    │
│  │ ≡  3. Dog-Friendly          │    │
│  └─────────────────────────────┘    │
│  ┌─────────────────────────────┐    │
│  │ ≡  4. Overall Feel          │    │
│  └─────────────────────────────┘    │
│           ...                       │
│                                     │
│   ┌─────────────────────────────┐   │
│   │         Continue            │   │
│   └─────────────────────────────┘   │
└─────────────────────────────────────┘
```

**Drag Interaction**:
- Long press (300ms) initiates drag
- Haptic feedback on pickup and drop
- Other items animate to show insertion point
- Alternative: Tap to select, then tap new position

---

### Weight Fine-Tuning Screen

```
┌─────────────────────────────────────┐
│  ←                         6 of 6   │
├─────────────────────────────────────┤
│  Adjust how much each               │  ← Title 2
│  factor matters                     │
│                                     │
│       ┌───────────────┐             │
│       │   [Pie Chart] │             │  ← Real-time updates
│       │               │             │
│       └───────────────┘             │
│                                     │
│  Total: 100%  ✓                     │  ← Validation indicator
│                                     │
│  Commute Time                  20%  │
│  ├────────────●────────────────┤ 🔒 │  ← Slider + Lock toggle
│                                     │
│  Home Office                   15%  │
│  ├────────●────────────────────┤    │
│                                     │
│  Overall Feel                  20%  │
│  ├────────────●────────────────┤ 🔒 │
│                                     │
│           ...                       │
│                                     │
│   ┌─────────────────────────────┐   │
│   │     Finish Setup            │   │
│   └─────────────────────────────┘   │
└─────────────────────────────────────┘
```

**Pie Chart**:
- Segments colored by category
- Labels on segments or legend below
- Animate smoothly on slider change

**Slider Behavior**:
- Locked sliders: Grayed lock icon, slider disabled
- Unlocked: Changes redistribute among other unlocked sliders
- Total must always equal 100%

---

### Property List (Dashboard)

```
┌─────────────────────────────────────┐
│  Properties              ⚙️  +      │  ← Title, Settings, Add
├─────────────────────────────────────┤
│  Sort: Score ▼                      │  ← Sort dropdown
│                                     │
│  ┌─────────────────────────────────┐│
│  │ [Property Card - Rank #1]       ││  ← Gold/yellow accent
│  └─────────────────────────────────┘│
│                                     │
│  ┌─────────────────────────────────┐│
│  │ [Property Card - Rank #2]       ││
│  └─────────────────────────────────┘│
│                                     │
│  ┌─────────────────────────────────┐│
│  │ [Property Card - Rank #3]       ││
│  └─────────────────────────────────┘│
│                                     │
├─────────────────────────────────────┤
│  🏠 Properties  📊 Compare  💰 Budget│  ← Tab Bar
└─────────────────────────────────────┘
```

**Empty State**:
```
┌─────────────────────────────────────┐
│                                     │
│           [House Icon]              │  ← 80pt, --text-secondary
│                                     │
│    No properties yet                │  ← Title 2
│                                     │
│    Add your first property to       │  ← Body, --text-secondary
│    start comparing homes            │
│                                     │
│   ┌─────────────────────────────┐   │
│   │     Add First Property      │   │
│   └─────────────────────────────┘   │
└─────────────────────────────────────┘
```

---

### Property Detail

```
┌─────────────────────────────────────┐
│  ←  Blue House on Oak St    ✏️  🗑️ │  ← Back, Title, Edit, Delete
├─────────────────────────────────────┤
│                                     │
│  ┌─────────────────────────────────┐│
│  │      [Property Photo(s)]        ││
│  └─────────────────────────────────┘│
│                                     │
│  $425,000                     #2    │  ← Title 1, Rank badge
│  123 Oak Street, Austin, TX 78701   │  ← Body, --text-secondary
│                                     │
│  ┌───────┬───────┬──────┬─────────┐ │
│  │ 3 bd  │ 2 ba  │ 1850 │  2018   │ │  ← Detail chips
│  └───────┴───────┴──────┴─────────┘ │
│                                     │
│  ── Scores ─────────────────────── │
│                                     │
│  Overall Score          ★ 8.2      │  ← Prominent display
│                                     │
│  Kitchen Quality    ████████░░  8  │  ← Progress bar + value
│  Commute Time       █████████░  9  │
│  Home Office        ███████░░░  7  │
│  Neighborhood       ████████░░  8  │
│                                     │
│  [Score More Criteria]              │  ← If incomplete
│                                     │
│  ── Monthly Cost ─────────────────  │
│                                     │
│  $2,847/mo                          │  ← Title 2
│  Mortgage $1,892 + Tax $340 + ...   │  ← Breakdown
│                                     │
└─────────────────────────────────────┘
```

---

### Comparison Matrix

```
┌─────────────────────────────────────────────────────────────┐
│  Compare Properties                                    ⚙️   │
├─────────────────────────────────────────────────────────────┤
│              │ Blue House │ Red Condo  │ Green Town │      │
│              │ #1  ★8.2   │ #2  ★7.8   │ #3  ★7.1   │      │
├──────────────┼────────────┼────────────┼────────────┤      │
│ Kitchen      │    8 🟢    │    7 🟡    │    6 🟡    │ ←    │
├──────────────┼────────────┼────────────┼────────────┤ Scroll│
│ Commute      │    9 🟢    │    5 🟡    │    8 🟢    │      │
├──────────────┼────────────┼────────────┼────────────┤      │
│ Home Office  │    7 🟡    │    9 🟢    │    6 🟡    │      │
├──────────────┼────────────┼────────────┼────────────┤      │
│ Neighborhood │    8 🟢    │    8 🟢    │    7 🟡    │      │
├──────────────┼────────────┼────────────┼────────────┤      │
│ Price/Value  │    7 🟡    │    8 🟢    │    9 🟢    │      │
├──────────────┼────────────┼────────────┼────────────┼──────┤
│ TOTAL        │    8.2     │    7.8     │    7.1     │      │
└──────────────┴────────────┴────────────┴────────────┴──────┘
                    ↑ Highlighted as winner (gold border)
```

**Interactions**:
- Tap cell to edit score inline
- Horizontal scroll for many properties
- Vertical scroll for many criteria
- Long-press column header for property actions
- Tap row header for criterion details

---

### Scoring Walkthrough

```
┌─────────────────────────────────────┐
│  ←  Scoring: Blue House    3 of 7   │
├─────────────────────────────────────┤
│                                     │
│  How would you rate this            │  ← Title 2
│  property's Kitchen Quality?        │
│                                     │
│  ┌─────────────────────────────────┐│
│  │ 10 = Dream kitchen, high-end   ││  ← Score anchor
│  │  5 = Acceptable, minor updates ││
│  │  1 = Needs complete renovation ││
│  └─────────────────────────────────┘│
│                                     │
│  ┌──────────────────────────────┐   │
│  │  1   2   3   4   5   6   7   │   │  ← Score buttons
│  │  ○   ○   ○   ○   ○   ○   ○   │   │     (44pt targets)
│  │                               │   │
│  │  8   9   10                   │   │
│  │  ●   ○   ○                    │   │  ← Selected = filled
│  └──────────────────────────────┘   │
│                                     │
│  Add a note (optional)              │
│  ┌──────────────────────────────┐   │
│  │ Granite counters, needs new  │   │
│  │ appliances                   │   │
│  └──────────────────────────────┘   │
│                                     │
│         Skip for now                │  ← Text button
│                                     │
│   ┌─────────────────────────────┐   │
│   │     Next: Commute Time      │   │  ← Primary button
│   └─────────────────────────────┘   │
└─────────────────────────────────────┘
```

**Score Button States**:
- Unselected: Border only, --separator
- Hover/Focus: Border --primary
- Selected: Filled background --primary, text white

---

## Interaction Patterns

### Gestures

| Gesture | Action |
|---------|--------|
| Tap | Primary interaction |
| Long Press | Context menu or drag start |
| Swipe Left | Delete action (with confirmation) |
| Swipe Right | Secondary action or undo |
| Pull Down | Refresh (where applicable) |
| Pinch | Zoom (photo gallery only) |

### Feedback

| Action | Feedback |
|--------|----------|
| Button tap | Subtle scale (0.97) + haptic |
| Score selection | Haptic + value announcement |
| Save success | Green toast + haptic |
| Error | Red toast + error haptic |
| Delete | Confirmation dialog first |

### Transitions

- **Page Navigation**: Slide from right (standard platform)
- **Modal**: Slide from bottom
- **Tab Switch**: Cross-fade
- **List Reorder**: Item lifts and animates
- **Score Change**: Number counter animation

---

## Responsive Breakpoints

| Breakpoint | Width | Layout |
|------------|-------|--------|
| Phone | < 600pt | Single column |
| Tablet Portrait | 600-900pt | Two columns where appropriate |
| Tablet Landscape / Desktop | > 900pt | Multi-column, side panels |

### Phone vs Tablet Layout

**Property List**:
- Phone: Single column cards, full width
- Tablet: 2-3 column grid, cards in grid

**Comparison Matrix**:
- Phone: Horizontal scroll, 2 properties visible
- Tablet: Full table visible, no scroll needed

**Settings**:
- Phone: Full-width sections, stacked
- Tablet: Sidebar navigation + content pane

---

## Error States

### Form Validation

```
┌──────────────────────────────────────┐
│  ⚠️ Please fix the following:        │  ← Warning banner
│  • Property nickname is required     │
│  • Asking price must be greater     │
│    than zero                         │
└──────────────────────────────────────┘
```

### Network Error (Sync)

```
┌──────────────────────────────────────┐
│  ⚠️ Sync failed                      │
│  Unable to connect to Dropbox.       │
│  Your data is saved locally.         │
│                                      │
│  [Retry]  [Work Offline]             │
└──────────────────────────────────────┘
```

### Empty States

Every list has an empty state with:
1. Relevant illustration or icon
2. Explanatory text
3. Primary action button

---

## Accessibility Annotations

Each wireframe should note:
- Focus order (numbered)
- Screen reader announcements
- Alternative gestures
- Color-independent indicators

See [08-accessibility-guidelines.md](./08-accessibility-guidelines.md) for full specifications.

---

## Platform-Specific Considerations

### iOS

- Use SF Symbols for icons
- Standard navigation bar height (44pt)
- Safe area insets respected
- Large title navigation where appropriate

### Android

- Material Design icons
- Standard app bar height (56dp)
- Navigation drawer or bottom nav
- Floating action button for primary add action

### Windows

- Segoe Fluent Icons
- Title bar integration
- Acrylic/Mica backgrounds (subtle)
- Keyboard shortcuts visible in menus

### macOS

- SF Symbols
- Standard macOS menu bar
- Window resizing with minimum sizes
- Native menu structure
