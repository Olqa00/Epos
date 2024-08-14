namespace Epos.Domain.Interfaces;

using Epos.Domain.Entities;

public interface IBookRepository
{
    Task<BookEntity?> LoadAsync(Guid bookId, CancellationToken cancellationToken = default);
    Task SaveAsync(BookEntity entity, CancellationToken cancellationToken = default);
}
