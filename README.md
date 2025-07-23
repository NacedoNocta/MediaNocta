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

The solution uses .NET 9.0 and implements modern patterns like API
gateways and distributed services.

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
* A section that hosts others projects, such as games
* A learning section 
* A section that describe me and the company
* Other functionnality-related sections (legal mentions, logins etc)

Despite being a fully-fledged Blazor MVC application, business logic should be handled
by dedicated components and accessed through APIs, in a "microservice" fashion.

*Sometimes eponymously referred in code as "media nocta"*

### **SharedLibrairy**

**Type:** Class Library

**Description:** Contains common functionality shared across multiple
projects in the solution.

### **BlogLibrairy**

**Type:** Class Library

**Description:** A shared library containing core blog domain
models and business logic. Referenced by the BlogApi project to provide
Blog and Author entity definitions and related functionality.

### **BlogApi**

**Type:** ASP.NET Core Web API

**Description:** A REST API service for blog management functionality. 
Contains controllers for blog operations including a `SimpleBlogController` 
that provides endpoints for retrieving blog posts with author information and images.

### **ActivityAPI**

**Type:** ASP.NET Core Web API

**Description:** A microservice dedicated to handling broader activity-related 
operations. 

An "activity" is a broad term that refer to blog posts, fragments, tech news, etc.
It is mainly used in "general" areas of the website "ie : main page". 
Read-only operations if possible.

### **Gateway**

**Type:** API Gateway Service

**Description:**  Implements an API gateway pattern using
YARP (Yet Another Reverse Proxy) to route requests between different
microservices in the Media Nocta ecosystem.
Regroups functionnality that should be common to all APIs, such as caching 
or authentification,if and when appropriate.

### **FragmentsLibrairy**

**Type:** Class Library

**Description:** A shared library containing 
fragment-related domain models and functionality for experimental creative content features. 
Fragments are short-form creative or technical content pieces that serve as a digital sketchbook.

### **DatabaseManager**

**Type:** Utility/Worker Project

**Description:** Handles database operations and management 
tasks for the Media Nocta platform. 
Should perform migration operation on the database and various other "one shot"
type of operations.

Ideally, this type of workers would be started and managed 
through a "backoffice" type of application.

### **YARPConfigLibrairy**

**Type:** Class Library

**Description:** Configuration library for YARP (Yet Another Reverse Proxy) 
settings, supporting the Gateway functionality.


### **medianocta.AppHost & medianocta.ServiceDefaults**

**Type:** .NET Aspire Projects

**Description:** .NET Aspire orchestration projects 
for managing the distributed application architecture and service defaults.

