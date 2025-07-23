# Tech Projects Section - Clarification Questions

## Static Content Structure

**Q1:** How should static project pages be organized in the file structure?
- **Context**: Need to define the folder structure and naming conventions for project components.
- **Options**:
  - A) Individual .razor files in `/Pages/Projects/`
  - B) Components in `/Components/Projects/` with routing
  - C) Mixed approach with index page and individual project components

Answer : add a /Tech/ folder inside the /Components/Components/ and /Components/Pages/ folder.
  
**Q2:** What information should be hardcoded in each project page?
- **Context**: Need to define the standard structure/template for project pages.
- **Required fields**: Project title, description, hero image, external links (live site, GitHub, demo), technical/functional content sections?

Answer : The index page has banners for all projects with title, description, illustration, last activity date, tags
("game", "tool" etc). 
Clicking on a banner opens a page with all informations : Title, description, illustration, buttons etc.

**Q3:** How should project identifiers be defined for activity association?
- **Context**: TechUpdate entities need ProjectId to link to static pages.
- **Options**:
  - A) Use component file names (e.g., "ProjectAlpha")
  - B) Define custom string identifiers in component code
  - C) Use route-based identifiers

Answer : each project has a slug identifier that the API accepts to filter related data. 
These project keys could be an hardcoded enum defined in the shared librairy. 

## Activity Feed Integration

**Q4:** What update types should be supported in the UpdateType field?
- **Context**: Definition mentions patch notes, updates, developer logs, commentary.
- **Specific types needed**: "patch", "update", "devlog", "commentary" - are these sufficient or are others needed?

Answer : These are functionnaly meta-like "tags", and bring no differences in function. it should support any number of them.


**Q5:** How should activity cards be displayed within project pages?
- **Context**: Need UI/UX details for activity feed integration.
- **Requirements**: Number of cards to show initially, pagination/infinite scroll, card layout design?

Answer : Paginated 2x3 grid at the bottom of the project pages. Cards can have a similar design as the "blog list" 
cards, with the proper theming 

**Q6:** Should tech updates appear in the main homepage activity feed?
- **Context**: Definition mentions activities appear on homepage, need confirmation.

Answer : Since they are "activities", they should appear in the home page.

## User Interface Design

**Q7:** What responsive grid layout should be used for the project listing?
- **Context**: Need specific Bootstrap grid specifications.
- **Requirements**: Columns per breakpoint (sm, md, lg, xl), card aspect ratios?

Answer : No specific responsiveness for now.

**Q8:** How should expandable information panels be implemented?
- **Context**: Technical/functional content sections need interaction design.
- **Options**:
  - A) Bootstrap collapse/accordion components
  - B) Custom toggle implementation
  - C) Modal overlays
  - D) Separate routed sections

Answer : separated routed sections for now. These routed pages will have a similar design to the "blog post" articles,
with a simplified "header" section and no "author" footer section.  

**Q9:** What navigation integration is needed?
- **Context**: Projects section needs to integrate with existing site navigation.
- **Requirements**: Main nav menu item, breadcrumbs, related links?

Answer : the "project list" section already has a button in the main nav menu ; everything else is a subsection of this.
Breadcrumb should work with project navigation using the project key.

## Content Guidelines

*Q10 and 12 were deemed irrelevant and  removed*

**Q11:** What image specifications are needed?
- **Context**: Hero images, thumbnails, and other visual assets.
- **Requirements**: Dimensions, file formats, size limits, optimization needs?

Answer : its hardcoded pages so this is irrelevant. Project-wide media serving server will be defined later to optimize image serving.

## Technical Implementation

**Q13:** Where should the TechUpdate entity be defined?
- **Context**: Following existing domain model patterns.
- **Options**:
  - A) Extend existing SharedLibrary
  - B) Create new TechProjectLibrary
  - C) Add to existing activity-related library

Answer : Create a new TechProjectLibrary and TechProjectAPI for serving Tech Updates.

**Q14:** How should the activity feed component handle real-time updates?
- **Context**: SignalR integration or periodic refresh for new activities.

Answer : No need for real-time updates ; the feed is API sourced and should only be updated on page load.

**Q15:** What caching strategy should be used for static content vs. dynamic feeds?
- **Context**: Performance optimization for mixed static/dynamic content.

Answer : Nothing specific for now; project wide caching strategies will be defined later, but
will probably only concern the feed.

## Routing and SEO

**Q16:** What URL structure should be used for project pages?
- **Context**: SEO-friendly routing patterns.
- **Options**:
  - A) `/projects/project-name`
  - B) `/projects/{id}/project-name`
  - C) Custom routing per project

Answer : Option A is fine. '/tech/{project key slug}' and '/tech/{project key slug}/{subsection or update id}'

**Q17:** How should SEO metadata be handled for individual project pages?
- **Context**: Meta tags, Open Graph data, structured data for each project.

Answer : SEO strategies will be defined site wide later.

## Additional Clarification Questions

**Q18:** Should the ProjectKeys enum be defined with specific values for existing projects?
- **Context**: Need to define the actual project slugs that will be used in the system.
- **Requirements**: What are the specific project identifiers/slugs that should be included in the enum?

Answer : for development, we are going to imagine a fake store websites for various products (one "project" = one product type ie clothes, pc parts etc...) 
This is just example test data. 
We will generate actual, definitive content once everything is ready.

**Q19:** How should the "last activity date" be calculated and displayed on project banners?
- **Context**: Project banners need to show last activity date from TechUpdate activities.
- **Requirements**: Real-time calculation vs cached value, date format, handling projects with no activities?

Answer : cached value based on the most recent update activity of the project. For project with no activity yet, write 'no activity'  
A dedicated API route could return them in bulk. 

**Q20:** What should be the specific structure of the TechProjectLibrary in relation to existing libraries?
- **Context**: Following existing domain library patterns for consistency.
- **Requirements**: Should it reference SharedLibrary for IActivity? What namespace conventions to follow?

Answer : mimic the already existing library for consistency, like BlogLibrary or FragmentLibrary.

**Q21:** How should the activity feed pagination work within the 2x3 grid?
- **Context**: Need specific pagination implementation details.
- **Requirements**: Items per page (6?), navigation controls, loading states?

Answer : similar implementation as the "blog list" page. Only the activity feed part should refresh when changing pagination,
which means this section should be its own component as an InteractiveServer type.

**Q22:** What routing strategy should be used for technical/functional content sections?
- **Context**: These are separate routed pages with blog-post-like design.
- **Requirements**: URL structure, parameter passing, breadcrumb integration?

Create a specific pages under /tech/{project-key}/{subsection-name}. 