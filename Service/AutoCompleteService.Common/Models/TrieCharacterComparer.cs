using System.Collections.Generic;

namespace AutoCompleteService.Common.Models
{
    internal class TrieCharacterComparer : IComparer<char>
    {
        public int Compare(char left, char right)
        {
            return left.CompareTo(right);
        }
    }
}
