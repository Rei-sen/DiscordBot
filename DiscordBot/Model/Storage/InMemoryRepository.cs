using System.Collections.Concurrent;

namespace DiscordBot.Model.Storage;

/// <summary>
/// Represents an in-memory repository implementation that stores entities of type T.
/// </summary>
/// <typeparam name="T">The type of entity.</typeparam>
internal class InMemoryRepository<T> : IRepository<T> where T : Entity
{
    private readonly ConcurrentDictionary<string, T> _repository = new();

    /// <summary>
    /// Retrieves an entity with the specified id from the repository.
    /// </summary>
    /// <param name="id">The id of the entity to retrieve.</param>
    /// <returns>The retrieved entity, or null if not found.</returns>
    public Task<T?> Get(string id) =>
        Task.FromResult(_repository.TryGetValue(id, out var item) ? item : null);

    /// <summary>
    /// Deletes the specified entity from the repository.
    /// </summary>
    /// <param name="item">The entity to delete.</param>
    /// <returns>True if the entity was deleted successfully, false otherwise.</returns>
    public Task<bool> Delete(T item) =>
        this.Delete(item.Id);

    /// <summary>
    /// Deletes the entity with the specified id from the repository.
    /// </summary>
    /// <param name="id">The id of the entity to delete.</param>
    /// <returns>True if the entity was deleted successfully, false otherwise.</returns>
    public Task<bool> Delete(string id) =>
        Task.FromResult(_repository.Remove(id, out _));

    /// <summary>
    /// Inserts the specified entity into the repository.
    /// </summary>
    /// <param name="item">The entity to insert.</param>
    /// <returns>True if the entity was inserted successfully, false otherwise.</returns>
    public Task<bool> Insert(T item) =>
        Task.FromResult(_repository.TryAdd(item.Id, item));

    /// <summary>
    /// Updates the specified entity in the repository.
    /// </summary>
    /// <param name="item">The entity to update.</param>
    /// <returns>True if the entity was updated successfully, false otherwise.</returns>
    public Task<bool> Update(T item)
    {
        if (!_repository.ContainsKey(item.Id))
            return Task.FromResult(false);

        _repository[item.Id] = item;
        return Task.FromResult(true);
    }

    /// <summary>
    /// Retrieves all entities from the repository.
    /// </summary>
    /// <returns>An IQueryable of all entities in the repository.</returns>
    public Task<IQueryable<T>> Query() =>
        Task.FromResult(_repository.Values.AsQueryable());
}
