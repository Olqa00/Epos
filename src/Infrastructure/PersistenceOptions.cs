namespace Epos.Infrastructure;

public sealed record PersistenceOptions
{
    public static readonly string SECTION_NAME = "Persistence";

    public string ConnectionString { get; init; } = "Server=localhost;Port=5432;Database=epos;User Id=postgres;Password=postgres;";
    public string ConnectionStringReadService { get; init; } = "Server=localhost;Port=5432;Database=eposreadservice;User Id=postgres;Password=postgres;";
}
