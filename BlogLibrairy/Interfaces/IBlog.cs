using SharedLibrairy.Interfaces;

namespace BlogLibrairy.Interfaces;

public interface IBlog : IActivity
{
    List<Tag> Tags { get; set; }
    Author Author { get; set; }
    new string ToJson();
}