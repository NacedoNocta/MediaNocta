namespace BlogLibrary.Interfaces;

public interface IAuthor
{
    Guid Id { get; init; }
    string Name { get; set; }
    string? Biography { get; set; }
    string? ImageUrl { get; set; }
    string ToJson();
}