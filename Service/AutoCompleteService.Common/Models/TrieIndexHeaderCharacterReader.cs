using System;
using System.Collections.Generic;

namespace AutoCompleteService.Common.Models
{
    public class TrieIndexHeaderCharacterReader
    {
        public static TrieIndexHeaderCharacterReader Instance = new TrieIndexHeaderCharacterReader();

        private IDictionary<TrieIndexHeader, bool> _isCharacterIndexCacheInitialized;
        private IDictionary<TrieIndexHeader, IDictionary<char, UInt16>> _characterIndexDictionary;

        public TrieIndexHeaderCharacterReader()
        {
            _isCharacterIndexCacheInitialized = new Dictionary<TrieIndexHeader, bool>();
            _characterIndexDictionary = new Dictionary<TrieIndexHeader, IDictionary<char, UInt16>>();
        }

      
        internal ushort? GetCharacterIndex(TrieIndexHeader header, char c)
        {
            InitCharacterCache(header);

            if (!_characterIndexDictionary[header].ContainsKey(c))
                return null;

            return _characterIndexDictionary[header][c];
        }

        internal char GetCharacterAtIndex(TrieIndexHeader header, UInt16 index)
        {
            InitCharacterCache(header);

            return header.CharacterList[index];
        }

        internal void InitCharacterCache(TrieIndexHeader header)
        {
            if (!_isCharacterIndexCacheInitialized.ContainsKey(header))
            {
                lock (this)
                {
                    if (!_isCharacterIndexCacheInitialized.ContainsKey(header))
                    {
                        _isCharacterIndexCacheInitialized.Add(header, true);
                        _characterIndexDictionary.Add(header, new Dictionary<char, UInt16>());

                        for (UInt16 i = 0; i < header.CharacterList.Count; i++)
                        {
                            if (header.CharacterList[i] != '\0')
                                _characterIndexDictionary[header].Add(header.CharacterList[i], i);
                        }
                    }
                }
            }
        }

    }
}