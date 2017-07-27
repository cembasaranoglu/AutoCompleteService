using System;
using System.Collections.Generic;
using System.Text;

namespace AutoCompleteService.Common.Models
{
    internal class TrieNode
    {
        
        public int Order { get; set; }

        public int ChildrenOffset { get; set; }
        
        internal int ChildrenCount { get; set; }
        
        public char Character { get; set; }
        
        public TrieNode Parent { get; set; }
        public IDictionary<char, TrieNode> Children { get; set; }
        
        public bool IsTerminal { get; set; }
        
        public int ChildIndex { get; set; }
        
        public uint? PositionOnTextFile { get; set; }
        
        public TrieNode(char character)
            : this()
        {
            Character = character;
        }
        
        public TrieNode()
        {
        }

        public void Add(TrieNode child)
        {
            if (child == null)
                throw new ArgumentException("child");

            if (Children == null)
                Children = new SortedDictionary<char, TrieNode>(new TrieCharacterComparer());
            
            if (Children.ContainsKey(child.Character))
            {
                TrieNode existsNode = Children[child.Character];
                if (child.Children != null)
                {
                    foreach (var item in child.Children)
                    {
                        existsNode.Add(item.Value);
                    }
                }
            }
            else
            {
                child.Parent = this;
                Children.Add(child.Character, child);
            }
        }

        public TrieNode GetNodeFromChildren(char character)
        {
            if (Children == null || !Children.ContainsKey(character))
                return null;

            return Children[character];
        }
        
        public string GetString()
        {
            StringBuilder sb = new StringBuilder();
            TrieNode currentNode = this;

            while (currentNode != null && currentNode.Parent != null)
            {
                sb.Insert(0, currentNode.Character);
                currentNode = currentNode.Parent;
            }

            return sb.ToString();
        }
        
        public static TrieNode CreateFrom(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword) || keyword.Length == 0)
                throw new ArgumentException("keyword");

            TrieNode returnValue = new TrieNode(keyword[0]);
            if (keyword.Length == 1)
            {
                returnValue.IsTerminal = true;
                return returnValue;
            }

            TrieNode currentNode = returnValue;
            
            for (int i = 1; i < keyword.Length; i++)
            {
                TrieNode newNode = new TrieNode(keyword[i]);

                currentNode.Add(newNode);
                
                if (i == keyword.Length - 1)
                {
                    newNode.IsTerminal = true;
                }

                currentNode = newNode;
            }

            return returnValue;
        }
    }
}