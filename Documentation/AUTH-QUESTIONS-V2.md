# Authentication System - Follow-up Questions

Based on your answers from AUTH-QUESTIONS.md, the specification has been updated. Here are the remaining questions that need clarification before implementation.

---

## 1. Database Encryption Strategy

### Q1.1: Token Encryption Requirement
**Question**: You asked "What needs to be encrypted?" in the original questions. Here's the clarification:

**Items that should be encrypted in the database**:
- AccessToken (contains sensitive user information and permissions)
- RefreshToken (long-lived credential that can be used to obtain new access tokens)
- IdToken (contains user identity information)

**Why encrypt**: If the database is compromised, attackers shouldn't be able to steal valid tokens and impersonate users.

**Encryption Options**:
- A) ASP.NET Data Protection API (automatic key management, easy to use)
- B) Custom encryption with explicit key management (stored in Azure Key Vault, AWS Secrets Manager, or environment variables)
- C) PostgreSQL pgcrypto extension (database-level encryption)
- D) Don't encrypt - rely on database security and transport security only

**Recommendation**: Option A (Data Protection API) for Prototype/V1 - simplest and secure. Option B for production if you need centralized key management.

**Answer**: Environment variable would be prefered : it can be used with both Aspire and an eventual
docker deployment ; if env variable are compatible with ASP.NET Data protection API, we can pair both.

---

## 2. Role Structure

### Q2.1: Role Definitions
**Question**: What roles will exist in the system?

**Context**: We need to define the initial roles that will be created in Keycloak and synced to the local database. These roles will be used for authorization throughout the application.

**Common patterns**:
- Basic: `user`, `admin`
- Content-focused: `user`, `editor`, `moderator`, `admin`
- Granular: `user`, `author`, `editor`, `publisher`, `moderator`, `admin`

**Prototype needs**: What roles do you need for the prototype to demonstrate authentication/authorization?

**Answer**: For now, the website should care about "user"s and "admin"s only.

---

### Q2.2: Role Claims in Keycloak
**Question**: How are roles represented in Keycloak tokens?

**Context**: Need to know how to extract roles from the token. Common approaches:
- A) Realm roles in the `realm_access.roles` claim
- B) Client roles in the `resource_access.{client-id}.roles` claim
- C) Custom claim (e.g., `roles` or `groups`)
- D) Combination of the above

**Recommendation**: Option A (realm roles) for simplicity

**Answer**:  Realm access roles.

---

### Q2.3: Role Authorization Usage
**Question**: How should roles be used for authorization in the application?

**Context**: Examples of how roles will control access:
- Can only `admin` users create/edit blog posts?
- Can only `editor` users manage content?
- Are there public vs. authenticated-only pages?

**Note**: You mentioned this was "out of scope" in Q3.2, but we need at least a basic example to implement role-based authorization correctly.

**Answer** (just one or two examples for prototype):
there are public pages that do not require authentications / can be accessed by empty "visitor" sessions, and there are pages that require specific auth.
Some pages will only be accessible with specific roles, such as "only admins can create blog posts" : pages for creation will require
the role to be present or return an error code.

---

## 3. Data Model Clarifications

### Q3.1: LocalAccount vs SocialAccount Separation
**Question**: Given the one-to-one relationship with Keycloak, do we still need separate LocalAccount and SocialAccount tables?

**Options**:
- A) Keep separate (better separation of concerns, future-proofing)
- B) Merge into single `User` table with provider columns (simpler for one-to-one relationship)

**Current design**: Separate tables with one-to-one relationship via Unique constraint on `LocalAccountId` in SocialAccount.

**Recommendation**: Option A (keep separate) - cleaner domain model even for one-to-one

**Answer**: Option A, keep separate.

---

### Q3.2: Role Storage Approach
**Question**: Should roles be stored as:

**Options**:
- A) Many-to-many relationship with Role table (as specified) - normalized, can add role metadata
- B) Simple string array/JSON column on LocalAccount - denormalized, simpler queries
- C) Both - normalized storage + cached string array for performance

**Recommendation**: Option A for prototype (proper normalization)

**Answer**:many to many relationship with Role table.

---

## 4. Session & Cookie Management

### Q4.1: Cookie Session Implementation
**Question**: For the cookie-based session, should we use:

**Options**:
- A) ASP.NET Core Cookie Authentication with custom session store (database)
- B) ASP.NET Core Session middleware + Authentication
- C) Custom cookie with manual session management

**Recommendation**: Option A - standard approach, integrates well with OIDC

**Answer**: If the ASP.NET core session middleware + Auth systems can be extended to be used directly, i would prefer that,
otherwise option A.

---

### Q4.2: Session Cookie Configuration
**Question**: What should the session cookie configuration be?

**Suggested configuration**:
```csharp
options.Cookie.Name = ".MediaNocta.Session";
options.Cookie.HttpOnly = true;
options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // HTTPS only
options.Cookie.SameSite = SameSiteMode.Strict;
options.ExpireTimespan = TimeSpan.FromHours(24); // Cookie lifetime (or unlimited?)
options.SlidingExpiration = true; // Extend on activity?
```

**Questions**:
- Cookie lifetime: Should cookies expire, or be session cookies (expire on browser close)?
- Sliding expiration: Extend cookie on each request?

**Answer**: cookies should not have a lifetime ; if the session linked cannot be found or has expired, the cookie should
be invalidated.

---

## 5. Gateway Authentication Details

### Q5.1: Gateway "Basic Approach" Clarification
**Question**: You mentioned "no middleware for auth verification for now, basic approach." What does this mean exactly?

**Interpretation options**:
- A) Gateway doesn't validate tokens in Prototype - just forwards requests (add validation in V1)
- B) Gateway validates tokens, but not through middleware - validates in each endpoint handler?
- C) Gateway has a simple middleware that validates JWT signature only (no complex policies)

**Recommendation**: Option C for Prototype - JWT validation middleware is essential for security

**Answer**: Gateway calls for JWT validation in each endpoint that requires it ; if an endpoint does not require auth,
it does not verify it.

---

### Q5.2: Gateway Token Extraction
**Question**: How should the Website include the access token when calling Gateway APIs?

**Options**:
- A) Extract access token from database session, add to Authorization header of HTTP calls
- B) Forward the session cookie, Gateway retrieves token from shared database
- C) Gateway accepts both cookie and Bearer token

**Recommendation**: Option A - explicit token passing, standard HTTP authentication

**Answer**: Option A : extract the token from the session and pass it. 
We will create a dedicated HTTPClient (that extends the regular one) that the website's services will extend and use. This custom
client will check if the user is authenticated, and if it is, it extracts the token and adds it to headers.
All of this happens *after* the middleware has made sure the session is valid and not expired.

---

## 6. GDPR Compliance Requirements

### Q6.1: GDPR Features Needed
**Question**: What specific GDPR features need to be implemented?

**Common GDPR requirements**:
1. **Right to Access**: User can download their personal data
2. **Right to Erasure**: User can request account deletion
3. **Right to Rectification**: User can update their information
4. **Consent Management**: Track and manage user consent for data processing
5. **Data Processing Log**: Audit trail of who accessed/modified user data
6. **Privacy Policy**: Display and require acceptance

**Priority classification** (using your preference):
- **Prototype**: (minimal for testing)
- **Version 1**: (must-have for launch)
- **Secondary**: (nice-to-have improvements)

**Answer**:

**Prototype**: Ignored for the prototype.

**Version 1**: Will be specified later.

**Secondary**: Will be specified later.

---

### Q6.2: Data Retention Policy
**Question**: How long should user data and sessions be retained?

**Context**:
- **Active sessions**: Kept until Keycloak token expires
- **Expired sessions**: How long before deletion?
- **Inactive accounts**: Should accounts with no activity be archived/deleted?
- **Audit logs**: How long to retain authentication logs?

**GDPR consideration**: Data should not be kept longer than necessary

**Answer**: Will be decided later.

---

## 7. Error Handling & User Experience

### Q7.1: Authentication Failure Messages
**Question**: What should users see when authentication fails?

**Scenarios**:
- Keycloak is down
- Token refresh fails (session expired)
- Invalid credentials at Keycloak
- User account disabled in Keycloak

**Options**:
- A) Generic error messages ("Authentication failed, please try again")
- B) Specific error messages per scenario
- C) Technical error details (for debugging in dev, generic in production)

**Recommendation**: Option C

**Answer**: Only detailed informations for now. We will make generic error messages for V1.

---

### Q7.2: Login/Logout Button Placement
**Question**: Where should login/logout buttons appear?

**Options**:
- A) Main navigation bar (always visible)
- B) User profile dropdown in navigation
- C) Both navigation and dedicated auth pages
- D) Other

**Answer**: A login button already exist on the website. This button should be replaced with a dropdown "Account",
with option "Profile"/"Logout" displayed.
The profile page will be a placeholder for now, the "Logout" page calls for a Keycloak Logout and destroys the local session afterwards.

---

## 8. Keycloak Configuration Details

### Q8.1: Keycloak Realm Configuration
**Question**: What should the Keycloak realm be named?

**Suggestion**: `medianocta` or `media-nocta`

**Answer**: A realm export already exists, and the website is already configured to login to keycloak.

---

### Q8.2: OIDC Client Configuration
**Question**: What should the OIDC client settings be?

**Suggested configuration**:
- **Client ID**: `medianocta-website` (or other name?)
- **Client Protocol**: openid-connect
- **Access Type**: confidential (requires client secret)
- **Valid Redirect URIs**: `https://localhost:7267/signin-oidc` (dev)
- **Post Logout Redirect URIs**: `https://localhost:7267/signout-callback-oidc`
- **Web Origins**: `https://localhost:7267`

**Answer** (confirm or provide different configuration):
"Authentication": {
"Keycloak": {
"Realm": "medianocta",
"ClientId": "medianocta-web",
"ClientSecret": "",
"RequireHttpsMetadata": false,
"SaveTokens": true,
"GetClaimsFromUserInfoEndpoint": true,
"UsePkce": true,
"Scopes": ["openid", "profile", "email", "roles"],
"CallbackPath": "/signin-oidc",
"SignedOutCallbackPath": "/signout-callback-oidc",
"RemoteSignOutPath": "/signout-oidc"
},
"Cookies": {
"LoginPath": "/Account/Login",
"LogoutPath": "/Account/Logout",
"AccessDeniedPath": "/Account/AccessDenied"
}
}

---

### Q8.3: Background Worker Realm Import
**Question**: How should the background worker handle realm configuration?

**Options**:
- A) Import realm JSON file on first startup only (check if realm exists)
- B) Import on every startup (overwrite existing configuration)
- C) Import if realm doesn't exist, update if exists but configuration differs
- D) Manual realm configuration (no background worker)

**Recommendation**: Option A for simplicity

**Answer**: The background worker already exists.

---

## 9. Implementation Scope

### Q9.1: Prototype Success Criteria
**Question**: What specific functionality must work for the prototype to be considered successful?

**Suggested criteria**:
- [YES] User can click "Login" and be redirected to Keycloak
- [NO] User can create account in Keycloak (self-registration)
- [YES] User can login with Keycloak credentials
- [YES] After login, user is redirected back to Website
- [YES] Website displays logged-in user's name and roles
- [YES] User can access a protected page (requires authentication)
- [YES] User can click "Logout" and session is terminated
- [YES] Local database contains user, social account, session, and role records
- [YES] Token refresh works seamlessly when token expires
- [YES] Website sends access token to gateway on request if present
- [YES] Gateway receives and verifies access token
---

### Q9.2: Out of Scope for Prototype
**Question**: What should explicitly NOT be implemented in the prototype?

**Suggestions**:
- Gateway authentication (focus on Website only)
- User profile editing
- GDPR features
- Session management UI
- Advanced error handling
- Other?

**Answer**: GDPR features, user profile edition, advanced error handling/resiliency are not part of the prototype

---

## 10. Technical Details

### Q10.1: Token Expiration Times
**Question**: What should the token expiration times be configured in Keycloak?

**Suggested values**:
- **Access Token Lifespan**: 5-15 minutes (short-lived)
- **Refresh Token Lifespan**: 30 days (long-lived)
- **SSO Session Idle**: 30 minutes
- **SSO Session Max**: 10 hours

**Answer** (confirm or provide different values): 
Already configured
---

### Q10.2: Proactive Refresh Threshold
**Question**: For V1 proactive refresh, how many minutes before expiration should we refresh the token?

**Suggestion**: 5 minutes (if token expires in < 5 minutes, refresh it)

**Answer**:Will be defined later

---

## 11. Testing & Validation

### Q11.1: Test Accounts
**Question**: Should the Keycloak setup create test accounts for development?

**Options**:
- A) Yes, create test accounts with different roles (e.g., testuser@example.com, testadmin@example.com)
- B) No, use self-registration to create accounts manually
- C) Create one admin account only

**Recommendation**: Option A for easier testing

**Answer**: Already configured

---

## Instructions

Please answer the questions above by filling in the "Answer:" sections. These answers will allow me to finalize the specification and begin implementation planning.

If any questions are unclear or you'd like to discuss options before answering, let me know!
