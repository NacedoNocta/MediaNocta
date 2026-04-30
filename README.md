## License

This project is licensed under the Creative Commons Attribution-NonCommercial-NoDerivatives 4.0 International License.

![CC BY-NC-ND 4.0](https://licensebuttons.net/l/by-nc-nd/4.0/88x31.png)

**You are free to:**
- **Share** — copy and redistribute the material in any medium or format

**Under the following terms:**
- **Attribution** — You must give appropriate credit, provide a link to the license, and indicate if changes were made
- **NonCommercial** — You may not use the material for commercial purposes
- **NoDerivatives** — If you remix, transform, or build upon the material, you may not distribute the modified material

For the full license text, see [LICENSE](./LICENSE) or visit https://creativecommons.org/licenses/by-nc-nd/4.0/

# Media Nocta

## Overall Architecture

"Media Nocta" is the personnal company of Nacedo NOCTA (me !), a junior developer, as well
as the name use to refer to this overall project. 

This project has a dual purpose :

- First, to be a content management platform with
a microservices architecture, featuring blog functionality,
fragment content (short thoughts), and integration of other related projects, 
accessible through a modern web frontend built with Blazor, which is the core of 
the project.


- Second, to act as a technical demonstration of my abilities as a developer. As such,
the project tries to match the usual "French Tech" project structure used by the 
companies I usually target when looking for work. To that end, the project tries
to be as "complete" as possible (including things like caching, monitoring, etc), 
even if it is obviously overkill when compared to the scale of what would
actually be necessary.

Additionally, though this was not a direct goal of the project, I use this opportunity
to train myself to use C# and .NET technologies such as Aspire or Blazor, as well as AI coding assistants.

The solution uses .NET 10.0 and implements modern patterns like API
gateways, distributed services, and OAuth/OIDC authentication.

## Company Philosophy & Identity

At the core of my personal brand lies a conviction that challenges a 
persistent and limiting belief: that science and art are inherently opposed. 
Too often, the role of a developer or computer scientist is reduced to a purely 
logical function, stripped of emotion, expression, or imagination. 
I reject this dichotomy. I believe that true intelligence reveals itself not 
only through structured reasoning, but through the capacity to create, to explore, 
and to challenge assumptions, as well as the ability to make sense of, and adapt to,
the unknown.

Creativity is not a luxury in the world of technology — it is a necessity. 
The same mind that designs an elegant algorithm can also craft a compelling 
narrative, devise unexpected solutions, and navigate ambiguity with purpose. 
In my work, I strive to embody this duality. I approach problems with the rigor 
and analytical mindset of a scientist, while remaining open to the intuition, 
experimentation, and aesthetic sensibilities of an artist.

Whether I am developing software, designing systems, 
or contributing to a collaborative project, I aim to bridge the technical 
and the creative. My goal is not only to solve problems, but to ask better 
questions. To build with intention. To push boundaries not just for the sake 
of innovation, but to create meaning, spark thought, and explore possibilities beyond
the usual "box".

This philosophy guides the way I code, the way I collaborate, 
and the way I think. It's an identity rooted in curiosity, driven by purpose, 
and shaped by the belief that the most impactful work lives at the intersection 
of logic and creativity.

## Project Descriptions

Most components are still very much in an incomplete/WIP state, with lots of things to
add and refine.

### **Website**

**Type:** Blazor Application

**Description:** The main web frontend application, structured as a
Blazor project with both server-side and client-side components.
This serves as the user-facing website for the Media Nocta platform.
It should mainly serve the following sections : 
* A general purpose personal blog 
* A "fragments" section for experimental creative content
* A section that hosts other projects, such as games
* A "Learn & Act" section
* A section that describes me and the company (including a resume page)
* Account/authentication pages (login, profile, access denied) backed by an external OIDC provider (Keycloak)
* Other functionality-related sections (legal mentions, etc.)

The project is organized using a feature-folder convention: cross-cutting
primitives live in `Components/Shared/`, layouts in `Layout/`, and each
domain area (Main, Blog, Fragment, Tech, AboutMe, Authentication, Account)
under `Features/{X}/{Pages,Components}`. The companion `Website.Client`
project mirrors this structure for any future WebAssembly-rendered components.

Despite being a fully-fledged Blazor application, business logic should be handled
by dedicated components and accessed through APIs, in a "microservice" fashion.

*Sometimes eponymously referred in code as "media nocta"*

### **Backoffice & Backoffice.Client**

**Type:** Blazor Application (Admin)

**Description:** A separate Blazor administration application providing
CRUD interfaces for managing platform content (blogs, authors, fragments,
tech projects). Access is restricted to administrators via the same
OIDC authentication stack as the main website. This fulfills the
"backoffice" role previously implied for managing DatabaseManager-style
operations and editorial content.

### **SharedLibrary**

**Type:** Class Library

**Description:** Contains common functionality shared across multiple
projects in the solution (base activity model, localized text keys, etc.).

### **AuthLibrary**

**Type:** Class Library

**Description:** Contains the authentication and authorization domain models
shared across services — local accounts, social accounts, roles, sessions —
along with the EF Core configurations used by the dedicated `AuthDbContext`.

### **BlogLibrary**

**Type:** Class Library

**Description:** A shared library containing core blog domain
models and business logic. Referenced by the BlogApi project to provide
Blog and Author entity definitions and related functionality.

### **BlogApi**

**Type:** ASP.NET Core Web API

**Description:** A REST API service for blog management functionality. 
Contains controllers for blog and author operations, providing endpoints
for listing, retrieving, and (via the Backoffice) creating/updating
blog posts and their authors.

### **ActivityAPI**

**Type:** ASP.NET Core Web API

**Description:** A microservice dedicated to handling broader activity-related 
operations. 

An "activity" is a broad term that refer to blog posts, fragments, tech news, etc.
It is mainly used in "general" areas of the website "ie : main page". 
Read-only operations if possible.

### **FragmentAPI**

**Type:** ASP.NET Core Web API

**Description:** A REST API service dedicated to fragment content
(short-form creative/technical pieces). Provides retrieval, search and
type-filtering endpoints, plus authenticated write operations consumed
by the Backoffice.

### **FragmentLibrary**

**Type:** Class Library

**Description:** A shared library containing 
fragment-related domain models and functionality for experimental creative content features. 
Fragments are short-form creative or technical content pieces that serve as a digital sketchbook.

### **TechAPI**

**Type:** ASP.NET Core Web API

**Description:** A REST API service exposing the tech-projects domain —
project descriptors, tech updates, and related metadata used by the
Tech section of the website.

### **TechLibrary**

**Type:** Class Library

**Description:** Domain library for the tech-projects area. Contains
the `ProjectInfo` descriptor and tech-update models shared between
TechAPI and the Website.

### **APIGateway**

**Type:** API Gateway Service

**Description:** Implements an API gateway pattern using
YARP (Yet Another Reverse Proxy) to route requests between different
microservices in the Media Nocta ecosystem.
Regroups functionality that should be common to all APIs, such as caching 
or authentication, if and when appropriate.

### **YarpConfigLibrary**

**Type:** Class Library

**Description:** Configuration library for YARP (Yet Another Reverse Proxy) 
settings, supporting the APIGateway functionality.

### **DatabaseManager**

**Type:** Utility/Worker Project

**Description:** Handles database operations and management 
tasks for the Media Nocta platform. Runs as an Aspire-orchestrated
worker that applies EF Core migrations for both the application database
(`AppDbContext`) and the authentication database (`AuthDbContext`) before
the rest of the services start. Also hosts coordinated one-shot
migration workers (including a Backoffice-specific auth migration).

### **DatabaseSeeder**

**Type:** Utility/Worker Project

**Description:** Companion worker to DatabaseManager responsible for
populating the database with seed data after migrations have completed.
Useful for development environments and bootstrapping the platform with
representative content.

### **medianocta.AppHost & medianocta.ServiceDefaults**

**Type:** .NET Aspire Projects

**Description:** .NET Aspire orchestration projects 
for managing the distributed application architecture and service defaults.
The AppHost wires up the dependency chain (Database → APIs → Gateway →
Website/Backoffice) and the shared ServiceDefaults provide common
telemetry, health checks, and resilience configuration to every service.

