namespace Epos.Domain.Constants;

using Epos.Domain.Enums;

public static class BookDetails
{
    public static readonly string EAN = "EAN";
    public static readonly string NUMBER_OF_PAGES = "NumberOfPages";
    public static readonly string PUBLISHING = "Publishing";
    public static readonly string SKU = "SKU";

    public static IReadOnlyDictionary<string, ExternalIdType> BOOKS_EXTERNAL_IDS
    {
        get
        {
            return new Dictionary<string, ExternalIdType>
            {
                { EAN, ExternalIdType.Ean },
                { SKU, ExternalIdType.Sku },
            };
        }
    }
}
