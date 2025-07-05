using SharedLibrary.Interfaces;

namespace FragmentLibrary.Interfaces
{
    public interface IFragment : IActivity
    {
        public string? TypeTag { get; set; }
        public string? AttachmentUrl { get; set; }
        public string? AttachmentType { get; set; }
        public int VibeCount { get; set; }
        public bool IsPublic { get; set; }
        public bool IsDeleted { get; set; }
    }
}