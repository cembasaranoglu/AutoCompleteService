﻿
using System.Collections.Generic;
using System.IO;
using System;
using AutoCompleteService.Common.Models;
using AutoCompleteService.Common.Extensions;

namespace AutoCompleteService.Common.Search
{
    public class InMemoryIndexSearcher : IndexSearcher
    {
        private const int FirstReadBufferSize = 10 * 1024;

        private static object _lockObject = new object();
        private static Dictionary<string, TrieIndexHeader> _headerDictionary;
        private static Dictionary<string, byte[]> _indexData;
        private static Dictionary<string, byte[]> _tailData;

        private string _headerFileName;
        private string _indexFileName;
        private string _tailFileName;

        public InMemoryIndexSearcher(string headerFileName, string indexFileName) :
            this(headerFileName, indexFileName, null)
        { }

        public InMemoryIndexSearcher(
                string headerFileName,
                string indexFileName,
                string tailFileName
            ) : base()
        {
            _headerFileName = headerFileName;
            _indexFileName = indexFileName;
            _tailFileName = tailFileName;

            _headerDictionary = new Dictionary<string, TrieIndexHeader>();
            _indexData = new Dictionary<string, byte[]>();
            _tailData = new Dictionary<string, byte[]>();
        }

        internal override TrieIndexHeader GetHeader()
        {
            if (!_headerDictionary.ContainsKey(_headerFileName))
            {
                lock (_lockObject)
                {
                    if (!_headerDictionary.ContainsKey(_headerFileName))
                    {
                        var currentHeader = TrieNodeHelperFileSystemExtensions.ReadHeaderFile(_headerFileName);

                        _headerDictionary.Add(_headerFileName, currentHeader);
                    }
                }
            }

            TrieIndexHeader header = _headerDictionary[_headerFileName];
            return header;
        }

        internal override Stream GetIndexStream()
        {
            if (!_indexData.ContainsKey(_indexFileName))
            {
                lock (_lockObject)
                {
                    if (!_indexData.ContainsKey(_indexFileName))
                    {
                        Stream stream = new FileStream(
                                                _indexFileName,
                                                FileMode.Open,
                                                FileAccess.Read,
                                                FileShare.Read,
                                                FirstReadBufferSize,
                                                FileOptions.RandomAccess
                                           );

                        stream.Position = 0;

                        byte[] streamBytes = new byte[stream.Length];
                        stream.Read(streamBytes, 0, streamBytes.Length);

                        stream.Dispose();
                        stream = null;

                        _indexData.Add(_indexFileName, streamBytes);
                    }
                }
            }

            return new ManagedInMemoryStream(_indexData[_indexFileName]);
        }

        internal override Stream GetTailStream()
        {
            if (_tailFileName == null)
                return null;
            
            if (!_tailData.ContainsKey(_tailFileName))
            {
                lock (_lockObject)
                {
                    if (!_tailData.ContainsKey(_tailFileName))
                    {
                        var streamBytes = GetBytesFromFile(_tailFileName);

                        if (streamBytes == null || streamBytes.Length == 0)
                            throw new InvalidProgramException("Tail file can not be null or empty");
                        
                        _tailData.Add(_tailFileName, streamBytes);
                    }
                }
            }

            return new ManagedInMemoryStream(_tailData[_tailFileName]);
        }

        private byte[] GetBytesFromFile(string path)
        {
            using (Stream stream = new FileStream(
                                                path,
                                                FileMode.Open,
                                                FileAccess.Read,
                                                FileShare.Read,
                                                FirstReadBufferSize,
                                                FileOptions.RandomAccess
            ))
            {
                byte[] streamBytes = new byte[stream.Length];
                stream.Read(streamBytes, 0, streamBytes.Length);
                return streamBytes;
            }
        }
    }
}