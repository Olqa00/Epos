namespace Epos.Infrastructure.Services;

using Epos.Domain.Entities;
using Epos.Domain.Interfaces;

internal sealed class BookRepository : IBookRepository
{
    private readonly IDocumentStore documentStore;

    public BookRepository(IDocumentStore documentStore)
    {
        this.documentStore = documentStore;
    }

    public async Task<BookEntity?> LoadAsync(Guid bookId, CancellationToken cancellationToken = default)
    {
        await using var session = this.documentStore.QuerySession();

        var bookEntity = await session.LoadAsync<BookEntity>(bookId, cancellationToken);

        if (bookEntity is null)
        {
            return default;
        }

        return bookEntity;
    }

    public async Task SaveAsync(BookEntity entity, CancellationToken cancellationToken = default)
    {
        await using var session = this.documentStore.LightweightSession();

        session.Store(entity);

        await session.SaveChangesAsync(cancellationToken);
    }
}
