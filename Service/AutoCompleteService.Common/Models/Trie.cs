
using AutoCompleteService.Common.Enumeration;
using System;
using System.Collections.Generic;

namespace AutoCompleteService.Common.Models
{
    internal class Trie
    {
        
        public readonly TrieNode Root;

        public Trie()
        {
            this.Root = new TrieNode();
            this.Root.Children = new SortedDictionary<char, TrieNode>(new TrieCharacterComparer());
        }

    
        public TrieNodeSearchResult SearchLastNodeFrom(string keyword)
        {
            if (keyword == null)
                throw new ArgumentNullException(nameof(keyword));

            TrieNode currentNode = this.Root;

            for (int i = 0; i < keyword.Length; i++)
            {
              
                var foundNode = currentNode.GetNodeFromChildren(keyword[i]);

                if (foundNode != null)
                {
                    currentNode = foundNode;
                    continue;
                }
                else
                {
                    return TrieNodeSearchResult.CreateFoundStartsWith(currentNode, i);
                }
            }
            
            return TrieNodeSearchResult.CreateFoundEquals(currentNode);
        }
        
        public bool Add(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                throw new ArgumentNullException(nameof(keyword));
            
            var result = SearchLastNodeFrom(keyword);

            if (
                result.Status == TrieNodeSearchResultType.Unkown ||
                result.Status == TrieNodeSearchResultType.NotFound)
            {
                return false;
            }
            else if (result.Status == TrieNodeSearchResultType.FoundStartsWith)
            {
                string prefix = keyword;
               
                if (result.LastKeywordIndex != null && result.LastKeywordIndex.HasValue && result.LastKeywordIndex.Value > 0)
                {
                    prefix = keyword.Substring(
                        result.LastKeywordIndex.Value,
                        keyword.Length - result.LastKeywordIndex.Value
                    );
                }

                var newTrie = TrieNode.CreateFrom(prefix);
                result.Node.Add(newTrie);

                return true;
            } 
            else if (result.Status == TrieNodeSearchResultType.FoundEquals)
            {
                if (!result.Node.IsTerminal)
                    result.Node.IsTerminal = true;
            }

            return false;
        }
    }
}