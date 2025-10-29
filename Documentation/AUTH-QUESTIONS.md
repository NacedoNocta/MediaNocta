# Authentication System - Questions & Clarifications

This document contains questions that need answers to finalize the authentication specification.

---

## 1. Token Management

### Q1.1: Token Storage Location
**Question**: Where should tokens be stored in the Website application?

**Options**:
- A) HTTP-only cookies (more secure, not accessible via JavaScript)
- B) Browser localStorage (easier access, but vulnerable to XSS)
- C) In-memory only with cookie session ID reference

**Recommendation**: Option A (HTTP-only cookies) for security

**Answer**:
I was thinking to create a local database that the website will use to store users,
currently active sessions etc. In that planning, tokens would be stored in-db. 
Cookies are only used to allow the user to keep its sessions across pages. 
---

### Q1.2: Token Refresh Strategy
**Question**: When should we attempt to refresh tokens?

**Options**:
- A) On every request if token expires within X minutes (proactive)
- B) Only when token is expired and request fails (reactive)
- C) Background job that refreshes before expiration
- D) Combination of A and C

**Recommendation**: Option D for best UX

**Answer**:
Option C would be better later. For a prototype : the local session contains metadata for the keycloak token,
which includes its expiration time ; the server should, if the token is expired, try to refresh it before continuing 
with the request (as to avoid raising an error). If the refresh fails, the local session is terminated. If it succeeds, 
the local session's access and refresh tokens are refresh, as well as roles.
This process should be entirely invisible to the user if it succeeds.
For a V1, we will extend the check to proactively refresh.

---

## 2. Gateway Authentication

### Q2.1: Gateway Token Validation Method
**Question**: How should the Gateway validate tokens?

**Options**:
- A) Direct validation with Keycloak's userinfo/introspection endpoint (most secure, higher latency)
- B) JWT signature validation using Keycloak's public keys (faster, cached keys)
- C) Trust Website-issued session cookie (fastest, but creates tight coupling)

**Recommendation**: Option B with key caching for production balance

**Answer**:
For prototype, JWT signature validation only. Later, we might add direct validation with introspection in case of failure.
---

### Q2.2: Token Forwarding to Backend APIs
**Question**: Should backend APIs (BlogApi, ActivityAPI) perform their own token validation, or trust the Gateway?

**Options**:
- A) Trust the Gateway (simpler, Gateway is authentication boundary)
- B) Independent validation in each API (defense in depth, more resilient)
- C) Hybrid: Gateway validates, APIs verify claims

**Recommendation**: Option A for microservices simplicity, with network security

**Answer**: 
APIs should trust the gateway ; the APIs are not publicly accessible anyway.

---

## 3. Authorization & Permissions

### Q3.1: Role Management
**Question**: Where should user roles and permissions be defined and stored?

**Options**:
- A) Entirely in Keycloak (roles in tokens)
- B) Local database with periodic sync from Keycloak
- C) Keycloak for authentication roles, local DB for application-specific permissions
- D) Entirely in local database

**Recommendation**: Option C for flexibility

**Answer**: 
Roles should entirely exist in Keycloak and be passed along the token, but the applications
will extract and store relevant roles locally every time an access token is passed (on initial login AND on refresh) :
if a needed role is found, it is added to the local user ; otherwise, it should be removed.

---

### Q3.2: Protected Endpoints
**Question**: Which endpoints/pages require authentication?

**Context**: Need to define:
- Public pages (e.g., blog reading)
- Authenticated pages (e.g., user profile)
- Admin pages (e.g., content management)
- API endpoints protection strategy

**Answer**: Out of scope, we only care about making the system work.
If possible and appropriate, we should be able to use C# 's auth mechanisms where possible, and our implemtentation
should extend them.

---

## 4. User Account Management

### Q4.1: User Registration
**Question**: Where do users register for accounts?

**Options**:
- A) Only through Keycloak (users manage everything there)
- B) Website has registration form, creates Keycloak account via Admin API
- C) No registration - admin creates accounts manually
- D) Self-registration in Keycloak, Website auto-creates local account on first login

**Recommendation**: Option D for simplicity

**Answer**: Option D : Self registration in Keycloak, Website auto-creates local account on first login.
This local account is impossible to connect to directly (it can only be used through a social login).
On first login, the local account automatically uses the username and email found in the keycloak Token.

---

### Q4.2: User Profile Synchronization
**Question**: Should we periodically sync user data from Keycloak to local database?

**Options**:
- A) Yes, sync on every login
- B) Yes, background job syncs periodically
- C) No, only fetch from Keycloak when needed
- D) Store minimal data locally, fetch from Keycloak for profile views

**Recommendation**: Option A (sync on login) for data consistency

**Answer**: Everytime the server receives a new Access token (on first login AND on refresh), the *roles* should be updated.
For a later version, the full profile should also update, but it is not necessary for prototyping.

---

### Q4.3: Multiple Social Accounts
**Question**: Can a single local account be linked to multiple social accounts/providers?

**Context**: Future-proofing for potential Google/GitHub OAuth providers

**Options**:
- A) Yes, one-to-many relationship
- B) No, one-to-one only (simpler for V1)

**Recommendation**: Design for A (one-to-many), but implement B for Prototype/V1

**Answer**: The website should implement a one-to-one link : the responsability to manage
multiple providers is delegated to Keycloak, the website should only care about the Keycloak's session.

---

## 5. Session Management

### Q5.1: Session Cleanup
**Question**: How should expired sessions be cleaned up?

**Options**:
- A) Background job runs every X hours to delete expired sessions
- B) Cleanup on user's next login
- C) Let database handle with table partitioning/retention policy
- D) Combination of A and B

**Recommendation**: Option D

**Answer**: For prototyping, option B (Cleanup on user's next login) is prefered. Later, a mix of A and C might be used
through a dedicated app.

---

### Q5.2: Session Invalidation
**Question**: When should local sessions be forcibly invalidated?

**Scenarios to consider**:
- User changes password in Keycloak
- User account disabled in Keycloak
- Security event detected
- Admin action

**Answer**: We should entirely trust keycloak sessions : as long as the access and refresh token are valid and usable (
which means the keycloak session still exists), the local sessions continues to exist ; if either are invalidated
and a valid keycloak session cannot be found, the local session is terminated.

---

### Q5.3: Concurrent Sessions
**Question**: Should users be allowed to have multiple active sessions (different devices)?

**Options**:
- A) Yes, unlimited sessions
- B) Yes, but limit to X sessions (e.g., 5)
- C) No, only one session allowed (logout elsewhere)

**Recommendation**: Option B for security and usability balance

**Answer**: One session allowed. If a new one is created, the previous one is overridden

---

## 6. Logout & Security

### Q6.1: Logout Flow
**Question**: What should happen when a user logs out?

**Options**:
- A) Invalidate local session only
- B) Invalidate local session + call Keycloak logout endpoint
- C) Option B + Single Logout (SLO) to logout from all services

**Recommendation**: Option B for V1, Option C for later

**Answer**: Option B : call to keycloak for logout. Keycloak will handle SLO itself though
back-channel logout mechanisms.

---

### Q6.2: Keycloak Unavailability
**Question**: What should happen if Keycloak is down/unreachable?

**Options**:
- A) Deny all authentication (fail closed)
- B) Allow existing sessions to continue, block new logins (graceful degradation)
- C) Temporary local authentication fallback

**Recommendation**: Option B for V1

**Answer**: If keycloak is unreachable, local sessions should be terminated ; pages that
do not require auth will still be accessible, but the other pages should be rendered inaccessible.

---

## 7. Development & Deployment

### Q7.1: Keycloak Deployment
**Question**: How will Keycloak be deployed?

**Options**:
- A) Docker Compose alongside application (development)
- B) Separate production-grade deployment (clustered, HA)
- C) Managed Keycloak service (e.g., Keycloak Cloud)

**Note**: Need separate instances for dev and production

**Answer**: Currently, Keycloak is managed through Aspire. Later, production will use its own keycloak instance.

---

### Q7.2: Keycloak Configuration
**Question**: Should Keycloak configuration (realm, clients, roles) be:

**Options**:
- A) Manually configured
- B) Automated via Keycloak Admin API
- C) Infrastructure-as-Code (Terraform, realm import/export)

**Recommendation**: Option C for reproducibility

**Answer**: Currently, a background worker manages the configuration through realm import/export.

---

## 8. Technical Implementation Details

### Q8.1: Blazor Authentication State
**Question**: How should Blazor components access authentication state?

**Options**:
- A) Built-in AuthenticationStateProvider
- B) Custom authentication service with state management
- C) Both (AuthenticationStateProvider backed by custom service)

**Recommendation**: Option C

**Answer**: I'd prefer to extend the Auth systems from blazor, so probably C ?

---

### Q8.2: YARP Middleware Order
**Question**: Where in the YARP pipeline should authentication middleware be placed?

**Context**: Need to ensure proper order with CORS, rate limiting, etc.

**Answer**: Yarp does not uses a middleware for auth verification for now, we want something more basic at first.

---

### Q8.3: Database Encryption
**Question**: What encryption approach for tokens in database?

**Options**:
- A) ASP.NET Data Protection API (key management handled)
- B) Custom encryption with Azure Key Vault / AWS KMS
- C) Database-level encryption (PostgreSQL encrypted columns)
- D) Application-level encryption with managed keys

**Recommendation**: Option A for simplicity in V1

**Answer**: Unsure ? What needs to be encrypted ?

---

## 9. User Experience

### Q9.1: Login UI/UX
**Question**: Should the login page be:

**Options**:
- A) Redirect to Keycloak's hosted login page
- B) Embedded Keycloak login iframe in Website
- C) Custom login form that calls Keycloak API

**Recommendation**: Option A (standard OIDC flow)

**Answer**: Option A should already be implemented.

---

### Q9.2: Session Timeout Behavior
**Question**: How should we handle session timeout while user is active?

**Options**:
- A) Hard timeout - user logged out regardless
- B) Sliding expiration - extend session on activity
- C) Show warning modal before timeout with option to extend

**Recommendation**: Option B with maximum session lifetime

**Answer**: The local session's duration should be limitless to allow its actual duration to be decided by Keycloak's session.
Since the session is verified on each request through token expiration checking, if keycloak's session cannot be
extended, the session should be a hard timeout.

---

## 10. Scope & Non-Functional Requirements

### Q10.1: Expected User Load
**Question**: What's the expected number of concurrent users?

**Context**: Helps determine caching strategy, session storage, Keycloak sizing

**Answer**: Unknown.

---

### Q10.2: Compliance Requirements
**Question**: Are there any compliance requirements (GDPR, CCPA, etc.)?

**Context**: Affects data retention, user data handling, consent management

**Answer**: GDPR.

---

## Instructions

Please answer the questions above by filling in the "Answer:" sections. If you're unsure about any question, we can discuss options together. Once answered, I'll update the AUTH-SPECIFICATION.md with the finalized details.
