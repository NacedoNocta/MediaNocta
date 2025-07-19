## 🛠️ Section Specification: **Tech Projects**

### **Overview**

The **"Tech Projects"** section serves as a portfolio-style hub that 
showcases a curated list of personal and professional technology 
projects. It is designed to provide visitors with an organized, 
interactive, and informative interface to explore individual projects, 
understand their technical and functional contexts, and track ongoing
activity such as updates or community engagement.

---

### **User Flow**

1. **Landing View – Project Banners**

   * The main view displays a **grid or list of project banners**.
   * Each banner contains:

     * Project name/title
     * Thumbnail/cover illustration
     * (Optional) brief tagline or icon
   * **Clicking a banner** opens a **dedicated project detail page**.

2. **Project Detail Page**
   Each project has its own standalone page featuring:

   #### A. **Header Section**

   * **Short description** of the project
   * **Hero illustration** or banner (optional interactive media)
   * **Primary action buttons**:

     * Redirect to the live project (external URL)
     * Access GitHub or other repo (optional)
     * View demo (if available)

   #### B. **Expandable Information Panels**

   * Buttons or toggles to open "learn more" sub-pages:

     * **Technical breakdowns** (architecture, stack, algorithms, etc.)
     * **Functional overviews** (UX design, user journeys, goals)
     * These subsections are static or markdown-based and **hardcoded** (not stored in database)

   #### C. **Activity Feed (Blog-style Cards)**

   * A series of dynamic **cards sourced from a database**

     * Each card corresponds to a type of project-related activity:

       * Patch notes
       * Updates
       * Developer logs
       * Commentary
     * These cards inherit metadata from the project, such as:

       * Project ID
     * Cards are filterable and chronologically sorted
     * Clicking on a card opens a blog-like article.
     * This feed is defined as an "activity", and as such will be displayed on the home page feed.

---

### **Content Structure**

* **Static Content (Hardcoded)**:

  * Project banners and basic metadata
  * Descriptive sections and technical breakdowns
* **Dynamic Content (API-Sourced)**:

  * Activity cards (blog-style updates, patch notes, comments)
  * Each card is linked to its parent project via a project ID

---

### **Technical Notes**

* The actual projects are **hosted externally** and are **not embedded** in the website.
* **Project buttons redirect** to those external platforms or repositories.

#### **Example Frontend Routes**

* `/projects`: Displays the grid of project banners
* `/projects/:id`: Loads the project detail page
* `/projects/:id/activity/:activityId`: Optional deep-link to a specific activity post

#### **Example Backend API Endpoints**

* `GET /api/projects`: Returns list of all project metadata for banners
* `GET /api/projects/:id`: Returns static + dynamic content for selected project
---

### **Goals**

* Present tech projects in a clean, organized, and discoverable way
* Allow users to explore both high-level and in-depth content
* Showcase ongoing work and updates with minimal friction
* Maintain separation of content sources (static vs dynamic)

---
