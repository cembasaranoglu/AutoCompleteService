using AutoCompleteService.Common.Enumeration;

namespace AutoCompleteService.Common.Models
{
    public class SearchResult
    {
        public string[] Items { get; set; }

        public TrieNodeSearchResultType ResultType { get; set; }
    }
}