using DatabaseManager;
using FragmentLibrary;
using BlogLibrary;
using BlogLibrary.Interfaces;
using SharedLibrary;
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
                new Fragment(
                    new LocalizedText("Welcome to Fragments", "Bienvenue dans les Fragments"), 
                    new LocalizedText("Introduction to the fragment system", "Introduction au système de fragments"), 
                    new LocalizedText("# Welcome to Fragments\n\nThis is the first fragment in the system. Fragments are short pieces of content that can be ideas, code snippets, quotes, or any other creative expression.\n\n## Features\n- Markdown support\n- Type categorization\n- Tagging system\n- Vibe counting",
                        "# Bienvenue dans les Fragments\n\nCeci est le premier fragment du système. Les fragments sont de courts éléments de contenu qui peuvent être des idées, des extraits de code, des citations ou toute autre expression créative.\n\n## Fonctionnalités\n- Support Markdown\n- Catégorisation par type\n- Système d'étiquetage\n- Comptage de vibrations"), 
                    "https://picsum.photos/400/300?random=1")
                {
                    TypeTag = "fragment",
                    AttachmentType = "image",
                    VibeCount = 3,
                    IsPublic = true,
                    Tags = new List<string> { "welcome", "introduction", "system" },
                    Meta = new Dictionary<string, string> { { "featured", "true" }, { "priority", "high" } }
                },

                new Fragment(
                    new LocalizedText("Code Organization Tips", "Conseils d'organisation du code"), 
                    new LocalizedText("Best practices for clean code", "Meilleures pratiques pour un code propre"), 
                    new LocalizedText("# Code Organization Tips\n\n```csharp\n// Use meaningful names\npublic class FragmentController : ControllerBase\n{\n    private readonly ILogger<FragmentController> _logger;\n    private readonly AppDbContext _context;\n    \n    // Constructor injection\n    public FragmentController(ILogger<FragmentController> logger, AppDbContext context)\n    {\n        _logger = logger;\n        _context = context;\n    }\n}\n```\n\n**Key principles:**\n- Single responsibility\n- Dependency injection\n- Clear naming conventions",
                        "# Conseils d'organisation du code\n\n```csharp\n// Utilisez des noms significatifs\npublic class FragmentController : ControllerBase\n{\n    private readonly ILogger<FragmentController> _logger;\n    private readonly AppDbContext _context;\n    \n    // Injection de dépendances\n    public FragmentController(ILogger<FragmentController> logger, AppDbContext context)\n    {\n        _logger = logger;\n        _context = context;\n    }\n}\n```\n\n**Principes clés :**\n- Responsabilité unique\n- Injection de dépendances\n- Conventions de nommage claires"), 
                    "https://picsum.photos/400/300?random=2")
                {
                    TypeTag = "code",
                    AttachmentType = "image",
                    VibeCount = 8,
                    IsPublic = true,
                    Tags = new List<string> { "code", "csharp", "best-practices", "clean-code" },
                    Meta = new Dictionary<string, string> { { "language", "csharp" }, { "difficulty", "intermediate" } }
                },

                new Fragment(
                    new LocalizedText("Daily Inspiration", "Inspiration quotidienne"), 
                    new LocalizedText("Motivation for developers", "Motivation pour les développeurs"), 
                    new LocalizedText("> \"The only way to do great work is to love what you do.\" - Steve Jobs\n\nThis quote reminds us that passion is the key to excellence in software development. When we love coding, debugging becomes problem-solving, and challenges become opportunities to learn and grow.",
                        "> \"La seule façon de faire du bon travail est d'aimer ce que vous faites.\" - Steve Jobs\n\nCette citation nous rappelle que la passion est la clé de l'excellence en développement logiciel. Quand nous aimons coder, le débogage devient de la résolution de problèmes, et les défis deviennent des opportunités d'apprendre et de grandir."), 
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
                new Tag("Technology", "primary"),
                new Tag("Programming", "info"),
                new Tag("Web Development", "success"),
                new Tag("C#", "warning"),
                new Tag("ASP.NET", "danger"),
                new Tag("Blazor", "info"),
                new Tag("Tutorial", "secondary"),
                new Tag("Best Practices", "success"),
                new Tag("Architecture", "primary"),
                new Tag("Performance", "warning"),
                new Tag("Security", "danger"),
                new Tag("Design Patterns", "dark")
            };

            await _context.Tags.AddRangeAsync(sampleTags);
            await _context.SaveChangesAsync();

            var sampleAuthors = new List<Author>
            {
                new Author("John Doe", 
                    new LocalizedText("Senior Software Engineer with 10+ years of experience in web development and system architecture.", 
                                     "Ingénieur logiciel senior avec plus de 10 ans d'expérience en développement web et architecture système."), 
                    "https://picsum.photos/150/150?random=101",
                    "john.doe@example.com", "https://johndoe.dev", "@johndoe", "https://linkedin.com/in/johndoe", "johndoe"),
                new Author("Jane Smith", 
                    new LocalizedText("Full-stack developer passionate about clean code and modern web technologies.", 
                                     "Développeuse full-stack passionnée par le code propre et les technologies web modernes."), 
                    "https://picsum.photos/150/150?random=102",
                    "jane.smith@example.com", "https://janesmith.dev", "@janesmith", "https://linkedin.com/in/janesmith", "janesmith"),
                new Author("Mike Johnson", 
                    new LocalizedText("DevOps engineer and cloud architecture specialist.", 
                                     "Ingénieur DevOps et spécialiste en architecture cloud."), 
                    "https://picsum.photos/150/150?random=103",
                    "mike.johnson@example.com", "https://mikej.dev", "@mikejohnson", "https://linkedin.com/in/mikejohnson", "mikejohnson"),
                new Author("Sarah Wilson", 
                    new LocalizedText("Frontend developer and UI/UX designer.", 
                                     "Développeuse frontend et conceptrice UI/UX."), 
                    "https://picsum.photos/150/150?random=104",
                    "sarah.wilson@example.com", "https://sarahwilson.design", "@sarahwilson", "https://linkedin.com/in/sarahwilson", "sarahwilson")
            };

            await _context.Authors.AddRangeAsync(sampleAuthors);
            await _context.SaveChangesAsync();

            var sampleBlogs = new List<Blog>
            {
                new Blog(
                    new LocalizedText("Getting Started with ASP.NET Core", "Débuter avec ASP.NET Core"),
                    new LocalizedText("A comprehensive guide to building modern web applications with ASP.NET Core.", 
                                     "Un guide complet pour créer des applications web modernes avec ASP.NET Core."),
                    new LocalizedText("# Getting Started with ASP.NET Core\n\nASP.NET Core is a cross-platform, high-performance framework for building modern web applications. In this guide, we'll explore the fundamentals of ASP.NET Core and how to get started with your first application.\n\n## Key Features\n\n- **Cross-platform**: Runs on Windows, macOS, and Linux\n- **High performance**: Built for speed and scalability\n- **Modular**: Use only the components you need\n- **Dependency injection**: Built-in support for DI\n- **Configuration**: Flexible configuration system\n\n## Creating Your First Application\n\n```bash\ndotnet new web -n MyFirstApp\ncd MyFirstApp\ndotnet run\n```\n\nThis creates a minimal web application that you can run immediately.\n\n## Next Steps\n\n1. Explore the project structure\n2. Add controllers and views\n3. Implement dependency injection\n4. Configure middleware\n5. Add authentication and authorization\n\nASP.NET Core provides a solid foundation for building robust web applications. Start with the basics and gradually add more features as your application grows.",
                                     "# Débuter avec ASP.NET Core\n\nASP.NET Core est un framework multiplateforme et haute performance pour créer des applications web modernes. Dans ce guide, nous explorerons les fondamentaux d'ASP.NET Core et comment commencer avec votre première application.\n\n## Fonctionnalités clés\n\n- **Multiplateforme** : Fonctionne sur Windows, macOS et Linux\n- **Haute performance** : Conçu pour la vitesse et l'évolutivité\n- **Modulaire** : Utilisez seulement les composants dont vous avez besoin\n- **Injection de dépendances** : Support intégré pour l'ID\n- **Configuration** : Système de configuration flexible\n\n## Créer votre première application\n\n```bash\ndotnet new web -n MyFirstApp\ncd MyFirstApp\ndotnet run\n```\n\nCela crée une application web minimale que vous pouvez exécuter immédiatement.\n\n## Prochaines étapes\n\n1. Explorer la structure du projet\n2. Ajouter des contrôleurs et des vues\n3. Implémenter l'injection de dépendances\n4. Configurer le middleware\n5. Ajouter l'authentification et l'autorisation\n\nASP.NET Core fournit une base solide pour créer des applications web robustes. Commencez par les bases et ajoutez progressivement plus de fonctionnalités au fur et à mesure que votre application grandit."),
                    sampleAuthors[0],
                    new List<Tag> { sampleTags[0], sampleTags[1], sampleTags[4], sampleTags[6] },
                    "https://picsum.photos/800/400?random=201",
                    new List<string> { "https://docs.microsoft.com/aspnet/core", "https://github.com/dotnet/aspnetcore" },
                    ContentType.Markdown,
                    BlogState.Published,
                    "getting-started-with-aspnet-core")
                {
                    PublishedAt = DateTime.UtcNow.AddDays(-30),
                    UpdatedAt = DateTime.UtcNow.AddDays(-30),
                    Views = 1250,
                    Featured = true,
                    ReadingTimeMinutes = 8
                },

                new Blog(
                    new LocalizedText("Building Interactive UIs with Blazor", "Créer des interfaces utilisateur interactives avec Blazor"),
                    new LocalizedText("Learn how to create rich, interactive web applications using Blazor and C#.", 
                                     "Apprenez à créer des applications web riches et interactives avec Blazor et C#."),
                    new LocalizedText("# Building Interactive UIs with Blazor\n\nBlazor is a revolutionary framework that allows you to build interactive web UIs using C# instead of JavaScript. This opens up new possibilities for .NET developers to create full-stack applications.\n\n## What is Blazor?\n\nBlazor is a framework for building interactive web UIs using C# and .NET. It can run on the server (Blazor Server) or in the browser via WebAssembly (Blazor WebAssembly).\n\n## Key Benefits\n\n- **Unified Development**: Use C# for both frontend and backend\n- **Component-based**: Reusable UI components\n- **Rich Ecosystem**: Leverage existing .NET libraries\n- **Strong Typing**: Compile-time error checking\n- **IntelliSense**: Full IDE support\n\n## Creating Components\n\n```razor\n@page \"/counter\"\n\n<h3>Counter</h3>\n\n<p>Current count: @currentCount</p>\n\n<button class=\"btn btn-primary\" @onclick=\"IncrementCount\">Click me</button>\n\n@code {\n    private int currentCount = 0;\n\n    private void IncrementCount()\n    {\n        currentCount++;\n    }\n}\n```\n\n## Advanced Features\n\n- **Data Binding**: Two-way data binding with forms\n- **JavaScript Interop**: Call JavaScript functions from C#\n- **Routing**: Client-side routing with parameters\n- **State Management**: Share state across components\n- **Authentication**: Integrate with Identity providers\n\nBlazor represents the future of web development for .NET developers, combining the power of C# with modern web technologies.",
                                     "# Créer des interfaces utilisateur interactives avec Blazor\n\nBlazor est un framework révolutionnaire qui vous permet de créer des interfaces utilisateur web interactives en utilisant C# au lieu de JavaScript. Cela ouvre de nouvelles possibilités pour les développeurs .NET de créer des applications full-stack.\n\n## Qu'est-ce que Blazor ?\n\nBlazor est un framework pour créer des interfaces utilisateur web interactives en utilisant C# et .NET. Il peut fonctionner sur le serveur (Blazor Server) ou dans le navigateur via WebAssembly (Blazor WebAssembly).\n\n## Avantages clés\n\n- **Développement unifié** : Utilisez C# pour le frontend et le backend\n- **Basé sur des composants** : Composants d'interface utilisateur réutilisables\n- **Écosystème riche** : Tirez parti des bibliothèques .NET existantes\n- **Typage fort** : Vérification d'erreurs à la compilation\n- **IntelliSense** : Support complet de l'IDE\n\n## Créer des composants\n\n```razor\n@page \"/counter\"\n\n<h3>Compteur</h3>\n\n<p>Compte actuel : @currentCount</p>\n\n<button class=\"btn btn-primary\" @onclick=\"IncrementCount\">Cliquez-moi</button>\n\n@code {\n    private int currentCount = 0;\n\n    private void IncrementCount()\n    {\n        currentCount++;\n    }\n}\n```\n\n## Fonctionnalités avancées\n\n- **Liaison de données** : Liaison de données bidirectionnelle avec les formulaires\n- **Interopérabilité JavaScript** : Appelez des fonctions JavaScript depuis C#\n- **Routage** : Routage côté client avec paramètres\n- **Gestion d'état** : Partagez l'état entre les composants\n- **Authentification** : Intégrez avec les fournisseurs d'identité\n\nBlazor représente l'avenir du développement web pour les développeurs .NET, combinant la puissance de C# avec les technologies web modernes."),
                    sampleAuthors[1],
                    new List<Tag> { sampleTags[0], sampleTags[2], sampleTags[5], sampleTags[6] },
                    "https://picsum.photos/800/400?random=202",
                    new List<string> { "https://blazor.net", "https://docs.microsoft.com/aspnet/core/blazor" },
                    ContentType.Markdown,
                    BlogState.Published,
                    "building-interactive-uis-with-blazor")
                {
                    PublishedAt = DateTime.UtcNow.AddDays(-25),
                    UpdatedAt = DateTime.UtcNow.AddDays(-25),
                    Views = 890,
                    Featured = false,
                    ReadingTimeMinutes = 12
                },

                new Blog(
                    "Microservices Architecture with .NET",
                    "Design and implement scalable microservices using .NET and modern cloud patterns.",
                    "# Microservices Architecture with .NET\n\nMicroservices architecture has become a popular approach for building scalable, maintainable applications. In this article, we'll explore how to implement microservices using .NET and modern cloud patterns.\n\n## What are Microservices?\n\nMicroservices are a architectural pattern where applications are built as a collection of small, independent services that communicate over well-defined APIs.\n\n## Benefits of Microservices\n\n- **Scalability**: Scale individual services independently\n- **Technology Diversity**: Use different technologies for different services\n- **Fault Isolation**: Failures in one service don't affect others\n- **Team Autonomy**: Teams can work independently on services\n- **Deployment Flexibility**: Deploy services independently\n\n## Key Patterns\n\n### API Gateway\n\n```csharp\nservices.AddReverseProxy()\n    .LoadFromConfig(Configuration.GetSection(\"ReverseProxy\"));\n```\n\n### Service Discovery\n\n```csharp\nservices.AddConsul(options =>\n{\n    options.Host = \"localhost\";\n    options.Port = 8500;\n});\n```\n\n### Circuit Breaker\n\n```csharp\nservices.AddHttpClient<IOrderService, OrderService>()\n    .AddPolicyHandler(GetRetryPolicy())\n    .AddPolicyHandler(GetCircuitBreakerPolicy());\n```\n\n## Challenges and Solutions\n\n- **Data Consistency**: Use saga pattern for distributed transactions\n- **Service Communication**: Implement async messaging with RabbitMQ or Azure Service Bus\n- **Monitoring**: Use distributed tracing with OpenTelemetry\n- **Security**: Implement OAuth 2.0 and JWT tokens\n\n## Best Practices\n\n1. **Start with a monolith** and decompose gradually\n2. **Design for failure** with circuit breakers and retries\n3. **Implement proper logging** and monitoring\n4. **Use containerization** with Docker and Kubernetes\n5. **Automate testing** and deployment pipelines\n\nMicroservices can provide significant benefits, but they also introduce complexity. Careful planning and the right tools are essential for success.",
                    sampleAuthors[2],
                    new List<Tag> { sampleTags[0], sampleTags[8], sampleTags[9], sampleTags[11] },
                    "https://picsum.photos/800/400?random=203",
                    new List<string> { "https://microservices.io", "https://docs.microsoft.com/dotnet/architecture/microservices" },
                    ContentType.Markdown,
                    BlogState.Published,
                    "microservices-architecture-with-dotnet")
                {
                    PublishedAt = DateTime.UtcNow.AddDays(-20),
                    UpdatedAt = DateTime.UtcNow.AddDays(-15),
                    Views = 2150,
                    Featured = true,
                    ReadingTimeMinutes = 15
                },

                new Blog(
                    "Secure Coding Practices in C#",
                    "Essential security practices every C# developer should follow to build secure applications.",
                    "# Secure Coding Practices in C#\n\nSecurity should be a primary concern in every software development project. This guide covers essential security practices that every C# developer should follow to build secure applications.\n\n## Input Validation\n\nAlways validate and sanitize user input:\n\n```csharp\npublic IActionResult CreateUser(UserModel model)\n{\n    if (!ModelState.IsValid)\n    {\n        return BadRequest(ModelState);\n    }\n    \n    // Additional validation\n    if (string.IsNullOrWhiteSpace(model.Email) || !IsValidEmail(model.Email))\n    {\n        return BadRequest(\"Invalid email address\");\n    }\n    \n    // Process the valid input\n    return Ok();\n}\n```\n\n## SQL Injection Prevention\n\nUse parameterized queries:\n\n```csharp\n// Good - Parameterized query\nvar user = context.Users\n    .FirstOrDefault(u => u.Email == email && u.Password == hashedPassword);\n\n// Bad - String concatenation\nvar query = $\"SELECT * FROM Users WHERE Email = '{email}' AND Password = '{password}'\";\n```\n\n## Authentication and Authorization\n\n```csharp\n[Authorize(Roles = \"Admin\")]\npublic IActionResult AdminOnly()\n{\n    // Only admin users can access this\n    return View();\n}\n\n[Authorize(Policy = \"RequireUserRole\")]\npublic IActionResult UserAction()\n{\n    // Policy-based authorization\n    return View();\n}\n```\n\n## Data Protection\n\n```csharp\n// Encrypt sensitive data\nvar dataProtector = serviceProvider.GetService<IDataProtector>();\nvar encryptedData = dataProtector.Protect(sensitiveData);\n\n// Hash passwords\nvar hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);\nvar isValid = BCrypt.Net.BCrypt.Verify(password, hashedPassword);\n```\n\n## HTTPS and Security Headers\n\n```csharp\nservices.AddHsts(options =>\n{\n    options.Preload = true;\n    options.IncludeSubDomains = true;\n    options.MaxAge = TimeSpan.FromDays(365);\n});\n\napp.UseHsts();\napp.UseHttpsRedirection();\n```\n\n## Error Handling\n\n```csharp\napp.UseExceptionHandler(\"/Error\");\napp.UseStatusCodePagesWithReExecute(\"/Error/{0}\");\n\n// Don't expose sensitive information in error messages\ntry\n{\n    // risky operation\n}\ncatch (Exception ex)\n{\n    logger.LogError(ex, \"Operation failed\");\n    return StatusCode(500, \"Internal server error\");\n}\n```\n\n## Security Checklist\n\n- ✅ Validate all input\n- ✅ Use parameterized queries\n- ✅ Implement proper authentication\n- ✅ Use HTTPS everywhere\n- ✅ Hash passwords properly\n- ✅ Implement rate limiting\n- ✅ Keep dependencies updated\n- ✅ Use security headers\n- ✅ Log security events\n- ✅ Regular security audits\n\nSecurity is an ongoing process, not a one-time task. Stay informed about new threats and update your practices accordingly.",
                    sampleAuthors[0],
                    new List<Tag> { sampleTags[0], sampleTags[3], sampleTags[10], sampleTags[7] },
                    "https://picsum.photos/800/400?random=204",
                    new List<string> { "https://owasp.org", "https://docs.microsoft.com/dotnet/standard/security" },
                    ContentType.Markdown,
                    BlogState.Published,
                    "secure-coding-practices-in-csharp")
                {
                    PublishedAt = DateTime.UtcNow.AddDays(-12),
                    UpdatedAt = DateTime.UtcNow.AddDays(-12),
                    Views = 1800,
                    Featured = true,
                    ReadingTimeMinutes = 18
                },

                // Add more simplified blog examples with the new properties
                new Blog(
                    "Modern Frontend Development with Blazor and Bootstrap",
                    "Create responsive, modern web interfaces using Blazor components and Bootstrap CSS framework.",
                    "# Modern Frontend Development with Blazor and Bootstrap\n\nCombining Blazor with Bootstrap creates a powerful foundation for building modern, responsive web applications. This guide shows you how to leverage both technologies effectively...",
                    sampleAuthors[3],
                    new List<Tag> { sampleTags[2], sampleTags[5], sampleTags[6], sampleTags[7] },
                    "https://picsum.photos/800/400?random=205",
                    new List<string> { "https://getbootstrap.com", "https://blazor.net" },
                    ContentType.Markdown,
                    BlogState.Published,
                    "modern-frontend-development-blazor-bootstrap")
                {
                    PublishedAt = DateTime.UtcNow.AddDays(-8),
                    UpdatedAt = DateTime.UtcNow.AddDays(-8),
                    Views = 670,
                    Featured = false,
                    ReadingTimeMinutes = 14
                },

                new Blog(
                    "Understanding Entity Framework Core Performance",
                    "Tips and tricks to optimize your EF Core queries for better performance and scalability.",
                    "# Understanding Entity Framework Core Performance\n\nEntity Framework Core is a powerful ORM, but poor performance can be a significant issue if not handled properly. This guide covers essential performance optimization techniques...",
                    sampleAuthors[2],
                    new List<Tag> { sampleTags[0], sampleTags[3], sampleTags[9], sampleTags[11] },
                    "https://picsum.photos/800/400?random=206",
                    new List<string> { "https://docs.microsoft.com/ef/core/performance", "https://www.entityframeworktutorial.net/efcore/entity-framework-core.aspx" },
                    ContentType.Markdown,
                    BlogState.Published,
                    "understanding-entity-framework-core-performance")
                {
                    PublishedAt = DateTime.UtcNow.AddDays(-5),
                    UpdatedAt = DateTime.UtcNow.AddDays(-5),
                    Views = 1320,
                    Featured = false,
                    ReadingTimeMinutes = 16
                },

                new Blog(
                    "Implementing Clean Architecture in .NET",
                    "Learn how to structure your .NET applications using Clean Architecture principles for maintainability and testability.",
                    "# Implementing Clean Architecture in .NET\n\nClean Architecture, popularized by Robert C. Martin (Uncle Bob), provides a way to structure applications that are independent of frameworks, databases, and external concerns...",
                    sampleAuthors[0],
                    new List<Tag> { sampleTags[0], sampleTags[8], sampleTags[11], sampleTags[7] },
                    "https://picsum.photos/800/400?random=207",
                    new List<string> { "https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html", "https://docs.microsoft.com/dotnet/architecture/modern-web-apps-azure" },
                    ContentType.Markdown,
                    BlogState.Draft,
                    "implementing-clean-architecture-in-dotnet")
                {
                    Views = 0,
                    Featured = false,
                    ReadingTimeMinutes = 22
                },

                new Blog(
                    "Advanced C# Features You Should Know",
                    "Explore powerful C# features that can make your code more efficient, readable, and maintainable.",
                    "# Advanced C# Features You Should Know\n\nC# continues to evolve with each release, introducing powerful features that can significantly improve your code quality and developer productivity...",
                    sampleAuthors[1],
                    new List<Tag> { sampleTags[3], sampleTags[1], sampleTags[7], sampleTags[9] },
                    "https://picsum.photos/800/400?random=209",
                    new List<string> { "https://docs.microsoft.com/dotnet/csharp/whats-new", "https://github.com/dotnet/csharplang" },
                    ContentType.Markdown,
                    BlogState.Published,
                    "advanced-csharp-features-you-should-know")
                {
                    PublishedAt = DateTime.UtcNow.AddDays(-3),
                    UpdatedAt = DateTime.UtcNow.AddDays(-2),
                    Views = 950,
                    Featured = true,
                    ReadingTimeMinutes = 20
                }
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