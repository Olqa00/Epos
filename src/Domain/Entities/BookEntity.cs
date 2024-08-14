namespace Epos.Domain.Entities;

using Epos.Domain.Exceptions;

public sealed class BookEntity
{
    public Dictionary<string, string> Details { get; set; } = new();
    public Guid Id { get; private set; }
    public string Title { get; private set; }

    public BookEntity(Guid id, string title)
    {
        this.Id = id;

        this.SetTitle(title);
    }

    public void AddDetails(string key, string value)
    {
        this.Details[key] = value;
    }

    public void SetTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new BookEmptyTitleException(this.Id);
        }

        this.Title = title;
    }
}
