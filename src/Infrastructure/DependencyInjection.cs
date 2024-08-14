namespace Epos.Infrastructure;

using Epos.Domain.Interfaces;
using Epos.Infrastructure.Services;

[ExcludeFromCodeCoverage]
public static class DependencyInjection
{
    public static void AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var options = configuration.GetSection(PersistenceOptions.SECTION_NAME).Get<PersistenceOptions>();

        options ??= new PersistenceOptions();

        // Marten
        var store = DocumentStore.For(storeOptions =>
        {
            storeOptions.Connection(options.ConnectionStringReadService);
            storeOptions.Schema.Include<PersistenceRegistry>();
            storeOptions.AutoCreateSchemaObjects = AutoCreate.CreateOrUpdate;
        });

        var storeReadService = DocumentStore.For(storeOptions =>
        {
            storeOptions.Connection(options.ConnectionStringReadService);
            storeOptions.Schema.Include<PersistenceRegistry>();
            storeOptions.AutoCreateSchemaObjects = AutoCreate.CreateOrUpdate;
        });

        // Services
        services.AddSingleton<IDocumentStore>(store);
        services.AddSingleton<IDocumentStore>(storeReadService);
        services.AddSingleton(options);

        services.AddSingleton<IBookRepository, BookRepository>();
    }
}
