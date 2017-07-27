using System.Collections.Generic;

namespace AutoCompleteService.Common.Models
{
    internal class TrieStringComparer : IComparer<string>
    {
        public int Compare(string left, string right)
        {
            return left.CompareTo(right);
        }
    }
}
