namespace Epos.Infrastructure.Interfaces;

using Epos.Domain.Entities;
using Epos.Infrastructure.Result;

public interface IBookReadService
{
    Task<IEnumerable<BookEntity>> GetBooksAsync(CancellationToken cancellationToken = default);
    Task<BookEntity?> GetByExternalIdAsync(string externalId, CancellationToken cancellationToken = default);
    Task<BookEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<BookEntity?> GetByNumberOfPagesAsync(int numberOfPages, CancellationToken cancellationToken = default);
    Task<BookNumberOfPagesResult?> GetByNumberOfPagesReturnBookResultWithDapperAsync(int numberOfPages, CancellationToken cancellationToken = default);
    Task<BookNumberOfPagesResult?> GetByNumberOfPagesReturnBookResultWithMartenAsync(int numberOfPages, CancellationToken cancellationToken = default);
    Task<IEnumerable<BookEntity>> GetByNumberOfPagesWithScriptJsonAsync(int numberOfPages, CancellationToken cancellationToken = default);
    Task InsertAsync(Guid bookId, string title, Dictionary<string, string> details, CancellationToken cancellationToken = default);
}
