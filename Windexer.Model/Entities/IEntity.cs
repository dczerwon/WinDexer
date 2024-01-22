namespace Windexer.Model.Entities;

public interface IEntity
{
    Guid Id { get; }

    string Path { get; }

    DateTime? IndexationDate { get; set; }
}
