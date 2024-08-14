namespace Epos.Infrastructure.Constants;

public static class BookDetails
{
    public const string EAN = "EAN";
    public const string NUMBER_OF_PAGES = "NumberOfPages";
    public const string PUBLISHING = "Publishing";
    public const string SKU = "SKU";

    public static readonly IReadOnlyDictionary<string, string> BOOK_DETAILS_MAPPINGS = new Dictionary<string, string>
    {
        { EAN, Domain.Constants.BookDetails.EAN },
        { SKU, Domain.Constants.BookDetails.SKU },
        { NUMBER_OF_PAGES, Domain.Constants.BookDetails.NUMBER_OF_PAGES },
        { PUBLISHING, Domain.Constants.BookDetails.PUBLISHING },
    };
}
