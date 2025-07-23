namespace SharedLibrary
{
    /// <summary>
    /// Enum defining project identifiers for tech project pages
    /// </summary>
    public enum ProjectKeys
    {
        // Test projects for implementation
        FashionStore,
        TechHardwareShop,
        GamingPortal,
        DigitalMarketplace,
        StreamingPlatform
    }

    /// <summary>
    /// Extension methods for ProjectKeys enum
    /// </summary>
    public static class ProjectKeysExtensions
    {
        /// <summary>
        /// Converts ProjectKeys enum to URL-friendly slug string
        /// </summary>
        public static string ToSlug(this ProjectKeys project)
        {
            return project switch
            {
                ProjectKeys.FashionStore => "fashion-store",
                ProjectKeys.TechHardwareShop => "tech-hardware-shop",
                ProjectKeys.GamingPortal => "gaming-portal",
                ProjectKeys.DigitalMarketplace => "digital-marketplace",
                ProjectKeys.StreamingPlatform => "streaming-platform",
                _ => project.ToString().ToLowerInvariant()
            };
        }

        /// <summary>
        /// Converts slug string back to ProjectKeys enum
        /// </summary>
        public static ProjectKeys? FromSlug(string slug)
        {
            return slug switch
            {
                "fashion-store" => ProjectKeys.FashionStore,
                "tech-hardware-shop" => ProjectKeys.TechHardwareShop,
                "gaming-portal" => ProjectKeys.GamingPortal,
                "digital-marketplace" => ProjectKeys.DigitalMarketplace,
                "streaming-platform" => ProjectKeys.StreamingPlatform,
                _ => null
            };
        }

        /// <summary>
        /// Gets display name for the project
        /// </summary>
        public static string GetDisplayName(this ProjectKeys project)
        {
            return project switch
            {
                ProjectKeys.FashionStore => "Fashion Store",
                ProjectKeys.TechHardwareShop => "Tech Hardware Shop", 
                ProjectKeys.GamingPortal => "Gaming Portal",
                ProjectKeys.DigitalMarketplace => "Digital Marketplace",
                ProjectKeys.StreamingPlatform => "Streaming Platform",
                _ => project.ToString()
            };
        }
    }
}