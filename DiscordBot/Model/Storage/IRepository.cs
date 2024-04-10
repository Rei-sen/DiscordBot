namespace DiscordBot.Model.Storage;

/// <summary>
/// Represents a generic repository interface for CRUD operations on entities.
/// </summary>
/// <typeparam name="T">The type of entity.</typeparam>
public interface IRepository<T> where T : Entity
{
    /// <summary>
    /// Retrieves an entity by its ID.
    /// </summary>
    /// <param name="id">The ID of the entity.</param>
    /// <returns>The entity with the specified ID, or null if not found.</returns>
    Task<T?> Get(string id);

    /// <summary>
    /// Deletes an entity.
    /// </summary>
    /// <param name="item">The entity to delete.</param>
    /// <returns>True if the entity was successfully deleted, false otherwise.</returns>
    Task<bool> Delete(T item);

    /// <summary>
    /// Deletes an entity by its ID.
    /// </summary>
    /// <param name="id">The ID of the entity to delete.</param>
    /// <returns>True if the entity was successfully deleted, false otherwise.</returns>
    Task<bool> Delete(string id);

    /// <summary>
    /// Inserts a new entity.
    /// </summary>
    /// <param name="item">The entity to insert.</param>
    /// <returns>True if the entity was successfully inserted, false otherwise.</returns>
    Task<bool> Insert(T item);

    /// <summary>
    /// Updates an existing entity.
    /// </summary>
    /// <param name="item">The entity to update.</param>
    /// <returns>True if the entity was successfully updated, false otherwise.</returns>
    Task<bool> Update(T item);

    /// <summary>
    /// Queries the repository for entities.
    /// </summary>
    /// <returns>An IQueryable of entities.</returns>
    Task<IQueryable<T>> Query();
}
