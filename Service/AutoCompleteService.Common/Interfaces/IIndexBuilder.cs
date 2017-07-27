using AutoCompleteService.Common.Models;
using System.Collections.Generic;

namespace AutoCompleteService.Common.Interfaces
{
    public interface IIndexBuilder
    {
        IndexBuilder Add(string keyword);

        IndexBuilder AddRange(IEnumerable<string> keywords);

        int Build();
    }
}