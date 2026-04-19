using SharedLibrary;
using TechLibrary;

namespace Website.Features.Tech.Pages;

public partial class TechIndexPage
{
    private List<ProjectInfo> GetAllProjects() =>
    [
        new ProjectInfo
        {
            Key = ProjectKeys.FashionStore,
            Title = "Fashion Store",
            Description = "Modern e-commerce platform for fashion retail with inventory management and customer analytics.",
            ImageUrl = "https://placehold.co/400x200/e3f2fd/1976d2?text=Fashion+Store",
            Tags = new[] { "E-commerce", "Fashion", "Retail" }
        },
        new ProjectInfo
        {
            Key = ProjectKeys.TechHardwareShop,
            Title = "Tech Hardware Shop",
            Description = "Comprehensive hardware marketplace with product comparisons and technical specifications.",
            ImageUrl = "https://placehold.co/400x200/f3e5f5/7b1fa2?text=Hardware+Shop",
            Tags = new[] { "Hardware", "E-commerce", "Technology" }
        },
        new ProjectInfo
        {
            Key = ProjectKeys.GamingPortal,
            Title = "Gaming Portal",
            Description = "Community-driven gaming platform with reviews, tournaments, and social features.",
            ImageUrl = "https://placehold.co/400x200/e8f5e8/388e3c?text=Gaming+Portal",
            Tags = new[] { "Gaming", "Community", "Social" }
        },
        new ProjectInfo
        {
            Key = ProjectKeys.DigitalMarketplace,
            Title = "Digital Marketplace",
            Description = "Multi-vendor platform for digital products with advanced seller tools and analytics.",
            ImageUrl = "https://placehold.co/400x200/fff3e0/f57c00?text=Digital+Marketplace",
            Tags = new[] { "Marketplace", "Digital", "Multi-vendor" }
        },
        new ProjectInfo
        {
            Key = ProjectKeys.StreamingPlatform,
            Title = "Streaming Platform",
            Description = "Video streaming service with live broadcasting capabilities and subscription management.",
            ImageUrl = "https://placehold.co/400x200/fce4ec/c2185b?text=Streaming+Platform",
            Tags = new[] { "Streaming", "Video", "Live" }
        }
    ];
}
