namespace DiscordBot.Model.Storage;

public interface IInMemoryRepository<T> : IRepository<T> where T : Entity
{
    Task Clear();
}