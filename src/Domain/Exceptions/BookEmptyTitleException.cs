namespace Epos.Domain.Exceptions;

public sealed class BookEmptyTitleException : DomainException
{
    public BookEmptyTitleException(Guid id)
        : base($"Book title can not be empty. Book id: {id}")
    {
        this.Id = id;
    }
}
