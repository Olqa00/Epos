namespace Epos.Infrastructure.Result;

public sealed class BookNumberOfPagesResult
{
    public required Guid Id { get; init; }
    public required int NumberOfPages { get; init; }
}
