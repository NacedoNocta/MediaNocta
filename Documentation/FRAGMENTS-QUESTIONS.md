# Fragment Implementation Questions

The following questions need clarification before proceeding with the Fragment section implementation:

## Content & Data Management

**1. Content Richness**
- Should fragments support rich text editing (markdown, HTML) or plain text only?
  - Yes, rich text should be supported. Markdown would be prefered for the prototype.
- Should code fragments have syntax highlighting? Which languages should be supported?
  - Simple syntax highlight similar to those seen in markdown code bloc would be fine. This is a nice-to-have.

**2. Attachment Strategy**
- How should attachments be stored? (Database BLOBs, file system, cloud storage?)
  - In the long run, for all apps, a media server/CDN microservice should be conceived for this, 
  saved on the file system of the media server, and saved as URL in the apps. This will be part of another project. Attachements will be saved as URL with their type.
- What file types should be supported for attachments?
  - For now, simple image, embed videos and external link urls should be fine. We will implement more on the go after V1.
- What are the size limits for attachments?
  - Since we only save URL, this is not a concern for now.

**3. Fragment Editing**
- Should fragments be editable after creation?
  - Admins only should be able to as a nice to have.
- Should there be version history for fragments?
  - Irrelevant. An actual system of "improvement upon" would be part of a nice-to-have. 
- Should there be a draft/published state system?
  - The point of fragment is to be a form of draft publication in itself, so just a public/hidden state is enough for V1.

## User Interaction

**4. Vibe Count System**
- Should there be rate limiting or IP-based restrictions on vibe counting?
  - Require users to be logged in to place a vibe. Limit to once per account per post.
- Should vibe counts be anonymous or should they track some basic metrics?
  - Go for anonymous for prototype.
- Should there be any anti-spam measures?
  - For a Version 1, public users should not be able to propose fragments. For vibes, something basic like a vibe limit per post per period is enough for example, is fine (ie : 3 vibes/unvibe per post per hour.)

**5. Fragment Browsing**
- How many fragments should be displayed per page?
- Should it be infinite scroll or traditional pagination?
- Should fragments be sorted by date, vibe count, or other criteria?
  - sort by date by default.

Global answer : Fragment could be displayed in a masonry form with infinite scrolling at V1 or after. 
For prototype, a traditionnal paginated list is enough.

## Content Organization

**6. Type Tag Validation**
- Should type tags be strictly enforced from a predefined list or allow custom tags?
  - Predefined list stored in a database.
- Should there be a maximum number of type tags per fragment?
  - No
- Should type tags be hierarchical or flat?
  - Flat

**7. Fragment Relationships**
- Should fragments be able to reference or link to other fragments?
  - For nice to haves, fragments could be turned into attachments for other fragments.
- Should there be fragment series or collections?
  - Fragments series would be part of a Secondary release.
- Should fragments be able to reference blog posts or other activities?
  - As a secondary feature, yes.

## Technical Features

**8. Search Functionality**
- Should fragments be searchable?
  - For a V1, yes.
- Should search include content, titles, metadata, or type tags?
  - For a V1, a basic search on title and content is enough.
- Should there be advanced filtering options?
  - For a V1, a filter by tag is enough.

**9. Export Features**
- Should fragments support export to formats like JSON, markdown, or RSS?
  - Export as markdown (for embedding) and RSS as a nice to have, yes.
- Should there be an RSS feed for fragments?
  - As a nice to have, yes.
- Should fragments support sharing to social media?
  - As a nice to have, yes.

## Administration

**10. Admin Interface**
- Should there be an admin interface for managing fragments (CRUD operations)?
  - As a nice to have, yes.
- Should there be content moderation features?
  - As a secondary feature, yes.
- Should there be analytics/metrics for fragment engagement?
  - Not planned for now.

## Performance & Scaling

**11. Caching Strategy**
- Should fragments be cached at the API level?
  - Caching implementation has yet to be decided at the project level. Ignore this for now.
- Should there be different caching strategies for different fragment types?
  - See last question.
- How should cache invalidation be handled?
  - See last question.
  
**12. Database Optimization**
- Should there be archiving for old fragments?
  - Not planned.
- Should there be soft delete functionality?
  - Yes for a V1
- Should there be database partitioning by date or type?
  - Undecided yet.

## Security & Privacy

**13. Content Validation**
- What level of content sanitization is needed?
  - For a V1, since only admins will be able to generate content, basic sanitization for now.
- Should there be content moderation or flagging systems?
  - As a secondary feature, yes.
- Should there be word filters or content policies?
  - When public fragments will be considered, yes. Secondary.

**14. Access Control**
- Should fragments have different visibility levels beyond public/private?
  - No.
- Should there be fragment permissions or access control?
  - No
- Should there be API rate limiting for fragment operations?
  - Not for V1 since no user-created content.

## Migration & Compatibility

**15. Data Migration**
- Should existing Art data be migrated to Fragments?
  - No, art database can be removed.
- Should there be a migration tool or script?
  - Migrations are handled by the Database Manager project.
- Should there be backward compatibility during transition?
  - No need as the "Art" section is to be entirely removed.

**16. Theme Integration**
- Should fragments have their own distinct visual theme?
  - The website uses color-coding to diffenciate sections. Fragment gains the former art's green (bootstrap "success") color for its sections. 
- Should type tags have specific color schemes or icons?
  - Tags should have a color associated to them.
- Should fragments integrate with the existing dark/futuristic aesthetic?
  - Fragment design should fit with the design of the other sections of the website, like the blog.
---

## Additional Implementation Questions

Based on the answered questions, here are remaining clarifications needed for implementation:

**17. Authentication Integration**
- How should we integrate with the existing authentication system for vibe counting?
  - The auth system is yet to be decided on how to implement on the website project. For the prototype, write a "Vibed with {Title}!" in the logs. V1 will use auth. 
- Should we use JWT tokens, session-based auth, or another method?
  - TBD

**18. Markdown Parsing**
- Should we use a specific markdown parser library (like Markdig) or implement custom parsing?
  - We have yet to decide how to implement this on the website project, no librairy has been decided on yet. 
- Do we need to support custom markdown extensions?
  - Not for prototype.

**19. Fragment Seeding**
- Should we create sample fragments for testing, or start with an empty database?
  - Start with an empty database.
- Should there be default fragment types seeded in the database?
  - Start empty.

**20. Image Handling**
- For the prototype, should we support image previews or just display URLs?
  - Support preview.
- How should we handle broken image URLs?
  - In V1, display a placeholder image with the text "NOT FOUND" from placehold.co.
  
**21. Type Tag Colors**
- Should each fragment type have a unique color scheme beyond the predefined bootstrap classes?
  - No
- Should colors be configurable or hardcoded?
  - On registering the tag, we pick a bootstrap color. 

---

**All Previous Questions Answered - Ready for Implementation**

**Priority:** Please answer the questions marked as high priority first, as they will affect the core architecture and database design.

**High Priority Questions:** 1, 2, 3, 6, 10, 15 ✅ **COMPLETED**
**Medium Priority Questions:** 4, 5, 7, 8, 13, 16 ✅ **COMPLETED**
**Low Priority Questions:** 9, 11, 12, 14 ✅ **COMPLETED**
**Implementation Questions:** 17, 18, 19, 20, 21 ✅ **COMPLETED**