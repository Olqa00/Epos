namespace Epos.Domain.Exceptions;

public abstract class DomainException : Exception
{
    public Guid Id { get; protected init; } = Guid.Empty;

    protected DomainException(string message, Exception? innerException = null)
        : base(message, innerException)
    {
    }
}
