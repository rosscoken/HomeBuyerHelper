# 07 - Security Review Guidelines

This document provides security review checklists and AI-assisted security analysis procedures for each phase of HomeBuyerHelper development.

---

## Security Philosophy

### Core Principles

1. **Privacy-First**: User data never leaves the device without explicit consent
2. **Defense in Depth**: Multiple layers of protection
3. **Secure by Default**: Safe configurations out of the box
4. **Minimal Attack Surface**: No unnecessary network exposure
5. **Transparency**: Security practices documented and verifiable

### Threat Model

HomeBuyerHelper has a favorable threat model due to its offline-first design:

| Threat | Mitigation |
|--------|------------|
| Server breach | No server storing user data |
| Network interception | Minimal network traffic (only for optional sync) |
| Malicious input | Input validation on all user data |
| Local data theft | Optional database encryption |
| Malicious sync content | Schema validation on import |

---

## AI-Assisted Security Analysis

### Using Claude Code for Security Review

At the end of each phase, run comprehensive security analysis using AI coding assistants:

**Prompt Template for Security Analysis**:
```
Please perform a security review of the following files in the HomeBuyerHelper project:

[List of new/modified files from this phase]

For each file, analyze:
1. Input validation - Are all user inputs validated?
2. Data handling - Is sensitive data protected?
3. SQL injection - Are queries parameterized?
4. Path traversal - Are file paths sanitized?
5. Secrets - Are there any hardcoded secrets?
6. Encryption - Is sensitive data encrypted at rest?
7. Authentication - Are OAuth flows secure?
8. Error handling - Do errors leak sensitive information?

Provide findings in this format:
- File: [filename]
- Severity: Critical/High/Medium/Low/Info
- Issue: [description]
- Recommendation: [fix]
```

**Example AI Security Review**:
```
File: src/HomeBuyerHelper.Core/Services/ExportService.cs
Severity: Medium
Issue: Export file path constructed from user input without sanitization
Line: 45
Recommendation: Use Path.Combine with validation to prevent path traversal

File: src/HomeBuyerHelper/Services/DropboxSyncService.cs
Severity: Low
Issue: OAuth token stored without platform secure storage
Line: 78
Recommendation: Use SecureStorage on MAUI instead of Preferences
```

---

## Phase-Specific Security Checklists

### Phase 1: MVP Security Checklist

**Data Storage**:
- [ ] SQLite database file permissions are restrictive
- [ ] Database location is in app-private storage
- [ ] No sensitive data logged to console
- [ ] Backup files excluded from cloud backup (unless intentional)

**Input Validation**:
- [ ] Property nickname length limited
- [ ] Address sanitized for display
- [ ] Numeric inputs bounded (price, sqft, etc.)
- [ ] URL inputs validated
- [ ] Score inputs constrained to 1-10

**Export/Import**:
- [ ] JSON export doesn't include internal IDs that could leak info
- [ ] JSON import validates schema before processing
- [ ] Import handles malformed data gracefully
- [ ] Large file import doesn't cause memory issues

**Platform Security**:
- [ ] iOS: App Transport Security enabled (though minimal network)
- [ ] Android: Network security config restricts cleartext
- [ ] Windows: App runs with minimal permissions

**Code Review Points**:
```csharp
// GOOD: Parameterized query
var property = await db.Table<PropertyEntity>()
    .Where(p => p.Id == propertyId)
    .FirstOrDefaultAsync();

// BAD: String concatenation (SQL injection risk)
var query = $"SELECT * FROM Properties WHERE Id = {propertyId}";

// GOOD: Path validation
var safePath = Path.Combine(
    FileSystem.AppDataDirectory,
    Path.GetFileName(userInput));

// BAD: Direct path usage (path traversal risk)
var unsafePath = $"{baseDir}/{userInput}";
```

---

### Phase 2: Budget + Desktop Security Checklist

**Financial Data**:
- [ ] Income and expense data never transmitted
- [ ] PDF reports generated locally only
- [ ] No financial data in crash reports
- [ ] Clear financial data on uninstall (optional setting)

**Desktop Considerations**:
- [ ] Windows: Runs in sandboxed AppContainer
- [ ] macOS: App Sandbox enabled
- [ ] No admin privileges required
- [ ] File dialog restricted to appropriate locations

**PDF Generation**:
- [ ] No external URLs embedded in PDFs
- [ ] No executable content in PDFs
- [ ] File size limits enforced
- [ ] Temp files cleaned up after generation

---

### Phase 3: Cloud Sync Security Checklist (CRITICAL)

**OAuth Implementation**:
- [ ] OAuth 2.0 with PKCE used (no implicit flow)
- [ ] State parameter used to prevent CSRF
- [ ] Tokens stored in platform secure storage:
  - iOS: Keychain
  - Android: EncryptedSharedPreferences or KeyStore
  - Windows: Credential Locker
  - macOS: Keychain
- [ ] Token refresh handled securely
- [ ] Logout clears all tokens

**Data in Transit**:
- [ ] HTTPS only for all cloud communication
- [ ] Certificate pinning considered for high-value operations
- [ ] No sensitive data in URL parameters

**Data at Rest (Cloud)**:
- [ ] User's data encrypted by cloud provider
- [ ] We don't store any copy of user data
- [ ] App-specific folders used for isolation

**Conflict Resolution**:
- [ ] Local backup before sync
- [ ] Merge operations are auditable
- [ ] No silent data loss

**Sync Security Verification**:
```csharp
// Token storage check
public async Task StoreTokenSecurely(string token)
{
    // GOOD: Platform secure storage
    await SecureStorage.SetAsync("oauth_token", token);

    // BAD: Regular preferences (not secure)
    // Preferences.Set("oauth_token", token);
}

// HTTPS verification
private HttpClient CreateSecureClient()
{
    var handler = new HttpClientHandler
    {
        // Enforce TLS 1.2+
        SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13
    };
    return new HttpClient(handler);
}
```

---

### Phase 4: Polish Security Checklist

**Sharing Features**:
- [ ] Shared exports don't contain unintended private data
- [ ] No tracking or analytics in shared files
- [ ] Expiration enforced on temporary shares
- [ ] Encrypted HTML exports use strong encryption

**Template Import**:
- [ ] Template JSON schema strictly validated
- [ ] No code execution from templates
- [ ] Large template files rejected
- [ ] Nested/recursive structures depth-limited

**Final Security Audit**:
- [ ] Complete codebase scan with AI assistant
- [ ] Dependency vulnerability check (NuGet packages)
- [ ] Static analysis tools run (built-in .NET analyzers)
- [ ] Secrets detection (no API keys, passwords in code)

---

## Security Testing

### Automated Security Tests

```csharp
public class SecurityTests
{
    [Fact]
    public void ImportJson_MalformedData_HandlesGracefully()
    {
        // Arrange
        var malformedJson = "{ invalid json }}}";

        // Act & Assert
        var act = () => _importService.ImportAsync(malformedJson);
        act.Should().NotThrow(); // Should handle, not crash
    }

    [Fact]
    public void ImportJson_ExcessivelyLarge_RejectsWithError()
    {
        // Arrange
        var largeJson = new string('a', 100_000_000); // 100MB

        // Act
        var result = _importService.ImportAsync(largeJson);

        // Assert
        result.Should().BeFailed();
        result.Error.Should().Contain("size limit");
    }

    [Fact]
    public void PropertyNickname_ExceedsLimit_Truncates()
    {
        // Arrange
        var longName = new string('x', 1000);

        // Act
        var property = new Property { Nickname = longName };
        var saved = _repository.CreateAsync(property);

        // Assert
        saved.Nickname.Length.Should().BeLessOrEqualTo(200);
    }

    [Fact]
    public void ExportFilePath_PathTraversal_Prevented()
    {
        // Arrange
        var maliciousInput = "../../etc/passwd";

        // Act & Assert
        var act = () => _exportService.ExportToPath(maliciousInput);
        act.Should().Throw<ArgumentException>();
    }
}
```

---

## Dependency Security

### NuGet Package Audit

Run regularly to check for known vulnerabilities:

```bash
# Check for vulnerabilities
dotnet list package --vulnerable

# Audit with more detail
dotnet list package --vulnerable --include-transitive
```

**Approved Packages** (version-pinned):

| Package | Approved Version | Notes |
|---------|-----------------|-------|
| sqlite-net-pcl | 1.9.x | Core dependency |
| CommunityToolkit.Mvvm | 8.2.x | Microsoft-maintained |
| CommunityToolkit.Maui | 7.x | Microsoft-maintained |
| LiveChartsCore | 2.x | Review changelog on update |
| QuestPDF | 2024.x | No network access |

**Prohibited Packages**:
- Any analytics/tracking SDKs
- Advertising SDKs
- Packages with known CVEs until patched

---

## Secure Development Practices

### Code Review Security Focus Areas

When reviewing PRs, pay special attention to:

1. **New Network Code**
   - Is HTTPS enforced?
   - Are certificates validated?
   - Is data minimized in transit?

2. **File Operations**
   - Are paths validated?
   - Are permissions appropriate?
   - Are temp files cleaned up?

3. **User Input Handling**
   - Is input bounded?
   - Is output encoded for display?
   - Are errors informative but not leaky?

4. **Cryptography**
   - Are standard libraries used?
   - No custom crypto implementations?
   - Appropriate key lengths?

### Security Anti-Patterns to Watch For

```csharp
// ANTI-PATTERN: Logging sensitive data
_logger.LogDebug($"User income: {income}");

// ANTI-PATTERN: Hardcoded secrets
private const string ApiKey = "sk-1234567890";

// ANTI-PATTERN: Disabled security checks
#pragma warning disable CA2000 // Dispose objects
ServicePointManager.ServerCertificateValidationCallback = (_, _, _, _) => true;

// ANTI-PATTERN: SQL string concatenation
var query = $"SELECT * FROM Users WHERE Name = '{username}'";

// ANTI-PATTERN: Storing secrets in preferences
Preferences.Set("oauth_token", token);
```

---

## Incident Response

### If a Security Issue is Found

1. **Assess Severity**
   - Does it affect user data?
   - Can it be exploited remotely?
   - Is user action required?

2. **Fix Priority**
   - Critical: Fix immediately, hotfix release
   - High: Fix in next release, expedite
   - Medium: Fix in planned release
   - Low: Backlog

3. **Disclosure**
   - Update privacy policy if needed
   - Notify users if data could be affected
   - Document in changelog

---

## Security Review Sign-Off

### Phase Completion Security Approval

Before marking a phase complete, security lead (or AI review) must sign off:

```
## Security Review Sign-Off

Phase: [1/2/3/4]
Date: [YYYY-MM-DD]
Reviewer: [Name or "AI-Assisted"]

### Checklist Completion
- [ ] All phase-specific security items checked
- [ ] AI security scan completed
- [ ] No Critical or High issues open
- [ ] Medium issues documented with mitigation timeline

### Findings Summary
- Critical: 0
- High: 0
- Medium: [count]
- Low: [count]
- Info: [count]

### Notes
[Any relevant observations or deferred items]

### Approval
[x] Phase approved for release
[ ] Phase blocked - issues must be resolved
```

---

## References

- [OWASP Mobile Top 10](https://owasp.org/www-project-mobile-top-10/)
- [.NET Security Best Practices](https://learn.microsoft.com/en-us/dotnet/standard/security/)
- [MAUI Secure Storage](https://learn.microsoft.com/en-us/dotnet/maui/platform-integration/storage/secure-storage)
- [OAuth 2.0 Security Best Practices](https://datatracker.ietf.org/doc/html/draft-ietf-oauth-security-topics)
