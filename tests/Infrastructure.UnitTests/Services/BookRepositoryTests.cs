namespace Epos.Infrastructure.UnitTests.Services;

using Epos.Domain.Entities;
using Epos.Infrastructure.Constants;
using Epos.Infrastructure.Services;

[TestClass]
public sealed class BookRepositoryTests
{
    private const string CONNECTION_STRING = "Server=localhost;Port=5432;Database=epos;User Id=postgres;Password=postgres;";
    private const int DETAILS_NUMBER_OF_PAGES = 32;
    private const string DETAILS_NUMBER_OF_PAGES_KEY = BookDetails.NUMBER_OF_PAGES;
    private const string TITLE = "title";
    private static readonly Guid BOOK_ID = Guid.NewGuid();
    private readonly NpgsqlConnection connection = new(CONNECTION_STRING);
    private readonly DocumentStore store;

    private Respawner? respawner = default;

    public BookRepositoryTests()
    {
        this.store = DocumentStore.For(storeOptions =>
        {
            storeOptions.Connection(CONNECTION_STRING);
            storeOptions.Schema.Include<PersistenceRegistry>();
        });
    }

    [TestMethod]
    public async Task LoadAsync_Should_Get_Book()
    {
        // Arrange
        var entity = new BookEntity(BOOK_ID, TITLE);
        entity.AddDetails(DETAILS_NUMBER_OF_PAGES_KEY, DETAILS_NUMBER_OF_PAGES.ToString());

        var repository = new BookRepository(this.store);
        await repository.SaveAsync(entity);

        // Act
        var result = await repository.LoadAsync(BOOK_ID, CancellationToken.None);

        // Assert
        result.Should()
            .BeEquivalentTo(entity)
            ;
    }

    [TestMethod]
    public async Task SaveAsync_Should_Add_Book()
    {
        var entity = new BookEntity(BOOK_ID, TITLE);
        entity.AddDetails(DETAILS_NUMBER_OF_PAGES_KEY, DETAILS_NUMBER_OF_PAGES.ToString());

        var repository = new BookRepository(this.store);

        // Act
        await repository.SaveAsync(entity, CancellationToken.None);

        // Assert
        var result = await repository.LoadAsync(BOOK_ID, CancellationToken.None);

        result.Should()
            .BeEquivalentTo(entity)
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

        //this.respawner = await Respawner.CreateAsync(this.connection, options);
    }

    [TestCleanup]
    public async Task Teardown()
    {
        //await this.respawner!.ResetAsync(this.connection);
        await this.connection.CloseAsync();
    }
}
