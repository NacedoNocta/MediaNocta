# Authentication System Specification

## Overview

Media Nocta will implement a secure authentication system using Keycloak as the primary identity provider (IdP), with OpenID Connect (OIDC) protocol for SSO-style authentication across all services.

**Note**: Keycloak server, realm (`medianocta`), client (`medianocta-web`), and test users are already configured and running via .NET Aspire. This specification focuses on implementing the Website authentication integration, Gateway token validation, and local session/account management.

## Architecture Components

### 1. Keycloak Identity Provider (Already Configured)
- **Purpose**: Central identity provider managing user accounts, authentication, and session validity
- **Status**: ✅ Already configured via .NET Aspire with realm `medianocta` and client `medianocta-web`
- **Responsibilities**:
  - User account management (creation, updates, deletion)
  - Authentication flows (login, logout)
  - Session management and token issuance
  - OAuth2/OIDC protocol implementation
  - Roles: `website-user` and `website-admin` (defined as realm roles)

### 2. API Gateway (YARP)
- **Purpose**: Request routing with endpoint-specific token validation
- **Responsibilities**:
  - Route requests to appropriate backend services
  - Validate JWT tokens **only for endpoints that require authentication**
  - Extract Bearer token from Authorization header
  - Validate JWT signature using Keycloak's public keys (cached)
  - Verify token expiration and issuer
  - Extract user claims and roles from token
  - Forward authenticated requests to backend APIs with user context
  - Backend APIs trust the Gateway as the authentication boundary
  - Return 401/403 responses for invalid tokens
  - Public endpoints (no auth required) bypass token validation

**Note**: Token validation is performed in each protected endpoint handler, not as global middleware.

### 3. Website (Blazor Server)
- **Purpose**: User-facing application with local session and account management
- **Configuration**: Already configured with OIDC client `medianocta-web`
- **Responsibilities**:
  - Initiate OIDC authentication flows (redirect to Keycloak)
  - Maintain local user, session, and role database
  - Auto-create local accounts on first Keycloak login
  - Manage Keycloak social account → local account mapping (one-to-one)
  - Handle token refresh and session expiration via authentication middleware
  - Sync roles (`website-user`, `website-admin`) from Keycloak `realm_access.roles` claim to local database
  - Use cookies for session persistence (session ID only, no expiration - validated against database)
  - Store tokens encrypted in database using ASP.NET Data Protection API with environment variable keys
  - Provide authentication state to Blazor components via extended AuthenticationStateProvider
  - Enforce single concurrent session per user (new session overrides old)
  - Custom HttpClient for API calls that automatically extracts and includes access token in Authorization header
  - UI: "Account" dropdown (replacing login button) with "Profile" (placeholder) and "Logout" options

## Authentication Flow

### Initial Login Flow

1. **User Access**: User attempts to access protected resource on Website
2. **Redirect to Keycloak**: OIDC middleware redirects to Keycloak hosted login page (`/signin-oidc`)
3. **User Authentication**: User provides credentials to Keycloak (self-registration disabled for prototype)
4. **Callback**: Keycloak redirects back to Website at `/signin-oidc` with authorization code
5. **Token Exchange**: Website exchanges authorization code for tokens (access, refresh, ID tokens)
6. **Account Creation/Linking**:
   - Extract user info (sub, username, email) from ID token
   - Extract roles from access token `realm_access.roles` claim
   - Check if Keycloak user ID (`sub` claim) exists in local database
   - If not, auto-create LocalAccount with username/email from token
   - Create/update SocialAccount record linking Keycloak user to LocalAccount
   - Sync roles from token to local database:
     - Add `website-user` and `website-admin` roles if present in token
     - Remove roles from local account if not present in token
7. **Session Creation**:
   - Check for existing active session for this user
   - If exists, invalidate/delete old session (single session enforcement)
   - Create new LocalSession with tokens and expiration metadata
   - Encrypt tokens using ASP.NET Data Protection API (keys from environment variables)
   - Store encrypted tokens in database
   - Issue session cookie to browser:
     - HttpOnly, Secure, SameSite=Strict
     - Contains session ID only (no token data)
     - No expiration (persistent cookie, validated against database session)
8. **User Access Granted**: User accesses protected resource with local session

### Session Validation & Refresh Flow

**Executed on every request to protected resources**

1. **Request Interception**: Authentication middleware intercepts each request
2. **Cookie Validation**: Extract session ID from cookie, look up LocalSession in database
3. **Token Expiration Check**: Check AccessToken expiration time from session metadata
4. **Token Refresh (if expired)**:
   - **Prototype**: If access token expired, attempt refresh with RefreshToken
   - **V1**: If access token expires within X minutes, proactively refresh
   - Call Keycloak token endpoint with refresh token
   - If successful:
     - Update LocalSession with new access/refresh tokens and expiration
     - Extract and update roles from new access token
     - Process invisible to user (seamless)
   - If failed (refresh token invalid/expired):
     - Delete/invalidate LocalSession
     - Redirect user to login
5. **Request Processing**: If session valid and token refreshed (if needed), continue with request

### API Request Flow (via Gateway)

1. **Frontend Request**: Website makes HTTP call to Gateway endpoint using custom HttpClient
2. **Custom HttpClient Processing**:
   - Check if user is authenticated (after middleware has validated session)
   - If authenticated, retrieve LocalSession from database
   - Decrypt and extract access token
   - Add access token to Authorization header as Bearer token
3. **Gateway Validation** (for protected endpoints only):
   - Check if endpoint requires authentication
   - If not required, forward request immediately (skip validation)
   - If required:
     - Extract Bearer token from Authorization header
     - Validate JWT signature using Keycloak's public keys (cached)
     - Verify token expiration and issuer
     - Extract user claims and roles from token
4. **Forward to Backend**:
   - If valid (or public endpoint): forward request to backend API (BlogApi, ActivityAPI)
   - Backend APIs trust Gateway validation (no re-validation)
   - If invalid: return 401 Unauthorized

**Note**: APIs are not publicly accessible; Gateway is the only entry point.

## Data Model

### Local Database Schema

#### Role Table
- `Id` (Primary Key, GUID)
- `Name` (Unique, String) - "user" or "admin"
- `Description` (String)
- `CreatedAt` (DateTime)

**Note**: Only two roles for prototype: `website-user` and `website-admin`. Synced from Keycloak realm roles.

#### LocalAccountRole Table (Junction)
- `LocalAccountId` (Foreign Key → LocalAccount)
- `RoleId` (Foreign Key → Role)
- **Primary Key**: (LocalAccountId, RoleId)
- `AssignedAt` (DateTime)

**Note**: Roles synced from Keycloak `realm_access.roles` claim on every token receipt (login + refresh). If role missing from token, it's removed from local account.

#### LocalAccount Table
- `Id` (Primary Key, GUID)
- `Username` (Unique, String) - from Keycloak token on creation
- `Email` (String) - from Keycloak token on creation
- `CreatedAt` (DateTime)
- `UpdatedAt` (DateTime)
- `IsActive` (Boolean)

**Note**: LocalAccount cannot be used for direct login - only accessible through Keycloak social login.

#### SocialAccount Table
- `Id` (Primary Key, GUID)
- `LocalAccountId` (Foreign Key → LocalAccount, Unique)
- `Provider` (String) - Always "keycloak" for now
- `ProviderUserId` (String) - Keycloak user ID (sub claim)
- `ProviderUsername` (String) - Synced from Keycloak
- `ProviderEmail` (String) - Synced from Keycloak
- `CreatedAt` (DateTime)
- `UpdatedAt` (DateTime)
- **Unique Constraint**: (Provider, ProviderUserId)

**Note**: One-to-one relationship with LocalAccount. Keycloak handles multiple identity providers; Website only sees unified Keycloak identity.

#### LocalSession Table
- `Id` (Primary Key, GUID)
- `LocalAccountId` (Foreign Key → LocalAccount, Unique) - enforces one session per user
- `AccessToken` (String) - Encrypted with ASP.NET Data Protection API (keys from env vars)
- `RefreshToken` (String) - Encrypted with ASP.NET Data Protection API
- `IdToken` (String) - Encrypted with ASP.NET Data Protection API
- `AccessTokenExpiration` (DateTime) - for expiration checking
- `RefreshTokenExpiration` (DateTime) - for cleanup
- `CreatedAt` (DateTime)
- `LastAccessedAt` (DateTime)
- `IsActive` (Boolean)

**Note**: Session duration is unlimited locally; actual duration controlled by Keycloak token expiration. Only one active session per user - new session creation deletes previous session.

## Implementation Priorities

### Prototype (Immediate Implementation)

**Goal**: Basic authentication flow working end-to-end

**Prerequisites**: ✅ Keycloak already configured with realm `medianocta`, client `medianocta-web`, and test users

1. **Website Authentication Middleware**:
   - Verify existing OIDC configuration in appsettings.json (already configured)
   - Install Microsoft.AspNetCore.Authentication.OpenIdConnect and Microsoft.AspNetCore.Authentication.Cookies
   - Configure ASP.NET Core Session middleware + Authentication (or Cookie Authentication with custom session store)
   - Configure cookie with no expiration (persistent, validated against database)
   - Implement OIDC callback handler (`/signin-oidc`) with account/session creation logic
   - Implement logout endpoint (`/Account/Logout`):
     - Call Keycloak logout endpoint (`/signout-oidc`)
     - Delete local session from database
     - Keycloak handles back-channel SLO
   - Configure ASP.NET Data Protection API with keys from environment variables

2. **Database Schema**:
   - Create entities: LocalAccount, SocialAccount, LocalSession, Role, LocalAccountRole
   - Configure one-to-one LocalAccount ↔ SocialAccount relationship
   - Configure one-to-one LocalAccount ↔ LocalSession relationship (single session)
   - Configure many-to-many LocalAccount ↔ Role relationship
   - Add to DatabaseManager DbContext
   - Generate EF Core migrations (do not apply - per guidelines)
   - Implement account auto-creation logic from Keycloak tokens (sub, username, email)
   - Implement role sync logic from `realm_access.roles` claim (add/remove user/admin)

3. **Token Refresh Logic**:
   - Implement authentication middleware that runs on each request to protected resources
   - Check access token expiration from session metadata
   - If access token expired:
     - Call Keycloak token endpoint with refresh token
     - Update session with new encrypted tokens and expiration times
     - Extract and sync roles from new access token
     - Continue request (invisible to user)
   - If refresh fails (Keycloak session expired):
     - Delete local session
     - Redirect to login
   - Show detailed error messages for debugging (generic messages in V1)

4. **Session Management**:
   - On user login, check for existing session and delete if found (single session enforcement)
   - Basic cleanup on user login: delete expired sessions from database

5. **Custom HttpClient for API Calls**:
   - Create custom HttpClient class that extends standard HttpClient
   - Website services use this custom client for Gateway API calls
   - Custom client logic:
     - Check if user is authenticated (after middleware validation)
     - Retrieve LocalSession from database
     - Decrypt and extract access token
     - Add access token to Authorization header as Bearer token
     - Make HTTP request

6. **Gateway Token Validation**:
   - Implement JWT validation helper/service
   - In each protected endpoint handler:
     - Check if endpoint requires authentication
     - If yes, call validation helper
     - Extract Bearer token from Authorization header
     - Validate JWT signature using Keycloak public keys (cache keys)
     - Verify expiration and issuer
     - Extract user claims and roles
     - Return user context to endpoint
   - Public endpoints skip validation

7. **UI Updates**:
   - Replace existing login button with "Account" dropdown
   - Dropdown shows when authenticated:
     - "Profile" option (placeholder page for now)
     - "Logout" option (calls `/Account/Logout`)
   - Display authenticated user's name and roles
   - Extend Blazor's AuthenticationStateProvider for auth state
   - Create protected test pages:
     - Authenticated-only page (requires any auth)
     - Admin-only page (requires `admin` role) - e.g., blog post creation
   - Use standard ASP.NET Core authorization attributes (`[Authorize]`, `[Authorize(Roles = "admin")]`)

### Prototype Success Criteria

- ✅ User can click "Account" and be redirected to Keycloak login
- ✅ User can login with existing Keycloak credentials (test accounts)
- ✅ After login, user is redirected back to Website (`/signin-oidc`)
- ✅ Website displays logged-in user's name and roles in UI
- ✅ User can access authenticated-only page
- ✅ User can access admin-only page (if has `admin` role)
- ✅ User is denied access to admin page without `admin` role (403)
- ✅ User can click "Logout" and session is terminated
- ✅ Local database contains LocalAccount, SocialAccount, LocalSession, and role records
- ✅ Token refresh works seamlessly when access token expires
- ✅ Website sends access token to Gateway via custom HttpClient
- ✅ Gateway receives and validates access token for protected endpoints
- ✅ Gateway allows public endpoint access without token

**Out of Scope**: User self-registration in Keycloak, GDPR features, user profile editing, advanced error handling/resiliency

### Version 1 (First Release)

**Builds on Prototype with production-ready features**

1. **Enhanced Session Management**:
   - Proactive token refresh (refresh when expiring within X minutes - threshold TBD)
   - Dedicated background service for session cleanup
   - Better error handling for Keycloak unavailability (fail closed)
   - Full user profile sync on token refresh (not just roles)
   - Generic user-friendly error messages (instead of detailed errors)

2. **Enhanced Gateway Integration**:
   - Improve Keycloak public key caching with expiration
   - Add fallback to Keycloak introspection endpoint on validation failure
   - Enhanced user context forwarding to backend APIs
   - More granular authorization rules based on roles

3. **Security Hardening**:
   - Token encryption in database (implement encryption strategy from Q8.3)
   - CSRF protection for state-changing operations
   - Secure cookie configuration (HttpOnly, Secure, SameSite=Strict)
   - HTTPS enforcement across all services
   - Audit logging for security events (login, logout, failed auth)

4. **User Profile**:
   - User profile page showing account info
   - Display linked social accounts
   - Logout functionality

5. **Error Handling**:
   - User-friendly error messages for auth failures
   - Comprehensive logging for security events
   - Graceful handling of Keycloak unavailability (terminate sessions, block auth pages)
   - Retry logic for transient Keycloak errors

6. **Configuration & Deployment**:
   - Environment-specific Keycloak settings (dev via Aspire, production separate instance)
   - Secure credential management (User Secrets, environment variables)
   - Separate Keycloak instances for dev/staging/production
   - Automated realm configuration export/import for reproducibility

7. **GDPR Compliance** (requirements TBD):
   - To be specified based on legal requirements
   - Potential features: data export, account deletion, audit trails, consent management

### Secondary / Nice-to-haves (Post-V1)

**Future enhancements and optimizations**

1. **Multi-Provider Support** (via Keycloak):
   - Configure additional identity providers in Keycloak (Google, GitHub, etc.)
   - Keycloak handles provider federation
   - Website continues to use unified Keycloak identity

2. **Advanced Authorization**:
   - Role-based access control (RBAC)
   - Permission management UI
   - Custom authorization policies

3. **Account Management**:
   - User profile editing
   - Account deletion/deactivation
   - Social account unlinking

4. **Session Management UI**:
   - View active sessions
   - Revoke sessions remotely
   - Session history/audit log

5. **Performance Optimizations**:
   - Token caching in Gateway
   - Distributed session storage (Redis)
   - Reduced Keycloak round-trips

6. **Advanced Features**:
   - Remember me functionality (longer refresh token expiration)
   - Two-factor authentication (2FA) via Keycloak
   - Enhanced SLO monitoring and logging
   - Refresh token rotation for added security
   - Support for concurrent sessions (remove single-session restriction)

## Security Considerations

- **Token Storage**: Tokens stored in database (encryption strategy TBD - see Q8.3). Session ID only in cookies.
- **Transport Security**: All communications over HTTPS/TLS
- **Cookie Security**: HttpOnly, Secure, SameSite=Strict flags for session cookies
- **CSRF Protection**: Anti-forgery tokens for state-changing operations
- **Token Validation**: Always validate tokens server-side in Gateway and Website
- **Single Session**: Only one active session per user to limit attack surface
- **Trust Boundaries**: Gateway validates tokens, backend APIs trust Gateway
- **Keycloak Dependency**: Local sessions fully dependent on Keycloak token validity
- **Principle of Least Privilege**: Grant minimum required roles/permissions
- **Secret Management**: Never commit secrets, use User Secrets/environment variables
- **Audit Logging**: Log all authentication events for security monitoring and GDPR compliance
- **Keycloak Unavailability**: Fail closed - terminate sessions if Keycloak unreachable

## Technology Stack

- **Identity Provider**: Keycloak (via .NET Aspire)
  - Realm: `medianocta`
  - Client: `medianocta-web` (confidential, PKCE enabled)
  - Scopes: `openid`, `profile`, `email`, `roles`
  - Roles: `user`, `admin` (realm roles in `realm_access.roles` claim)
- **Protocol**: OpenID Connect (OIDC) with OAuth2
- **Website Framework**: ASP.NET Core with Blazor Server
- **Authentication Libraries**:
  - Microsoft.AspNetCore.Authentication.OpenIdConnect
  - Microsoft.AspNetCore.Authentication.Cookies (or Session middleware + Auth)
  - Microsoft.AspNetCore.Components.Authorization (for Blazor auth state)
- **Gateway Framework**: YARP (Yet Another Reverse Proxy)
- **Database**: PostgreSQL (existing)
- **ORM**: Entity Framework Core
- **Token Handling**: System.IdentityModel.Tokens.Jwt
- **Encryption**: ASP.NET Data Protection API with keys from environment variables

## Key Design Decisions (from Q&A)

1. **Keycloak**: ✅ Already configured via Aspire - NOT part of this implementation
2. **Token Storage**: Database with encryption (Data Protection API + env vars), cookies for session ID only
3. **Token Encryption**: ASP.NET Data Protection API with keys from environment variables
4. **Cookie Lifetime**: No expiration - persistent cookie validated against database session
5. **Token Refresh**: Reactive (prototype) → Proactive (V1, threshold TBD)
6. **Gateway Auth**: Endpoint-specific JWT validation (not global middleware)
7. **Backend APIs**: Trust Gateway validation (no independent validation)
8. **Role Management**: Two roles (`website-user`, `website-admin`), synced from `realm_access.roles` claim
9. **Authorization**: Public pages (no auth), authenticated pages, role-specific pages (e.g., admin-only)
10. **User Registration**: Use existing test accounts (self-registration disabled for prototype)
11. **Account Linking**: One-to-one Keycloak → LocalAccount (Keycloak handles multi-provider)
12. **Data Model**: Separate LocalAccount and SocialAccount tables, many-to-many role relationship
13. **Session Policy**: Single session per user (new overrides old)
14. **Session Implementation**: ASP.NET Core Session middleware + Auth (if extendable), else Cookie Authentication
15. **Session Cleanup**: On next login (prototype), background job (V1)
16. **Session Invalidation**: Fully dependent on Keycloak token validity
17. **Logout**: Call Keycloak `/signout-oidc`, delete local session, Keycloak handles back-channel SLO
18. **UI**: "Account" dropdown with "Profile" (placeholder) and "Logout"
19. **API Calls**: Custom HttpClient extracts token from session and adds to Authorization header
20. **Error Messages**: Detailed (prototype) → Generic user-friendly (V1)
21. **Blazor Auth**: Extend built-in AuthenticationStateProvider
22. **GDPR**: Deferred to V1 (requirements TBD)

## Deferred to V1

The following items are out of scope for the prototype and will be specified/implemented in V1:

1. **GDPR Compliance**: Specific requirements and features TBD
2. **Data Retention Policy**: Session and account cleanup policies TBD
3. **Proactive Token Refresh**: Threshold for proactive refresh TBD
4. **User Profile Editing**: Full profile management UI
5. **Advanced Error Handling**: Retry logic, resilience patterns
6. **User Self-Registration**: Enable Keycloak self-registration

## Implementation Ready

This specification is now complete and ready for implementation. All questions have been answered, and the scope is clearly defined:

**Prototype Focus**:
- Website OIDC integration with existing Keycloak
- Local database entities for accounts, sessions, and roles
- Token refresh middleware
- Custom HttpClient for authenticated API calls
- Gateway endpoint-specific JWT validation
- Basic UI with Account dropdown and protected pages

**Prerequisites**: ✅ Keycloak already configured - no setup required
