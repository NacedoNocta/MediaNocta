namespace BlogLibrairy.Interfaces;

public interface ITag
{
    Guid Id { get; init; }
    string Name { get; set; }
    string ToJson();
}