using DatabaseManager;
using FragmentLibrary;
using Microsoft.EntityFrameworkCore;

namespace FragmentAPI.Services
{
    public class FragmentSeedService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<FragmentSeedService> _logger;

        public FragmentSeedService(AppDbContext context, ILogger<FragmentSeedService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task SeedFragmentTypesAsync()
        {
            try
            {
                // Check if fragment types already exist
                var existingTypes = await _context.FragmentTypes.CountAsync();
                if (existingTypes > 0)
                {
                    _logger.LogInformation("Fragment types already exist in database, skipping seed");
                    return;
                }

                // Add default fragment types
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
                // Check if fragments already exist
                var existingFragments = await _context.Fragments.CountAsync();
                if (existingFragments > 0)
                {
                    _logger.LogInformation("Fragments already exist in database, skipping seed");
                    return;
                }

                // Create sample fragments
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
    }
}