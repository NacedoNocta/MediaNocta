using SharedLibrary.Interfaces;

namespace TechLibrary.Interfaces
{
    /// <summary>
    /// Interface for tech update entities
    /// </summary>
    public interface ITechUpdate : IActivity
    {
        /// <summary>
        /// Project identifier slug from ProjectKeys enum
        /// </summary>
        string ProjectId { get; set; }
        
        /// <summary>
        /// Type of update (flexible meta-tag system)
        /// </summary>
        string UpdateType { get; set; }
    }
}