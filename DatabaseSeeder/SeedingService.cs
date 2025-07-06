using DatabaseManager;
using FragmentLibrary;
using BlogLibrary;
using Microsoft.EntityFrameworkCore;

namespace DatabaseSeeder;

public class SeedingService
{
    private readonly AppDbContext _context;
    private readonly ILogger<SeedingService> _logger;

    public SeedingService(AppDbContext context, ILogger<SeedingService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task SeedAllAsync()
    {
        _logger.LogInformation("Starting database seeding process...");
        
        await SeedFragmentTypesAsync();
        await SeedFragmentsAsync();
        await SeedBlogDataAsync();
        
        _logger.LogInformation("Database seeding process completed.");
    }

    public async Task SeedFragmentTypesAsync()
    {
        try
        {
            var existingTypes = await _context.FragmentTypes.CountAsync();
            if (existingTypes > 0)
            {
                _logger.LogInformation("Fragment types already exist in database, skipping seed");
                return;
            }

            var defaultTypes = FragmentTypes.DefaultTypes.ToList();
            await _context.FragmentTypes.AddRangeAsync(defaultTypes);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Seeded {Count} fragment types", defaultTypes.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error seeding fragment types");
        }
    }

    public async Task SeedFragmentsAsync()
    {
        try
        {
            var existingFragments = await _context.Fragments.CountAsync();
            if (existingFragments > 0)
            {
                _logger.LogInformation("Fragments already exist in database, skipping seed");
                return;
            }

            var sampleFragments = new List<Fragment>
            {
                new Fragment("Welcome to Fragments", "Introduction to the fragment system", 
                    "# Welcome to Fragments\n\nThis is the first fragment in the system. Fragments are short pieces of content that can be ideas, code snippets, quotes, or any other creative expression.\n\n## Features\n- Markdown support\n- Type categorization\n- Tagging system\n- Vibe counting", 
                    "https://picsum.photos/400/300?random=1")
                {
                    TypeTag = "fragment",
                    AttachmentType = "image",
                    VibeCount = 3,
                    IsPublic = true,
                    Tags = new List<string> { "welcome", "introduction", "system" },
                    Meta = new Dictionary<string, string> { { "featured", "true" }, { "priority", "high" } }
                },

                new Fragment("Code Organization Tips", "Best practices for clean code", 
                    "# Code Organization Tips\n\n```csharp\n// Use meaningful names\npublic class FragmentController : ControllerBase\n{\n    private readonly ILogger<FragmentController> _logger;\n    private readonly AppDbContext _context;\n    \n    // Constructor injection\n    public FragmentController(ILogger<FragmentController> logger, AppDbContext context)\n    {\n        _logger = logger;\n        _context = context;\n    }\n}\n```\n\n**Key principles:**\n- Single responsibility\n- Dependency injection\n- Clear naming conventions", 
                    "https://picsum.photos/400/300?random=2")
                {
                    TypeTag = "code",
                    AttachmentType = "image",
                    VibeCount = 8,
                    IsPublic = true,
                    Tags = new List<string> { "code", "csharp", "best-practices", "clean-code" },
                    Meta = new Dictionary<string, string> { { "language", "csharp" }, { "difficulty", "intermediate" } }
                },

                new Fragment("Daily Inspiration", "Motivation for developers", 
                    "> \"The only way to do great work is to love what you do.\" - Steve Jobs\n\nThis quote reminds us that passion is the key to excellence in software development. When we love coding, debugging becomes problem-solving, and challenges become opportunities to learn and grow.", 
                    "https://picsum.photos/400/300?random=3")
                {
                    TypeTag = "quote",
                    AttachmentType = "image",
                    VibeCount = 12,
                    IsPublic = true,
                    Tags = new List<string> { "inspiration", "motivation", "jobs", "passion" },
                    Meta = new Dictionary<string, string> { { "author", "Steve Jobs" }, { "category", "motivation" } }
                },

                new Fragment("Creative Block Solution", "Ideas for overcoming creative blocks", 
                    "# Breaking Through Creative Blocks\n\n💡 **When you're stuck, try:**\n\n1. **Change your environment** - Work from a different location\n2. **Take a walk** - Let your mind wander\n3. **Talk to someone** - Explain your problem out loud\n4. **Sleep on it** - Your subconscious works while you rest\n5. **Start with something small** - Momentum builds momentum\n\nSometimes the best ideas come when we're not actively trying to have them!", 
                    "https://picsum.photos/400/300?random=4")
                {
                    TypeTag = "idea",
                    AttachmentType = "image",
                    VibeCount = 5,
                    IsPublic = true,
                    Tags = new List<string> { "creativity", "productivity", "inspiration", "tips" },
                    Meta = new Dictionary<string, string> { { "category", "productivity" }, { "usefulness", "high" } }
                },

                new Fragment("Dream Project", "A vision for the future", 
                    "# The Perfect Development Environment\n\n🌟 **Imagine a world where:**\n\n- Code writes itself based on natural language descriptions\n- Bugs are caught before they're written\n- Documentation updates automatically\n- Tests generate themselves from specifications\n- Deployment is as easy as saving a file\n\nWhile we're not there yet, each new tool and framework brings us closer to this dream. The future of development is exciting!", 
                    "https://picsum.photos/400/300?random=5")
                {
                    TypeTag = "dream",
                    AttachmentType = "image",
                    VibeCount = 15,
                    IsPublic = true,
                    Tags = new List<string> { "future", "ai", "automation", "development", "vision" },
                    Meta = new Dictionary<string, string> { { "timeframe", "future" }, { "feasibility", "possible" } }
                },

                new Fragment("The Glitch in the Matrix", "When technology surprises us", 
                    "# 🔴 System Anomaly Detected\n\n```error\nERROR: Reality.exe has stopped working\nStack trace:\n  at Universe.Physics.Quantum.Superposition()\n  at Reality.Timeline.Process()\n  at Consciousness.Observe()\n```\n\n*Sometimes the most interesting discoveries happen when things don't work as expected. Embrace the glitches - they often lead to breakthroughs.*\n\n⚠️ **WARNING:** This fragment may cause existential questioning about the nature of reality and simulation theory.", 
                    "https://picsum.photos/400/300?random=6")
                {
                    TypeTag = "glitch",
                    AttachmentType = "image",
                    VibeCount = 7,
                    IsPublic = true,
                    Tags = new List<string> { "matrix", "reality", "simulation", "philosophy", "humor" },
                    Meta = new Dictionary<string, string> { { "genre", "philosophical-humor" }, { "risk", "mind-bending" } }
                },

                new Fragment("The Digital Nomad's Tale", "A short story about remote work", 
                    "# The Last Commit\n\n*A short story*\n\nSarah closed her laptop as the sun set over the Balinese rice terraces. Her final commit of the day pushed the feature that would revolutionize how people connected across the globe. \n\nShe had written it from a bamboo café, debugged it on a beach in Thailand, and deployed it from a mountain village in Nepal. The code carried traces of every place she'd been, every person she'd met, every sunset she'd watched while solving problems that seemed impossible just hours before.\n\nIn the distance, a temple bell rang, marking the end of another day in paradise. Tomorrow, she would wake up in a new place, with new challenges, and the whole world as her office.\n\n*\"The best code is written with passion, debugged with patience, and deployed with love.\"*", 
                    "https://picsum.photos/400/300?random=7")
                {
                    TypeTag = "fiction",
                    AttachmentType = "image",
                    VibeCount = 9,
                    IsPublic = true,
                    Tags = new List<string> { "fiction", "remote-work", "travel", "inspiration", "story" },
                    Meta = new Dictionary<string, string> { { "genre", "tech-fiction" }, { "length", "short" }, { "mood", "inspirational" } }
                }
            };

            await _context.Fragments.AddRangeAsync(sampleFragments);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Seeded {Count} sample fragments", sampleFragments.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error seeding fragments");
        }
    }

    public async Task SeedBlogDataAsync()
    {
        try
        {
            var existingBlogs = await _context.Blogs.CountAsync();
            if (existingBlogs > 0)
            {
                _logger.LogInformation("Blogs already exist in database, skipping seed");
                return;
            }

            var sampleTags = new List<Tag>
            {
                new Tag("Technology"),
                new Tag("Programming"),
                new Tag("Web Development"),
                new Tag("C#"),
                new Tag("ASP.NET"),
                new Tag("Blazor"),
                new Tag("Tutorial"),
                new Tag("Best Practices"),
                new Tag("Architecture"),
                new Tag("Performance"),
                new Tag("Security"),
                new Tag("Design Patterns")
            };

            await _context.Tags.AddRangeAsync(sampleTags);
            await _context.SaveChangesAsync();

            var sampleAuthors = new List<Author>
            {
                new Author("John Doe", "Senior Software Engineer with 10+ years of experience in web development and system architecture.", "https://picsum.photos/150/150?random=101"),
                new Author("Jane Smith", "Full-stack developer passionate about clean code and modern web technologies.", "https://picsum.photos/150/150?random=102"),
                new Author("Mike Johnson", "DevOps engineer and cloud architecture specialist.", "https://picsum.photos/150/150?random=103"),
                new Author("Sarah Wilson", "Frontend developer and UI/UX designer.", "https://picsum.photos/150/150?random=104")
            };

            await _context.Authors.AddRangeAsync(sampleAuthors);
            await _context.SaveChangesAsync();

            var sampleBlogs = new List<Blog>
            {
                new Blog(
                    "Getting Started with ASP.NET Core",
                    "A comprehensive guide to building modern web applications with ASP.NET Core.",
                    "# Getting Started with ASP.NET Core\n\nASP.NET Core is a cross-platform, high-performance framework for building modern web applications. In this guide, we'll explore the fundamentals of ASP.NET Core and how to get started with your first application.\n\n## Key Features\n\n- **Cross-platform**: Runs on Windows, macOS, and Linux\n- **High performance**: Built for speed and scalability\n- **Modular**: Use only the components you need\n- **Dependency injection**: Built-in support for DI\n- **Configuration**: Flexible configuration system\n\n## Creating Your First Application\n\n```bash\ndotnet new web -n MyFirstApp\ncd MyFirstApp\ndotnet run\n```\n\nThis creates a minimal web application that you can run immediately.\n\n## Next Steps\n\n1. Explore the project structure\n2. Add controllers and views\n3. Implement dependency injection\n4. Configure middleware\n5. Add authentication and authorization\n\nASP.NET Core provides a solid foundation for building robust web applications. Start with the basics and gradually add more features as your application grows.",
                    sampleAuthors[0],
                    new List<Tag> { sampleTags[0], sampleTags[1], sampleTags[4], sampleTags[6] },
                    "https://picsum.photos/800/400?random=201",
                    new List<string> { "https://docs.microsoft.com/aspnet/core", "https://github.com/dotnet/aspnetcore" }
                ),

                new Blog(
                    "Building Interactive UIs with Blazor",
                    "Learn how to create rich, interactive web applications using Blazor and C#.",
                    "# Building Interactive UIs with Blazor\n\nBlazor is a revolutionary framework that allows you to build interactive web UIs using C# instead of JavaScript. This opens up new possibilities for .NET developers to create full-stack applications.\n\n## What is Blazor?\n\nBlazor is a framework for building interactive web UIs using C# and .NET. It can run on the server (Blazor Server) or in the browser via WebAssembly (Blazor WebAssembly).\n\n## Key Benefits\n\n- **Unified Development**: Use C# for both frontend and backend\n- **Component-based**: Reusable UI components\n- **Rich Ecosystem**: Leverage existing .NET libraries\n- **Strong Typing**: Compile-time error checking\n- **IntelliSense**: Full IDE support\n\n## Creating Components\n\n```razor\n@page \"/counter\"\n\n<h3>Counter</h3>\n\n<p>Current count: @currentCount</p>\n\n<button class=\"btn btn-primary\" @onclick=\"IncrementCount\">Click me</button>\n\n@code {\n    private int currentCount = 0;\n\n    private void IncrementCount()\n    {\n        currentCount++;\n    }\n}\n```\n\n## Advanced Features\n\n- **Data Binding**: Two-way data binding with forms\n- **JavaScript Interop**: Call JavaScript functions from C#\n- **Routing**: Client-side routing with parameters\n- **State Management**: Share state across components\n- **Authentication**: Integrate with Identity providers\n\nBlazor represents the future of web development for .NET developers, combining the power of C# with modern web technologies.",
                    sampleAuthors[1],
                    new List<Tag> { sampleTags[0], sampleTags[2], sampleTags[5], sampleTags[6] },
                    "https://picsum.photos/800/400?random=202",
                    new List<string> { "https://blazor.net", "https://docs.microsoft.com/aspnet/core/blazor" }
                ),

                new Blog(
                    "Microservices Architecture with .NET",
                    "Design and implement scalable microservices using .NET and modern cloud patterns.",
                    "# Microservices Architecture with .NET\n\nMicroservices architecture has become a popular approach for building scalable, maintainable applications. In this article, we'll explore how to implement microservices using .NET and modern cloud patterns.\n\n## What are Microservices?\n\nMicroservices are a architectural pattern where applications are built as a collection of small, independent services that communicate over well-defined APIs.\n\n## Benefits of Microservices\n\n- **Scalability**: Scale individual services independently\n- **Technology Diversity**: Use different technologies for different services\n- **Fault Isolation**: Failures in one service don't affect others\n- **Team Autonomy**: Teams can work independently on services\n- **Deployment Flexibility**: Deploy services independently\n\n## Key Patterns\n\n### API Gateway\n\n```csharp\nservices.AddReverseProxy()\n    .LoadFromConfig(Configuration.GetSection(\"ReverseProxy\"));\n```\n\n### Service Discovery\n\n```csharp\nservices.AddConsul(options =>\n{\n    options.Host = \"localhost\";\n    options.Port = 8500;\n});\n```\n\n### Circuit Breaker\n\n```csharp\nservices.AddHttpClient<IOrderService, OrderService>()\n    .AddPolicyHandler(GetRetryPolicy())\n    .AddPolicyHandler(GetCircuitBreakerPolicy());\n```\n\n## Challenges and Solutions\n\n- **Data Consistency**: Use saga pattern for distributed transactions\n- **Service Communication**: Implement async messaging with RabbitMQ or Azure Service Bus\n- **Monitoring**: Use distributed tracing with OpenTelemetry\n- **Security**: Implement OAuth 2.0 and JWT tokens\n\n## Best Practices\n\n1. **Start with a monolith** and decompose gradually\n2. **Design for failure** with circuit breakers and retries\n3. **Implement proper logging** and monitoring\n4. **Use containerization** with Docker and Kubernetes\n5. **Automate testing** and deployment pipelines\n\nMicroservices can provide significant benefits, but they also introduce complexity. Careful planning and the right tools are essential for success.",
                    sampleAuthors[2],
                    new List<Tag> { sampleTags[0], sampleTags[8], sampleTags[9], sampleTags[11] },
                    "https://picsum.photos/800/400?random=203",
                    new List<string> { "https://microservices.io", "https://docs.microsoft.com/dotnet/architecture/microservices" }
                ),

                new Blog(
                    "Secure Coding Practices in C#",
                    "Essential security practices every C# developer should follow to build secure applications.",
                    "# Secure Coding Practices in C#\n\nSecurity should be a primary concern in every software development project. This guide covers essential security practices that every C# developer should follow to build secure applications.\n\n## Input Validation\n\nAlways validate and sanitize user input:\n\n```csharp\npublic IActionResult CreateUser(UserModel model)\n{\n    if (!ModelState.IsValid)\n    {\n        return BadRequest(ModelState);\n    }\n    \n    // Additional validation\n    if (string.IsNullOrWhiteSpace(model.Email) || !IsValidEmail(model.Email))\n    {\n        return BadRequest(\"Invalid email address\");\n    }\n    \n    // Process the valid input\n    return Ok();\n}\n```\n\n## SQL Injection Prevention\n\nUse parameterized queries:\n\n```csharp\n// Good - Parameterized query\nvar user = context.Users\n    .FirstOrDefault(u => u.Email == email && u.Password == hashedPassword);\n\n// Bad - String concatenation\nvar query = $\"SELECT * FROM Users WHERE Email = '{email}' AND Password = '{password}'\";\n```\n\n## Authentication and Authorization\n\n```csharp\n[Authorize(Roles = \"Admin\")]\npublic IActionResult AdminOnly()\n{\n    // Only admin users can access this\n    return View();\n}\n\n[Authorize(Policy = \"RequireUserRole\")]\npublic IActionResult UserAction()\n{\n    // Policy-based authorization\n    return View();\n}\n```\n\n## Data Protection\n\n```csharp\n// Encrypt sensitive data\nvar dataProtector = serviceProvider.GetService<IDataProtector>();\nvar encryptedData = dataProtector.Protect(sensitiveData);\n\n// Hash passwords\nvar hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);\nvar isValid = BCrypt.Net.BCrypt.Verify(password, hashedPassword);\n```\n\n## HTTPS and Security Headers\n\n```csharp\nservices.AddHsts(options =>\n{\n    options.Preload = true;\n    options.IncludeSubDomains = true;\n    options.MaxAge = TimeSpan.FromDays(365);\n});\n\napp.UseHsts();\napp.UseHttpsRedirection();\n```\n\n## Error Handling\n\n```csharp\napp.UseExceptionHandler(\"/Error\");\napp.UseStatusCodePagesWithReExecute(\"/Error/{0}\");\n\n// Don't expose sensitive information in error messages\ntry\n{\n    // risky operation\n}\ncatch (Exception ex)\n{\n    logger.LogError(ex, \"Operation failed\");\n    return StatusCode(500, \"Internal server error\");\n}\n```\n\n## Security Checklist\n\n- ✅ Validate all input\n- ✅ Use parameterized queries\n- ✅ Implement proper authentication\n- ✅ Use HTTPS everywhere\n- ✅ Hash passwords properly\n- ✅ Implement rate limiting\n- ✅ Keep dependencies updated\n- ✅ Use security headers\n- ✅ Log security events\n- ✅ Regular security audits\n\nSecurity is an ongoing process, not a one-time task. Stay informed about new threats and update your practices accordingly.",
                    sampleAuthors[0],
                    new List<Tag> { sampleTags[0], sampleTags[3], sampleTags[10], sampleTags[7] },
                    "https://picsum.photos/800/400?random=204",
                    new List<string> { "https://owasp.org", "https://docs.microsoft.com/dotnet/standard/security" }
                ),

                new Blog(
                    "Modern Frontend Development with Blazor and Bootstrap",
                    "Create responsive, modern web interfaces using Blazor components and Bootstrap CSS framework.",
                    "# Modern Frontend Development with Blazor and Bootstrap\n\nCombining Blazor with Bootstrap creates a powerful foundation for building modern, responsive web applications. This guide shows you how to leverage both technologies effectively.\n\n## Why Blazor + Bootstrap?\n\n- **Rapid Development**: Pre-built components and utilities\n- **Responsive Design**: Mobile-first approach\n- **Consistent UI**: Unified design system\n- **Accessibility**: Built-in accessibility features\n- **Customization**: Easy to theme and customize\n\n## Setting Up Bootstrap in Blazor\n\n1. Install Bootstrap via CDN or npm\n2. Include CSS and JavaScript files\n3. Create reusable Blazor components\n\n```html\n<!-- In your layout file -->\n<link href=\"https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css\" rel=\"stylesheet\">\n<script src=\"https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/js/bootstrap.bundle.min.js\"></script>\n```\n\n## Creating Reusable Components\n\n### Alert Component\n\n```razor\n@* AlertComponent.razor *@\n<div class=\"alert alert-@AlertType @(Dismissible ? \"alert-dismissible\" : \"\")\" role=\"alert\">\n    @if (Dismissible)\n    {\n        <button type=\"button\" class=\"btn-close\" data-bs-dismiss=\"alert\" aria-label=\"Close\"></button>\n    }\n    @Message\n</div>\n\n@code {\n    [Parameter] public string AlertType { get; set; } = \"info\";\n    [Parameter] public string Message { get; set; } = \"\";\n    [Parameter] public bool Dismissible { get; set; } = false;\n}\n```\n\n### Card Component\n\n```razor\n@* CardComponent.razor *@\n<div class=\"card @CssClass\">\n    @if (!string.IsNullOrEmpty(ImageUrl))\n    {\n        <img src=\"@ImageUrl\" class=\"card-img-top\" alt=\"@ImageAlt\" />\n    }\n    <div class=\"card-body\">\n        @if (!string.IsNullOrEmpty(Title))\n        {\n            <h5 class=\"card-title\">@Title</h5>\n        }\n        @if (!string.IsNullOrEmpty(Text))\n        {\n            <p class=\"card-text\">@Text</p>\n        }\n        @ChildContent\n    </div>\n</div>\n\n@code {\n    [Parameter] public string Title { get; set; } = \"\";\n    [Parameter] public string Text { get; set; } = \"\";\n    [Parameter] public string ImageUrl { get; set; } = \"\";\n    [Parameter] public string ImageAlt { get; set; } = \"\";\n    [Parameter] public string CssClass { get; set; } = \"\";\n    [Parameter] public RenderFragment ChildContent { get; set; }\n}\n```\n\n## Responsive Layout\n\n```razor\n<div class=\"container-fluid\">\n    <div class=\"row\">\n        <div class=\"col-md-3 col-lg-2 sidebar\">\n            <NavMenu />\n        </div>\n        <div class=\"col-md-9 col-lg-10 main-content\">\n            @Body\n        </div>\n    </div>\n</div>\n```\n\n## Form Handling\n\n```razor\n<EditForm Model=\"@user\" OnValidSubmit=\"@HandleValidSubmit\">\n    <DataAnnotationsValidator />\n    <ValidationSummary />\n    \n    <div class=\"mb-3\">\n        <label class=\"form-label\">Name</label>\n        <InputText @bind-Value=\"user.Name\" class=\"form-control\" />\n        <ValidationMessage For=\"@(() => user.Name)\" />\n    </div>\n    \n    <div class=\"mb-3\">\n        <label class=\"form-label\">Email</label>\n        <InputText @bind-Value=\"user.Email\" class=\"form-control\" type=\"email\" />\n        <ValidationMessage For=\"@(() => user.Email)\" />\n    </div>\n    \n    <button type=\"submit\" class=\"btn btn-primary\">Submit</button>\n</EditForm>\n```\n\n## Best Practices\n\n1. **Use Bootstrap utilities** for spacing and layout\n2. **Create component libraries** for reusable UI elements\n3. **Implement responsive design** with Bootstrap's grid system\n4. **Customize Bootstrap** with CSS variables\n5. **Test on different devices** and screen sizes\n6. **Use semantic HTML** for better accessibility\n7. **Optimize performance** by lazy-loading components\n\n## Performance Tips\n\n- Use `@key` directive for list items\n- Implement virtual scrolling for large lists\n- Lazy load components with `@using` statements\n- Minimize re-renders with `ShouldRender()`\n\nBootstrap and Blazor complement each other perfectly, providing a solid foundation for modern web development. Start with the basics and gradually build more complex components as your application grows.",
                    sampleAuthors[3],
                    new List<Tag> { sampleTags[2], sampleTags[5], sampleTags[6], sampleTags[7] },
                    "https://picsum.photos/800/400?random=205",
                    new List<string> { "https://getbootstrap.com", "https://blazor.net" }
                ),

                new Blog(
                    "Understanding Entity Framework Core Performance",
                    "Tips and tricks to optimize your EF Core queries for better performance and scalability.",
                    "# Understanding Entity Framework Core Performance\n\nEntity Framework Core is a powerful ORM, but poor performance can be a significant issue if not handled properly. This guide covers essential performance optimization techniques.\n\n## Common Performance Issues\n\n### 1. N+1 Query Problem\n\n```csharp\n// Bad - Causes N+1 queries\nvar blogs = context.Blogs.ToList();\nforeach (var blog in blogs)\n{\n    Console.WriteLine($\"{blog.Title} by {blog.Author.Name}\");\n}\n\n// Good - Uses Include to eagerly load\nvar blogs = context.Blogs\n    .Include(b => b.Author)\n    .ToList();\n```\n\n### 2. Loading Too Much Data\n\n```csharp\n// Bad - Loads entire entities\nvar blogTitles = context.Blogs\n    .ToList()\n    .Select(b => b.Title);\n\n// Good - Projects only needed data\nvar blogTitles = context.Blogs\n    .Select(b => b.Title)\n    .ToList();\n```\n\n## Performance Best Practices\n\n### Use AsNoTracking for Read-Only Queries\n\n```csharp\nvar blogs = await context.Blogs\n    .AsNoTracking()\n    .Where(b => b.IsPublished)\n    .ToListAsync();\n```\n\n### Implement Proper Pagination\n\n```csharp\npublic async Task<List<Blog>> GetBlogsAsync(int page, int pageSize)\n{\n    return await context.Blogs\n        .OrderByDescending(b => b.CreatedAt)\n        .Skip((page - 1) * pageSize)\n        .Take(pageSize)\n        .AsNoTracking()\n        .ToListAsync();\n}\n```\n\n### Use Split Queries for Multiple Includes\n\n```csharp\nvar blogs = await context.Blogs\n    .AsSplitQuery()\n    .Include(b => b.Author)\n    .Include(b => b.Tags)\n    .Include(b => b.Comments)\n    .ToListAsync();\n```\n\n## Monitoring and Debugging\n\n### Enable Sensitive Data Logging (Development Only)\n\n```csharp\nbuilder.Services.AddDbContext<AppDbContext>(options =>\n    options.UseNpgsql(connectionString)\n           .EnableSensitiveDataLogging()\n           .LogTo(Console.WriteLine));\n```\n\n### Use EF Core Interceptors\n\n```csharp\npublic class QueryLoggingInterceptor : DbCommandInterceptor\n{\n    public override InterceptionResult<DbDataReader> ReaderExecuting(\n        DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result)\n    {\n        if (command.CommandText.Contains(\"SELECT\"))\n        {\n            Console.WriteLine($\"Executing query: {command.CommandText}\");\n        }\n        return result;\n    }\n}\n```\n\n## Performance Checklist\n\n- ✅ Use projections for read-only data\n- ✅ Implement AsNoTracking for queries\n- ✅ Avoid N+1 query problems with Include\n- ✅ Use pagination for large datasets\n- ✅ Consider split queries for multiple includes\n- ✅ Index frequently queried columns\n- ✅ Use compiled queries for repeated operations\n- ✅ Monitor query execution plans\n\nProper EF Core performance optimization can dramatically improve your application's responsiveness and scalability.",
                    sampleAuthors[2],
                    new List<Tag> { sampleTags[0], sampleTags[3], sampleTags[9], sampleTags[11] },
                    "https://picsum.photos/800/400?random=206",
                    new List<string> { "https://docs.microsoft.com/ef/core/performance", "https://www.entityframeworktutorial.net/efcore/entity-framework-core.aspx" }
                ),

                new Blog(
                    "Implementing Clean Architecture in .NET",
                    "Learn how to structure your .NET applications using Clean Architecture principles for maintainability and testability.",
                    "# Implementing Clean Architecture in .NET\n\nClean Architecture, popularized by Robert C. Martin (Uncle Bob), provides a way to structure applications that are independent of frameworks, databases, and external concerns.\n\n## Core Principles\n\n### 1. Dependency Rule\n\nSource code dependencies must point inward toward higher-level policies. Inner layers should not know about outer layers.\n\n### 2. Layer Structure\n\n```\n┌─────────────────────────────────────┐\n│           Presentation              │  ← Controllers, Views, APIs\n├─────────────────────────────────────┤\n│           Application               │  ← Use Cases, Services\n├─────────────────────────────────────┤\n│             Domain                  │  ← Entities, Business Rules\n├─────────────────────────────────────┤\n│         Infrastructure              │  ← Data Access, External APIs\n└─────────────────────────────────────┘\n```\n\n## Project Structure\n\n```\nMyApp.Domain/\n├── Entities/\n│   ├── Blog.cs\n│   └── Author.cs\n├── Interfaces/\n│   └── IBlogRepository.cs\n└── ValueObjects/\n    └── Email.cs\n\nMyApp.Application/\n├── UseCases/\n│   ├── CreateBlog/\n│   │   ├── CreateBlogCommand.cs\n│   │   └── CreateBlogHandler.cs\n│   └── GetBlogs/\n│       ├── GetBlogsQuery.cs\n│       └── GetBlogsHandler.cs\n└── Interfaces/\n    └── IUnitOfWork.cs\n\nMyApp.Infrastructure/\n├── Persistence/\n│   ├── AppDbContext.cs\n│   └── BlogRepository.cs\n└── Services/\n    └── EmailService.cs\n\nMyApp.Presentation/\n├── Controllers/\n│   └── BlogController.cs\n└── Program.cs\n```\n\n## Domain Layer Implementation\n\n```csharp\n// Domain Entity\npublic class Blog : Entity\n{\n    public string Title { get; private set; }\n    public string Content { get; private set; }\n    public Author Author { get; private set; }\n    public DateTime PublishedAt { get; private set; }\n    \n    private Blog() { } // EF Constructor\n    \n    public Blog(string title, string content, Author author)\n    {\n        Title = Guard.Against.NullOrEmpty(title);\n        Content = Guard.Against.NullOrEmpty(content);\n        Author = Guard.Against.Null(author);\n        PublishedAt = DateTime.UtcNow;\n    }\n    \n    public void UpdateContent(string newContent)\n    {\n        Content = Guard.Against.NullOrEmpty(newContent);\n        // Raise domain event if needed\n    }\n}\n```\n\n## Application Layer with CQRS\n\n```csharp\n// Command\npublic record CreateBlogCommand(string Title, string Content, Guid AuthorId) : IRequest<Guid>;\n\n// Handler\npublic class CreateBlogHandler : IRequestHandler<CreateBlogCommand, Guid>\n{\n    private readonly IBlogRepository _blogRepository;\n    private readonly IAuthorRepository _authorRepository;\n    private readonly IUnitOfWork _unitOfWork;\n    \n    public CreateBlogHandler(IBlogRepository blogRepository, \n                           IAuthorRepository authorRepository,\n                           IUnitOfWork unitOfWork)\n    {\n        _blogRepository = blogRepository;\n        _authorRepository = authorRepository;\n        _unitOfWork = unitOfWork;\n    }\n    \n    public async Task<Guid> Handle(CreateBlogCommand request, CancellationToken cancellationToken)\n    {\n        var author = await _authorRepository.GetByIdAsync(request.AuthorId);\n        Guard.Against.Null(author, nameof(author));\n        \n        var blog = new Blog(request.Title, request.Content, author);\n        await _blogRepository.AddAsync(blog);\n        await _unitOfWork.SaveChangesAsync(cancellationToken);\n        \n        return blog.Id;\n    }\n}\n```\n\n## Infrastructure Layer\n\n```csharp\n// Repository Implementation\npublic class BlogRepository : IBlogRepository\n{\n    private readonly AppDbContext _context;\n    \n    public BlogRepository(AppDbContext context)\n    {\n        _context = context;\n    }\n    \n    public async Task<Blog?> GetByIdAsync(Guid id)\n    {\n        return await _context.Blogs\n            .Include(b => b.Author)\n            .FirstOrDefaultAsync(b => b.Id == id);\n    }\n    \n    public async Task AddAsync(Blog blog)\n    {\n        await _context.Blogs.AddAsync(blog);\n    }\n}\n```\n\n## Dependency Injection Setup\n\n```csharp\n// Program.cs\nbuilder.Services.AddScoped<IBlogRepository, BlogRepository>();\nbuilder.Services.AddScoped<IUnitOfWork, UnitOfWork>();\nbuilder.Services.AddMediatR(typeof(CreateBlogHandler));\n```\n\n## Benefits of Clean Architecture\n\n- **Testability**: Business logic is isolated and easily testable\n- **Independence**: Framework and database agnostic\n- **Maintainability**: Clear separation of concerns\n- **Flexibility**: Easy to swap implementations\n- **Scalability**: Well-defined boundaries for team development\n\n## Common Pitfalls to Avoid\n\n1. **Over-engineering**: Don't apply Clean Architecture to simple CRUD applications\n2. **Leaky abstractions**: Ensure domain doesn't depend on infrastructure\n3. **Anemic domain models**: Put business logic in domain entities\n4. **Too many layers**: Don't create layers just for the sake of it\n\nClean Architecture provides excellent structure for complex applications but requires careful consideration of when and how to apply it.",
                    sampleAuthors[0],
                    new List<Tag> { sampleTags[0], sampleTags[8], sampleTags[11], sampleTags[7] },
                    "https://picsum.photos/800/400?random=207",
                    new List<string> { "https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html", "https://docs.microsoft.com/dotnet/architecture/modern-web-apps-azure" }
                ),

                new Blog(
                    "Docker Containerization for .NET Applications",
                    "Complete guide to containerizing .NET applications with Docker for development and production environments.",
                    "# Docker Containerization for .NET Applications\n\nContainerization has revolutionized application deployment and development workflows. This guide covers everything you need to know about Docker and .NET.\n\n## Why Containerize .NET Applications?\n\n- **Consistency**: Same environment across development, testing, and production\n- **Portability**: Run anywhere Docker is supported\n- **Scalability**: Easy horizontal scaling with orchestrators\n- **Isolation**: Applications run in isolated environments\n- **Efficiency**: Better resource utilization than VMs\n\n## Basic Dockerfile for .NET\n\n```dockerfile\n# Multi-stage build for optimization\nFROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base\nWORKDIR /app\nEXPOSE 8080\nEXPOSE 8081\n\nFROM mcr.microsoft.com/dotnet/sdk:9.0 AS build\nWORKDIR /src\nCOPY [\"MyApp/MyApp.csproj\", \"MyApp/\"]\nRUN dotnet restore \"MyApp/MyApp.csproj\"\nCOPY . .\nWORKDIR \"/src/MyApp\"\nRUN dotnet build \"MyApp.csproj\" -c Release -o /app/build\n\nFROM build AS publish\nRUN dotnet publish \"MyApp.csproj\" -c Release -o /app/publish /p:UseAppHost=false\n\nFROM base AS final\nWORKDIR /app\nCOPY --from=publish /app/publish .\nENTRYPOINT [\"dotnet\", \"MyApp.dll\"]\n```\n\n## Optimized Production Dockerfile\n\n```dockerfile\n# Use Alpine Linux for smaller image size\nFROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS base\nWORKDIR /app\nEXPOSE 8080\n\n# Install necessary packages\nRUN apk add --no-cache icu-libs\nENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false\n\nFROM mcr.microsoft.com/dotnet/sdk:9.0 AS build\nWORKDIR /src\n\n# Copy csproj files first for better layer caching\nCOPY [\"src/MyApp.Api/MyApp.Api.csproj\", \"src/MyApp.Api/\"]\nCOPY [\"src/MyApp.Domain/MyApp.Domain.csproj\", \"src/MyApp.Domain/\"]\nCOPY [\"src/MyApp.Infrastructure/MyApp.Infrastructure.csproj\", \"src/MyApp.Infrastructure/\"]\n\nRUN dotnet restore \"src/MyApp.Api/MyApp.Api.csproj\"\n\nCOPY . .\nWORKDIR \"/src/src/MyApp.Api\"\nRUN dotnet build \"MyApp.Api.csproj\" -c Release -o /app/build\n\nFROM build AS publish\nRUN dotnet publish \"MyApp.Api.csproj\" -c Release -o /app/publish \\\n    --self-contained false \\\n    --no-restore\n\nFROM base AS final\nWORKDIR /app\n\n# Create non-root user for security\nRUN addgroup -g 1001 -S appuser && \\\n    adduser -S appuser -G appuser -u 1001\n\nCOPY --from=publish /app/publish .\nRUN chown -R appuser:appuser /app\nUSER appuser\n\nENTRYPOINT [\"dotnet\", \"MyApp.Api.dll\"]\n```\n\n## Docker Compose for Development\n\n```yaml\n# docker-compose.yml\nversion: '3.8'\n\nservices:\n  web:\n    build:\n      context: .\n      dockerfile: Dockerfile\n    ports:\n      - \"5000:8080\"\n    environment:\n      - ASPNETCORE_ENVIRONMENT=Development\n      - ConnectionStrings__DefaultConnection=Host=db;Database=myapp;Username=postgres;Password=password\n    depends_on:\n      - db\n    volumes:\n      - ./src:/app/src:ro\n    networks:\n      - myapp-network\n\n  db:\n    image: postgres:15-alpine\n    environment:\n      POSTGRES_DB: myapp\n      POSTGRES_USER: postgres\n      POSTGRES_PASSWORD: password\n    ports:\n      - \"5432:5432\"\n    volumes:\n      - postgres_data:/var/lib/postgresql/data\n    networks:\n      - myapp-network\n\n  redis:\n    image: redis:7-alpine\n    ports:\n      - \"6379:6379\"\n    networks:\n      - myapp-network\n\nvolumes:\n  postgres_data:\n\nnetworks:\n  myapp-network:\n    driver: bridge\n```\n\n## Health Checks and Monitoring\n\n```dockerfile\n# Add health check to Dockerfile\nHEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \\\n  CMD curl -f http://localhost:8080/health || exit 1\n```\n\n```csharp\n// Add health checks in Program.cs\nbuilder.Services.AddHealthChecks()\n    .AddDbContext<AppDbContext>()\n    .AddRedis(builder.Configuration.GetConnectionString(\"Redis\"))\n    .AddCheck(\"self\", () => HealthCheckResult.Healthy());\n\napp.MapHealthChecks(\"/health\");\napp.MapHealthChecks(\"/health/ready\", new HealthCheckOptions\n{\n    Predicate = check => check.Tags.Contains(\"ready\")\n});\n```\n\n## Production Deployment with Kubernetes\n\n```yaml\n# deployment.yaml\napiVersion: apps/v1\nkind: Deployment\nmetadata:\n  name: myapp-deployment\nspec:\n  replicas: 3\n  selector:\n    matchLabels:\n      app: myapp\n  template:\n    metadata:\n      labels:\n        app: myapp\n    spec:\n      containers:\n      - name: myapp\n        image: myregistry/myapp:latest\n        ports:\n        - containerPort: 8080\n        env:\n        - name: ConnectionStrings__DefaultConnection\n          valueFrom:\n            secretKeyRef:\n              name: myapp-secrets\n              key: connection-string\n        livenessProbe:\n          httpGet:\n            path: /health\n            port: 8080\n          initialDelaySeconds: 30\n          periodSeconds: 10\n        readinessProbe:\n          httpGet:\n            path: /health/ready\n            port: 8080\n          initialDelaySeconds: 5\n          periodSeconds: 5\n        resources:\n          requests:\n            memory: \"256Mi\"\n            cpu: \"250m\"\n          limits:\n            memory: \"512Mi\"\n            cpu: \"500m\"\n```\n\n## Docker Best Practices\n\n### Security\n- Use non-root users in containers\n- Scan images for vulnerabilities\n- Use minimal base images (Alpine)\n- Keep secrets out of images\n\n### Performance\n- Use multi-stage builds\n- Optimize layer caching\n- Use .dockerignore effectively\n- Consider ReadOnlyRootFilesystem\n\n### Development Workflow\n- Use Docker Compose for local development\n- Implement hot reload for faster development\n- Use volume mounts for source code\n- Standardize development environments\n\nContainerization with Docker provides a robust foundation for modern .NET application deployment and development workflows.",
                    sampleAuthors[2],
                    new List<Tag> { sampleTags[0], sampleTags[1], sampleTags[8], sampleTags[9] },
                    "https://picsum.photos/800/400?random=208",
                    new List<string> { "https://docs.docker.com/", "https://docs.microsoft.com/dotnet/core/docker/introduction" }
                ),

                new Blog(
                    "Advanced C# Features You Should Know",
                    "Explore powerful C# features that can make your code more efficient, readable, and maintainable.",
                    "# Advanced C# Features You Should Know\n\nC# continues to evolve with each release, introducing powerful features that can significantly improve your code quality and developer productivity.\n\n## Pattern Matching Evolution\n\n### Switch Expressions (C# 8.0)\n\n```csharp\npublic static string GetDayType(DayOfWeek day) => day switch\n{\n    DayOfWeek.Saturday or DayOfWeek.Sunday => \"Weekend\",\n    DayOfWeek.Monday => \"Start of work week\",\n    DayOfWeek.Friday => \"TGIF!\",\n    _ => \"Weekday\"\n};\n```\n\n### Property Patterns (C# 10.0)\n\n```csharp\npublic static string AnalyzeObject(object obj) => obj switch\n{\n    string { Length: > 10 } => \"Long string\",\n    string { Length: 0 } => \"Empty string\",\n    List<int> { Count: 0 } => \"Empty list\",\n    List<int> { Count: > 100 } => \"Large list\",\n    Person { Age: >= 18, Name.Length: > 0 } => \"Adult with name\",\n    _ => \"Something else\"\n};\n```\n\n### List Patterns (C# 11.0)\n\n```csharp\npublic static string AnalyzeList(int[] numbers) => numbers switch\n{\n    [] => \"Empty array\",\n    [var x] => $\"Single element: {x}\",\n    [var first, .., var last] => $\"First: {first}, Last: {last}\",\n    [1, 2, 3] => \"Exactly 1, 2, 3\",\n    [1, .., 3] => \"Starts with 1, ends with 3\",\n    _ => \"Complex pattern\"\n};\n```\n\n## Records and Value Types\n\n### Record Types (C# 9.0)\n\n```csharp\n// Immutable record\npublic record Person(string FirstName, string LastName)\n{\n    public string FullName => $\"{FirstName} {LastName}\";\n}\n\n// Usage\nvar person1 = new Person(\"John\", \"Doe\");\nvar person2 = person1 with { LastName = \"Smith\" }; // Non-destructive mutation\n\n// Value equality\nConsole.WriteLine(person1 == new Person(\"John\", \"Doe\")); // True\n```\n\n### Record Structs (C# 10.0)\n\n```csharp\npublic readonly record struct Point(double X, double Y)\n{\n    public double DistanceFromOrigin => Math.Sqrt(X * X + Y * Y);\n}\n```\n\n## Nullable Reference Types (C# 8.0)\n\n```csharp\n#nullable enable\n\npublic class UserService\n{\n    public User? FindUser(string? email)\n    {\n        if (string.IsNullOrEmpty(email))\n            return null;\n            \n        // Compiler knows email is not null here\n        return database.Users.FirstOrDefault(u => u.Email == email);\n    }\n    \n    public void ProcessUser(User user) // user cannot be null\n    {\n        Console.WriteLine(user.Name); // Safe without null check\n    }\n}\n```\n\n## Async Streams (C# 8.0)\n\n```csharp\npublic static async IAsyncEnumerable<int> GenerateNumbersAsync(\n    [EnumeratorCancellation] CancellationToken cancellationToken = default)\n{\n    for (int i = 0; i < 100; i++)\n    {\n        cancellationToken.ThrowIfCancellationRequested();\n        await Task.Delay(100, cancellationToken);\n        yield return i;\n    }\n}\n\n// Usage\nawait foreach (var number in GenerateNumbersAsync())\n{\n    Console.WriteLine(number);\n}\n```\n\n## Primary Constructors (C# 12.0)\n\n```csharp\npublic class BlogService(ILogger<BlogService> logger, IBlogRepository repository)\n{\n    public async Task<Blog> CreateBlogAsync(string title, string content)\n    {\n        logger.LogInformation(\"Creating blog: {Title}\", title);\n        \n        var blog = new Blog(title, content);\n        await repository.AddAsync(blog);\n        \n        return blog;\n    }\n}\n```\n\n## Generic Math (C# 11.0)\n\n```csharp\npublic static T Add<T>(T left, T right) where T : INumber<T>\n{\n    return left + right;\n}\n\n// Works with any numeric type\nvar intResult = Add(5, 10);           // int\nvar doubleResult = Add(3.14, 2.86);   // double\nvar decimalResult = Add(1.5m, 2.5m);  // decimal\n```\n\n## File-Scoped Namespaces (C# 10.0)\n\n```csharp\nnamespace MyApp.Services; // File-scoped namespace\n\nusing Microsoft.Extensions.Logging;\n\npublic class EmailService\n{\n    // Class implementation\n}\n```\n\n## Raw String Literals (C# 11.0)\n\n```csharp\n// Perfect for JSON, SQL, or any multi-line string\nstring json = \"\"\"\n    {\n        \"name\": \"John Doe\",\n        \"email\": \"john@example.com\",\n        \"roles\": [\"admin\", \"user\"]\n    }\n    \"\"\";\n\nstring sql = \"\"\"\n    SELECT u.Name, u.Email, COUNT(b.Id) as BlogCount\n    FROM Users u\n    LEFT JOIN Blogs b ON u.Id = b.AuthorId\n    WHERE u.IsActive = true\n    GROUP BY u.Id, u.Name, u.Email\n    ORDER BY BlogCount DESC\n    \"\"\";\n```\n\n## Required Members (C# 11.0)\n\n```csharp\npublic class Person\n{\n    public required string FirstName { get; init; }\n    public required string LastName { get; init; }\n    public string? MiddleName { get; init; }\n}\n\n// Compiler enforces required properties\nvar person = new Person\n{\n    FirstName = \"John\",\n    LastName = \"Doe\"\n    // MiddleName is optional\n};\n```\n\n## Collection Expressions (C# 12.0)\n\n```csharp\n// Various ways to create collections\nint[] array = [1, 2, 3, 4, 5];\nList<string> list = [\"apple\", \"banana\", \"cherry\"];\nSpan<int> span = [1, 2, 3];\n\n// Spread operator\nint[] numbers1 = [1, 2, 3];\nint[] numbers2 = [4, 5, 6];\nint[] combined = [..numbers1, ..numbers2]; // [1, 2, 3, 4, 5, 6]\n```\n\n## Performance Tips\n\n### Use Span<T> and Memory<T>\n\n```csharp\npublic static void ProcessData(ReadOnlySpan<byte> data)\n{\n    // No allocation, works with arrays, strings, stack memory\n    foreach (byte b in data)\n    {\n        // Process byte\n    }\n}\n```\n\n### ValueTask for High-Performance Async\n\n```csharp\npublic ValueTask<User> GetUserFromCacheAsync(int id)\n{\n    var cachedUser = cache.Get<User>(id);\n    if (cachedUser != null)\n        return ValueTask.FromResult(cachedUser); // No allocation\n        \n    return GetUserFromDatabaseAsync(id); // Returns Task<User>\n}\n```\n\n## Best Practices\n\n1. **Embrace nullable reference types** for better null safety\n2. **Use records for immutable data** transfer objects\n3. **Leverage pattern matching** for cleaner conditional logic\n4. **Prefer ValueTask** for hot async paths\n5. **Use primary constructors** to reduce boilerplate\n6. **Apply required members** for better API design\n\nThese advanced C# features can significantly improve code quality, performance, and maintainability when used appropriately.",
                    sampleAuthors[1],
                    new List<Tag> { sampleTags[3], sampleTags[1], sampleTags[7], sampleTags[9] },
                    "https://picsum.photos/800/400?random=209",
                    new List<string> { "https://docs.microsoft.com/dotnet/csharp/whats-new", "https://github.com/dotnet/csharplang" }
                ),

                new Blog(
                    "Building Resilient APIs with Polly",
                    "Learn how to implement retry policies, circuit breakers, and other resilience patterns in your .NET APIs.",
                    "# Building Resilient APIs with Polly\n\nIn distributed systems, failures are inevitable. Network issues, service outages, and transient errors can disrupt your application. Polly is a .NET resilience and transient-fault-handling library that helps you handle these scenarios gracefully.\n\n## Why Resilience Matters\n\n- **Improved User Experience**: Graceful handling of failures\n- **Better System Stability**: Prevent cascade failures\n- **Reduced Support Burden**: Fewer incidents requiring manual intervention\n- **Higher Availability**: Systems that self-heal and adapt\n\n## Installing Polly\n\n```bash\ndotnet add package Polly\ndotnet add package Polly.Extensions.Http\ndotnet add package Microsoft.Extensions.Http.Polly\n```\n\n## Basic Retry Policies\n\n### Simple Retry\n\n```csharp\nusing Polly;\n\npublic class ApiService\n{\n    private readonly HttpClient _httpClient;\n    private readonly IAsyncPolicy<HttpResponseMessage> _retryPolicy;\n    \n    public ApiService(HttpClient httpClient)\n    {\n        _httpClient = httpClient;\n        _retryPolicy = Policy\n            .HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)\n            .Or<HttpRequestException>()\n            .WaitAndRetryAsync(\n                retryCount: 3,\n                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), // Exponential backoff\n                onRetry: (outcome, timespan, retryCount, context) =>\n                {\n                    Console.WriteLine($\"Retry #{retryCount} after {timespan} seconds\");\n                });\n    }\n    \n    public async Task<string> GetDataAsync(string endpoint)\n    {\n        var response = await _retryPolicy.ExecuteAsync(() => \n            _httpClient.GetAsync(endpoint));\n            \n        return await response.Content.ReadAsStringAsync();\n    }\n}\n```\n\n### Advanced Retry with Jitter\n\n```csharp\npublic static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()\n{\n    var jitterer = new Random();\n    \n    return Policy\n        .HandleResult<HttpResponseMessage>(r => \n            r.StatusCode >= HttpStatusCode.InternalServerError ||\n            r.StatusCode == HttpStatusCode.RequestTimeout)\n        .WaitAndRetryAsync(\n            retryCount: 5,\n            sleepDurationProvider: (retryAttempt, context) =>\n            {\n                var baseDelay = TimeSpan.FromSeconds(Math.Pow(2, retryAttempt));\n                var jitter = TimeSpan.FromMilliseconds(jitterer.Next(0, 1000));\n                return baseDelay + jitter;\n            },\n            onRetry: (outcome, timespan, retryCount, context) =>\n            {\n                var logger = context.GetLogger();\n                logger?.LogWarning(\"Retry #{RetryCount} after {Delay}ms. Reason: {Reason}\", \n                    retryCount, timespan.TotalMilliseconds, outcome.Exception?.Message ?? outcome.Result.StatusCode.ToString());\n            });\n}\n```\n\n## Circuit Breaker Pattern\n\n```csharp\npublic class ResilientApiService\n{\n    private readonly HttpClient _httpClient;\n    private readonly IAsyncPolicy<HttpResponseMessage> _circuitBreakerPolicy;\n    \n    public ResilientApiService(HttpClient httpClient, ILogger<ResilientApiService> logger)\n    {\n        _httpClient = httpClient;\n        _circuitBreakerPolicy = Policy\n            .HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)\n            .CircuitBreakerAsync(\n                handledEventsAllowedBeforeBreaking: 3,\n                durationOfBreak: TimeSpan.FromSeconds(30),\n                onBreak: (result, timespan) =>\n                {\n                    logger.LogWarning(\"Circuit breaker opened for {Duration} seconds\", timespan.TotalSeconds);\n                },\n                onReset: () =>\n                {\n                    logger.LogInformation(\"Circuit breaker closed\");\n                },\n                onHalfOpen: () =>\n                {\n                    logger.LogInformation(\"Circuit breaker half-open\");\n                });\n    }\n    \n    public async Task<ApiResponse<T>> GetAsync<T>(string endpoint)\n    {\n        try\n        {\n            var response = await _circuitBreakerPolicy.ExecuteAsync(() => \n                _httpClient.GetAsync(endpoint));\n                \n            if (response.IsSuccessStatusCode)\n            {\n                var content = await response.Content.ReadAsStringAsync();\n                var data = JsonSerializer.Deserialize<T>(content);\n                return ApiResponse<T>.Success(data);\n            }\n            \n            return ApiResponse<T>.Failure($\"API returned {response.StatusCode}\");\n        }\n        catch (CircuitBreakerOpenException)\n        {\n            return ApiResponse<T>.Failure(\"Service temporarily unavailable\");\n        }\n    }\n}\n```\n\n## Combining Policies with PolicyWrap\n\n```csharp\npublic static IAsyncPolicy<HttpResponseMessage> GetCombinedPolicy()\n{\n    // Retry policy\n    var retryPolicy = Policy\n        .HandleResult<HttpResponseMessage>(r => \n            r.StatusCode >= HttpStatusCode.InternalServerError)\n        .WaitAndRetryAsync(\n            retryCount: 3,\n            sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));\n    \n    // Circuit breaker policy\n    var circuitBreakerPolicy = Policy\n        .HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)\n        .CircuitBreakerAsync(\n            handledEventsAllowedBeforeBreaking: 5,\n            durationOfBreak: TimeSpan.FromSeconds(30));\n    \n    // Timeout policy\n    var timeoutPolicy = Policy.TimeoutAsync<HttpResponseMessage>(10);\n    \n    // Combine policies: Timeout -> CircuitBreaker -> Retry\n    return Policy.WrapAsync(timeoutPolicy, circuitBreakerPolicy, retryPolicy);\n}\n```\n\n## Using Polly with HttpClient Factory\n\n```csharp\n// Program.cs\nbuilder.Services.AddHttpClient<ExternalApiService>(client =>\n{\n    client.BaseAddress = new Uri(\"https://api.external-service.com/\");\n    client.Timeout = TimeSpan.FromSeconds(30);\n})\n.AddPolicyHandler(GetRetryPolicy())\n.AddPolicyHandler(GetCircuitBreakerPolicy());\n\npublic static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()\n{\n    return HttpPolicyExtensions\n        .HandleTransientHttpError() // Handles HttpRequestException and 5XX, 408 status codes\n        .WaitAndRetryAsync(\n            retryCount: 3,\n            sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),\n            onRetry: (outcome, timespan, retryCount, context) =>\n            {\n                var logger = context.GetLogger();\n                logger?.LogWarning(\"Delaying for {Delay}ms, then making retry #{Retry}\", \n                    timespan.TotalMilliseconds, retryCount);\n            });\n}\n```\n\n## Fallback Strategies\n\n```csharp\npublic class WeatherService\n{\n    private readonly HttpClient _httpClient;\n    private readonly IMemoryCache _cache;\n    private readonly ILogger<WeatherService> _logger;\n    \n    public WeatherService(HttpClient httpClient, IMemoryCache cache, ILogger<WeatherService> logger)\n    {\n        _httpClient = httpClient;\n        _cache = cache;\n        _logger = logger;\n    }\n    \n    public async Task<WeatherData> GetWeatherAsync(string city)\n    {\n        var fallbackPolicy = Policy<WeatherData>\n            .Handle<Exception>()\n            .FallbackAsync(\n                fallbackValue: GetCachedWeather(city) ?? GetDefaultWeather(city),\n                onFallback: (delegateResult, context) =>\n                {\n                    _logger.LogWarning(\"Fallback triggered for weather data: {City}\", city);\n                    return Task.CompletedTask;\n                });\n        \n        var retryPolicy = Policy\n            .Handle<HttpRequestException>()\n            .WaitAndRetryAsync(\n                retryCount: 2,\n                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(retryAttempt));\n        \n        var combinedPolicy = Policy.WrapAsync(fallbackPolicy, retryPolicy);\n        \n        return await combinedPolicy.ExecuteAsync(async () =>\n        {\n            var response = await _httpClient.GetAsync($\"/weather?city={city}\");\n            response.EnsureSuccessStatusCode();\n            \n            var content = await response.Content.ReadAsStringAsync();\n            var weather = JsonSerializer.Deserialize<WeatherData>(content);\n            \n            // Cache successful response\n            _cache.Set($\"weather_{city}\", weather, TimeSpan.FromMinutes(30));\n            \n            return weather;\n        });\n    }\n    \n    private WeatherData? GetCachedWeather(string city)\n    {\n        return _cache.Get<WeatherData>($\"weather_{city}\");\n    }\n    \n    private WeatherData GetDefaultWeather(string city)\n    {\n        return new WeatherData\n        {\n            City = city,\n            Temperature = \"N/A\",\n            Description = \"Weather data temporarily unavailable\",\n            IsDefault = true\n        };\n    }\n}\n```\n\n## Bulkhead Isolation\n\n```csharp\npublic class RateLimitedService\n{\n    private readonly SemaphoreSlim _semaphore;\n    \n    public RateLimitedService()\n    {\n        // Allow maximum 10 concurrent operations\n        _semaphore = new SemaphoreSlim(10, 10);\n    }\n    \n    public async Task<T> ExecuteAsync<T>(Func<Task<T>> operation)\n    {\n        var bulkheadPolicy = Policy\n            .BulkheadAsync<T>(\n                maxParallelization: 10,\n                maxQueuingActions: 20,\n                onBulkheadRejected: context =>\n                {\n                    throw new InvalidOperationException(\"Bulkhead capacity exceeded\");\n                });\n        \n        return await bulkheadPolicy.ExecuteAsync(operation);\n    }\n}\n```\n\n## Monitoring and Metrics\n\n```csharp\npublic class PolicyMetrics\n{\n    private readonly IMetrics _metrics;\n    \n    public IAsyncPolicy<HttpResponseMessage> CreateInstrumentedPolicy()\n    {\n        return Policy\n            .HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)\n            .WaitAndRetryAsync(\n                retryCount: 3,\n                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),\n                onRetry: (outcome, timespan, retryCount, context) =>\n                {\n                    _metrics.Counter(\"http_retries_total\")\n                           .WithTag(\"retry_attempt\", retryCount.ToString())\n                           .Increment();\n                });\n    }\n}\n```\n\n## Best Practices\n\n1. **Choose appropriate timeouts** - Not too short, not too long\n2. **Use exponential backoff with jitter** to prevent thundering herd\n3. **Monitor policy executions** with metrics and logging\n4. **Test failure scenarios** in your integration tests\n5. **Configure circuit breakers carefully** - Consider your system's characteristics\n6. **Implement proper fallbacks** - Cached data, default values, or graceful degradation\n7. **Use PolicyWrap judiciously** - Order matters (Timeout → CircuitBreaker → Retry)\n\nPolly helps you build robust, self-healing applications that gracefully handle the inevitable failures in distributed systems.",
                    sampleAuthors[2],
                    new List<Tag> { sampleTags[0], sampleTags[8], sampleTags[9], sampleTags[7] },
                    "https://picsum.photos/800/400?random=210",
                    new List<string> { "https://github.com/App-vNext/Polly", "https://docs.microsoft.com/dotnet/architecture/microservices/implement-resilient-applications" }
                )
            };

            await _context.Blogs.AddRangeAsync(sampleBlogs);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Seeded {Count} sample blogs with {TagCount} tags and {AuthorCount} authors", 
                sampleBlogs.Count, sampleTags.Count, sampleAuthors.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error seeding blog data");
        }
    }
}