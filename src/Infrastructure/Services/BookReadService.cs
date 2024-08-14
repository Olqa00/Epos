namespace Epos.Infrastructure.Services;

using Epos.Domain.Entities;
using Epos.Infrastructure.Constants;
using Epos.Infrastructure.Interfaces;
using Epos.Infrastructure.Result;

internal sealed class BookReadService : IBookReadService
{
    private const string GET_BY_NUMBER_OF_PAGES = """
                                                  WHERE (data -> 'Details' ->> 'NumberOfPages') = @Pages;
                                                  """;

    private const string GET_ID_AND_NUMBER = """
                                             SELECT (data ->> 'Id')::uuid AS "Id", (data -> 'Details' ->> 'NumberOfPages')::integer AS "NumberOfPages"
                                             FROM public.mt_doc_bookentity
                                             WHERE (data -> 'Details' ->> 'NumberOfPages') = @Pages;
                                             """;

    private readonly PersistenceOptions options;

    private readonly IDocumentStore storeReadService;

    public BookReadService(PersistenceOptions options, IDocumentStore storeReadService)
    {
        this.options = options;
        this.storeReadService = storeReadService;
    }

    public async Task<IEnumerable<BookEntity>> GetBooksAsync(CancellationToken cancellationToken = default)
    {
        await using var session = this.storeReadService.QuerySession();

        var books = await session.Query<BookEntity>().ToListAsync(cancellationToken);

        return books;
    }

    public async Task<BookEntity?> GetByExternalIdAsync(string externalId, CancellationToken cancellationToken = default)
    {
        await using var session = this.storeReadService.QuerySession();

        var book = await session.Query<BookEntity>()
            .Where(bookEntity =>
                bookEntity.Details.ContainsKey(BookDetails.SKU) && bookEntity.Details[BookDetails.SKU] == externalId
                || bookEntity.Details.ContainsKey(BookDetails.EAN) && bookEntity.Details[BookDetails.EAN] == externalId)
            .FirstOrDefaultAsync(cancellationToken);

        return book;
    }

    public async Task<BookEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await using var session = this.storeReadService.QuerySession();

        var book = await session.Query<BookEntity>().FirstOrDefaultAsync(dbEntity => dbEntity.Id == id, cancellationToken);

        return book;
    }

    public async Task<BookEntity?> GetByNumberOfPagesAsync(int numberOfPages, CancellationToken cancellationToken = default)
    {
        await using var session = this.storeReadService.QuerySession();

        var book = await session.Query<BookEntity>()
            .Where(bookEntity => bookEntity.Details.ContainsKey(BookDetails.NUMBER_OF_PAGES) && bookEntity.Details[BookDetails.NUMBER_OF_PAGES] == numberOfPages.ToString())
            .FirstOrDefaultAsync(cancellationToken);

        return book;
    }

    public async Task<BookNumberOfPagesResult?> GetByNumberOfPagesReturnBookResultWithDapperAsync(int numberOfPages, CancellationToken cancellationToken = default)
    {
        await using var session = this.storeReadService.QuerySession();

        var parameters = new
        {
            Pages = numberOfPages.ToString(),
        };

        await using var connection = new NpgsqlConnection(this.options.ConnectionStringReadService);
        //await connection.Open();

        var result = (await connection.QueryAsync<BookNumberOfPagesResult>(GET_ID_AND_NUMBER, parameters)).FirstOrDefault();

        return result;
    }

    public async Task<IEnumerable<BookEntity>> GetByNumberOfPagesWithScriptJsonAsync(int numberOfPages, CancellationToken cancellationToken = default)
    {
        await using var session = this.storeReadService.QuerySession();

        var parameters = new
        {
            Pages = numberOfPages.ToString(),
        };

        var books = await session.QueryAsync<BookEntity>(GET_BY_NUMBER_OF_PAGES, cancellationToken, parameters);

        return books;
    }

    public async Task InsertAsync(Guid bookId, string title, Dictionary<string, string> details, CancellationToken cancellationToken = default)
    {
        await using var session = this.storeReadService.LightweightSession();

        var book = new BookEntity(bookId, title)
        {
            Details = details,
        };

        session.Insert(book);

        await session.SaveChangesAsync(cancellationToken);
    }
}
