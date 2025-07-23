namespace BlogLibrary.Interfaces;

public interface ITag
{
    Guid Id { get; init; }
    string Name { get; set; }
    string Color { get; set; }
    string ToJson();
}