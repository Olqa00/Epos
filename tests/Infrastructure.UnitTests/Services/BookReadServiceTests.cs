namespace Epos.Infrastructure.UnitTests.Services;

using Epos.Domain.Entities;
using Epos.Infrastructure.Constants;
using Epos.Infrastructure.Result;
using Epos.Infrastructure.Services;
using Weasel.Core;

[TestClass]
public sealed class BookReadServiceTests
{
    private const string CONNECTION_STRING = "Server=localhost;Port=5432;Database=eposreadservice;User Id=postgres;Password=postgres;";
    private const string DETAILS_EAN_KEY = BookDetails.EAN;
    private const int DETAILS_NUMBER_OF_PAGES = 32;
    private const string DETAILS_NUMBER_OF_PAGES_KEY = BookDetails.NUMBER_OF_PAGES;
    private const string DETAILS_SKU_KEY = BookDetails.SKU;
    private const string EAN = "123123123123";
    private const string SKU = "BOO-RED-BLU-POL";
    private const string TITLE = "title";
    private const string TITLE_2 = "title2";

    private static readonly Guid BOOK_ID_1 = Guid.NewGuid();
    private static readonly Guid BOOK_ID_2 = Guid.NewGuid();

    private static readonly Dictionary<string, string> DETAILS = new()
    {
        { DETAILS_NUMBER_OF_PAGES_KEY, DETAILS_NUMBER_OF_PAGES.ToString() },
    };

    private static readonly BookEntity BOOK_ENTITY_1 = new(BOOK_ID_1, TITLE)
    {
        Details = DETAILS,
    };

    private static readonly Dictionary<string, string> DETAILS_WITH_EXTERNAL_ID = new()
    {
        { DETAILS_SKU_KEY, SKU },
        { DETAILS_EAN_KEY, EAN },
    };

    private static readonly BookEntity BOOK_ENTITY_2 = new(BOOK_ID_2, TITLE_2)
    {
        Details = DETAILS_WITH_EXTERNAL_ID,
    };

    private static readonly List<BookEntity> BOOK_ENTITIES =
    [
        BOOK_ENTITY_1,
        BOOK_ENTITY_2,
    ];

    private static readonly BookNumberOfPagesResult BOOK_NUMBER_OF_PAGES_RESULT = new()
    {
        Id = BOOK_ID_1,
        NumberOfPages = DETAILS_NUMBER_OF_PAGES,
    };

    private readonly NpgsqlConnection connection = new(CONNECTION_STRING);
    private readonly DocumentStore store;

    private Respawner? respawner = default;

    public BookReadServiceTests()
    {
        this.store = DocumentStore.For(storeOptions =>
        {
            storeOptions.Connection(CONNECTION_STRING);
            storeOptions.Schema.Include<PersistenceRegistry>();
            storeOptions.AutoCreateSchemaObjects = AutoCreate.All;
        });
    }

    [TestMethod]
    public async Task GetBooksAsync_Should_GetBooks()
    {
        // Arrange
        var readService = new BookReadService(this.store);
        await readService.InsertAsync(BOOK_ID_1, TITLE, DETAILS, CancellationToken.None);
        await readService.InsertAsync(BOOK_ID_2, TITLE_2, DETAILS_WITH_EXTERNAL_ID, CancellationToken.None);

        // Act
        var result = await readService.GetBooksAsync(CancellationToken.None);

        // Assert
        result.Should()
            .BeEquivalentTo(BOOK_ENTITIES)
            ;
    }

    [TestMethod]
    public async Task GetByIdAsync_Should_GetBook()
    {
        // Arrange
        var readService = new BookReadService(this.store);
        await readService.InsertAsync(BOOK_ID_1, TITLE, DETAILS, CancellationToken.None);

        // Act
        var result = await readService.GetByIdAsync(BOOK_ID_1, CancellationToken.None);

        // Assert
        result.Should()
            .BeEquivalentTo(BOOK_ENTITY_1)
            ;
    }

    [TestMethod]
    public async Task GetByNumberOfPagesAsync_Should_GetBook()
    {
        // Arrange
        var readService = new BookReadService(this.store);
        await readService.InsertAsync(BOOK_ID_1, TITLE, DETAILS, CancellationToken.None);

        // Act
        var result = await readService.GetByNumberOfPagesAsync(DETAILS_NUMBER_OF_PAGES, CancellationToken.None);

        // Assert
        result.Should()
            .BeEquivalentTo(BOOK_ENTITY_1)
            ;
    }

    [TestMethod]
    public async Task GetByNumberOfPagesReturnBookResultAsync_Should_GetBook()
    {
        // Arrange
        var readService = new BookReadService(this.store);
        await readService.InsertAsync(BOOK_ID_1, TITLE, DETAILS, CancellationToken.None);

        // Act
        var result = await readService.GetByNumberOfPagesReturnBookResultAsync(DETAILS_NUMBER_OF_PAGES, CancellationToken.None);

        // Assert
        result.Should()
            .BeEquivalentTo(BOOK_NUMBER_OF_PAGES_RESULT)
            ;
    }

    [TestMethod]
    public async Task GetByNumberOfPagesWithScriptJsonAsync_Should_GetBook()
    {
        // Arrange
        var readService = new BookReadService(this.store);
        await readService.InsertAsync(BOOK_ID_1, TITLE, DETAILS, CancellationToken.None);

        // Act
        var result = await readService.GetByNumberOfPagesWithScriptJsonAsync(DETAILS_NUMBER_OF_PAGES, CancellationToken.None);

        // Assert
        var entities = new List<BookEntity>
        {
            BOOK_ENTITY_1,
        };

        result.Should()
            .BeEquivalentTo(entities)
            ;
    }

    [TestMethod]
    public async Task InsertAsync_Should_InsertBook()
    {
        // Arrange
        var readService = new BookReadService(this.store);

        // Act
        await readService.InsertAsync(BOOK_ID_1, TITLE, DETAILS, CancellationToken.None);
        await readService.InsertAsync(BOOK_ID_2, TITLE_2, DETAILS_WITH_EXTERNAL_ID, CancellationToken.None);

        // Assert
        var result = await readService.GetByIdAsync(BOOK_ID_1, CancellationToken.None);

        result.Should()
            .BeEquivalentTo(BOOK_ENTITY_1)
            ;
    }

    [TestInitialize]
    public async Task Setup()
    {
        // Arrange
        await this.connection.OpenAsync();

        var options = new RespawnerOptions
        {
            DbAdapter = DbAdapter.Postgres,
        };

        this.respawner = await Respawner.CreateAsync(this.connection, options);
    }

    [TestCleanup]
    public async Task Teardown()
    {
        await this.respawner!.ResetAsync(this.connection);
        await this.connection.CloseAsync();
    }
}
