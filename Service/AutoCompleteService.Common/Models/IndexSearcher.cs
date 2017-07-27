﻿
using AutoCompleteService.Common.Enumeration;
using AutoCompleteService.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;

namespace AutoCompleteService.Common.Models
{
    public abstract class IndexSearcher : IIndexSearcher
    {
        private Stream _indexStream;
        private Stream _headerStream;
        private Stream _tailStream;

        private TrieIndexHeader _header;

        public IndexSearcher()
        {}

        public IndexSearcher(Stream headerStream, Stream indexStream)
            :this(headerStream, indexStream, null)
        {}

        public IndexSearcher(Stream headerStream, Stream indexStream, Stream tailStream)
        {
            _indexStream = indexStream;
            _headerStream = headerStream;
            _tailStream = tailStream;
        }

        public virtual SearchResult Search(string term, int maxItemCount, bool suggestWhenNotFound)
        {
            SearchOptions searchOptions = new SearchOptions();
            searchOptions.Term = term;
            searchOptions.MaxItemCount = maxItemCount;
            searchOptions.SuggestWhenFoundStartsWith = suggestWhenNotFound;

            return Search(searchOptions);
        }

        public SearchResult Search(SearchOptions options)
        {
            if (options == null)
                throw new ArgumentException("options");

            if (_header == null)
                _header = GetHeader();

            var reader = CreateTrieBinaryReader();
            var node = reader.SearchLastNode(0, options.Term);

            return CreateResultFromNode(reader, node, options.Term, options);
        }

        private SearchResult CreateResultFromNode(TrieBinaryReader trieBinaryReader, TrieNodeStructSearchResult node, string keyword, SearchOptions options)
        {
            var searchResult = new SearchResult();
            if (node.Status == TrieNodeSearchResultType.NotFound || node.Status == TrieNodeSearchResultType.Unkown)
            {
                searchResult.ResultType = node.Status;
                return searchResult;
            }

            string prefix = null;
            if (node.Status == TrieNodeSearchResultType.FoundStartsWith)
            {
                if (!options.SuggestWhenFoundStartsWith)
                {
                    searchResult.ResultType = node.Status;
                    return searchResult;
                }

                prefix = keyword.Substring(0, node.LastFoundCharacterIndex - 1);
            }
            else if (node.Status == TrieNodeSearchResultType.FoundEquals)
            {
               
                if (GetTailStream() == null)
                {
                    prefix = keyword.Substring(0, keyword.Length - 1);
                }
                else
                {
                    prefix = keyword;
                }
            }

            searchResult.ResultType = node.Status;
            searchResult.Items = trieBinaryReader.GetAutoCompleteNodes(
                                                                    node.LastFoundNodePosition,
                                                                    prefix,
                                                                    options.MaxItemCount,
                                                                    new List<string>(),
                                                                    GetTailStream()
                                                                ).ToArray();

            return searchResult;
        }

        internal virtual TrieIndexHeader GetHeader()
        {
            var serializer = new TrieIndexHeaderSerializer();
            var header = serializer.Deserialize(_headerStream);
            return header;
        }

        internal virtual Stream GetIndexStream()
        {
            return _indexStream;
        }

        internal virtual Stream GetTailStream()
        {
            return _tailStream;
        }

        private TrieBinaryReader CreateTrieBinaryReader()
        {
            Stream stream = GetIndexStream();
            BinaryReader binaryReader = new BinaryReader(stream);
            var trieBinaryReader = new TrieBinaryReader(binaryReader, _header);

            return trieBinaryReader;
        }
    }
}