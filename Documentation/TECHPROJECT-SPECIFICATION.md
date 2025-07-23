# Tech Projects Section - Technical Specification

## 1. Overview

The Tech Projects section provides a portfolio showcase for technology projects through static web pages with dynamic activity feeds. Project pages are hardcoded Blazor components, while activity feeds (updates, patch notes, dev commentary) are sourced from the database using the existing IActivity system.

## 2. Priority Classification

### Prototype
- Basic project listing page with hardcoded project banners
- Simple static project detail pages with hardcoded content
- Basic activity feed integration using existing blog infrastructure

### Version 1
- Complete project detail pages with expandable information panels
- Full activity feed with filtering and chronological sorting
- Responsive design and navigation integration
- SEO optimization and metadata management

### Secondary / Nice-to-haves
- Advanced filtering and search capabilities for activity feeds
- Project analytics and engagement tracking
- Rich media support for project showcases
- Interactive demos or embedded previews

## 3. Architecture Integration

### Domain Model
- **TechUpdate**: New concrete implementation inheriting from `IActivity` (TechProjectLibrary)
- **ProjectKeys**: Hardcoded enum in SharedLibrary defining project slug identifiers
- **ActivityType**: Add "TechUpdate" to existing activity type system
- **ThemeColor**: Define Bootstrap CSS class for tech project activities

### Service Structure
- New **TechProjectAPI** to handle tech update operations
- New **TechProjectLibrary** for domain models
- Use existing **APIGateway** routing patterns for tech update data
- Frontend integration through **Website** project with static pages in `/Components/Tech/`

### Database Schema
- Extend existing `AppDbContext` in DatabaseManager
- New `TechUpdates` table with EF Core entity for activity feed items
- Use ProjectId field (enum-based slug) to associate activities with specific static project pages

## 4. Data Models

### TechUpdate Entity
```csharp
public class TechUpdate : IActivity
{
    // Inherited from IActivity
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string ActivityType => "TechUpdate";
    public string ThemeColor { get; set; }
    
    // TechUpdate-specific properties
    public string ProjectId { get; set; } // Project slug from ProjectKeys enum
    public string UpdateType { get; set; } // Flexible meta-tag system, supports any string
    public string Content { get; set; } // Markdown content
}
```

### Static Project Pages
- Project information stored as hardcoded Blazor components/pages
- Each project has a unique identifier for activity association
- Project metadata (title, description, links) embedded in component code

## 5. API Endpoints

### New TechProjectAPI
```
GET /api/tech-updates - Get tech updates
GET /api/tech-updates?projectId={slug} - Get tech updates filtered by project slug
GET /api/tech-projects/last-activity - Get cached last activity dates for all projects in bulk
```

### ActivityAPI Integration
```
GET /api/activities/recent - Should return all activities indifferentiated, including the newly added tech updates.
GET /api/activities/tech - Get all tech updates for homepage feed (handled by existing ActivityAPI)
```

## 6. Frontend Implementation

### Page Structure
```
/tech - Static project listing with hardcoded banner grid
/tech/{project-slug} - Static project detail pages
/tech/{project-slug}/{subsection-name} - Specific routed pages for technical/functional content sections
```

### Components (Located in `/Components/Tech/`)
- **ProjectBanner**: Grid item component showing title, description, illustration, last activity date (cached value, "no activity" if none), tags
- **ProjectDetail**: Static project page components (one per project) with all project information
- **ProjectActivity**: Activity feed component in 2x3 paginated grid, similar to blog cards with proper theming (InteractiveServer component for pagination)
- **TechnicalContent**: Separate routed pages for technical/functional content, similar to blog post design but simplified

### Project Index Page Banners
- Display: title, description, illustration, last activity date (cached value, "no activity" if none)
- Tags: "game", "tool", etc.
- Clicking banner opens full project detail page
- Last activity dates fetched via bulk API endpoint

### Blazor Integration
- Static project pages as individual Razor components
- Use existing activity display components for dynamic feeds
- Integrate with existing navigation and layout systems

## 7. Content Management

### Static Content
- Project pages are hardcoded Blazor components in `/Components/Pages/Tech/` and `/Components/Components/Tech/`
- Project metadata, descriptions, and technical content embedded in component code
- Images and assets handled by project-wide media serving (to be defined later)
- Project listing page hardcoded with project banners linking to detail pages
- Technical/functional content in separate routed sections with blog-post-like design (simplified header, no author footer)

### Dynamic Content
- TechUpdate activities appear in homepage activity feed
- ProjectId field (enum slug) links activities to specific static project pages
- Activity feed in project pages: 2x3 paginated grid at bottom of pages (InteractiveServer component)
- No real-time updates - feed refreshed on page load only
- Pagination similar to blog list implementation - only activity feed refreshes
- Last activity dates cached and fetched in bulk for project banners
- Chronological sorting and filtering through existing patterns

## 8. Technical Implementation Notes

### JSON Serialization
- Add `TechUpdate` to existing `JsonContext` classes
- Follow established serialization patterns

### Database Migrations
- Generate EF Core migration for `TechUpdates` table
- Extend existing database context patterns

### Routing Configuration
- Static routing for project pages in Blazor
- Existing YARP gateway handles activity API calls

## 9. Development Workflow

### Phase 1 (Prototype)
1. Create `TechProjectLibrary` with `TechUpdate` entity and `ProjectKeys` enum
2. Create `TechProjectAPI` with tech update endpoints
3. Add database migration for TechUpdates
4. Create hardcoded project listing page with banners in `/Components/Pages/Tech/`
5. Implement static project detail pages in `/Components/Components/Tech/`
6. Add activity feed component to project pages (2x3 paginated grid)

### Phase 2 (Version 1)
1. Complete separate routed sections for technical/functional content
2. Integrate activity feed with filtering and proper theming
3. Add breadcrumb navigation using project keys
4. Implement proper error handling and loading states

### Phase 3 (Secondary)
1. Advanced filtering and search for activity feeds
2. Analytics integration
3. Rich media support
4. Performance optimizations

## 10. Dependencies

### Existing Systems
- IActivity interface and inheritance patterns
- ActivityAPI infrastructure
- Blog/Activity display components
- YARP gateway configuration for API calls
- Entity Framework context and migrations

### New Components
- TechProjectLibrary with TechUpdate domain model and ProjectKeys enum (following BlogLibrary/FragmentLibrary patterns)
- Static project page components in `/Components/Tech/` folders
- New TechProjectAPI controllers for tech updates and last activity caching
- Activity feed integration for project pages with blog-card styling (InteractiveServer component)

### Development Test Data
- ProjectKeys enum with fake store website examples (clothes, PC parts, etc.)
- Test projects representing different product types
- Actual definitive content to be generated after implementation completion

## 11. Static Project Structure

### Project Listing Page
- Hardcoded grid of project banners
- Each banner contains project title, thumbnail, and brief description
- Links route to individual static project pages

### Individual Project Pages
- Separate Razor component for each project
- Hardcoded project information and content
- Embedded activity feed component filtered by project ID
- Expandable sections for technical and functional details

### Project File Structure
```
Components/
├── Pages/Tech/
│   ├── Index.razor (project listing with banners)
│   ├── ProjectAlpha.razor (individual project detail)
│   ├── ProjectAlpha/
│   │   ├── Technical.razor (technical content section)
│   │   └── Functional.razor (functional content section)
│   └── ProjectBeta.razor (individual project detail)
└── Components/Tech/
    ├── ProjectBanner.razor (project listing banners with cached last activity)
    ├── ProjectActivity.razor (activity feed 2x3 grid - InteractiveServer)
    └── TechnicalContent.razor (blog-post-like content pages)
```

### Navigation Integration
- Main nav menu already has "project list" button
- All project pages are subsections of main projects section
- Breadcrumb navigation works with project keys for project navigation