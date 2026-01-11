# Phase 1 App Store Preparation

This document contains all materials and requirements for submitting HomeBuyerHelper to the Apple App Store and Google Play Store.

---

## App Information

### Basic Details

| Field | Value |
|-------|-------|
| **App Name** | HomeBuyerHelper |
| **Bundle ID (iOS)** | com.homebuyerhelper.app |
| **Package Name (Android)** | com.homebuyerhelper.app |
| **Version** | 1.0.0 |
| **Build Number** | 1 |
| **Category** | Finance / Utilities |
| **Content Rating** | Everyone / 4+ |
| **Price** | Free |

---

## App Store Descriptions

### Short Description (80 characters)
```
Compare homes, score features, and find your perfect property with confidence.
```

### Full Description
```
HomeBuyerHelper takes the stress out of your home search by helping you objectively compare properties using a personalized scoring system.

KEY FEATURES:

• Personalized Criteria - Define what matters most to you: commute time, kitchen quality, school district, yard size, and more.

• Smart Scoring System - Rate each property on your criteria using a simple 1-10 scale with helpful guidance.

• Side-by-Side Comparison - See all your properties ranked and compared in an easy-to-read matrix.

• Weighted Priorities - Automatically balance your criteria weights so the most important factors have the biggest impact on scores.

• Financial Calculator - Estimate monthly payments including mortgage, taxes, insurance, and HOA fees.

• Complete Privacy - All your data stays on your device. No accounts, no cloud, no tracking.

PERFECT FOR:
- First-time homebuyers feeling overwhelmed by choices
- Families balancing multiple priorities like schools, commute, and space
- Anyone wanting to make a data-driven decision on one of life's biggest purchases

Get started in minutes with our guided setup that helps you identify your priorities and create a custom evaluation framework.

Whether you're comparing 2 houses or 20, HomeBuyerHelper keeps you organized and confident throughout your home search journey.
```

### Keywords (iOS - 100 characters max)
```
home buying,house hunting,property comparison,real estate,home search,mortgage calculator,first home
```

### Keywords (Android - 5 tags max)
```
Home Buying
House Hunting
Property Comparison
Real Estate
Mortgage Calculator
```

---

## Screenshots Required

### iOS Screenshots (Required sizes)

| Device | Size (pixels) | Count Needed |
|--------|---------------|--------------|
| iPhone 6.7" (14 Pro Max) | 1290 x 2796 | 5-10 |
| iPhone 6.5" (11 Pro Max) | 1242 x 2688 | 5-10 |
| iPhone 5.5" (8 Plus) | 1242 x 2208 | 5-10 |
| iPad Pro 12.9" | 2048 x 2732 | 5-10 (if supporting iPad) |

### Android Screenshots

| Device | Size (pixels) | Count Needed |
|--------|---------------|--------------|
| Phone | 1080 x 1920 minimum | 4-8 |
| 7" Tablet | 1200 x 1920 (if supporting) | 4-8 |
| 10" Tablet | 1920 x 1200 (if supporting) | 4-8 |

### Screenshot Content

1. **Dashboard** - "Your home search at a glance"
2. **Property List** - "All your properties ranked"
3. **Scoring Walkthrough** - "Rate each feature objectively"
4. **Comparison Matrix** - "Side-by-side comparison"
5. **Property Detail** - "Complete property overview"
6. **Cost Calculator** - "Know your monthly costs"
7. **Onboarding** - "Personalized to your needs"

---

## App Icons

### iOS Icon Requirements
- 1024 x 1024 px (App Store)
- No transparency
- No rounded corners (system adds them)
- @1x, @2x, @3x sizes for in-app icons

### Android Icon Requirements
- 512 x 512 px (Play Store)
- Adaptive icon (foreground + background layers)
- 108 x 108 dp safe zone

### Icon Design
- Simple house outline with checkmark
- Primary brand color (#007AFF)
- Clean, modern, recognizable at small sizes

---

## Privacy Policy

**URL Required**: https://homebuyerhelper.com/privacy

### Privacy Policy Content

```markdown
# HomeBuyerHelper Privacy Policy

Last Updated: [Date]

## Summary
HomeBuyerHelper is designed with privacy as a core principle. All your data stays on your device.

## Data Collection
HomeBuyerHelper does NOT collect:
- Personal information
- Location data
- Usage analytics
- Advertising identifiers

## Data Storage
All data you enter (properties, scores, preferences) is stored locally on your device using encrypted SQLite storage.

## Data Sharing
We do not share any data with third parties because we do not collect any data.

## Data Export
You can export your data at any time using the Export feature. This creates a JSON file that you control.

## Data Deletion
Uninstalling the app removes all data. You can also clear data manually in Settings > Export/Import > Clear All Data.

## Changes
We will update this policy if our practices change. Material changes will be announced in the app.

## Contact
Questions? Email privacy@homebuyerhelper.com
```

---

## App Review Notes

### For Apple Review Team

```
Thank you for reviewing HomeBuyerHelper!

This app helps users compare properties during their home search. Key points:

1. NO SIGN-UP REQUIRED - The app works immediately with no account creation.

2. COMPLETELY OFFLINE - All data is stored locally. No network calls are made.

3. NO IN-APP PURCHASES - The app is fully free with no hidden costs.

4. TEST THE APP - You can:
   - Go through the onboarding flow
   - Add sample properties (just enter a nickname and price)
   - Score properties on your criteria
   - View the comparison matrix

5. ACCESSIBILITY - The app supports VoiceOver and Dynamic Type.

If you have any questions, please contact: review@homebuyerhelper.com
```

### For Google Review Team

```
HomeBuyerHelper helps users compare homes during their property search.

- No login required
- All data stored locally (no network permissions needed)
- Free with no in-app purchases
- Family-friendly content (no user-generated content)
- Tested on Android 10+ (API 29+)

For questions: review@homebuyerhelper.com
```

---

## Technical Requirements

### iOS Requirements

| Requirement | Value |
|-------------|-------|
| Minimum iOS Version | 15.0 |
| Required Device Capabilities | armv7 |
| App Transport Security | N/A (no network) |
| Background Modes | None |
| Permissions | None required |

### Android Requirements

| Requirement | Value |
|-------------|-------|
| Minimum SDK | API 26 (Android 8.0) |
| Target SDK | API 34 (Android 14) |
| Permissions | None required |
| Play Feature Delivery | Base APK only |

---

## Release Checklist

### Pre-Submission

- [ ] All features tested on physical devices
- [ ] Privacy policy published at URL
- [ ] App icons in all required sizes
- [ ] Screenshots for all required device sizes
- [ ] Version number updated in project
- [ ] Release notes prepared
- [ ] App Review notes written

### iOS Submission

- [ ] Archive created in Xcode
- [ ] App uploaded to App Store Connect
- [ ] App Information completed
- [ ] Pricing and Availability set
- [ ] Screenshots uploaded
- [ ] App Review Information provided
- [ ] Submit for Review

### Android Submission

- [ ] Signed release APK/AAB generated
- [ ] App uploaded to Google Play Console
- [ ] Store listing completed
- [ ] Content rating questionnaire completed
- [ ] Privacy policy URL added
- [ ] Review release and publish

---

## Version 1.0 Release Notes

```
HomeBuyerHelper 1.0 - Your Personal Home Comparison Tool

Introducing HomeBuyerHelper - the easiest way to compare homes during your property search.

Features:
• Personalized evaluation criteria based on your priorities
• Simple 1-10 scoring system with helpful guidance
• Side-by-side property comparison matrix
• Monthly cost calculator with mortgage, taxes, and fees
• Data export/import for backup
• Complete privacy - all data stays on your device

Perfect for first-time homebuyers and anyone wanting to make an informed decision on their next home.

Start comparing properties today!
```

---

## Support Information

| Field | Value |
|-------|-------|
| Support URL | https://homebuyerhelper.com/support |
| Support Email | support@homebuyerhelper.com |
| Marketing URL | https://homebuyerhelper.com |

---

## Age Rating Questionnaire

### iOS Content Rating

| Question | Answer |
|----------|--------|
| Cartoon or Fantasy Violence | No |
| Realistic Violence | No |
| Prolonged Graphic or Sadistic Violence | No |
| Profanity or Crude Humor | No |
| Mature/Suggestive Themes | No |
| Horror/Fear Themes | No |
| Medical/Treatment Information | No |
| Alcohol, Tobacco, or Drug Use | No |
| Simulated Gambling | No |
| Sexual Content or Nudity | No |
| Unrestricted Web Access | No |
| Contests | No |

**Result: Rated 4+ / Everyone**

### Android Content Rating (IARC)

- Violence: None
- Sexuality: None
- Language: None
- Controlled Substances: None
- User Interaction: None
- Shares Location: No
- Digital Purchases: No

**Result: Rated for Everyone**
