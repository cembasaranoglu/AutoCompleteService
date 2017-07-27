﻿using AutoCompleteService.Common.Interfaces;
using AutoCompleteService.Common.Models;
using AutoCompleteService.Common.Search;
using System;
using System.IO;

namespace AutoCompleteService.Console
{
    class Program
    {
        static bool useTailFile = true;
        static string headerPath = "Headers.txt";
        static string indexPath = "index.bin";
        static string tailPath = "TailWords.txt";

        public static void Main(string[] args)
        {
            GetReadyForSearch();
        }

        private static void GetReadyForSearch()
        {
            BuildIndex();

            var searcher = CreateSearcher();

            while (true)
            {
                System.Console.WriteLine("Type any term");
                string term = System.Console.ReadLine();

                var searchResult = searcher.Search(term, 5, false);

                if (searchResult != null && searchResult.Items != null)
                {
                    foreach (var item in searchResult.Items)
                    {
                        System.Console.WriteLine(item);
                    }
                }

            }
        }

        private static IIndexSearcher CreateSearcher()
        {
            IIndexSearcher searcher = null;
            if (useTailFile)
                searcher = new InMemoryIndexSearcher(headerPath, indexPath, tailPath);
            else
                searcher = new InMemoryIndexSearcher(headerPath, indexPath);
            return searcher;
        }

        private static void BuildIndex()
        {
            if (File.Exists(headerPath))
                File.Delete(headerPath);

            if (File.Exists(indexPath))
                File.Delete(indexPath);

            if (File.Exists(tailPath))
                File.Delete(tailPath);

            using (var header = new FileStream(headerPath, FileMode.Create))
            {
                using (var index = new FileStream(indexPath, FileMode.Create))
                {
                    using (var tail = new FileStream(tailPath, FileMode.Create))
                    {
                        IndexBuilder builder = null;
                        if (useTailFile)
                            builder = new IndexBuilder(header, index, tail);
                        else
                            builder = new IndexBuilder(header, index);

                        foreach (var item in File.ReadLines("Words.txt"))
                        {
                            if (!string.IsNullOrWhiteSpace((item)))
                                builder.Add(item);
                        }

                        builder.Build();
                    }
                }
            }
        }
    }
}