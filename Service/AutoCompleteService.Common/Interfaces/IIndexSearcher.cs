

using AutoCompleteService.Common.Models;

namespace AutoCompleteService.Common.Interfaces
{
    public interface IIndexSearcher
    {
        SearchResult Search(string term, int maxItemCount, bool suggestWhenNotFound);

        SearchResult Search(SearchOptions options);
    }
}