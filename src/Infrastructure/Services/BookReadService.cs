namespace Epos.Infrastructure.Services;

using Epos.Domain.Entities;
using Epos.Infrastructure.Constants;
using Epos.Infrastructure.Extensions;
using Epos.Infrastructure.Interfaces;
using Epos.Infrastructure.Result;
using Npgsql;
using NpgsqlTypes;

internal sealed class BookReadService : IBookReadService
{
    private const string GET_BY_NUMBER_OF_PAGES = """
                                                  WHERE (data -> 'Details' ->> 'NumberOfPages') = ?;
                                                  """;

    private const string GET_ID_AND_NUMBER = """
                                             SELECT (data ->> 'id'), (data -> 'Details' ->> 'NumberOfPages') AS "NumberOfPages"
                                             FROM public.mt_doc_bookentity
                                             """;

    private const string GET_NUMBER_OF_PAGES = """
                                               SELECT to_json(data -> 'Details' ->> 'NumberOfPages') AS "NumberOfPages"
                                               FROM public.mt_doc_bookentity
                                               WHERE (data -> 'Details' ->> 'NumberOfPages') is not null;
                                               """;

    private readonly IDocumentStore storeReadService;

    public BookReadService(IDocumentStore storeReadService)
    {
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

    public async Task<BookNumberOfPagesResult?> GetByNumberOfPagesReturnBookResultAsync(int numberOfPages, CancellationToken cancellationToken = default)
    {
        await using var session = this.storeReadService.QuerySession();

        var parameter = new NpgsqlParameter("Pages", NpgsqlDbType.Text)
        {
            Value = numberOfPages.ToString(),
        };

        var numberOfPagesJson = numberOfPages.ToString();

        var (id, number) = session.AdvancedSql.Query<int, string>(GET_ID_AND_NUMBER)[0];

        var result = new BookNumberOfPagesResult
        {
            Id = Guid.NewGuid(), //id,
            NumberOfPages = 123, //number,
        };

        return result;
    }

    public async Task<IEnumerable<BookEntity>> GetByNumberOfPagesWithScriptJsonAsync(int numberOfPages, CancellationToken cancellationToken = default)
    {
        await using var session = this.storeReadService.QuerySession();

        var numberOfPagesJson = numberOfPages.ToJson();

        var parameter = new NpgsqlParameter("Pages", NpgsqlDbType.Integer)
        {
            Value = numberOfPages.ToString(),
        };

        var books = await session.QueryAsync<BookEntity>(GET_BY_NUMBER_OF_PAGES, cancellationToken, numberOfPagesJson);

        //var books = session.Query<BookEntity>(GET_BY_NUMBER_OF_PAGES, numberOfPagesJson);

        //var book = session.Query<BookEntity>($"WHERE(data-> 'Details'->> 'NumberOfPages') = '{numberOfPages}';");

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
