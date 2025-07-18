using FragmentLibrary;
using SharedLibrary;

namespace DatabaseSeeder.SeedData;

public static class FragmentSeedData
{
    public static List<Fragment> GetSampleFragments()
    {
        return new List<Fragment>
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

            new Fragment(
                new LocalizedText("Creative Block Solution", "Solution aux blocages créatifs"), 
                new LocalizedText("Ideas for overcoming creative blocks", "Idées pour surmonter les blocages créatifs"), 
                new LocalizedText("# Breaking Through Creative Blocks\n\n💡 **When you're stuck, try:**\n\n1. **Change your environment** - Work from a different location\n2. **Take a walk** - Let your mind wander\n3. **Talk to someone** - Explain your problem out loud\n4. **Sleep on it** - Your subconscious works while you rest\n5. **Start with something small** - Momentum builds momentum\n\nSometimes the best ideas come when we're not actively trying to have them!",
                    "# Surmonter les blocages créatifs\n\n💡 **Quand vous êtes bloqué, essayez :**\n\n1. **Changer d'environnement** - Travailler depuis un autre endroit\n2. **Faire une promenade** - Laissez votre esprit vagabonder\n3. **Parler à quelqu'un** - Expliquez votre problème à voix haute\n4. **Dormir dessus** - Votre subconscient travaille pendant que vous vous reposez\n5. **Commencer par quelque chose de petit** - L'élan crée l'élan\n\nParfois les meilleures idées viennent quand on n'essaie pas activement de les avoir !"), 
                "https://picsum.photos/400/300?random=4")
            {
                TypeTag = "idea",
                AttachmentType = "image",
                VibeCount = 5,
                IsPublic = true,
                Tags = new List<string> { "creativity", "productivity", "inspiration", "tips" },
                Meta = new Dictionary<string, string> { { "category", "productivity" }, { "usefulness", "high" } }
            },

            new Fragment(
                new LocalizedText("Dream Project", "Projet de rêve"), 
                new LocalizedText("A vision for the future", "Une vision pour l'avenir"), 
                new LocalizedText("# The Perfect Development Environment\n\n🌟 **Imagine a world where:**\n\n- Code writes itself based on natural language descriptions\n- Bugs are caught before they're written\n- Documentation updates automatically\n- Tests generate themselves from specifications\n- Deployment is as easy as saving a file\n\nWhile we're not there yet, each new tool and framework brings us closer to this dream. The future of development is exciting!",
                    "# L'environnement de développement parfait\n\n🌟 **Imaginez un monde où :**\n\n- Le code s'écrit lui-même basé sur des descriptions en langage naturel\n- Les bugs sont attrapés avant d'être écrits\n- La documentation se met à jour automatiquement\n- Les tests se génèrent eux-mêmes à partir des spécifications\n- Le déploiement est aussi facile que sauvegarder un fichier\n\nBien que nous n'y soyons pas encore, chaque nouvel outil et framework nous rapproche de ce rêve. L'avenir du développement est passionnant !"), 
                "https://picsum.photos/400/300?random=5")
            {
                TypeTag = "dream",
                AttachmentType = "image",
                VibeCount = 15,
                IsPublic = true,
                Tags = new List<string> { "future", "ai", "automation", "development", "vision" },
                Meta = new Dictionary<string, string> { { "timeframe", "future" }, { "feasibility", "possible" } }
            },

            new Fragment(
                new LocalizedText("Regex Cheat Sheet", "Aide-mémoire Regex"), 
                new LocalizedText("Common regex patterns for developers", "Motifs regex courants pour les développeurs"), 
                new LocalizedText("# Regex Cheat Sheet\n\n```regex\n// Email validation\n^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,}$\n\n// Phone number (US format)\n^\\+?1?[-.\\s]?\\(?[0-9]{3}\\)?[-.\\s]?[0-9]{3}[-.\\s]?[0-9]{4}$\n\n// URL validation\n^https?:\\/\\/(www\\.)?[a-zA-Z0-9@:%._\\+~#=]{1,256}\\.[a-zA-Z0-9()]{1,6}\\b([-a-zA-Z0-9()@:%_\\+.~#?&//=]*)$\n\n// Password strength (8+ chars, uppercase, lowercase, number)\n^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)[a-zA-Z\\d@$!%*?&]{8,}$\n```\n\n**Common metacharacters:**\n- `.` - Any character\n- `*` - Zero or more\n- `+` - One or more\n- `?` - Zero or one\n- `^` - Start of string\n- `$` - End of string",
                    "# Aide-mémoire Regex\n\n```regex\n// Validation d'email\n^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,}$\n\n// Numéro de téléphone (format français)\n^(?:(?:\\+|00)33|0)\\s*[1-9](?:[\\s.-]*\\d{2}){4}$\n\n// Validation d'URL\n^https?:\\/\\/(www\\.)?[a-zA-Z0-9@:%._\\+~#=]{1,256}\\.[a-zA-Z0-9()]{1,6}\\b([-a-zA-Z0-9()@:%_\\+.~#?&//=]*)$\n\n// Force du mot de passe (8+ chars, majuscule, minuscule, nombre)\n^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)[a-zA-Z\\d@$!%*?&]{8,}$\n```\n\n**Métacaractères courants :**\n- `.` - N'importe quel caractère\n- `*` - Zéro ou plus\n- `+` - Un ou plus\n- `?` - Zéro ou un\n- `^` - Début de chaîne\n- `$` - Fin de chaîne"), 
                "https://picsum.photos/400/300?random=6")
            {
                TypeTag = "code",
                AttachmentType = "image",
                VibeCount = 7,
                IsPublic = true,
                Tags = new List<string> { "regex", "reference", "patterns", "validation" },
                Meta = new Dictionary<string, string> { { "category", "reference" }, { "difficulty", "intermediate" } }
            },

            new Fragment(
                new LocalizedText("Git Command Shortcuts", "Raccourcis de commandes Git"), 
                new LocalizedText("Essential Git commands for daily use", "Commandes Git essentielles pour l'usage quotidien"), 
                new LocalizedText("# Git Command Shortcuts\n\n```bash\n# Quick status and add\ngit status -s\ngit add .\ngit commit -m \"message\"\n\n# Create and switch to new branch\ngit checkout -b feature/new-feature\n\n# Push new branch to remote\ngit push -u origin feature/new-feature\n\n# Quick log view\ngit log --oneline --graph --all\n\n# Undo last commit (keep changes)\ngit reset --soft HEAD~1\n\n# Stash changes quickly\ngit stash\ngit stash pop\n\n# Clean up merged branches\ngit branch --merged | grep -v main | xargs -n 1 git branch -d\n```\n\n**Pro tips:**\n- Use aliases for common commands\n- Configure global gitignore\n- Use conventional commit messages",
                    "# Raccourcis de commandes Git\n\n```bash\n# Statut rapide et ajout\ngit status -s\ngit add .\ngit commit -m \"message\"\n\n# Créer et basculer vers une nouvelle branche\ngit checkout -b feature/nouvelle-fonctionnalite\n\n# Pousser une nouvelle branche vers le distant\ngit push -u origin feature/nouvelle-fonctionnalite\n\n# Vue rapide du log\ngit log --oneline --graph --all\n\n# Annuler le dernier commit (garder les changements)\ngit reset --soft HEAD~1\n\n# Mettre en réserve rapidement\ngit stash\ngit stash pop\n\n# Nettoyer les branches mergées\ngit branch --merged | grep -v main | xargs -n 1 git branch -d\n```\n\n**Conseils pro :**\n- Utilisez des alias pour les commandes communes\n- Configurez un gitignore global\n- Utilisez des messages de commit conventionnels"), 
                "https://picsum.photos/400/300?random=7")
            {
                TypeTag = "code",
                AttachmentType = "image",
                VibeCount = 9,
                IsPublic = true,
                Tags = new List<string> { "git", "commands", "shortcuts", "version-control" },
                Meta = new Dictionary<string, string> { { "category", "reference" }, { "usefulness", "high" } }
            },

            new Fragment(
                new LocalizedText("CSS Grid Layout", "Mise en page CSS Grid"), 
                new LocalizedText("Modern CSS grid techniques", "Techniques modernes de grille CSS"), 
                new LocalizedText("# CSS Grid Layout\n\n```css\n.container {\n    display: grid;\n    grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));\n    gap: 1rem;\n    padding: 1rem;\n}\n\n.item {\n    background: #f0f0f0;\n    padding: 1rem;\n    border-radius: 8px;\n}\n\n/* Responsive grid */\n@media (max-width: 768px) {\n    .container {\n        grid-template-columns: 1fr;\n    }\n}\n\n/* Named grid areas */\n.layout {\n    display: grid;\n    grid-template-areas:\n        \"header header\"\n        \"sidebar main\"\n        \"footer footer\";\n    grid-template-columns: 200px 1fr;\n    min-height: 100vh;\n}\n\n.header { grid-area: header; }\n.sidebar { grid-area: sidebar; }\n.main { grid-area: main; }\n.footer { grid-area: footer; }\n```\n\n**Grid is perfect for:**\n- Card layouts\n- Dashboard designs\n- Complex page layouts\n- Responsive design",
                    "# Mise en page CSS Grid\n\n```css\n.container {\n    display: grid;\n    grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));\n    gap: 1rem;\n    padding: 1rem;\n}\n\n.item {\n    background: #f0f0f0;\n    padding: 1rem;\n    border-radius: 8px;\n}\n\n/* Grille responsive */\n@media (max-width: 768px) {\n    .container {\n        grid-template-columns: 1fr;\n    }\n}\n\n/* Zones de grille nommées */\n.layout {\n    display: grid;\n    grid-template-areas:\n        \"header header\"\n        \"sidebar main\"\n        \"footer footer\";\n    grid-template-columns: 200px 1fr;\n    min-height: 100vh;\n}\n\n.header { grid-area: header; }\n.sidebar { grid-area: sidebar; }\n.main { grid-area: main; }\n.footer { grid-area: footer; }\n```\n\n**Grid est parfait pour :**\n- Mises en page de cartes\n- Conceptions de tableau de bord\n- Mises en page complexes\n- Design responsive"), 
                "https://picsum.photos/400/300?random=8")
            {
                TypeTag = "code",
                AttachmentType = "image",
                VibeCount = 6,
                IsPublic = true,
                Tags = new List<string> { "css", "grid", "layout", "responsive" },
                Meta = new Dictionary<string, string> { { "language", "css" }, { "difficulty", "intermediate" } }
            },

            new Fragment(
                new LocalizedText("Productivity Hack", "Astuce de productivité"), 
                new LocalizedText("Pomodoro technique for developers", "Technique Pomodoro pour les développeurs"), 
                new LocalizedText("# Pomodoro Technique for Developers\n\n🍅 **The 25-minute rule:**\n\n1. **Choose a task** - Pick one specific thing to work on\n2. **Set timer for 25 minutes** - No distractions allowed\n3. **Work intensely** - Focus only on the chosen task\n4. **Take a 5-minute break** - Step away from the screen\n5. **Repeat** - After 4 pomodoros, take a longer break (15-30 min)\n\n**Benefits for developers:**\n- Reduces burnout\n- Improves focus\n- Makes large tasks manageable\n- Prevents endless debugging sessions\n- Encourages regular breaks\n\n**Pro tip:** Use different pomodoro lengths for different types of work:\n- 25 min: Coding, debugging\n- 45 min: Architecture, design\n- 15 min: Code reviews, documentation",
                    "# Technique Pomodoro pour les développeurs\n\n🍅 **La règle des 25 minutes :**\n\n1. **Choisir une tâche** - Sélectionner une chose spécifique à travailler\n2. **Régler le minuteur sur 25 minutes** - Aucune distraction autorisée\n3. **Travailler intensément** - Se concentrer uniquement sur la tâche choisie\n4. **Prendre une pause de 5 minutes** - S'éloigner de l'écran\n5. **Répéter** - Après 4 pomodoros, prendre une pause plus longue (15-30 min)\n\n**Avantages pour les développeurs :**\n- Réduit l'épuisement\n- Améliore la concentration\n- Rend les grandes tâches gérables\n- Prévient les sessions de débogage sans fin\n- Encourage les pauses régulières\n\n**Conseil pro :** Utilisez différentes durées de pomodoro pour différents types de travail :\n- 25 min : Codage, débogage\n- 45 min : Architecture, conception\n- 15 min : Révisions de code, documentation"), 
                "https://picsum.photos/400/300?random=9")
            {
                TypeTag = "idea",
                AttachmentType = "image",
                VibeCount = 11,
                IsPublic = true,
                Tags = new List<string> { "productivity", "pomodoro", "time-management", "focus" },
                Meta = new Dictionary<string, string> { { "category", "productivity" }, { "technique", "pomodoro" } }
            },

            new Fragment(
                new LocalizedText("API Design Philosophy", "Philosophie de conception d'API"), 
                new LocalizedText("Principles for great API design", "Principes pour une excellente conception d'API"), 
                new LocalizedText("# API Design Philosophy\n\n## Core Principles\n\n**1. Consistency is King**\n- Use the same patterns throughout your API\n- Consistent naming conventions\n- Predictable response formats\n\n**2. Make it Intuitive**\n- RESTful resource names\n- Clear HTTP status codes\n- Self-documenting endpoints\n\n**3. Error Handling**\n```json\n{\n  \"error\": {\n    \"code\": \"VALIDATION_ERROR\",\n    \"message\": \"Invalid email format\",\n    \"details\": {\n      \"field\": \"email\",\n      \"value\": \"invalid-email\"\n    }\n  }\n}\n```\n\n**4. Versioning Strategy**\n- Use semantic versioning\n- Deprecate gradually\n- Support multiple versions\n\n**5. Security First**\n- Authentication required\n- Input validation\n- Rate limiting\n- HTTPS everywhere\n\n**Remember:** Your API is a contract with developers. Make it reliable, predictable, and well-documented.",
                    "# Philosophie de conception d'API\n\n## Principes fondamentaux\n\n**1. La cohérence est reine**\n- Utilisez les mêmes modèles dans toute votre API\n- Conventions de nommage cohérentes\n- Formats de réponse prévisibles\n\n**2. Rendez-la intuitive**\n- Noms de ressources RESTful\n- Codes de statut HTTP clairs\n- Points de terminaison auto-documentés\n\n**3. Gestion des erreurs**\n```json\n{\n  \"error\": {\n    \"code\": \"VALIDATION_ERROR\",\n    \"message\": \"Format d'email invalide\",\n    \"details\": {\n      \"field\": \"email\",\n      \"value\": \"email-invalide\"\n    }\n  }\n}\n```\n\n**4. Stratégie de versioning**\n- Utilisez le versioning sémantique\n- Dépréciation progressive\n- Support de plusieurs versions\n\n**5. Sécurité d'abord**\n- Authentification requise\n- Validation des entrées\n- Limitation de taux\n- HTTPS partout\n\n**Rappel :** Votre API est un contrat avec les développeurs. Rendez-la fiable, prévisible et bien documentée."), 
                "https://picsum.photos/400/300?random=10")
            {
                TypeTag = "code",
                AttachmentType = "image",
                VibeCount = 8,
                IsPublic = true,
                Tags = new List<string> { "api", "design", "rest", "architecture" },
                Meta = new Dictionary<string, string> { { "category", "architecture" }, { "complexity", "intermediate" } }
            },

            new Fragment(
                new LocalizedText("Database Optimization", "Optimisation de base de données"), 
                new LocalizedText("Quick wins for database performance", "Gains rapides pour les performances de base de données"), 
                new LocalizedText("# Database Optimization Quick Wins\n\n## 1. Index Strategy\n```sql\n-- Create composite indexes for common queries\nCREATE INDEX idx_user_email_status ON users(email, status);\n\n-- Index foreign keys\nCREATE INDEX idx_orders_user_id ON orders(user_id);\n\n-- Partial indexes for filtered queries\nCREATE INDEX idx_active_users ON users(created_at) WHERE status = 'active';\n```\n\n## 2. Query Optimization\n```sql\n-- Use LIMIT for pagination\nSELECT * FROM posts ORDER BY created_at DESC LIMIT 10 OFFSET 20;\n\n-- Avoid SELECT *\nSELECT id, title, created_at FROM posts WHERE status = 'published';\n\n-- Use EXISTS instead of IN for subqueries\nSELECT * FROM users u WHERE EXISTS (\n    SELECT 1 FROM orders o WHERE o.user_id = u.id\n);\n```\n\n## 3. Connection Management\n- Use connection pooling\n- Set appropriate timeout values\n- Monitor connection usage\n\n## 4. Caching Strategy\n- Application-level caching\n- Database query result caching\n- CDN for static assets\n\n**Remember:** Profile before optimizing. Measure the impact of changes.",
                    "# Gains rapides pour l'optimisation de base de données\n\n## 1. Stratégie d'index\n```sql\n-- Créer des index composites pour les requêtes communes\nCREATE INDEX idx_user_email_status ON users(email, status);\n\n-- Indexer les clés étrangères\nCREATE INDEX idx_orders_user_id ON orders(user_id);\n\n-- Index partiels pour les requêtes filtrées\nCREATE INDEX idx_active_users ON users(created_at) WHERE status = 'active';\n```\n\n## 2. Optimisation des requêtes\n```sql\n-- Utiliser LIMIT pour la pagination\nSELECT * FROM posts ORDER BY created_at DESC LIMIT 10 OFFSET 20;\n\n-- Éviter SELECT *\nSELECT id, title, created_at FROM posts WHERE status = 'published';\n\n-- Utiliser EXISTS au lieu de IN pour les sous-requêtes\nSELECT * FROM users u WHERE EXISTS (\n    SELECT 1 FROM orders o WHERE o.user_id = u.id\n);\n```\n\n## 3. Gestion des connexions\n- Utiliser le pooling de connexions\n- Définir des valeurs de timeout appropriées\n- Surveiller l'utilisation des connexions\n\n## 4. Stratégie de cache\n- Cache au niveau application\n- Cache des résultats de requêtes de base de données\n- CDN pour les ressources statiques\n\n**Rappel :** Profilez avant d'optimiser. Mesurez l'impact des changements."), 
                "https://picsum.photos/400/300?random=11")
            {
                TypeTag = "code",
                AttachmentType = "image",
                VibeCount = 10,
                IsPublic = true,
                Tags = new List<string> { "database", "optimization", "performance", "sql" },
                Meta = new Dictionary<string, string> { { "category", "performance" }, { "difficulty", "intermediate" } }
            },

            new Fragment(
                new LocalizedText("Team Communication", "Communication d'équipe"), 
                new LocalizedText("Effective communication in dev teams", "Communication efficace dans les équipes de développement"), 
                new LocalizedText("# Effective Team Communication\n\n## Daily Standups\n- **What did you do yesterday?**\n- **What will you do today?**\n- **Any blockers?**\n\nKeep it short (15 minutes max) and focus on coordination, not detailed discussions.\n\n## Code Reviews\n- Be constructive, not critical\n- Focus on the code, not the person\n- Ask questions instead of making statements\n- Acknowledge good work\n\n## Documentation\n- README files for every project\n- API documentation\n- Architecture decision records (ADRs)\n- Onboarding guides\n\n## Async Communication\n- Use threaded conversations\n- Provide context\n- Be clear about urgency\n- Respect time zones\n\n## Conflict Resolution\n1. Address issues early\n2. Listen to understand\n3. Find common ground\n4. Focus on solutions\n5. Follow up\n\n**Remember:** Over-communicate rather than under-communicate. When in doubt, ask questions.",
                    "# Communication d'équipe efficace\n\n## Standups quotidiens\n- **Qu'avez-vous fait hier ?**\n- **Que ferez-vous aujourd'hui ?**\n- **Y a-t-il des blocages ?**\n\nGardez-le court (15 minutes max) et concentrez-vous sur la coordination, pas sur les discussions détaillées.\n\n## Révisions de code\n- Soyez constructif, pas critique\n- Concentrez-vous sur le code, pas sur la personne\n- Posez des questions au lieu de faire des déclarations\n- Reconnaissez le bon travail\n\n## Documentation\n- Fichiers README pour chaque projet\n- Documentation API\n- Enregistrements de décisions d'architecture (ADR)\n- Guides d'intégration\n\n## Communication asynchrone\n- Utilisez des conversations en fil\n- Fournissez du contexte\n- Soyez clair sur l'urgence\n- Respectez les fuseaux horaires\n\n## Résolution de conflits\n1. Adresser les problèmes tôt\n2. Écouter pour comprendre\n3. Trouver un terrain d'entente\n4. Se concentrer sur les solutions\n5. Faire le suivi\n\n**Rappel :** Sur-communiquez plutôt que sous-communiquez. En cas de doute, posez des questions."), 
                "https://picsum.photos/400/300?random=12")
            {
                TypeTag = "idea",
                AttachmentType = "image",
                VibeCount = 4,
                IsPublic = true,
                Tags = new List<string> { "communication", "teamwork", "collaboration", "agile" },
                Meta = new Dictionary<string, string> { { "category", "soft-skills" }, { "importance", "high" } }
            },

            new Fragment(
                new LocalizedText("Testing Philosophy", "Philosophie de tests"), 
                new LocalizedText("Approach to writing maintainable tests", "Approche pour écrire des tests maintenables"), 
                new LocalizedText("# Testing Philosophy\n\n## Test Pyramid\n```\n     /\\     Unit Tests (70%)\n    /  \\    - Fast, isolated\n   /____\\   - Test business logic\n  /      \\  \n /________\\ Integration Tests (20%)\n/__________\\ - Test component interactions\n              E2E Tests (10%)\n              - Test user workflows\n```\n\n## AAA Pattern\n```csharp\n[Test]\npublic void CalculateTotal_WithValidItems_ReturnsCorrectSum()\n{\n    // Arrange\n    var calculator = new OrderCalculator();\n    var items = new[] { new Item(10), new Item(20), new Item(30) };\n    \n    // Act\n    var result = calculator.CalculateTotal(items);\n    \n    // Assert\n    Assert.AreEqual(60, result);\n}\n```\n\n## Best Practices\n- **Test behavior, not implementation**\n- **One assertion per test**\n- **Descriptive test names**\n- **Test edge cases**\n- **Keep tests simple**\n- **Use test doubles wisely**\n\n## Red-Green-Refactor\n1. **Red**: Write a failing test\n2. **Green**: Write minimal code to pass\n3. **Refactor**: Improve code while keeping tests green\n\n**Remember:** Tests are documentation. They should clearly express what the code is supposed to do.",
                    "# Philosophie de tests\n\n## Pyramide de tests\n```\n     /\\     Tests unitaires (70%)\n    /  \\    - Rapides, isolés\n   /____\\   - Testent la logique métier\n  /      \\  \n /________\\ Tests d'intégration (20%)\n/__________\\ - Testent les interactions de composants\n              Tests E2E (10%)\n              - Testent les workflows utilisateur\n```\n\n## Pattern AAA\n```csharp\n[Test]\npublic void CalculateTotal_WithValidItems_ReturnsCorrectSum()\n{\n    // Arrange\n    var calculator = new OrderCalculator();\n    var items = new[] { new Item(10), new Item(20), new Item(30) };\n    \n    // Act\n    var result = calculator.CalculateTotal(items);\n    \n    // Assert\n    Assert.AreEqual(60, result);\n}\n```\n\n## Meilleures pratiques\n- **Tester le comportement, pas l'implémentation**\n- **Une assertion par test**\n- **Noms de tests descriptifs**\n- **Tester les cas limites**\n- **Garder les tests simples**\n- **Utiliser les doublures de test judicieusement**\n\n## Rouge-Vert-Refactoring\n1. **Rouge** : Écrire un test qui échoue\n2. **Vert** : Écrire le code minimal pour passer\n3. **Refactoring** : Améliorer le code en gardant les tests verts\n\n**Rappel :** Les tests sont de la documentation. Ils doivent exprimer clairement ce que le code est censé faire."), 
                "https://picsum.photos/400/300?random=13")
            {
                TypeTag = "idea",
                AttachmentType = "image",
                VibeCount = 6,
                IsPublic = true,
                Tags = new List<string> { "testing", "tdd", "unit-tests", "quality" },
                Meta = new Dictionary<string, string> { { "category", "testing" }, { "methodology", "tdd" } }
            },

            new Fragment(
                new LocalizedText("Mindful Coding", "Codage en pleine conscience"), 
                new LocalizedText("Bringing mindfulness to programming", "Apporter la pleine conscience à la programmation"), 
                new LocalizedText("# Mindful Coding\n\n## The Art of Focused Development\n\n**Before you code:**\n- Take a deep breath\n- Clear your mind\n- Understand the problem fully\n- Set a clear intention\n\n**While coding:**\n- Stay present with each line\n- Notice when your mind wanders\n- Take breaks when frustrated\n- Celebrate small wins\n\n**Code meditation:**\n```javascript\n// Write code as if you're writing poetry\nconst findInnerPeace = (chaos) => {\n    return chaos\n        .filter(thought => thought.isPositive)\n        .map(thought => thought.transform())\n        .reduce((peace, thought) => peace + thought, 0);\n};\n```\n\n**Benefits:**\n- Reduced stress\n- Better problem-solving\n- Cleaner code\n- Improved focus\n- Greater job satisfaction\n\n**Remember:** Code is not just logic; it's an expression of human creativity and intelligence.",
                    "# Codage en pleine conscience\n\n## L'art du développement concentré\n\n**Avant de coder :**\n- Prendre une grande respiration\n- Vider son esprit\n- Comprendre pleinement le problème\n- Fixer une intention claire\n\n**Pendant le codage :**\n- Rester présent avec chaque ligne\n- Remarquer quand votre esprit divague\n- Prendre des pauses quand frustré\n- Célébrer les petites victoires\n\n**Méditation du code :**\n```javascript\n// Écrire du code comme si vous écriviez de la poésie\nconst findInnerPeace = (chaos) => {\n    return chaos\n        .filter(thought => thought.isPositive)\n        .map(thought => thought.transform())\n        .reduce((peace, thought) => peace + thought, 0);\n};\n```\n\n**Avantages :**\n- Stress réduit\n- Meilleure résolution de problèmes\n- Code plus propre\n- Concentration améliorée\n- Plus grande satisfaction au travail\n\n**Rappel :** Le code n'est pas que de la logique ; c'est une expression de créativité et d'intelligence humaine."), 
                "https://picsum.photos/400/300?random=14")
            {
                TypeTag = "idea",
                AttachmentType = "image",
                VibeCount = 13,
                IsPublic = true,
                Tags = new List<string> { "mindfulness", "philosophy", "wellness", "productivity" },
                Meta = new Dictionary<string, string> { { "category", "wellness" }, { "approach", "mindful" } }
            },

            new Fragment(
                new LocalizedText("Future Tech Trends", "Tendances technologiques futures"), 
                new LocalizedText("Emerging technologies to watch", "Technologies émergentes à surveiller"), 
                new LocalizedText("# Future Tech Trends to Watch\n\n## 🤖 AI & Machine Learning\n- **Large Language Models**: More sophisticated AI assistants\n- **Code Generation**: AI writing entire applications\n- **Automated Testing**: AI-powered test generation\n- **Predictive Analytics**: Better user behavior prediction\n\n## 🌐 Web Technologies\n- **WebAssembly**: Near-native performance in browsers\n- **Progressive Web Apps**: Bridging web and native apps\n- **WebXR**: Virtual and augmented reality on the web\n- **Edge Computing**: Faster response times globally\n\n## 🔗 Blockchain & Distributed Systems\n- **Decentralized Identity**: User-controlled authentication\n- **Smart Contracts**: Automated business logic\n- **Distributed Storage**: IPFS and similar technologies\n- **Cryptocurrency Integration**: Native crypto payments\n\n## 🛡️ Security & Privacy\n- **Zero-Trust Architecture**: Never trust, always verify\n- **Homomorphic Encryption**: Compute on encrypted data\n- **Quantum-Resistant Cryptography**: Post-quantum security\n- **Privacy-Preserving Analytics**: Data insights without exposure\n\n## 🌿 Sustainability\n- **Green Computing**: Energy-efficient algorithms\n- **Carbon-Aware Development**: Optimize for low-carbon times\n- **Sustainable Architecture**: Minimize environmental impact\n\n**Stay curious, keep learning, and prepare for an exciting future!**",
                    "# Tendances technologiques futures à surveiller\n\n## 🤖 IA et apprentissage automatique\n- **Modèles de langage volumineux** : Assistants IA plus sophistiqués\n- **Génération de code** : IA écrivant des applications entières\n- **Tests automatisés** : Génération de tests alimentée par l'IA\n- **Analyse prédictive** : Meilleure prédiction du comportement utilisateur\n\n## 🌐 Technologies web\n- **WebAssembly** : Performance quasi-native dans les navigateurs\n- **Applications web progressives** : Pont entre web et applications natives\n- **WebXR** : Réalité virtuelle et augmentée sur le web\n- **Edge Computing** : Temps de réponse plus rapides globalement\n\n## 🔗 Blockchain et systèmes distribués\n- **Identité décentralisée** : Authentification contrôlée par l'utilisateur\n- **Contrats intelligents** : Logique métier automatisée\n- **Stockage distribué** : IPFS et technologies similaires\n- **Intégration cryptomonnaie** : Paiements crypto natifs\n\n## 🛡️ Sécurité et confidentialité\n- **Architecture Zero-Trust** : Ne jamais faire confiance, toujours vérifier\n- **Chiffrement homomorphe** : Calcul sur données chiffrées\n- **Cryptographie résistante quantique** : Sécurité post-quantique\n- **Analyse préservant la vie privée** : Insights sans exposition des données\n\n## 🌿 Durabilité\n- **Informatique verte** : Algorithmes éco-énergétiques\n- **Développement conscient du carbone** : Optimiser pour les moments à faible carbone\n- **Architecture durable** : Minimiser l'impact environnemental\n\n**Restez curieux, continuez à apprendre, et préparez-vous pour un futur passionnant !**"), 
                "https://picsum.photos/400/300?random=15")
            {
                TypeTag = "code",
                AttachmentType = "image",
                VibeCount = 14,
                IsPublic = true,
                Tags = new List<string> { "future", "trends", "technology", "innovation" },
                Meta = new Dictionary<string, string> { { "category", "trends" }, { "timeframe", "2025-2030" } }
            }
        };
    }
}