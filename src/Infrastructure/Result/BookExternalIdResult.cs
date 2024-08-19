namespace Epos.Infrastructure.Result;

public sealed class BookExternalIdResult
{
    public required string EAN { get; init; }
    public required Guid Id { get; init; }
    public required string SKU { get; init; }
}
