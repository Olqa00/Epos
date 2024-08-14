namespace Epos.Domain.UnitTests.Entities;

using Epos.Domain.Entities;
using Epos.Domain.Exceptions;

[TestClass]
public sealed class BookEntityTests
{
    private static readonly Guid BOOK_ID = Guid.NewGuid();

    [DataTestMethod, DataRow(""), DataRow("    ")]
    public void Create_Should_ThrowBookEmptyTitleException_When_Title_Is_Empty(string title)
    {
        // Arrange

        // Act
        var action = () => new BookEntity(BOOK_ID, title);

        // Assert
        action.Should()
            .Throw<BookEmptyTitleException>()
            ;
    }
}
