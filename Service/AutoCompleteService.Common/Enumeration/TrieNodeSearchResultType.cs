namespace AutoCompleteService.Common.Enumeration
{
    public enum TrieNodeSearchResultType
    {
        Unkown = 0,
        FoundEquals = 10,
        FoundStartsWith = 20,
        NotFound = 30,
    }
}