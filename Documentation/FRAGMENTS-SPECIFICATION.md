# Fragment Section Implementation Specification

## Overview
This specification outlines the implementation plan for creating a new Fragments section in Media Nocta, following the distributed .NET Aspire architecture and existing patterns. The implementation is organized by priority levels: **Prototype**, **Version 1**, and **Secondary**.

## Priority System

### 🚀 **Prototype** - Core MVP for immediate implementation
Essential features needed to get a working fragments section ASAP and validate the concept.

### 📦 **Version 1** - First complete release 
Features that must be included in the first production-ready release, building upon the prototype.

### 🔮 **Secondary** - Future enhancements
Nice-to-have features that will be added incrementally over time after Version 1.

---

## Architecture Integration

### 1. Domain Model Changes

**New Library Project Structure:**
- **Create:** `FragmentsLibrary` project
- **Main Entity:** `Fragment` class implementing `IFragment` interface
- **Inheritance:** `Fragment` inherits from `Activity` (SharedLibrary)
- **ActivityType:** `TextKeys.fragmentTypeKey` ("Fragment")
- **ThemeColor:** `TextKeys.fragmentThemeColor` ("success") // Green theme

**Fragment Domain Model:**

**🚀 Prototype:**
```csharp
public class Fragment : Activity, IFragment
{
    public string? TypeTag { get; set; }           // Single type tag from predefined list
    public string? AttachmentUrl { get; set; }     // URL to external attachment
    public string? AttachmentType { get; set; }    // "image", "video", "link"
    public int VibeCount { get; set; }             // Anonymous resonance counter
    public bool IsPublic { get; set; }             // Visibility flag
    public bool IsDeleted { get; set; }            // Soft delete flag
    
    // Inherited from Activity: Title, Summary, Content (markdown), ImageUrl, Links, CreatedAt, Id
}
```

**📦 Version 1 Additions:**
```csharp
public class Fragment : Activity, IFragment
{
    // ... prototype fields ...
    public Dictionary<string, string>? Meta { get; set; }    // Key-value metadata
    public List<string>? Tags { get; set; }                  // Multiple type tags support
    public DateTime? LastVibeDate { get; set; }              // Track last vibe for rate limiting
}
```

**🔮 Secondary Additions:**
```csharp
public class Fragment : Activity, IFragment
{
    // ... V1 fields ...
    public List<Guid>? RelatedFragmentIds { get; set; }      // Fragment relationships
    public Guid? SeriesId { get; set; }                      // Fragment series
    public List<Guid>? RelatedActivityIds { get; set; }      // References to blogs/activities
}
```

### 2. Database Schema Changes

**🚀 Prototype Schema:**
```sql
CREATE TABLE Fragments (
    Id UUID PRIMARY KEY,
    Title VARCHAR(255),
    Content TEXT,                    -- Markdown content
    Summary TEXT,
    ImageUrl TEXT,
    Links TEXT,                      -- JSON array (from Activity)
    CreatedAt TIMESTAMP,
    TypeTag VARCHAR(50),             -- Single tag from predefined list
    AttachmentUrl TEXT,              -- External URL only
    AttachmentType VARCHAR(20),      -- "image", "video", "link"
    VibeCount INTEGER DEFAULT 0,
    IsPublic BOOLEAN DEFAULT true,
    IsDeleted BOOLEAN DEFAULT false
);

CREATE TABLE FragmentTypes (
    Id UUID PRIMARY KEY,
    Name VARCHAR(50) UNIQUE,
    Color VARCHAR(20),               -- CSS color class
    CreatedAt TIMESTAMP
);
```

**📦 Version 1 Schema Additions:**
```sql
ALTER TABLE Fragments ADD COLUMN Meta TEXT;              -- JSON metadata
ALTER TABLE Fragments ADD COLUMN LastVibeDate TIMESTAMP;

-- Support for multiple tags
CREATE TABLE FragmentTags (
    FragmentId UUID REFERENCES Fragments(Id),
    FragmentTypeId UUID REFERENCES FragmentTypes(Id),
    PRIMARY KEY (FragmentId, FragmentTypeId)
);
```

**🔮 Secondary Schema Additions:**
```sql
-- Fragment relationships and series
CREATE TABLE FragmentSeries (
    Id UUID PRIMARY KEY,
    Name VARCHAR(255),
    Description TEXT,
    CreatedAt TIMESTAMP
);

CREATE TABLE FragmentRelationships (
    FromFragmentId UUID REFERENCES Fragments(Id),
    ToFragmentId UUID REFERENCES Fragments(Id),
    RelationType VARCHAR(50),        -- "references", "attachment", etc.
    PRIMARY KEY (FromFragmentId, ToFragmentId)
);

ALTER TABLE Fragments ADD COLUMN SeriesId UUID REFERENCES FragmentSeries(Id);
```

### 3. API Layer Changes

**🚀 Prototype Endpoints:**
```csharp
// ActivityAPI Controller additions
[HttpGet("fragments")]
public async Task<IEnumerable<Fragment>> GetFragments(int page = 1, int pageSize = 10)

[HttpGet("fragments/{id}")]
public async Task<Fragment?> GetFragment(Guid id)

[HttpPost("fragments/{id}/vibe")]
public async Task<IActionResult> AddVibe(Guid id) // Anonymous only

[HttpGet("fragments/types")]
public async Task<IEnumerable<FragmentType>> GetFragmentTypes()
```

**📦 Version 1 Endpoint Additions:**
```csharp
[HttpGet("fragments/search")]
public async Task<IEnumerable<Fragment>> SearchFragments(string query, string? typeTag = null)

[HttpPost("fragments/{id}/unvibe")]
public async Task<IActionResult> RemoveVibe(Guid id)

[HttpGet("fragments/by-type/{typeTag}")]
public async Task<IEnumerable<Fragment>> GetFragmentsByType(string typeTag)
```

**🔮 Secondary Endpoint Additions:**
```csharp
[HttpGet("fragments/export/{id}")]
public async Task<IActionResult> ExportFragment(Guid id, string format) // markdown, json

[HttpGet("fragments/rss")]
public async Task<IActionResult> GetFragmentsRss()

[HttpGet("fragments/series/{seriesId}")]
public async Task<IEnumerable<Fragment>> GetFragmentsBySeries(Guid seriesId)

// Admin endpoints
[HttpPost("fragments")]
[HttpPut("fragments/{id}")]
[HttpDelete("fragments/{id}")]
```

### 4. Frontend Implementation

**🚀 Prototype Pages:**
- **Route:** `/fragments` (replace `/art`)
- **Components:**
  - `Fragments.razor` - Main listing page with pagination
  - `FragmentCard.razor` - Individual fragment display
  - `VibeButton.razor` - Simple vibe interaction

**📦 Version 1 Additions:**
- **Components:**
  - `FragmentSearch.razor` - Search and filter interface
  - `FragmentTypeFilter.razor` - Filter by type tags
  - `FragmentDetail.razor` - Full fragment view with metadata

**🔮 Secondary Additions:**
- **Components:**
  - `FragmentAdmin.razor` - Admin CRUD interface
  - `FragmentSeries.razor` - Series management
  - `FragmentExport.razor` - Export functionality
  - `FragmentMasonry.razor` - Masonry layout with infinite scroll

### 5. Service Layer Updates

**🚀 Prototype Services:**
```csharp
// Website/Services/FragmentService.cs
public class FragmentService
{
    Task<IEnumerable<Fragment>> GetFragmentsAsync(int page, int pageSize);
    Task<Fragment?> GetFragmentAsync(Guid id);
    Task<bool> AddVibeAsync(Guid fragmentId);
    Task<IEnumerable<FragmentType>> GetFragmentTypesAsync();
}
```

**📦 Version 1 Service Additions:**
```csharp
public class FragmentService
{
    // ... prototype methods ...
    Task<IEnumerable<Fragment>> SearchFragmentsAsync(string query, string? typeTag);
    Task<bool> RemoveVibeAsync(Guid fragmentId);
    Task<IEnumerable<Fragment>> GetFragmentsByTypeAsync(string typeTag);
}
```

**🔮 Secondary Service Additions:**
```csharp
public class FragmentService
{
    // ... V1 methods ...
    Task<string> ExportFragmentAsync(Guid id, string format);
    Task<IEnumerable<Fragment>> GetFragmentsBySeriesAsync(Guid seriesId);
}

public class FragmentAdminService
{
    Task<Fragment> CreateFragmentAsync(Fragment fragment);
    Task<Fragment> UpdateFragmentAsync(Fragment fragment);
    Task<bool> DeleteFragmentAsync(Guid id);
}
```

### 6. UI/UX Implementation by Priority

**🚀 Prototype UI:**
- **Layout:** Simple paginated list view
- **Fragment Card:** Title, content preview, type tag, vibe count
- **Styling:** Bootstrap cards with green theme
- **Interaction:** Click to expand, simple vibe button (logs "Vibed with {Title}!")
- **Images:** Display image previews for AttachmentUrl when type is "image"

**📦 Version 1 UI Enhancements:**
- **Search Bar:** Basic text search with type filter dropdown
- **Content Rendering:** Markdown to HTML conversion (library TBD)
- **Responsive Design:** Mobile-friendly layout
- **Loading States:** Skeleton loaders for better UX
- **Authentication:** Real user authentication for vibe counting (method TBD)
- **Error Handling:** Placeholder images from placehold.co for broken URLs

**🔮 Secondary UI Features:**
- **Masonry Layout:** Pinterest-style grid with infinite scroll
- **Advanced Filters:** Multiple tag selection, date ranges
- **Export Buttons:** Download as markdown/JSON
- **Admin Interface:** Full CRUD operations
- **Fragment Series:** Grouped fragment display

### 7. Configuration and Integration

**🚀 Prototype Configuration:**
- **Navigation:** Add `/fragments` route to `NavMenu.razor`
- **Theme:** Use green ("success") theme for fragment section
- **JSON Context:** Basic Fragment serialization
- **Database:** Add Fragment and FragmentType DbSets (start empty - no seeding)
- **Fragment Types:** Bootstrap color selection on type registration

**📦 Version 1 Configuration:**
- **Search Integration:** Add search to activity service
- **Rate Limiting:** Implement vibe count restrictions
- **Metadata:** Support for Meta dictionary serialization

**🔮 Secondary Configuration:**
- **RSS Feed:** Add RSS endpoint configuration
- **Export:** File download handling
- **Admin Auth:** Role-based access control
- **Series Management:** UI for creating/managing series

## Content Model Implementation

### Fragment Types (Predefined)
```csharp
public static class FragmentTypes
{
    public static readonly FragmentType[] DefaultTypes = 
    {
        new() { Name = "idea", Color = "warning" },      // Yellow
        new() { Name = "code", Color = "info" },         // Blue  
        new() { Name = "quote", Color = "secondary" },   // Gray
        new() { Name = "glitch", Color = "danger" },     // Red
        new() { Name = "dream", Color = "primary" },     // Purple
        new() { Name = "fragment", Color = "success" },  // Green
        new() { Name = "fiction", Color = "dark" }       // Dark
    };
}
```

### Content Validation

**🚀 Prototype:**
- Basic content validation (no markdown parsing yet)
- URL validation for attachments
- XSS prevention for rendered content
- Image preview support for attachment URLs

**📦 Version 1:**
- Enhanced markdown parsing with code highlighting
- Image URL validation and display
- Content length limits

**🔮 Secondary:**
- Advanced content moderation
- User-generated content validation
- Rich metadata validation

## Migration Strategy

### 🚀 Prototype Implementation:
1. Create FragmentsLibrary project
2. Add Fragment and FragmentType entities to database (no seeding)
3. Add fragments navigation to menu
4. Create basic fragments listing page with image preview support
5. Implement simple vibe functionality (console logging only)

### 📦 Version 1 Implementation:
1. Add search functionality
2. Implement multiple tag support
3. Add advanced UI components
4. Implement rate limiting

### 🔮 Secondary Implementation:
1. Add admin interface
2. Implement export functionality
3. Add series management
4. Implement advanced relationships

## Project Reference Updates

**Add to relevant projects:**
```xml
<ProjectReference Include="../FragmentsLibrary/FragmentsLibrary.csproj" />
```

## Success Criteria

**🚀 Prototype Success:**
- Fragments page displays list of fragments
- Users can view individual fragments
- Vibe button works (anonymous)
- Type tags display correctly
- Markdown content renders properly

**📦 Version 1 Success:**
- Search functionality works
- Multiple tag filtering works
- Rate limiting prevents spam
- Mobile responsive design
- Performance acceptable for 100+ fragments

**🔮 Secondary Success:**
- Admin can create/edit/delete fragments
- Export functionality works
- Series management functional
- Advanced relationships work
- RSS feed available

## Implementation Details - All Questions Answered ✅

**Authentication Integration:** 
- **Prototype:** Console logging only ("Vibed with {Title}!")
- **Version 1:** Real authentication (method TBD)

**Markdown Parsing:** 
- **Prototype:** Plain text content only
- **Version 1:** Markdown library (TBD)

**Fragment Seeding:** 
- **All Versions:** Start with empty database, no seeding

**Image Handling:** 
- **Prototype:** Support image previews for AttachmentUrl
- **Version 1:** Add placeholder images from placehold.co for broken URLs

**Type Tag Colors:** 
- **All Versions:** Bootstrap colors selected on type registration

## Next Steps

This specification is now complete and ready for prototype implementation. All questions have been answered and the implementation details are clear for rapid development.