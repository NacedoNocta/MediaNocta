using SharedLibrary;

namespace BlogLibrary.Interfaces;

public interface IAuthor
{
    Guid Id { get; init; }
    string Name { get; set; }
    LocalizedText? Biography { get; set; }
    string? ImageUrl { get; set; }
    string? Email { get; set; }
    string? Website { get; set; }
    string? Twitter { get; set; }
    string? LinkedIn { get; set; }
    string? GitHub { get; set; }
    string ToJson();
}