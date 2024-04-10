namespace DiscordBot.Model.Storage;

/// <summary>
/// Represents an abstract entity in the storage.
/// </summary>
public record class Entity
{
    /// <summary>
    /// Gets or sets the unique identifier of the entity.
    /// </summary>
    public string Id { get; init; } = Guid.NewGuid().ToString().Substring(0, 8);
}
