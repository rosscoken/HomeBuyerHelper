# 11 - Future: BYOM & Year 2+ Features

This document outlines the implementation plan for Year 2+ features, with primary focus on the **Bring Your Own Model (BYOM)** AI document review capability.

---

## BYOM Overview

### What is BYOM?

BYOM (Bring Your Own Model) allows users to optionally connect their own AI model API keys to get intelligent assistance with reviewing home buying documents. This maintains the app's core privacy commitment—users control their own AI interactions, and no document data passes through our servers.

### Privacy-First AI Integration

| Principle | Implementation |
|-----------|----------------|
| **User's Own API Key** | Users provide their own API key from their chosen provider—we never see it |
| **Direct API Calls** | Requests go directly from user's device to AI provider—not through our servers |
| **Explicit Opt-In** | Feature is completely disabled by default; requires intentional setup |
| **No Data Retention** | Documents are not stored after analysis; processed in-memory only |
| **Transparent Costs** | Users pay their own API costs directly to provider at their rates |

---

## Phase 5: BYOM Implementation

### Phase Overview

| Aspect | Details |
|--------|---------|
| **Goal** | Privacy-preserving AI document review using user's own API keys |
| **Dependencies** | Phase 4 complete |
| **Platforms** | All platforms (iOS, Android, Windows, macOS) |
| **Providers** | Claude API, OpenAI API, Ollama (local) |

---

## Task Sections

1. [Provider Configuration](#1-provider-configuration)
2. [Document Capture](#2-document-capture)
3. [Document Analysis](#3-document-analysis)
4. [Analysis Results UI](#4-analysis-results-ui)
5. [Conversation Follow-up](#5-conversation-follow-up)
6. [Local Model Support](#6-local-model-support-ollama)
7. [Phase 5 Quality Gates](#7-phase-5-quality-gates)

---

## 1. Provider Configuration

### P5-CFG-001: BYOM Settings Page

**Status**: `[ ]` Not Started

**Dependencies**: Phase 4 complete

**Acceptance Criteria**:
- [ ] Settings page for AI Document Review configuration
- [ ] Clear privacy messaging explaining BYOM model
- [ ] Feature disabled by default
- [ ] Provider selection interface

**Files to Create**:
- `src/HomeBuyerHelper/Pages/Settings/AIDocumentReviewPage.xaml`
- `src/HomeBuyerHelper/ViewModels/Settings/AIDocumentReviewViewModel.cs`
- `src/HomeBuyerHelper.Core/Models/BYOMConfig.cs`

**UX Requirements**:
```
┌─────────────────────────────────────┐
│  ←  AI Document Review              │
├─────────────────────────────────────┤
│                                     │
│  ┌─────────────────────────────────┐│
│  │ ℹ️  How This Works              ││
│  │                                 ││
│  │ This feature uses YOUR OWN AI  ││
│  │ API key. Documents go directly ││
│  │ to your chosen provider—we     ││
│  │ never see them.                ││
│  │                                 ││
│  │ You pay the AI provider        ││
│  │ directly at their rates.       ││
│  └─────────────────────────────────┘│
│                                     │
│  Enable AI Document Review          │
│  ┌─────────────────────────────[○]─┐│  ← Toggle OFF by default
│                                     │
│  (Settings below appear when on)    │
│                                     │
└─────────────────────────────────────┘
```

---

### P5-CFG-002: Claude API Configuration

**Status**: `[ ]` Not Started

**Dependencies**: P5-CFG-001

**Acceptance Criteria**:
- [ ] Claude (Anthropic) provider option
- [ ] API key input with secure storage
- [ ] Model selection (claude-3-opus, claude-3-sonnet, claude-3-haiku)
- [ ] "Test Connection" button to verify key
- [ ] Link to Anthropic API key management

**Form Fields**:
| Field | Type | Notes |
|-------|------|-------|
| API Key | Password | Stored in platform SecureStorage |
| Model | Dropdown | claude-3-opus-20240229, claude-3-sonnet-20240229, claude-3-haiku-20240307 |
| Max Tokens | Number | Default 4096, user configurable |

**Security Requirements**:
- [ ] API key stored in SecureStorage (Keychain/Keystore)
- [ ] Key never logged or transmitted except to Anthropic
- [ ] Key masked in UI after entry

**Test Requirements**:
- [ ] Unit test: Secure storage integration
- [ ] Integration test: API connection test

---

### P5-CFG-003: OpenAI API Configuration

**Status**: `[ ]` Not Started

**Dependencies**: P5-CFG-001

**Acceptance Criteria**:
- [ ] OpenAI provider option
- [ ] API key input with secure storage
- [ ] Model selection (gpt-4o, gpt-4-turbo, gpt-3.5-turbo)
- [ ] "Test Connection" button
- [ ] Link to OpenAI API key management

---

### P5-CFG-004: Provider Abstraction Layer

**Status**: `[ ]` Not Started

**Dependencies**: P5-CFG-002, P5-CFG-003

**Acceptance Criteria**:
- [ ] Common interface for all AI providers
- [ ] Request/response normalization
- [ ] Error handling per provider
- [ ] Token counting estimation

**Interface Design**:
```csharp
public interface IAIDocumentProvider
{
    string ProviderName { get; }
    Task<bool> TestConnectionAsync();
    Task<DocumentAnalysisResult> AnalyzeDocumentAsync(
        string documentContent,
        DocumentType documentType,
        string? additionalContext = null);
    Task<string> AskFollowUpAsync(
        string conversationHistory,
        string question);
    int EstimateTokens(string content);
}

public class ClaudeProvider : IAIDocumentProvider { }
public class OpenAIProvider : IAIDocumentProvider { }
public class OllamaProvider : IAIDocumentProvider { }
```

**Files to Create**:
- `src/HomeBuyerHelper.Core/Interfaces/IAIDocumentProvider.cs`
- `src/HomeBuyerHelper.Core/Services/AI/ClaudeProvider.cs`
- `src/HomeBuyerHelper.Core/Services/AI/OpenAIProvider.cs`

---

## 2. Document Capture

### P5-DOC-001: Document Type Selection

**Status**: `[ ]` Not Started

**Dependencies**: P5-CFG-001

**Acceptance Criteria**:
- [ ] Select document type before capture/upload
- [ ] Types: Purchase Agreement, Seller Disclosure, HOA Documents, Inspection Report, Closing Documents, Other
- [ ] Type selection influences analysis prompts

**Document Types**:
| Type | Analysis Focus |
|------|----------------|
| Purchase Agreement | Unusual clauses, contingencies, timeline, obligations |
| Seller Disclosure | Red flags, incomplete disclosures, repair history |
| HOA Documents | Restrictions, fees, reserves, rules, pending litigation |
| Inspection Report | Critical issues, cost estimates, priority items |
| Closing Documents | Fee verification, terms matching, timing |
| Other | General document summarization |

---

### P5-DOC-002: Camera Document Capture

**Status**: `[ ]` Not Started

**Dependencies**: P5-DOC-001

**Acceptance Criteria**:
- [ ] Camera capture for physical documents
- [ ] Multi-page capture with page ordering
- [ ] Edge detection and perspective correction (nice-to-have)
- [ ] Preview and retake option

**Files to Create**:
- `src/HomeBuyerHelper/Pages/Documents/DocumentCapturePage.xaml`
- `src/HomeBuyerHelper/Services/DocumentCaptureService.cs`

---

### P5-DOC-003: PDF Upload

**Status**: `[ ]` Not Started

**Dependencies**: P5-DOC-001

**Acceptance Criteria**:
- [ ] File picker for PDF documents
- [ ] PDF text extraction
- [ ] Large PDF handling (chunking)
- [ ] Progress indicator for processing

**Implementation Notes**:
- Use PdfPig or similar library for PDF text extraction
- Chunk documents > 100k characters for API limits

---

### P5-DOC-004: Document Text Extraction

**Status**: `[ ]` Not Started

**Dependencies**: P5-DOC-002, P5-DOC-003

**Acceptance Criteria**:
- [ ] OCR for camera-captured images
- [ ] Text extraction from PDFs
- [ ] Clean and normalize extracted text
- [ ] Handle multi-column layouts

**Libraries**:
- Camera OCR: Platform OCR APIs (iOS Vision, Android ML Kit, Windows OCR)
- PDF: PdfPig or iText7

**Files to Create**:
- `src/HomeBuyerHelper.Core/Services/DocumentExtractionService.cs`

---

## 3. Document Analysis

### P5-ANA-001: Analysis Request Service

**Status**: `[ ]` Not Started

**Dependencies**: P5-CFG-004, P5-DOC-004

**Acceptance Criteria**:
- [ ] Send document to configured AI provider
- [ ] Use document-type-specific prompts
- [ ] Handle API rate limits gracefully
- [ ] Display cost estimate before analysis
- [ ] Cancel option during processing

**Prompts by Document Type**:

**Purchase Agreement Prompt**:
```
You are a real estate document reviewer helping a home buyer understand a purchase agreement.

Analyze this purchase agreement and provide:
1. KEY TERMS: Summarize the essential terms (price, closing date, contingencies, deposits)
2. UNUSUAL CLAUSES: Flag any clauses that deviate from standard contracts or seem unusual
3. BUYER OBLIGATIONS: List all obligations and deadlines the buyer must meet
4. CONTINGENCIES: Explain each contingency and its timeline
5. POTENTIAL CONCERNS: Highlight anything that may need clarification or negotiation
6. QUESTIONS TO ASK: Suggest questions the buyer should ask their agent or attorney

Be specific, cite relevant sections, and explain implications in plain language.
```

**Seller Disclosure Prompt**:
```
You are a real estate document reviewer helping a home buyer understand seller disclosures.

Analyze this seller disclosure document and provide:
1. DISCLOSED ISSUES: List all disclosed problems, repairs, or defects
2. RED FLAGS: Identify any concerning disclosures that warrant further investigation
3. INCOMPLETE SECTIONS: Note any sections marked "unknown" or left blank
4. REPAIR HISTORY: Summarize any repairs or improvements mentioned
5. INSURANCE CLAIMS: Note any disclosed insurance claims or damage history
6. FOLLOW-UP ITEMS: Recommend items to investigate during inspection

Focus on material facts that could affect the buyer's decision or the property's value.
```

**Files to Create**:
- `src/HomeBuyerHelper.Core/Services/AI/DocumentAnalysisService.cs`
- `src/HomeBuyerHelper.Core/Data/AnalysisPrompts.cs`

---

### P5-ANA-002: Cost Estimation

**Status**: `[ ]` Not Started

**Dependencies**: P5-ANA-001

**Acceptance Criteria**:
- [ ] Estimate tokens before sending
- [ ] Display estimated cost based on provider pricing
- [ ] Confirm before proceeding with analysis
- [ ] Track cumulative usage (local only)

**Pricing Display**:
```
┌─────────────────────────────────────┐
│  Document Analysis                  │
├─────────────────────────────────────┤
│                                     │
│  Document: Purchase_Agreement.pdf   │
│  Size: ~15,000 tokens               │
│                                     │
│  Estimated Cost: ~$0.45             │
│  (Claude 3 Sonnet @ $3/M input)     │
│                                     │
│  ┌─────────────────────────────┐    │
│  │      Analyze Document       │    │
│  └─────────────────────────────┘    │
│                                     │
│          Cancel                     │
└─────────────────────────────────────┘
```

---

### P5-ANA-003: Analysis Progress UI

**Status**: `[ ]` Not Started

**Dependencies**: P5-ANA-001

**Acceptance Criteria**:
- [ ] Progress indicator during analysis
- [ ] Cancel button to abort
- [ ] Streaming response display (if provider supports)
- [ ] Error handling with retry option

---

## 4. Analysis Results UI

### P5-RES-001: Analysis Results Page

**Status**: `[ ]` Not Started

**Dependencies**: P5-ANA-001

**Acceptance Criteria**:
- [ ] Display structured analysis results
- [ ] Collapsible sections for each category
- [ ] Highlight critical findings
- [ ] Copy to clipboard functionality
- [ ] Save analysis with property

**Results Layout**:
```
┌─────────────────────────────────────┐
│  ←  Analysis Results                │
├─────────────────────────────────────┤
│                                     │
│  Purchase Agreement Analysis        │
│  Blue House on Oak Street           │
│  Analyzed: Jan 6, 2026              │
│                                     │
│  ▼ Key Terms                        │
│  ┌─────────────────────────────────┐│
│  │ • Purchase Price: $425,000     ││
│  │ • Closing Date: Feb 15, 2026   ││
│  │ • Earnest Money: $10,000       ││
│  │ • Contingencies: Financing,    ││
│  │   Inspection, Appraisal        ││
│  └─────────────────────────────────┘│
│                                     │
│  ▼ ⚠️ Potential Concerns (2)        │
│  ┌─────────────────────────────────┐│
│  │ 1. Inspection contingency      ││
│  │    period is only 5 days       ││
│  │    (typically 7-10 days)...    ││
│  └─────────────────────────────────┘│
│                                     │
│  ► Buyer Obligations (5)            │
│  ► Contingencies (3)                │
│  ► Questions to Ask (4)             │
│                                     │
│  ┌─────────────────────────────────┐│
│  │      Ask Follow-up Question     ││
│  └─────────────────────────────────┘│
│                                     │
└─────────────────────────────────────┘
```

**Files to Create**:
- `src/HomeBuyerHelper/Pages/Documents/AnalysisResultsPage.xaml`
- `src/HomeBuyerHelper/ViewModels/Documents/AnalysisResultsViewModel.cs`

---

### P5-RES-002: Save Analysis to Property

**Status**: `[ ]` Not Started

**Dependencies**: P5-RES-001

**Acceptance Criteria**:
- [ ] Link analysis to a property
- [ ] View past analyses from property detail
- [ ] Multiple analyses per property (different documents)
- [ ] Delete analysis option

**Data Model**:
```csharp
public class DocumentAnalysis
{
    public int Id { get; set; }
    public int? PropertyId { get; set; }
    public DocumentType DocumentType { get; set; }
    public string DocumentName { get; set; }
    public DateTime AnalyzedAt { get; set; }
    public string Provider { get; set; }
    public string Model { get; set; }
    public string AnalysisJson { get; set; }  // Structured results
    public int TokensUsed { get; set; }
}
```

---

### P5-RES-003: Analysis Export

**Status**: `[ ]` Not Started

**Dependencies**: P5-RES-001

**Acceptance Criteria**:
- [ ] Export analysis to PDF
- [ ] Export analysis to plain text
- [ ] Include in property comparison report (optional)

---

## 5. Conversation Follow-up

### P5-FOL-001: Follow-up Question Interface

**Status**: `[ ]` Not Started

**Dependencies**: P5-RES-001

**Acceptance Criteria**:
- [ ] Text input for follow-up questions
- [ ] Conversation history maintained in session
- [ ] Previous context included in follow-ups
- [ ] Cost estimate per question

**UX Flow**:
1. User views analysis results
2. User taps "Ask Follow-up Question"
3. User types question (e.g., "What does the arbitration clause mean?")
4. Show cost estimate
5. User confirms, response displayed
6. Can ask additional follow-ups

---

### P5-FOL-002: Suggested Questions

**Status**: `[ ]` Not Started

**Dependencies**: P5-FOL-001

**Acceptance Criteria**:
- [ ] Generate suggested follow-up questions
- [ ] Based on document type and findings
- [ ] One-tap to ask suggested question

**Example Suggested Questions**:
- "What is a typical earnest money amount for this price range?"
- "Can you explain the financing contingency timeline in more detail?"
- "What should I look for in the inspection regarding the disclosed roof repair?"

---

## 6. Local Model Support (Ollama)

### P5-OLL-001: Ollama Provider Configuration

**Status**: `[ ]` Not Started

**Dependencies**: P5-CFG-004

**Acceptance Criteria**:
- [ ] Ollama as provider option (desktop only)
- [ ] Server URL configuration (default: localhost:11434)
- [ ] Model selection from available local models
- [ ] Connection test

**Ollama Benefits**:
- Fully offline analysis (no API costs)
- Maximum privacy (no data leaves device)
- Good for users with capable hardware

**Limitations**:
- Desktop only (Windows/macOS)
- Requires user to install and run Ollama
- Slower than cloud APIs
- Quality depends on local model

---

### P5-OLL-002: Ollama Setup Guide

**Status**: `[ ]` Not Started

**Dependencies**: P5-OLL-001

**Acceptance Criteria**:
- [ ] In-app guide for Ollama setup
- [ ] Link to Ollama download
- [ ] Recommended models for document analysis
- [ ] Troubleshooting common issues

**Recommended Models**:
| Model | Size | Quality | Speed |
|-------|------|---------|-------|
| llama3:70b | 40GB | Excellent | Slow |
| llama3:8b | 4.7GB | Good | Fast |
| mixtral:8x7b | 26GB | Very Good | Medium |
| mistral:7b | 4.1GB | Good | Fast |

---

## 7. Phase 5 Quality Gates

### P5-QG-001: Test Coverage

**Status**: `[ ]` Not Started

**Acceptance Criteria**:
- [ ] Unit tests for all provider implementations
- [ ] Integration tests with mock API responses
- [ ] Document extraction tests
- [ ] UI tests for analysis flow

---

### P5-QG-002: Security Review (CRITICAL)

**Status**: `[ ]` Not Started

**Acceptance Criteria**:
- [ ] API key storage security audit
- [ ] No API keys in logs or error reports
- [ ] HTTPS enforced for all API calls
- [ ] No document content cached after analysis
- [ ] Memory cleared after processing
- [ ] AI security analysis on all BYOM code

**Security Checklist**:
- [ ] API keys stored in SecureStorage only
- [ ] Keys never logged (even in debug)
- [ ] Keys never included in crash reports
- [ ] Document content cleared from memory after analysis
- [ ] No plaintext storage of documents
- [ ] HTTPS certificate validation enabled
- [ ] User warned about sending documents to third-party APIs

---

### P5-QG-003: Privacy Verification

**Status**: `[ ]` Not Started

**Acceptance Criteria**:
- [ ] Verify no data sent to our servers
- [ ] Document direct API call flow
- [ ] Update privacy policy for BYOM feature
- [ ] Clear user consent flow before first use

---

### P5-QG-004: Accessibility Audit

**Status**: `[ ]` Not Started

**Acceptance Criteria**:
- [ ] Analysis results accessible to screen readers
- [ ] Document capture flow accessible
- [ ] Progress indicators announced
- [ ] Error states properly communicated

---

## Phase 6+: Other Future Features

These features are tracked for future planning but not detailed at this time:

### Listing URL Auto-fill

- Parse property details from popular listing sites
- Auto-populate property entry form
- On-device parsing (no server)
- Supported sites: Zillow, Redfin, Realtor.com

### Mortgage Calculator API Integration

- Optional connection to mortgage rate APIs
- Real-time rate quotes
- User-initiated only
- Multiple lender comparison

### Community Templates

- Share criteria templates with other users
- Browse community-contributed templates
- Rating and feedback system
- Privacy-preserving (no personal data in templates)

### Localization

- Spanish language support
- Canadian market adaptations (CAD, provincial taxes)
- UK market adaptations (GBP, stamp duty)
- Date/number format localization

### Widget Support

- iOS Home Screen widgets (property scores, monthly costs)
- Android widgets
- Windows widgets
- Quick glance at top property

---

## Phase 5 Summary

| Metric | Target |
|--------|--------|
| Total Tasks | ~25 |
| Providers | Claude, OpenAI, Ollama |
| Document Types | 6 |
| Security Review | Critical (API key handling) |
| Privacy | User's own API, no our-server involvement |

---

## References

- [Anthropic Claude API Documentation](https://docs.anthropic.com/)
- [OpenAI API Documentation](https://platform.openai.com/docs/)
- [Ollama Documentation](https://ollama.ai/)
- [DesignSpec.md Section 8.5.1](../DesignSpec.md) - BYOM Design Specification
