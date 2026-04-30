namespace CacheLibrary;

public static class CacheNamespaces
{
    public const string Main = "main";
    public const string Website = "website";
    public const string Backoffice = "backoffice";
}

public static class CacheProducers
{
    public const string BlogApi = "blog-api";
    public const string ActivityApi = "activity-api";
    public const string FragmentApi = "fragment-api";
    public const string TechApi = "tech-api";
    public const string Gateway = "gateway";
    public const string WebsiteClient = "website-client";
    public const string CacheAdmin = "cache-admin";
}

public static class CacheKeys
{
    public const string SolutionPrefix = "mn";
}

public static class CacheTags
{
    public static string For(string ns, string producer, string resource) =>
        $"{ns}:{producer}:{resource}";

    public static class BlogApi
    {
        public static readonly string List = For(CacheNamespaces.Main, CacheProducers.BlogApi, "blog-list");
        public static readonly string Count = For(CacheNamespaces.Main, CacheProducers.BlogApi, "blog-count");
        public static readonly string DetailAll = For(CacheNamespaces.Main, CacheProducers.BlogApi, "blog-detail");
        public static string Detail(Guid id) => $"{DetailAll}:{id}";
    }

    public static class ActivityApi
    {
        public static readonly string Recent = For(CacheNamespaces.Main, CacheProducers.ActivityApi, "recent");
        public static readonly string Pinned = For(CacheNamespaces.Main, CacheProducers.ActivityApi, "pinned");
        public static readonly string Random = For(CacheNamespaces.Main, CacheProducers.ActivityApi, "random");
    }

    public static class FragmentApi
    {
        public static readonly string List = For(CacheNamespaces.Main, CacheProducers.FragmentApi, "fragment-list");
        public static readonly string Search = For(CacheNamespaces.Main, CacheProducers.FragmentApi, "fragment-search");
        public static readonly string Types = For(CacheNamespaces.Main, CacheProducers.FragmentApi, "fragment-types");
        public static readonly string ByType = For(CacheNamespaces.Main, CacheProducers.FragmentApi, "fragment-by-type");
        public static readonly string DetailAll = For(CacheNamespaces.Main, CacheProducers.FragmentApi, "fragment-detail");
        public static string Detail(Guid id) => $"{DetailAll}:{id}";
    }

    public static class TechApi
    {
        public static readonly string Updates = For(CacheNamespaces.Main, CacheProducers.TechApi, "tech-updates");
        public static readonly string UpdatesCount = For(CacheNamespaces.Main, CacheProducers.TechApi, "tech-updates-count");
        public static readonly string UpdateDetailAll = For(CacheNamespaces.Main, CacheProducers.TechApi, "tech-update-detail");
        public static string UpdateDetail(Guid id) => $"{UpdateDetailAll}:{id}";
        public static readonly string ProjectActivity = For(CacheNamespaces.Main, CacheProducers.TechApi, "tech-project-activity");
    }
}
